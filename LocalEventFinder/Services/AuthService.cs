using LocalEventFinder.Models;
using LocalEventFinder.Models.DTO;
using LocalEventFinder.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LocalEventFinder.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IRepository<User> userRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerDto)
        {
            // Проверяем, существует ли пользователь с таким email
            var existingUsersByEmail = await _userRepository.FindAsync(u => u.Email == registerDto.Email);
            if (existingUsersByEmail.Any())
            {
                throw new ArgumentException("Пользователь с таким email уже существует");
            }

            // Проверяем, существует ли пользователь с таким username
            var existingUsersByUsername = await _userRepository.FindAsync(u => u.Username == registerDto.Username);
            if (existingUsersByUsername.Any())
            {
                throw new ArgumentException("Пользователь с таким именем уже существует");
            }

            // Создаем нового пользователя
            var user = new User
            {
                Username = registerDto.Username.Trim(),
                Email = registerDto.Email.Trim().ToLower(),
                PasswordHash = HashPassword(registerDto.Password),
                Role = string.IsNullOrWhiteSpace(registerDto.Role) ? "User" : registerDto.Role.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);
            _logger.LogInformation("Зарегистрирован новый пользователь: {Email}", createdUser.Email);

            // Генерируем JWT токен
            var token = GenerateJwtToken(createdUser);

            return new AuthResponseDto
            {
                UserId = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Role = createdUser.Role,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(GetTokenLifetime()),
                Message = "Пользователь успешно зарегистрирован"
            };
        }

        /// <summary>
        /// Вход пользователя
        /// </summary>
        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginDto)
        {
            // Ищем пользователя по email
            var users = await _userRepository.FindAsync(u => u.Email == loginDto.Email.Trim().ToLower());
            var user = users.FirstOrDefault();

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Неверный email или пароль");
            }

            // Генерируем JWT токен
            var token = GenerateJwtToken(user);

            _logger.LogInformation("Пользователь вошел в систему: {Email}", user.Email);

            return new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(GetTokenLifetime()),
                Message = "Вход выполнен успешно"
            };
        }

        /// <summary>
        /// Проверка существования пользователя по email
        /// </summary>
        public async Task<bool> UserExistsAsync(string email)
        {
            var users = await _userRepository.FindAsync(u => u.Email == email);
            return users.Any();
        }

        /// <summary>
        /// Получение пользователя по ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Генерация JWT токена
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            if (key.Length < 32)
            {
                throw new ArgumentException("JWT ключ должен быть не менее 32 символов");
            }

            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(GetTokenLifetime()),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Хеширование пароля
        /// </summary>
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        /// <summary>
        /// Проверка пароля
        /// </summary>
        private bool VerifyPassword(string password, string passwordHash)
        {
            var hash = HashPassword(password);
            return hash == passwordHash;
        }

        /// <summary>
        /// Получение времени жизни токена из конфигурации
        /// </summary>
        private int GetTokenLifetime()
        {
            return _configuration.GetValue<int>("Jwt:TokenLifetimeHours", 24);
        }
    }
}

using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SEP490_G154_Service.DTOs.MaLogin;
using SEP490_G154_Service.Interface;
using SEP490_G154_Service.Models;
using SEP490_G154_Service.sHub;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SEP490_G154_Service.Service
{
    public class LoginService : ILogin
    {
        private readonly G154context _context;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly EmailService _emailService;

        public LoginService(G154context context, IConfiguration configuration, IMemoryCache cache, EmailService service)
        {
            _context = context;
            _configuration = configuration;
            _cache = cache;
            _emailService = service;
        }
        // ========= LOGIN THƯỜNG =========
        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            var user = await _context.Users.Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null) throw new UnauthorizedAccessException("Invalid email or password");

            bool isValidPassword = false;

            if (user.PasswordHash.StartsWith("$2"))
            {
                isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            }
            else if (request.Password == user.PasswordHash)
            {
                isValidPassword = true;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

            if (!isValidPassword) throw new UnauthorizedAccessException("Invalid email or password");

            var role = user.Roles.Select(r => r.Name).FirstOrDefault() ?? "Customer";
            var token = GenerateJwtToken(user.Email, role);

            return new LoginResponseDTO { Email = user.Email, Role = role, Token = token };
        }

        // ========= REGISTER =========
        public async Task<object> RegisterAsync(CreateAcc newAcc, int defaultRoleId)
        {
            if (await _context.Users.AnyAsync(u => u.Email == newAcc.Email))
                throw new InvalidOperationException("Email already exists");
            if (await _context.Users.AnyAsync(u => u.Phone == newAcc.Phone))
                throw new InvalidOperationException("Phone already exists");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newAcc.Password);

            var newUser = new User
            {
                Email = newAcc.Email,
                PasswordHash = hashedPassword,
                FullName = newAcc.FullName,
                Phone = newAcc.Phone,
                Status = 1,
                Roles = new List<Role>()
            };

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == defaultRoleId);
            if (role == null) throw new InvalidOperationException($"Role {defaultRoleId} not found");

            newUser.Roles.Add(role);
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new { message = "Registered", newUser.UserId, newUser.Email, Role = role.Name };
        }

        // ========= LOGIN GOOGLE =========
        public async Task<object> LoginWithGoogleAsync(GoogleLoginDTO request, int roleId)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                // để test với Playground  (openid email profile)
                Audience = new[] { _configuration["GoogleAuth:ClientId"], "407408718192.apps.googleusercontent.com" }
            });

            var email = payload.Email;
            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    PasswordHash = Guid.NewGuid().ToString(),
                    FullName = payload.Name,
                    AvatarUrl = payload.Picture,
                    Status = 1,
                    Roles = new List<Role>()
                };

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
                if (role == null) throw new InvalidOperationException($"Role {roleId} not found");

                user.Roles.Add(role);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var roleName = user.Roles.Select(r => r.Name).FirstOrDefault() ?? "Customer";
            var token = GenerateJwtToken(user.Email, roleName);

            return new { user.Email, user.FullName, Role = roleName, Token = token };
        }

        // ========= LOGIN FACEBOOK =========
        public async Task<object> LoginWithFacebookAsync(FacebookLoginDTO request, int roleId)
        {
            using var httpClient = new HttpClient();
            var url = $"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={request.AccessToken}";
            var response = await httpClient.GetStringAsync(url);
            dynamic fbUser = JsonConvert.DeserializeObject(response);

            string email = fbUser.email != null ? fbUser.email.ToString() : $"{fbUser.id}@facebook.com";

            var user = await _context.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    PasswordHash = Guid.NewGuid().ToString(),
                    FullName = fbUser.name,
                    AvatarUrl = fbUser.picture?.data?.url?.ToString(),
                    Status = 1,
                    Roles = new List<Role>()
                };

                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
                if (role == null) throw new InvalidOperationException($"Role {roleId} not found");

                user.Roles.Add(role);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var roleName = user.Roles.Select(r => r.Name).FirstOrDefault() ?? "Customer";
            var token = GenerateJwtToken(user.Email, roleName);

            return new { user.Email, user.FullName, Role = roleName, Token = token };
        }

        // ========== FORGOT PASSWORD ==========
        public async Task<object> ForgotPasswordAsync(ForgotPasswordDTO request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null)
                    throw new KeyNotFoundException("Email not found");

                // Sinh OTP ngẫu nhiên 6 số
                var otp = new Random().Next(100000, 999999).ToString();

                // Lưu OTP vào cache trong 5 phút
                _cache.Set($"otp_{user.Email}", otp, TimeSpan.FromMinutes(5));

                // Gửi OTP qua Email
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Password Reset OTP",
                    $"<h3>Your OTP code is: <b>{otp}</b></h3><p>This code will expire in 5 minutes.</p>"
                );

                return new { success = true, message = "OTP has been sent to your email." };
            }
            catch (Exception ex)
            {
                // Log ra console hoặc log file
                Console.WriteLine($"[ForgotPasswordAsync] Error: {ex.Message}");
                return new { success = false, message = "Failed to send OTP", error = ex.Message };
            }
        }

        // ========== RESET PASSWORD WITH OTP ==========
        public async Task<object> ResetPasswordWithOtpAsync(ResetPasswordWithOtpDTO request)
        {
            try
            {
                if (!_cache.TryGetValue($"otp_{request.Email}", out string? cachedOtp) || cachedOtp != request.Otp)
                    throw new UnauthorizedAccessException("Invalid or expired OTP");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user == null) throw new KeyNotFoundException("User not found");

                // Cập nhật password
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                _context.Update(user);
                await _context.SaveChangesAsync();

                // Xoá OTP sau khi dùng
                _cache.Remove($"otp_{request.Email}");

                return new { success = true, message = "Password reset successful" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ResetPasswordWithOtpAsync] Error: {ex.Message}");
                return new { success = false, message = "Password reset failed", error = ex.Message };
            }
        }


        // ========= HELPER =========
        private string GenerateJwtToken(string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
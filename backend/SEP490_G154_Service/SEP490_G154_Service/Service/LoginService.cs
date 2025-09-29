using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SEP490_G154_Service.DTOs.MaLogin;
using SEP490_G154_Service.Interface;
using SEP490_G154_Service.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SEP490_G154_Service.Service
{
    public class LoginService : ILogin
    {
        private readonly G154context _context;
        private readonly IConfiguration _configuration;

        public LoginService(G154context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

        // ========= FORGOT PASSWORD =========
        public async Task<object> ForgotPasswordAsync(ForgotPasswordDTO request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) throw new KeyNotFoundException("Email not found");

            var resetToken = GenerateJwtToken(user.Email, "ResetPassword");

            user.Status = 2;
            await _context.SaveChangesAsync();

            return new { message = "Reset token created", resetToken };
        }

        // ========= RESET PASSWORD =========
        public async Task<object> ResetPasswordWithTokenAsync(ResetPasswordWithTokenDTO request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var principal = tokenHandler.ValidateToken(request.Token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) throw new UnauthorizedAccessException("Invalid token");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) throw new KeyNotFoundException("User not found");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            _context.Update(user);
            await _context.SaveChangesAsync();

            return new { message = "Password reset successful" };
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
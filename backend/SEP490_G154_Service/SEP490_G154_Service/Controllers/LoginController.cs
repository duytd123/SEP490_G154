using Google.Apis.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SEP490_G154_Service.DTOs;
using SEP490_G154_Service.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace SEP490_G154_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly G154context _context;
        private readonly IConfiguration _configuration;

        public LoginController(G154context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return BadRequest("Email & Password are required");

            var user = _context.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.Email == email);

            if (user == null)
                return Unauthorized("Invalid email or password");

            bool isValidPassword = false;

            // Nếu password đã hash (BCrypt format)
            if (user.PasswordHash.StartsWith("$2"))
            {
                isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            else
            {
                // Dữ liệu cũ chưa hash
                if (password == user.PasswordHash)
                {
                    isValidPassword = true;

                    // Hash lại để đồng bộ hóa
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
                    _context.Users.Update(user);
                    _context.SaveChanges();
                }
            }

            if (!isValidPassword)
                return Unauthorized("Invalid email or password");

            // Lấy role
            var role = user.Roles.Select(r => r.Name).FirstOrDefault() ?? "Customer";

            // Tạo JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, role)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Email = user.Email,
                Token = tokenString
            });
        }



        // register Admin with default role = 1
        [HttpPost("RegisterAdmin")]
        public IActionResult RegisterAdmin([FromBody] CreateAcc newAcc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == newAcc.Email);
            if (existingUser != null)
                return Conflict("Email already exists");

            var existPhone = _context.Users.FirstOrDefault(u => u.Phone == newAcc.Phone);
            if (existPhone != null)
                return Conflict("Phone number already exists");

            // Hash password trước khi lưu
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newAcc.Password);

            var newUser = new User
            {
                Email = newAcc.Email,
                PasswordHash = hashedPassword,
                FullName = newAcc.FullName,
                Phone = newAcc.Phone,
                Status = 1
            };

            var defaultRole = _context.Roles.FirstOrDefault(r => r.RoleId == 1);
            if (defaultRole == null)
                return NotFound("Default role with RoleId = 1 not found");

            newUser.Roles.Add(defaultRole);

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new
            {
                Message = "User registered successfully with default role = 1",
                UserId = newUser.UserId,
                Email = newUser.Email,
                Role = defaultRole.Name
            });
        }



        // register Seller with default role = 2
        [HttpPost("RegisterSeller")]
        public IActionResult RegisterSeller([FromBody] CreateAcc newAcc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == newAcc.Email);
            if (existingUser != null)
                return Conflict("Email already exists");
            var existPhone = _context.Users.FirstOrDefault(u => u.Phone == newAcc.Phone);
            if (existPhone != null)
                return Conflict("Phone number already exists");
            // Hash password trước khi lưu
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newAcc.Password);

            var newUser = new User
            {
                Email = newAcc.Email,
                PasswordHash = hashedPassword,
                FullName = newAcc.FullName,
                Phone = newAcc.Phone,
                Status = 1
            };

            var defaultRole = _context.Roles.FirstOrDefault(r => r.RoleId == 2);
            if (defaultRole == null)
                return NotFound("Default role with RoleId = 2 not found");

            newUser.Roles.Add(defaultRole);

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new
            {
                Message = "User registered successfully with default role = 2",
                UserId = newUser.UserId,
                Email = newUser.Email,
                Role = defaultRole.Name
            });
        }



        // register Host with default role = 3
        [HttpPost("RegisterHost")]
        public IActionResult RegisterHost([FromBody] CreateAcc newAcc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == newAcc.Email);
            if (existingUser != null)
                return Conflict("Email already exists");
            var existPhone = _context.Users.FirstOrDefault(u => u.Phone == newAcc.Phone);
            if (existPhone != null)
                return Conflict("Phone number already exists");
            // Hash password trước khi lưu
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newAcc.Password);

            var newUser = new User
            {
                Email = newAcc.Email,
                PasswordHash = hashedPassword,
                FullName = newAcc.FullName,
                Phone = newAcc.Phone,
                Status = 1
            };

            var defaultRole = _context.Roles.FirstOrDefault(r => r.RoleId == 3);
            if (defaultRole == null)
                return NotFound("Default role with RoleId = 3 not found");

            newUser.Roles.Add(defaultRole);

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new
            {
                Message = "User registered successfully with default role = 3",
                UserId = newUser.UserId,
                Email = newUser.Email,
                Role = defaultRole.Name
            });
        }




        // register customer with default role = 4 
        [HttpPost("RegisterCustomer")]
        public IActionResult RegisterCustomer([FromBody] CreateAcc newAcc)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == newAcc.Email);
            if (existingUser != null)
                return Conflict("Email already exists");
            var existPhone = _context.Users.FirstOrDefault(u => u.Phone == newAcc.Phone);
            if (existPhone != null)
                return Conflict("Phone number already exists");
            // Hash password trước khi lưu
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newAcc.Password);

            var newUser = new User
            {
                Email = newAcc.Email,
                PasswordHash = hashedPassword,
                FullName = newAcc.FullName,
                Phone = newAcc.Phone,
                Status = 1
            };

            var defaultRole = _context.Roles.FirstOrDefault(r => r.RoleId == 1);
            if (defaultRole == null)
                return NotFound("Default role with RoleId = 1 not found");

            newUser.Roles.Add(defaultRole);

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok(new
            {
                Message = "User registered successfully with default role = 1",
                UserId = newUser.UserId,
                Email = newUser.Email,
                Role = defaultRole.Name
            });
        }




        [HttpPost("LoginWithGoogleSeller")]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginDTO request)
        {
            try
            {
                // Xác thực token với Google
                //var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                //{
                //    Audience = new[] {
                //        _configuration["GoogleAuth:ClientId"]

                //    }
                //});

                // để test với Playground  (openid email profile)
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[]
    {
        _configuration["GoogleAuth:ClientId"], // client ID app của bạn
        "407408718192.apps.googleusercontent.com" // client ID mặc định của Playground
    }
                });

                var email = payload.Email;
                var fullName = payload.Name;
                var avatarUrl = payload.Picture;

                // Kiểm tra user đã tồn tại chưa
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    // Tạo user mới với role = 2
                    user = new User
                    {
                        Email = email,
                        PasswordHash = Guid.NewGuid().ToString(), // random vì Google login không cần password
                        FullName = fullName ?? "Google User",
                        AvatarUrl = avatarUrl,
                        Status = 1
                    };

                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == 2);
                    if (role == null)
                        return NotFound("Default role with RoleId = 2 not found");

                    user.Roles.Add(role);

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Lấy role
                var roleName = user.Roles.Select(r => r.Name).FirstOrDefault() ?? "Customer";

                // Tạo JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new
                {
                    Email = user.Email,
                    Name = user.FullName,
                    Role = roleName,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Google login failed", error = ex.Message });
            }
        }



        [HttpPost("LoginWithGoogleHost")]
        public async Task<IActionResult> LoginWithGoogleHost([FromBody] GoogleLoginDTO request)
        {
            try
            {
                // Xác thực token với Google
                //var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                //{
                //    Audience = new[] {
                //        _configuration["GoogleAuth:ClientId"]

                //    }
                //});

                // để test với Playground  (openid email profile)
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[]
    {
        _configuration["GoogleAuth:ClientId"], // client ID app của bạn
        "407408718192.apps.googleusercontent.com" // client ID mặc định của Playground
    }
                });

                var email = payload.Email;
                var fullName = payload.Name;
                var avatarUrl = payload.Picture;

                // Kiểm tra user đã tồn tại chưa
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    // Tạo user mới với role = 2
                    user = new User
                    {
                        Email = email,
                        PasswordHash = Guid.NewGuid().ToString(), // random vì Google login không cần password
                        FullName = fullName ?? "Google User",
                        AvatarUrl = avatarUrl,
                        Status = 1
                    };

                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == 3);
                    if (role == null)
                        return NotFound("Default role with RoleId = 3 not found");

                    user.Roles.Add(role);

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Lấy role
                var roleName = user.Roles.Select(r => r.Name).FirstOrDefault() ?? "Customer";

                // Tạo JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new
                {
                    Email = user.Email,
                    Name = user.FullName,
                    Role = roleName,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Google login failed", error = ex.Message });
            }
        }



        [HttpPost("LoginWithGoogleCustomer")]
        public async Task<IActionResult> LoginWithGoogleCustomer([FromBody] GoogleLoginDTO request)
        {
            try
            {
                // Xác thực token với Google
                //var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                //{
                //    Audience = new[] {
                //        _configuration["GoogleAuth:ClientId"]

                //    }
                //});

                // để test với Playground  (openid email profile)
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[]
    {
        _configuration["GoogleAuth:ClientId"], // client ID app của bạn
        "407408718192.apps.googleusercontent.com" // client ID mặc định của Playground
    }
                });

                var email = payload.Email;
                var fullName = payload.Name;
                var avatarUrl = payload.Picture;

                // Kiểm tra user đã tồn tại chưa
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    // Tạo user mới với role = 2
                    user = new User
                    {
                        Email = email,
                        PasswordHash = Guid.NewGuid().ToString(), // random vì Google login không cần password
                        FullName = fullName ?? "Google User",
                        AvatarUrl = avatarUrl,
                        Status = 1
                    };

                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == 4);
                    if (role == null)
                        return NotFound("Default role with RoleId = 4 not found");

                    user.Roles.Add(role);

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Lấy role
                var roleName = user.Roles.Select(r => r.Name).FirstOrDefault() ?? "Customer";

                // Tạo JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new
                {
                    Email = user.Email,
                    Name = user.FullName,
                    Role = roleName,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Google login failed", error = ex.Message });
            }
        }





        [HttpPost("LoginWithFacebookSeller")]
        public async Task<IActionResult> LoginWithFacebookSeller([FromBody] FacebookLoginDTO request)
        {
            try
            {
                using var httpClient = new HttpClient();

                // Gọi Graph API để lấy thông tin user từ access token
                var url = $"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={request.AccessToken}";
                var response = await httpClient.GetStringAsync(url);
                dynamic fbUser = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

                // Email có thể null nếu user chưa cho phép -> fallback dùng FacebookId
                string email = fbUser.email != null ? fbUser.email.ToString() : $"{fbUser.id}@facebook.com";
                string fullName = fbUser.name != null ? fbUser.name.ToString() : "Facebook User";
                string avatarUrl = fbUser.picture?.data?.url?.ToString();

                // Kiểm tra user có trong DB chưa
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    // Tạo user mới với role = 2 (Seller chẳng hạn)
                    user = new User
                    {
                        Email = email,
                        PasswordHash = Guid.NewGuid().ToString(), // random vì login FB không cần pass
                        FullName = fullName,
                        AvatarUrl = avatarUrl,
                        Status = 1,
                        Roles = new List<Role>() 
                    };

                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == 2);
                    if (role != null)
                    {
                        user.Roles.Add(role);
                    }

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                var roleName = user.Roles.Select(r => r.Name).FirstOrDefault() ?? "Customer";

                // Tạo JWT token của hệ thống bạn
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new
                {
                    Email = user.Email,
                    Name = user.FullName,
                    Role = roleName,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Facebook login failed",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpPost("LoginWithFacebookHost")]
        public async Task<IActionResult> LoginWithFacebookHost([FromBody] FacebookLoginDTO request)
        {
            try
            {
                using var httpClient = new HttpClient();

                // Gọi Graph API để lấy thông tin user từ access token
                var url = $"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={request.AccessToken}";
                var response = await httpClient.GetStringAsync(url);
                dynamic fbUser = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

                // Email có thể null nếu user chưa cho phép -> fallback dùng FacebookId
                string email = fbUser.email != null ? fbUser.email.ToString() : $"{fbUser.id}@facebook.com";
                string fullName = fbUser.name != null ? fbUser.name.ToString() : "Facebook User";
                string avatarUrl = fbUser.picture?.data?.url?.ToString();

                // Kiểm tra user có trong DB chưa
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    // Tạo user mới với role = 3 (Host)
                    user = new User
                    {
                        Email = email,
                        PasswordHash = Guid.NewGuid().ToString(), 
                        FullName = fullName,
                        AvatarUrl = avatarUrl,
                        Status = 1,
                        Roles = new List<Role>()
                    };

                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == 3);
                    if (role != null)
                    {
                        user.Roles.Add(role);
                    }

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                var roleName = user.Roles.Select(r => r.Name).FirstOrDefault() ?? "Customer";

                // Tạo JWT token của hệ thống bạn
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new
                {
                    Email = user.Email,
                    Name = user.FullName,
                    Role = roleName,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Facebook login failed",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
        [HttpPost("LoginWithFacebookCustomer")]
        public async Task<IActionResult> LoginWithFacebookCustomer([FromBody] FacebookLoginDTO request)
        {
            try
            {
                using var httpClient = new HttpClient();

                // Gọi Graph API để lấy thông tin user từ access token
                var url = $"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={request.AccessToken}";
                var response = await httpClient.GetStringAsync(url);
                dynamic fbUser = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

                // Email có thể null nếu user chưa cho phép -> fallback dùng FacebookId
                string email = fbUser.email != null ? fbUser.email.ToString() : $"{fbUser.id}@facebook.com";
                string fullName = fbUser.name != null ? fbUser.name.ToString() : "Facebook User";
                string avatarUrl = fbUser.picture?.data?.url?.ToString();

                // Kiểm tra user có trong DB chưa
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    // Tạo user mới với role = 4 (Customer)
                    user = new User
                    {
                        Email = email,
                        PasswordHash = Guid.NewGuid().ToString(), // random vì login FB không cần pass
                        FullName = fullName,
                        AvatarUrl = avatarUrl,
                        Status = 1,
                        Roles = new List<Role>()
                    };

                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleId == 4);
                    if (role != null)
                    {
                        user.Roles.Add(role);
                    }

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                var roleName = user.Roles.Select(r => r.Name).FirstOrDefault() ?? "Customer";

                // Tạo JWT token của hệ thống bạn
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName)
            }),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new
                {
                    Email = user.Email,
                    Name = user.FullName,
                    Role = roleName,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Facebook login failed",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

    }


}

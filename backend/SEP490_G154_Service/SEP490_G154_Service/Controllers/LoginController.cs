using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEP490_G154_Service.DTOs.MaLogin;
using SEP490_G154_Service.Interface;

namespace SEP490_G154_Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILogin _loginService;

        public LoginController(ILogin loginService)
        {
            _loginService = loginService;
        }

        // ========== ĐĂNG NHẬP ==========
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu đăng nhập không hợp lệ.",
                    errors
                });
            }

            var result = await _loginService.LoginAsync(request);
            return Ok(result);
        }

        // ========== ĐĂNG KÝ NGƯỜI DÙNG ==========
        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] CreateAcc request)
            => await HandleRegister(request, 1);

        [HttpPost("RegisterSeller")]
        public async Task<IActionResult> RegisterSeller([FromBody] CreateAcc request)
            => await HandleRegister(request, 2);

        [HttpPost("RegisterHost")]
        public async Task<IActionResult> RegisterHost([FromBody] CreateAcc request)
            => await HandleRegister(request, 3);

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CreateAcc request)
            => await HandleRegister(request, 4);

        private async Task<IActionResult> HandleRegister(CreateAcc request, int roleId)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    message = "Dữ liệu đăng ký không hợp lệ.",
                    errors
                });
            }

            var result = await _loginService.RegisterAsync(request, roleId);
            return Ok(result);
        }

        // ========== GOOGLE LOGIN ==========
        [HttpPost("LoginWithGoogleSeller")]
        public async Task<IActionResult> LoginWithGoogleSeller([FromBody] GoogleLoginDTO request)
            => await HandleGoogleLogin(request, 2);

        [HttpPost("LoginWithGoogleHost")]
        public async Task<IActionResult> LoginWithGoogleHost([FromBody] GoogleLoginDTO request)
            => await HandleGoogleLogin(request, 3);

        [HttpPost("LoginWithGoogleCustomer")]
        public async Task<IActionResult> LoginWithGoogleCustomer([FromBody] GoogleLoginDTO request)
            => await HandleGoogleLogin(request, 4);

        private async Task<IActionResult> HandleGoogleLogin(GoogleLoginDTO request, int roleId)
        {
            if (string.IsNullOrWhiteSpace(request.IdToken))
                return BadRequest(new { success = false, message = "Thiếu mã xác thực Google (IdToken)." });

            try
            {
                var result = await _loginService.LoginWithGoogleAsync(request, roleId);
                return Ok(result);
            }
            catch (InvalidJwtException)
            {
                return BadRequest(new { success = false, message = "Mã xác thực Google không hợp lệ hoặc đã hết hạn." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GoogleLogin] Error: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi đăng nhập Google.", error = ex.Message });
            }
        }

        // ========== FACEBOOK LOGIN ==========
        [HttpPost("LoginWithFacebookSeller")]
        public async Task<IActionResult> LoginWithFacebookSeller([FromBody] FacebookLoginDTO request)
            => await HandleFacebookLogin(request, 2);

        [HttpPost("LoginWithFacebookHost")]
        public async Task<IActionResult> LoginWithFacebookHost([FromBody] FacebookLoginDTO request)
            => await HandleFacebookLogin(request, 3);

        [HttpPost("LoginWithFacebookCustomer")]
        public async Task<IActionResult> LoginWithFacebookCustomer([FromBody] FacebookLoginDTO request)
            => await HandleFacebookLogin(request, 4);

        private async Task<IActionResult> HandleFacebookLogin(FacebookLoginDTO request, int roleId)
        {
            if (string.IsNullOrWhiteSpace(request.AccessToken))
                return BadRequest(new { success = false, message = "Thiếu mã xác thực Facebook (AccessToken)." });

            try
            {
                var result = await _loginService.LoginWithFacebookAsync(request, roleId);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return BadRequest(new { success = false, message = "Không thể kết nối đến Facebook API.", error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[FacebookLogin] Error: {ex.Message}");
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống khi đăng nhập Facebook.", error = ex.Message });
            }
        }

        // ========== QUÊN MẬT KHẨU ==========
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { success = false, message = "Vui lòng nhập email để nhận mã OTP." });

            var result = await _loginService.ForgotPasswordAsync(request);
            return Ok(result);
        }

        // ========== ĐẶT LẠI MẬT KHẨU ==========
        [HttpPost("ResetPasswordWithOtp")]
        public async Task<IActionResult> ResetPasswordWithOtp([FromBody] ResetPasswordWithOtpDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.Otp))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Vui lòng nhập đầy đủ Email, OTP và mật khẩu mới."
                });
            }

            var result = await _loginService.ResetPasswordWithOtpAsync(request);
            return Ok(result);
        }
    }
}

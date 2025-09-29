using Google.Apis.Auth;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SEP490_G154_Service.DTOs;
using SEP490_G154_Service.DTOs.MaLogin;
using SEP490_G154_Service.Interface;
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
        private readonly ILogin _loginService;

        public LoginController(ILogin loginService)
        {
            _loginService = loginService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
            => Ok(await _loginService.LoginAsync(request));

        [HttpPost("RegisterAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] CreateAcc request)
            => Ok(await _loginService.RegisterAsync(request, 1));

        [HttpPost("RegisterSeller")]
        public async Task<IActionResult> RegisterSeller([FromBody] CreateAcc request)
            => Ok(await _loginService.RegisterAsync(request, 2));

        [HttpPost("RegisterHost")]
        public async Task<IActionResult> RegisterHost([FromBody] CreateAcc request)
            => Ok(await _loginService.RegisterAsync(request, 3));

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CreateAcc request)
            => Ok(await _loginService.RegisterAsync(request, 4));

        [HttpPost("LoginWithGoogleSeller")]
        public async Task<IActionResult> LoginWithGoogleSeller([FromBody] GoogleLoginDTO request)
            => Ok(await _loginService.LoginWithGoogleAsync(request, 2));

        [HttpPost("LoginWithGoogleHost")]
        public async Task<IActionResult> LoginWithGoogleHost([FromBody] GoogleLoginDTO request)
            => Ok(await _loginService.LoginWithGoogleAsync(request, 3));

        [HttpPost("LoginWithGoogleCustomer")]
        public async Task<IActionResult> LoginWithGoogleCustomer([FromBody] GoogleLoginDTO request)
            => Ok(await _loginService.LoginWithGoogleAsync(request, 4));

        [HttpPost("LoginWithFacebookSeller")]
        public async Task<IActionResult> LoginWithFacebookSeller([FromBody] FacebookLoginDTO request)
            => Ok(await _loginService.LoginWithFacebookAsync(request, 2));

        [HttpPost("LoginWithFacebookHost")]
        public async Task<IActionResult> LoginWithFacebookHost([FromBody] FacebookLoginDTO request)
            => Ok(await _loginService.LoginWithFacebookAsync(request, 3));

        [HttpPost("LoginWithFacebookCustomer")]
        public async Task<IActionResult> LoginWithFacebookCustomer([FromBody] FacebookLoginDTO request)
            => Ok(await _loginService.LoginWithFacebookAsync(request, 4));

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO request)
            => Ok(await _loginService.ForgotPasswordAsync(request));

        [HttpPost("ResetPasswordWithToken")]
        public async Task<IActionResult> ResetPasswordWithToken([FromBody] ResetPasswordWithTokenDTO request)
            => Ok(await _loginService.ResetPasswordWithTokenAsync(request));
    }
}


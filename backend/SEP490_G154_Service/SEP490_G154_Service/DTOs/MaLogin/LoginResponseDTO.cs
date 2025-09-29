namespace SEP490_G154_Service.DTOs.MaLogin
{
    public class LoginResponseDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";
        public string Token { get; set; } = string.Empty;
    }
}

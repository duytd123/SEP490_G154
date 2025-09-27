namespace SEP490_G154_Service.DTOs
{
    public class LoginResponseDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}

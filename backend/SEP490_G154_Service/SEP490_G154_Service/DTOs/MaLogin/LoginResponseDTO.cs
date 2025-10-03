namespace SEP490_G154_Service.DTOs.MaLogin
{
    public class LoginResponseDTO
    {
        public bool Success { get; set; } = true;       
        public string Message { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "Customer";
        public string Token { get; set; } = string.Empty;
    }
}

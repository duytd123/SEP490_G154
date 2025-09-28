namespace SEP490_G154_Service.DTOs
{
    public class ResetPasswordWithTokenDTO
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}

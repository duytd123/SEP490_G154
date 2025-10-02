namespace SEP490_G154_Service.DTOs.MaLogin
{
    public class ResetPasswordWithTokenDTO
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}

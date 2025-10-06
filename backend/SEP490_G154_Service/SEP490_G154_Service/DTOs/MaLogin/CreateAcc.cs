using System.ComponentModel.DataAnnotations;

namespace SEP490_G154_Service.DTOs.MaLogin
{
    public class CreateAcc
    {
        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [RegularExpression(
            @"^[a-zA-Z0-9._%+-]+@(gmail\.com|fpt\.edu\.vn)$",
            ErrorMessage = "Email phải kết thúc bằng @gmail.com hoặc @fpt.edu.vn.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).+$",
            ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ hoa, 1 chữ thường và 1 ký tự đặc biệt.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
        [MinLength(3, ErrorMessage = "Họ tên phải có ít nhất 3 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(
            @"^(0|\+84)(3|5|7|8|9)[0-9]{8}$",
            ErrorMessage = "Số điện thoại không hợp lệ. Vui lòng nhập đúng định dạng Việt Nam (10 số, bắt đầu bằng 0 hoặc +84).")]
        public string Phone { get; set; } = string.Empty;
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace SalesManagement.Api.Dtos
{
    public record RegisterRequest(
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        string Email,

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        string Password,

        [Required(ErrorMessage = "Tên không được để trống")]
        string Name,

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        string Phone
    );

    public record LoginRequest(
        [Required(ErrorMessage = "Email không được để trống")]
        string Email,
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        string Password
    );

    public record AuthResponse(
        string AccessToken,
        string RefreshToken
    );

    public record RegisterResponse(
        Guid Id,
        string Email,
        string Name,
        string TenantCode
    );
}

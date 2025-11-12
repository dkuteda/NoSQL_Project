using System.ComponentModel.DataAnnotations;

namespace NoSQL_Project.ViewModels
{
	public class ResetPasswordViewModel
	{
		[Required(ErrorMessage = "Email address is requiered.")]
		[EmailAddress(ErrorMessage ="Please, enter the correct emaild address.")]

		public string Email { get; set; } = null!;

		[Required(ErrorMessage = "Password is requiered.")]
		[DataType(DataType.Password, ErrorMessage = "Invalid password format.")]

		public string Password { get; set; } = null!;

		[Required(ErrorMessage = "Please, confirm your password.")]
		[DataType(DataType.Password, ErrorMessage = "Invalid password format.")]
		[Display(Name = "Confirm password.")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]

		public string ConfirmPassword { get; set; } = null!;

		[Required(ErrorMessage = "Password reset token is requiered")]
		public string Token { get; set; } = null!;
	}
}

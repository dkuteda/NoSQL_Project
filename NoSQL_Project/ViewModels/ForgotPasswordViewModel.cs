using System.ComponentModel.DataAnnotations;

namespace NoSQL_Project.ViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required(ErrorMessage = "Please enter your email address.")]
		[EmailAddress(ErrorMessage = "The email address is not valid")]
		public string Email { get; set; } = null!;
	}
}


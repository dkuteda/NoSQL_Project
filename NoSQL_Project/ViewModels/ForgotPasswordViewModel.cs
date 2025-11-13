using System.ComponentModel.DataAnnotations;

namespace NoSQL_Project.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}


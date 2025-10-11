namespace NoSQL_Project.Models;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

	public class LoginModel
	{
		
        [Required(ErrorMessage = "First name is required.")]  
          public string FirstName{ get; set; } = string.Empty;

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = string.Empty;
	}


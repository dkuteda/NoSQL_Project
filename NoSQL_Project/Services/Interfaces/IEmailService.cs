namespace NoSQL_Project.Services.Interfaces
{
	public interface IEmailService
	{
		Task SendPasswordResetEmailAsync(string toEmail, string firstName, string resetLink);
	}
}

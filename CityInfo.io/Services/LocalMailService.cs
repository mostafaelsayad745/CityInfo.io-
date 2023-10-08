namespace CityInfo.io.Services
{
    public class LocalMailService : IMailService
    {
        
        private readonly string _mailTo = "admin@mycompany.com";
        private readonly string _mailfrom = "moreply@mycompany.com";

        public LocalMailService(IConfiguration configuration)
        {
            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailfrom = configuration["mailSettings:mailFromAddress"];
            
        }

        public void send(string subject, string message)
        {
            // send mail 0output to console window 
            Console.WriteLine($"mail from {_mailfrom} to {_mailTo}, " +
                $"with {nameof(LocalMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}

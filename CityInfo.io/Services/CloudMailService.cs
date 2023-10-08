namespace CityInfo.io.Services
{
    public class CloudMailService : IMailService
    {
       
        private readonly string _mailTo = string.Empty;
        private readonly string _mailfrom = string.Empty;

        public CloudMailService(IConfiguration configuration)
        {
            _mailTo = configuration["mailSettings:mailToAddress"];
            _mailfrom = configuration["mailSettings:mailFromAddress"];
        }

        public void send(string subject, string message)
        {
            // send mail 0output to console window 
            Console.WriteLine($"mail from {_mailfrom} to {_mailTo}, " +
                $"with {nameof(CloudMailService)}.");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }

}

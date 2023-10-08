namespace CityInfo.io.Services
{
    public interface IMailService
    {
        void send(string subject, string message);
    }
}
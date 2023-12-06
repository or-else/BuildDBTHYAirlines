using Microsoft.Extensions.Configuration;

namespace ConfigurationRegistry
{
    public class ActiveMQSettings
    {
        public string CorrelationID { get; set; }
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ReconnectionInterval { get; set; }
    }


    public interface IStartup
    {
        static dynamic _LoaderUI;
        static IStartup? _startup = null;
        static IConfiguration? configuration;
        static IServiceProvider? provider;

        public IServiceProvider? Provider => provider;
        public IConfiguration? Configuration => configuration;
        public dynamic? LoaderUI => _LoaderUI;

        public void SetProvider(IServiceProvider ServiceProvider);
        

    }
}
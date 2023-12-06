//using IFace;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ConfigurationRegistry
{
   

    public class StartupContext : IStartup
    {

        public static string ARNName { get; set; }
        public static string LatestDBZFile { get; set; }
        public static string Exception { get; set; }
        public static bool ExceptionOccurred { get; set; }
        public static string FolderContents { get; set; }


        private static dynamic _LoaderUI;

        private static IStartup _startup;
        private static IConfiguration? _configuration;
        private static IServiceProvider _provider;
        private static IConfigurationSetter _configSetter;
        private static Object _mainForm;
        private static ActiveMQSettings _activeMQSettings;

        public static dynamic? LoaderUI => _LoaderUI;
        public static IServiceProvider? Provider => _provider;
        public static IConfiguration? Configuration => _configuration;
        public static IConfigurationSetter? ConfigSetter => _configSetter;

        public static Object? MainForm => _mainForm;

        private static string _environment { get; set; }


        public StartupContext()
        {

            if (_startup == null)
            {
                StartupContext._configSetter = new ConfigurationSetter();

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                _environment = environment;
                StartupContext._configSetter.SetEnvironment(environment);

                StartupContext._configuration = new ConfigurationBuilder().SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName).AddJsonFile
                    ("appsettings.json", optional: false, reloadOnChange: true).AddJsonFile($"appsettings.{environment}.json",
                    optional: true).AddEnvironmentVariables().Build();


                _startup = this;
               
            }

        }

        public static void ReloadConfiguration()
        {
            if (!string.IsNullOrEmpty(_environment))
            {
                StartupContext._configuration = null;

                StartupContext._configuration = new ConfigurationBuilder().SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName).AddJsonFile
                    ("appsettings.json", optional: false, reloadOnChange: true).AddJsonFile($"appsettings.{_environment}.json",
                    optional: true).AddEnvironmentVariables().Build();
            }
        }

        public  void SetProvider(IServiceProvider ServiceProvider)
        {
            StartupContext._provider = ServiceProvider;
        }
       

        public static void SetMainForm(Object MainFormContext)
        {
            StartupContext._mainForm = MainFormContext;
        }

        public static void SetLoaderUI(dynamic Loader)
        {
            StartupContext._LoaderUI = Loader;
        }


        public static ActiveMQSettings GetActiveMQConnectionDetails()
        {
            var logger = StartupContext.Provider.GetRequiredService<IRegistryLoggingProvider<StartupContext>>();

            try
            {
            
                if (StartupContext._activeMQSettings == null)
                {
                    ActiveMQSettings settings = new ActiveMQSettings()
                    {
                        Url = String.Empty,
                        UserName = String.Empty,
                        Password = String.Empty,
                        ReconnectionInterval = String.Empty,
                        CorrelationID = String.Empty

                    };


                    if (Configuration.GetValue<bool>("UseActiveMQSecret") == true)
                    {

                        string _secretString = Configuration.GetSection("ActiveMQ-Secret").GetValue<string>("Secret");

                       
                        logger.LogInformation($"AppSettings-Startup: SecretManager lookup {_secretString}");
                       
                       
                       
                        //logger.LogInformation($"AppSettings-Startup: SecretManager found connection {"ssl://b-b272c4ac-770c-4382-8bcf-c81203a48927-1.mq.us-east-1.amazonaws.com:61617"}");

                        //settings.Url = "ssl://b-b272c4ac-770c-4382-8bcf-c81203a48927-1.mq.us-east-1.amazonaws.com:61617"; 
                        //settings.UserName = "ambAmqUserdev";
                        //settings.Password = "aD3t0b@Vv?L3!CQ_@#0Y?U;{CO1u#K\"V";


                        settings.ReconnectionInterval = "5";
                    }
                    else
                    {
                        settings.Url = Configuration.GetSection("ActiveMQ").GetValue<string>("Url");
                        settings.UserName = Configuration.GetSection("ActiveMQ").GetValue<string>("UserName");
                        settings.Password = Configuration.GetSection("ActiveMQ").GetValue<string>("Password");
                        settings.ReconnectionInterval = Configuration.GetSection("ActiveMQ").GetValue<string>("ReconnectionInterval");
                        settings.CorrelationID = Configuration.GetSection("ActiveMQ").GetValue<string>("CorrelationID");
                    }

                    StartupContext._activeMQSettings = settings;
                }
            }
            catch(Exception ex)
            {
                string message = "AppSettings Exception Starting Service"; 
               
                logger.LogException(ex, $"AppSettings-Startup: {message}:{ex.Message} at {ex.StackTrace}",null);

            }

            return StartupContext._activeMQSettings;
        }

    }

}

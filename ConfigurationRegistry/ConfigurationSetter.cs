using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationRegistry
{
    public class ConfigurationSetter : IConfigurationSetter
    {        
        private string _environment { get; set; }

        // AddOrUpdateAppSetting("Registry:MessageServer","InQueue");
        //format for key,value when making add/update to appsetting
        //compare with base in appsettings.json
        public void AddOrUpdateAppSetting<T>(string key, T value)
        {
            try
            {                   
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMNET") ?? _environment;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"appsettings.{environment}.json");                
                string json = File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                var sectionPath = key.Split(":")[0];

                if (!string.IsNullOrEmpty(sectionPath))
                {
                    var keyPath = key.Split(":")[1];
                    jsonObj[sectionPath][keyPath] = value;
                }
                else
                {
                    jsonObj[sectionPath] = value; // if no sectionpath just set the value
                }

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);


                this.AddOrUpdateAppSettingRuntime<T>(key, value);

               
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }

        private void AddOrUpdateAppSettingRuntime<T>(string key, T value)
        {
            try
            {

                //new ConfigurationBuilder().SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)

                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMNET") ?? _environment;
                var filePath = Path.Combine(Directory.GetParent(AppContext.BaseDirectory).FullName, $"appsettings.{environment}.json");
                string json = File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                var sectionPath = key.Split(":")[0];

                if (!string.IsNullOrEmpty(sectionPath))
                {
                    var keyPath = key.Split(":")[1];
                    jsonObj[sectionPath][keyPath] = value;
                }
                else
                {
                    jsonObj[sectionPath] = value; // if no sectionpath just set the value
                }

                string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);

                StartupContext.ReloadConfiguration();

            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
            
        }


        public void SetEnvironment(string Environment)
        {
            this._environment = Environment;
        }

    }
}

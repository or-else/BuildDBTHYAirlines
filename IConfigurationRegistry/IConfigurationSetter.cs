using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationRegistry
{
    public interface IConfigurationSetter
    {

        
        public void AddOrUpdateAppSetting<T>(string key, T value);
        public void SetEnvironment(string Environment);
    }
}

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationRegistry
{
    public class SETestServices : ISETestServices
    {
        IConfiguration configuration;
        public SETestServices(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void StartTest()
        {
            //var value = configuration.GetSection("Registry").GetValue<string>("MessageServer");
            //Console.WriteLine(string.Format("Value of Registry value of property X: {0}", value)); 
        }
    }
}

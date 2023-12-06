using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigurationRegistry
{
    public class SecretResource
    {
        public Dictionary<string,string>? Data { get; set; }
        public string? Username  { 
            get
            {
                if (Data != null && Data.ContainsKey("username"))
                {
                    return Data["username"];
                }
                else
                {
                    return null;
                }
            } 
        }
        public string? Secret  {
            get
            {
                if (Data != null && Data.ContainsKey("password"))
                {
                    return Data["password"];
                }
                else
                {
                    return null;
                }
            }
        }
        public string? Connection
        {
            get
            {
                if (Data != null && Data.ContainsKey("connection"))
                {
                    return Data["connection"];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
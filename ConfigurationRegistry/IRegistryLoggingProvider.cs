using System;
using System.Collections.Generic;
using System.Text;


namespace ConfigurationRegistry
{
    public interface IRegistryLoggingProvider<T> where T : class
    {
        void LogInformation(string message);
        void LogInformation(string message, Dictionary<string, dynamic> attributes);
        
        void LogException(System.Exception ex, string actionName, Dictionary<string, dynamic> attributes);
        void LogException(System.Exception ex, string actionName);

        void LogError(string actionName, Dictionary<string, dynamic> attributes);
        void LogError(string actionName);
        void LogDebug(string message, Dictionary<string, dynamic> attributes);
        void LogDebug(string message);
    }
}

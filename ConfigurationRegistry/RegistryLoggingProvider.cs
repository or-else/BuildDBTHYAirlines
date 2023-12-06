using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Text;


namespace ConfigurationRegistry
{
    public class RegistryLoggingProvider<T> : IRegistryLoggingProvider<T> where T : class
    {
        private readonly RegistryLoggerWrapper<T> _logger;
		private const string Description = "Description";
        private const string Exception = "Exception";
        private const string Message = "Message";
        private const string ExceptionMessage = "ExceptionMessage";
        private const string ActionName = "ActionName";
        private const string Severity = "Severity";
        private const string TrackingID = "TrackingID";
        private const string LogType = "LogType";
        private const string Error = "Error";

		
        public RegistryLoggingProvider(RegistryLoggerWrapper<T> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
           LogInformation(null, message, null);
        }

        public void LogInformation(string actionName, string message, string description = "")
        {
            using (LogContext.PushProperty(Description, description))
            using (LogContext.PushProperty(ActionName, actionName))
            using (LogContext.PushProperty(Message, message))
            {
                _logger.LogInformation(message);
            }
        }

        public void LogException(Exception ex, string actionName, string message, string trackingId)//, ErrorLevel severity)
        {
            using (LogContext.PushProperty(Description, Exception))
            using (LogContext.PushProperty(ActionName, actionName))
            //using (LogContext.PushProperty(Severity, severity))
            using (LogContext.PushProperty(TrackingID, trackingId))
            using (LogContext.PushProperty(ExceptionMessage, ex.Message))
            using (LogContext.PushProperty(LogType, Error))
            {
                _logger.LogError(ex, actionName + " " + message);
            }
        }

        public void LogException(Exception ex, string actionName)
        {
            using (LogContext.PushProperty(Description, Exception))
            using (LogContext.PushProperty(ActionName, actionName))
            //using (LogContext.PushProperty(Severity, ErrorLevel.Error))
            using (LogContext.PushProperty(ExceptionMessage, ex.Message))
            using (LogContext.PushProperty(LogType, Error))
            {
                _logger.LogError(ex, actionName + " " + ex.Message);
            }
        }

        public void LogException(Exception ex, string actionName, Dictionary<string, dynamic> attributes)
        {
            _logger.LogError(ex, actionName, attributes);
        }

      
        public void LogError(string actionName, string message, string trackingId)
        {
            using (LogContext.PushProperty(Description, Exception))
            using (LogContext.PushProperty(ActionName, actionName))
            using (LogContext.PushProperty(ExceptionMessage, message))
            using (LogContext.PushProperty(LogType, Error))
            using (LogContext.PushProperty(TrackingID, trackingId))
            {
                _logger.LogError(null, message);
            }
        }

        public void LogError(string actionName, Dictionary<string, dynamic> attributes)
        {
            _logger.LogError(null,actionName, attributes);
        }

        public void LogError(string actionName)
        {
            LogError(actionName, null);
        }

        public void LogInformation(string message, Dictionary<string, dynamic> attributes)
        {
            _logger.LogInformation(message, attributes);
        }
        public void LogDebug(string message, Dictionary<string, dynamic> attributes)
        {
            _logger.LogDebug(message, attributes);
        }
        public void LogDebug(string message)
        {
            _logger.LogDebug(message, null);
        }

       
    }
}

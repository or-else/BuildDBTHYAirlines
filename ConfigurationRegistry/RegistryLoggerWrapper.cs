using Microsoft.Extensions.Logging;
using Serilog.Context;
using Serilog.Core.Enrichers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigurationRegistry
{
    public class RegistryLoggerWrapper<T> where T : class
    {
        private readonly ILogger<T> _logger;
        public RegistryLoggerWrapper(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogError(Exception ex, string message)
        {
            _logger.LogError(ex, "{message}", message);
        }

        public void LogError(Exception ex, string message, Dictionary<string, dynamic> attrs)
        {
            if (attrs != null && attrs.Any())
            {
                var propertyEnrichers = GetPropertyEnrichers(attrs);
                using (LogContext.Push(propertyEnrichers))

                    _logger.LogError(ex, message, attrs);
                return;
            }
            _logger.LogError(ex, message, attrs);
        }



        private PropertyEnricher[] GetPropertyEnrichers(Dictionary<string, dynamic> attrs)
        {
            var enrichers = new PropertyEnricher[attrs.Count];
            var index = 0;
            foreach(var attr in attrs)
            {
                enrichers[index] = new PropertyEnricher(attr.Key, attr.Value);
                index += 1;
            }
            return enrichers;
        }

        public void LogInformation(string message, Dictionary<string, dynamic> attrs)
        {
            if (attrs != null && attrs.Any())
            {
                var propertyEnrichers = GetPropertyEnrichers(attrs);
                using (LogContext.Push(propertyEnrichers))

                    _logger.LogInformation("{message}", message);
                return;
            }
            _logger.LogInformation("{message}", message);
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation("{message}", message);
        }
        public void LogDebug(string message, Dictionary<string, dynamic> attrs)
        {
            _logger.LogDebug(message + "{@propertyValues}", attrs);
        }
    }
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomLogger.Services
{
    public class DebugMailService : IMailService
    {
        private ILogger<DebugMailService> _logger;

        public DebugMailService(ILogger<DebugMailService> logger)
        {
            _logger = logger;
        }

        public Task<bool> SendMailAsync(string name, string email, List<string> ccEmails, string subject, string msg)
        {
            string ccList = string.Join(";", ccEmails);
            _logger.LogInformation($"[DebugMailService] Mail Sending to {name}/{email}, cc to {ccList} for {subject}: {Environment.NewLine}{msg}");
            return Task.FromResult(true);
        }
    }
}
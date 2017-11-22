using IP_Restriction.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace IP_Restriction.Middleware
{
    public class IpRestrictionMiddleware
    {
        public readonly RequestDelegate Next;
        public readonly ILogger<IpRestrictionMiddleware> Logger;
        public readonly IpSecuritySettings IpSecuritySettings;

        public IpRestrictionMiddleware(RequestDelegate next, ILogger<IpRestrictionMiddleware> logger, IOptions<IpSecuritySettings> ipSecuritySettings)
        {
            Next = next;
            Logger = logger;
            IpSecuritySettings = ipSecuritySettings.Value;
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = (string)context.Connection.RemoteIpAddress?.ToString();
            if (!IpSecuritySettings.AllowedIPsList.Contains(ipAddress))
            {
                context.Response.StatusCode = 403;
                Logger.LogInformation($"Forbidden Request from Remote IP address: {ipAddress}");
                return;
            }

            await Next(context);
        }
    }
}

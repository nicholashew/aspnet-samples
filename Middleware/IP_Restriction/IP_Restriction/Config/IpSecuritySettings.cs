using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IP_Restriction.Config
{
    public class IpSecuritySettings
    {
        public string AllowedIPs { get; set; }

        public string[] AllowedIPsList
        {
            get
            {
                return AllowedIPs
                    .Split(new Char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(s => s.Trim())
                    .ToArray();
                //return Regex.Split(AllowedIPs.Trim(), @"\s*[,;]\s*");
                //return !string.IsNullOrEmpty(AllowedIPs) ? AllowedIPs.Split(',').ToList() : new List<string>(); 
            }
        }
    }
}
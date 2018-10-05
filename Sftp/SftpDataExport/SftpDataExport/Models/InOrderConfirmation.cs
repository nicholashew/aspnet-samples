using System;

namespace SftpDataExport.Models
{
    [Serializable]
    public class InOrderConfirmation
    {
        public string SubscriberKey { get; set; }

        public string OrderNumber { get; set; }

        public string OrderXML { get; set; }
    }
}

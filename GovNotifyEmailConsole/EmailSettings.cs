using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GovNotifyEmailConsole
{
    internal class EmailSettings
    {
        public string TestTemplateId { get; init; }
        public string DeliveryStatusToken { get; init; }
        public string SupportEmailAddress { get; init; }
        public string GovNotifyApiKey { get; init; }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

namespace PowerBiService.Models;
internal class PcaOption : PublicClientApplicationOptions
{
    public string ClientId;
    public string TenantId;
    public string RedirectUri;

    public PcaOption(IConfigurationRoot? configuration)
    {
        ClientId = configuration["ClientId"];
        TenantId = configuration["TenantId"];
        RedirectUri = configuration["RedirectUri"];
    }
}

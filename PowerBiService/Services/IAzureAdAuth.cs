using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api;
using PowerBiService.Models;

namespace PowerBiService.Services;
public interface IAzureAdAuth
{
    Task<PowerBIClient> AuthenticateAsync();
}

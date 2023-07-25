using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBiService.Services;
public interface ISecretManager
{
    Task<String> GetSecret(string secretName);
}

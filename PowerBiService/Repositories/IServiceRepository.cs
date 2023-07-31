using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBiService.Repositories
{
    public interface IServiceRepository
    {
        Task InvokeServiceAsync();
    }
}

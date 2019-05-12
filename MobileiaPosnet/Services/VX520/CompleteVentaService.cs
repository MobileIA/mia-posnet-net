using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileiaPosnet.Services.VX520
{
    public interface CompleteVentaService
    {
        void onCompleteSell(string code, string message, string client, string card);
    }
}

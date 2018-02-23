using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileiaPosnetUWP.Services.VX520
{
    public class ServiceVX520 : Service
    {
        public ServiceVx520(CompleteService listener) : base(listener)
        {

        }

        public override uint BytesToRead()
        {
            throw new NotImplementedException();
        }

        public override string WriteData(string hex)
        {
            throw new NotImplementedException();
        }
    }
}

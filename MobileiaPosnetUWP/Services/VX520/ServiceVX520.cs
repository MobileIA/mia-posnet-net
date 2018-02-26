using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileiaPosnetUWP.Services.VX520
{
    public class ServiceVX520 : Service
    {
        public ServiceVX520(CompleteService listener) : base(listener)
        {

        }

        public ServiceVX520() { }

        public override uint BytesToRead()
        {
            throw new NotImplementedException();
        }

        public override bool IsWaitingResponse(string hex)
        {
            throw new NotImplementedException();
        }

        public override string WriteData(string hex)
        {
            throw new NotImplementedException();
        }
    }
}

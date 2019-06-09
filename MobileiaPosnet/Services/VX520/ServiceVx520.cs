using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileiaPosnet.Services.VX520
{
    public class ServiceVx520 : Service
    {
        public ServiceVx520(CompleteService listener) : base(listener)
        {

        }

        public ServiceVx520() { }

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

        public string SumarXOR(String hex)
        {
            string[] hexArray = hex.Split(' ');
            int result = 0;
            for (int i = 0; i < hexArray.Length; i++)
            {
                string h = hexArray[i];
                if (i == 0)
                {
                    result = Convert.ToInt32(h, 16);
                    continue;
                }
                int dec = Convert.ToInt32(h, 16);
                result = result ^ dec;
            }
            return result.ToString("X");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileiaPosnet.Services.VX520
{
    public class TestService : ServiceVx520
    {
        public TestService(CompleteService listener) : base(listener)
        {
            _totalStep = 3;
            _command = "54 45 53";
        }

        public override string WriteData(string hex)
        {
            // Inicia el servicio
            if (_numStep == 0)
            {
                _numStep++;
                return StartCommand();
            }else if (_numStep == 1)
            {
                if (hex.CompareTo("06") == 0)
                {
                    _numStep++;
                    return "02 54 45 53 00 00 03 41";
                }
            }else if (_numStep == 2)
            {
                if (hex.CompareTo("06 ") == 0)
                {
                    _numStep++;
                    return "06";
                }
            }else if (_numStep == 3)
            {
                if (hex.CompareTo("02 54 45 53 30 30 31 00 00 03 70 ") == 0)
                {
                    _numStep++;
                    _listener.complete(true, "");
                    return "06";
                }
            }

            return null;
        }
    }
}

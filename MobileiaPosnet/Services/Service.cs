using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileiaPosnet.Services
{
    public class Service
    {
        /// <summary>
        /// Representa en que estado esta el servicio.
        /// </summary>
        protected int _numStep = 0;
        /// <summary>
        /// Determina el numero total de pasos para ejecurtar elservicio
        /// </summary>
        protected int _totalStep = 0;
        /// <summary>
        /// Almacena el escuchador para cuando termina de ejecutarse el servicio
        /// </summary>
        protected CompleteService _listener;

        public Service(CompleteService listener)
        {
            _listener = listener;
        }

        public string WriteData(string hex)
        {
            return "";
        }
       
    }
}

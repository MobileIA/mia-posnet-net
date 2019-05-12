using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileiaPosnet.Services
{
    abstract public class Service
    {
        /// <summary>
        /// Representa en que estado esta el servicio.
        /// </summary>
        protected int _numStep = 0;
        /// <summary>
        /// Determina el numero total de pasos para ejecutar el servicio
        /// </summary>
        protected int _totalStep = 0;
        /// <summary>
        /// Almacena el HEX del comando a utilizar
        /// </summary>
        protected string _command = "";
        /// <summary>
        /// Almacena el escuchador para cuando termina de ejecutarse el servicio
        /// </summary>
        protected CompleteService _listener;

        public Service(CompleteService listener)
        {
            _listener = listener;
        }

        public Service() { }

        abstract public string WriteData(string hex);

        abstract public uint BytesToRead();

        abstract public Boolean IsWaitingResponse(string hex);

        /// <summary>
        /// Funcion que devuelve el parametro de inicio del servicio
        /// </summary>
        /// <returns></returns>
        protected string StartCommand()
        {
            return "05";
        }
        /// <summary>
        /// Funcion que convierte un string en Hexadecimal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected string StringToHex(string input)
        {
            string result = "";
            char[] values = input.ToCharArray();
            foreach (char letter in values)
            {
                // Get the integral value of the character.
                int value = Convert.ToInt32(letter);
                // Convert the decimal value to a hexadecimal value in string form.
                string hexOutput = String.Format("{0:X}", value);
                result += hexOutput + " ";
            }
            return result;
        }
        public static string FromHex(string hex)
        {
            hex = hex.Replace("-", "");
            hex = hex.Replace(" ", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return Encoding.ASCII.GetString(raw);
        }
    }
}

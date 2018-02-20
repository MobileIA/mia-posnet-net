using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using MobileiaPosnet.Services;

namespace MobileiaPosnet
{
    public class Posnet
    {
        /// <summary>
        /// Almacena instancia del puerto
        /// </summary>
        private SerialPort comPort = new SerialPort();

        public Posnet()
        {
            // Seteamos la configuracion por defecto del puerto
            comPort.BaudRate = int.Parse(_baudRate);
            comPort.DataBits = int.Parse(_dataBits);
            comPort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits);
            comPort.Parity = (Parity)Enum.Parse(typeof(Parity), _parity);
            comPort.PortName = _portName;
            // asignamos el evento para recibir la informacion desde el puerto
            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }
        /// <summary>
        /// Funcion que se encarga de ejecutar un servicio
        /// </summary>
        /// <param name="service"></param>
        public void ExecuteService(Service service)
        {

        }

        /// <summary>
        /// Funcion que se encarga de abrir el puerto
        /// </summary>
        /// <returns></returns>
        public bool OpenPort()
        {
            try
            {
                //Verificar si el puerto esta abierto y cerrarlo
                if (comPort.IsOpen == true) {
                    comPort.Close();
                }
                // Abrir puerto
                comPort.Open();
                Console.WriteLine("Puerto abierto a las: " + DateTime.Now);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo abrir el puerto, error: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Funcion que se encarga de cerrar el puerto
        /// </summary>
        /// <returns></returns>
        public bool ClosePort()
        {
            try
            {
                //Verificar si el puerto esta abierto y cerrarlo
                if (comPort.IsOpen == true)
                {
                    comPort.Close();
                    Console.WriteLine("Se ha cerrado el puerto");
                }
                else
                {
                    Console.WriteLine("El puerto ya se encontraba cerrado");
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Se ha producido un error al intentar cerrar el puerto: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// metodo que se llama cuando se recibe informacion del puerto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }

        // Variables privadas estaticas
        private static string _baudRate = string.Empty;
        private static string _parity = string.Empty;
        private static string _stopBits = string.Empty;
        private static string _dataBits = string.Empty;
        private static string _portName = string.Empty;
        /// <summary>
        /// Metodo que guarda la configuracion de la conexion
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        /// <param name="dataBits"></param>
        public static void Config(string portName, string baudRate, string parity, string stopBits, string dataBits)
        {
            _portName = portName;
            _baudRate = baudRate;
            _parity = parity;
            _stopBits = stopBits;
            _dataBits = dataBits;
        }
    }
}

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
        /// <summary>
        /// Almacena el servicio que se esta ejecutando
        /// </summary>
        private Service currentService;

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
            // Almacenamos el servicio
            currentService = service;
            // Abrir puerto
            OpenPort();
            // Enviar parametros
            comPort_SendData(currentService.WriteData(""));
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
        /// Funcion que se encarga de enviar informacion al puerto
        /// </summary>
        /// <param name="msg"></param>
        private void comPort_SendData(string msg)
        {
            // Verificar si el mensaje no es nulo
            if (msg == null||msg.Length==0)
            {
                // Cerrar puerto
                ClosePort();
                return;
            }

            try
            {
                //convert the message to byte array
                byte[] newMsg = HexToByte(msg);
                //send the message to the port
                comPort.Write(newMsg, 0, newMsg.Length);
                //convert back to hex and display
                string hex = ByteToHex(newMsg);
                comPort_SendData(currentService.WriteData(hex));
                Console.WriteLine(hex);
            }
            catch (FormatException ex)
            {
                //display error message
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se llama cuando se recibe informacion del puerto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //retrieve number of bytes in the buffer
            int bytes = comPort.BytesToRead;
            //create a byte array to hold the awaiting data
            byte[] comBuffer = new byte[bytes];
            //read the data and store it
            comPort.Read(comBuffer, 0, bytes);
            //display the data to the user
            Console.WriteLine(ByteToHex(comBuffer));
        }

        #region HexToByte
        /// <summary>
        /// method to convert hex string into a byte array
        /// </summary>
        /// <param name="msg">string to convert</param>
        /// <returns>a byte array</returns>
        private byte[] HexToByte(string msg)
        {
            //remove any spaces from the string
            msg = msg.Replace(" ", "");
            //create a byte array the length of the
            //divided by 2 (Hex is 2 characters in length)
            byte[] comBuffer = new byte[msg.Length / 2];
            //loop through the length of the provided string
            for (int i = 0; i < msg.Length; i += 2)
                //convert each set of 2 characters to a byte
                //and add to the array
                comBuffer[i / 2] = (byte)Convert.ToByte(msg.Substring(i, 2), 16);
            //return the array
            return comBuffer;
        }
        #endregion

        #region ByteToHex
        /// <summary>
        /// method to convert a byte array into a hex string
        /// </summary>
        /// <param name="comByte">byte array to convert</param>
        /// <returns>a hex string</returns>
        private string ByteToHex(byte[] comByte)
        {
            //create a new StringBuilder object
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            //loop through each byte in the array
            foreach (byte data in comByte)
                //convert the byte to a string and add to the stringbuilder
                builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
            //return the converted value
            return builder.ToString().ToUpper();
        }
        #endregion

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

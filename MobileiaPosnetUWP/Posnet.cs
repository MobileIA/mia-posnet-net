using MobileiaPosnetUWP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace MobileiaPosnetUWP
{
    public class Posnet
    {
        /// <summary>
        /// El dispositivo conectado
        /// </summary>
        private SerialDevice serialDevice;
        /// <summary>
        /// Objeto usado par recibir la informacion del puerto
        /// </summary>
        private DataReader dataReaderObject;
        /// <summary>
        /// Servicio para enviar datos al puerto
        /// </summary>
        private DataWriter dataWriterObject;
        /// <summary>
        /// Flag that indicates if COM port is open
        /// </summary>
        public bool IsOpen { get; private set; }
        /// <summary>
        /// Almacena el servicio que se esta ejecutando
        /// </summary>
        private Service currentService;


        /// <summary>
        /// Funcion que se encarga de ejecutar un servicio
        /// </summary>
        /// <param name="service"></param>
        public async void ExecuteService(Service service)
        {
            // Almacenamos el servicio
            currentService = service;
            // Abrir puerto
            await OpenPort();
            // Enviar parametros
            WriteAsync(currentService.WriteData(""));
        }

        public void WriteAsync(string msg)
        {
            byte[] bytes = HexToByte(msg);
            WriteAsync(bytes);
        }

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Write data to the open COM port
        /// </summary>
        /// <param name="data">Array of data byes to be written</param>
        public async void WriteAsync(byte[] data)
        {
            // Write block of data to serial port
            this.dataWriterObject.WriteBytes(data);

            // Transfer data to the serial device now
            await this.dataWriterObject.StoreAsync();

            // Flush the data out to the serial device now
            await this.dataWriterObject.FlushAsync();
        }

        /// <summary>
        /// Read a byte from the open COM port
        /// </summary>
        /// <returns>The next byte received by the specified port</returns>
        public byte ReadByte()
        {
            // Get the next byte of data from the port
            return (byte)this.dataReaderObject.ReadByte();
        }

        /// <summary>
        /// Return the number of bytes to read from COM port
        /// </summary>
        public uint BytesToRead(uint bufferSize)
        {
            // Load buffer of bytes from port
            LoadSerialDataAsync(bufferSize);

            // Total bytes to read are those unconsumed from the port
            return this.dataReaderObject.UnconsumedBufferLength;
        }

        /// <summary>
        /// Load serial data from the serial port
        /// </summary>
        public void LoadSerialDataAsync(uint bufferSize)
        {
            // If no data left to consume in buffer...
            if (dataReaderObject.UnconsumedBufferLength == 0)
            {
                try
                {
                    IAsyncOperation<uint> taskLoad = dataReaderObject.LoadAsync(bufferSize);
                    taskLoad.AsTask().Wait(0);
                }
                // Dump exceptions
                catch { }
            }
        }

        /// <summary>
        /// Funcion que se encarga de abrir el puerto
        /// </summary>
        /// <returns></returns>
        public async Task<bool> OpenPort()
        {
            // Close open port
            ClosePort();

            // Get a device selector from the given port name
            string selector = SerialDevice.GetDeviceSelector(_portName);

            // Get a list of devices that match the given name
            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);

            // If any device found...
            if (devices.Any())
            {
                // Get first device (should be only device)
                DeviceInformation deviceInfo = devices.First();

                // Create a serial port device from the COM port device ID
                this.serialDevice = await SerialDevice.FromIdAsync(deviceInfo.Id);

                // If serial device is valid...
                if (this.serialDevice != null)
                {
                    // Setup serial port configuration
                    this.serialDevice.StopBits = _stopBits;
                    this.serialDevice.Parity = _parity;
                    this.serialDevice.BaudRate = _baudRate;
                    this.serialDevice.DataBits = _dataBits;

                    // Create a single device writer for this port connection
                    this.dataWriterObject = new DataWriter(this.serialDevice.OutputStream);

                    // Create a single device reader for this port connection
                    this.dataReaderObject = new DataReader(this.serialDevice.InputStream);

                    // Allow partial reads of the input stream
                    this.dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;

                    // Port is now open
                    this.IsOpen = true;
                }
            }

            return this.IsOpen;
        }

        /// <summary>
        /// Funcion que se encarga de cerrar el puerto
        /// </summary>
        public void ClosePort()
        {
            // If serial device defined...
            if (this.serialDevice != null)
            {
                // Dispose and clear device
                this.serialDevice.Dispose();
                this.serialDevice = null;
            }

            // If data reader defined...
            if (this.dataReaderObject != null)
            {
                // Detatch reader stream
                this.dataReaderObject.DetachStream();

                // Dispose and clear data reader
                this.dataReaderObject.Dispose();
                this.dataReaderObject = null;
            }

            // If data writer defined...
            if (this.dataWriterObject != null)
            {
                // Detatch writer stream
                this.dataWriterObject.DetachStream();

                // Dispose and clear data writer
                this.dataWriterObject.Dispose();
                this.dataWriterObject = null;
            }

            // Port now closed
            this.IsOpen = false;
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
        private static uint _baudRate = 19200;
        private static SerialParity _parity = SerialParity.None;
        private static SerialStopBitCount _stopBits = SerialStopBitCount.One;
        private static ushort _dataBits = 8;
        private static string _portName = "COM3";
        /// <summary>
        /// Metodo que guarda la configuracion de la conexion
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        /// <param name="dataBits"></param>
        public static void Config(string portName, uint baudRate, SerialParity parity, SerialStopBitCount stopBits, ushort dataBits)
        {
            _portName = portName;
            _baudRate = baudRate;
            _parity = parity;
            _stopBits = stopBits;
            _dataBits = dataBits;
        }
    }
}

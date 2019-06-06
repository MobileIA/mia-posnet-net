using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileiaPosnet.Services.VX520
{
    public class VentaService : ServiceVx520, CompleteService
    {
        protected double _amount;
        protected CompleteVentaService _listenerSell;

        public VentaService(CompleteVentaService listener, double amount)
        {
            _listenerSell = listener;
            _listener = this;
            //setListener(this);
            _amount = amount;
        }

        private string AmountToHex()
        {
            // Convertimos total en string
            string amount = _amount.ToString();
            int startpunto = amount.IndexOf(",");
            if (startpunto > 0)
            {
                string entero = amount.Substring(0, startpunto);
                string decimals = amount.Substring(startpunto + 1);
                if (decimals.Length == 2)
                {
                    amount = entero + decimals;
                }
                else
                {
                    amount = entero + decimals + "0";
                }
            }
            else
            {
                amount = amount + "00";
            }
            // Recorremos hasta completar con 12 digitos
            while (amount.Length != 12)
            {
                amount = "0" + amount;
            }

            return StringToHex(amount);
        }

        private string InvoiceToHex()
        {
            return StringToHex("100000089012");
        }

        private string LocalCodeToHex()
        {
            return StringToHex("14127043       "); // 15
        }

        private string LocalNameToHex()
        {
            return StringToHex("GULCH                  "); // 23
        }

        private string CuitToHex()
        {
            return StringToHex("30-71599725-4          "); // 23
        }

        public override string WriteData(string hex)
        {
            // Inicia el servicio
            if (_numStep == 0)
            {
                _numStep++;
                return StartCommand();
            }
            else if (_numStep == 1)
            {
                if (hex.CompareTo("06 ") == 0)
                {
                    _numStep++;
                    return "02 56 45 4E 68 00 30 30 30 30 30 30 30 30 30 31 30 30 31 30 30 30 30 30 30 38 39 30 31 32 30 31 30 56 49 30 30 30 30 30 30 30 30 30 30 30 30 30 30 33 36 35 39 33 30 37 20 20 20 20 20 20 20 50 52 49 53 4D 41 20 4D 50 20 20 20 20 20 20 20 20 20 20 20 20 20 20 33 30 2D 35 39 38 39 31 30 30 34 2D 35 20 20 20 20 20 20 20 20 20 20 01 03 11";
                    /*return "02 " + // STX
                        "56 45 4E " + // VEN
                        "68 00 " + // LEN tamaño en la cantidad de campos/parametros a enviar en este caso 104bytes
                        AmountToHex() + // Monto de la venta 12 bytes
                        InvoiceToHex() + // Numero de la factura 12bytes
                        "30 31 " + // Cantidad de cuotas: 01
                        "30 56 49 " + // Codigo de tarjeta: VVI (este permite todas las tarjetas)
                        "30 " + // Codigo del plan
                        "30 30 30 30 30 30 30 30 30 30 30 30 " + // Monto de propina
                        LocalCodeToHex() + // Codigo del comercio
                        LocalNameToHex() + // Nomber del comercio
                        CuitToHex() + // CUIT del comercio
                        "01 " + // Si es online o offline
                        "03 11";*/
                }
            }
            else if (_numStep == 2)
            {
                _numStep++;
                return "Waiting";
            }
            else if (_numStep == 3)
            {
                _numStep++;
                // Obtener datos de la transaccion
                _listener.complete(true, hex);
                return "06";
            }
            else if (_numStep == 4)
            {
                _numStep++;
                return "";
            }
            else if (_numStep == 5)
            {
                _numStep++;
                return "";
            }


            return null;
        }

        public override uint BytesToRead()
        {
            if (_numStep == 1)
            {
                return 1;
            }
            else if (_numStep == 2)
            {
                return 1;
            }
            else if (_numStep == 3)
            {
                return 0;
            }

            return 0;
        }

        public override bool IsWaitingResponse(string hex)
        {
            if (_numStep == 1 && hex.CompareTo("06 ") != 0)
            {
                return true;
            }
            else if (_numStep == 2)
            {
                if (hex.IndexOf("03") > 0)
                {
                    return false;
                }
                return true;
            }
            else if (_numStep == 3)
            {
                return false;
            }
            return false;
        }

        public void complete(bool success, string data)
        {
            // Verificar si se obtuvieron los datos correctamente
            if (data.Length < 124)
            {
                // Enviar al listener
                _listenerSell.onCompleteSell("99", "No se pudo conectar con el servidor", "", "");
                return;
            }
            data = data.Replace(" ", "");
            // Procesar informacion y enviar al listener de venta con los parametros
            string start = data.Substring(0, 10 * 2);
            // codigo de respuesta
            string codeResponse = data.Substring(10 * 2, 2 * 2);
            // Mensaje de respuesta
            string messageResponse = data.Substring(12 * 2, 32 * 2);
            // codigo de autorizacion
            string codeAuthorization = data.Substring(44 * 2, 6 * 2);
            // Numero de cupon
            string nroCupon = data.Substring(50 * 2, 7 * 2);
            // numero de lote
            string nroLote = data.Substring(57 * 2, 3 * 2);
            // Nombre del cliente
            string client = data.Substring(60 * 2, 26 * 2);
            // Ultimos 4 digitos de la tarjeta
            string card = data.Substring(86 * 2, 4 * 2);
            // Primeros 6 digitos de la tarjeta
            string firstCard = data.Substring(90 * 2, 6 * 2);
            // Fecha de transaccion
            string date = data.Substring(96 * 2, 10 * 2);
            // hora de la transaccion
            string hour = data.Substring(106 * 2, 8 * 2);
            // Terminal ID
            string terminalId = data.Substring(114 * 2, 8 * 2);
            // Enviar al listener
            _listenerSell.onCompleteSell(FromHex(codeResponse), FromHex(messageResponse), FromHex(client), FromHex(card));
        }
    }
}

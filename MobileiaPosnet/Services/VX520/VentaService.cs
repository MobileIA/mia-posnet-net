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
        protected String paymentMethod = "0VI";

        public VentaService(CompleteVentaService listener, double amount)
        {
            _listenerSell = listener;
            _listener = this;
            //setListener(this);
            _amount = amount;
        }

        public void SetPaymentVisa()
        {
            paymentMethod = "0VI";
        }

        public void SetPaymentVisaDebito()
        {
            paymentMethod = "0EL";
        }

        public void SetPaymentMastercard()
        {
            paymentMethod = "0MC";
        }

        public void SetPaymentMastercard2()
        {
            paymentMethod = "0PD";
        }

        public void SetPaymentMaestro()
        {
            paymentMethod = "0MA";
        }

        public void SetPaymentCabal()
        {
            paymentMethod = "0CA";
        }

        public void SetPaymentAmex()
        {
            paymentMethod = "0AM";
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
            String invoice = "100000089012";
            while (invoice.Length != 12)
            {
                invoice = "0" + invoice;
            }

            return StringToHex(invoice);
            //return StringToHex("100000089012");
        }

        private string LocalCodeToHex()
        {
            String code = Posnet.localCode;
            while (code.Length != 15)
            {
                code = code + " ";
            }
            return StringToHex(code);
        }

        private string LocalNameToHex()
        {
            String name = Posnet.localName;
            while (name.Length != 23)
            {
                name = name + " ";
            }
            return StringToHex(name);
        }

        private string CuitToHex()
        {
            String cuit = Posnet.localCuit;
            while (cuit.Length != 23)
            {
                cuit = cuit + " ";
            }
            return StringToHex(cuit);
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
                    //return "02 56 45 4E 68 00 30 30 30 30 30 30 30 30 30 31 30 30 31 30 30 30 30 30 30 38 39 30 31 32 30 31 30 56 49 30 30 30 30 30 30 30 30 30 30 30 30 30 30 33 36 35 39 33 30 37 20 20 20 20 20 20 20 50 52 49 53 4D 41 20 4D 50 20 20 20 20 20 20 20 20 20 20 20 20 20 20 33 30 2D 35 39 38 39 31 30 30 34 2D 35 20 20 20 20 20 20 20 20 20 20 01 03 11";
                    String codeHex = "56 45 4E " + // VEN
                        "68 00 " + // LEN tamaño en la cantidad de campos/parametros a enviar en este caso 104bytes
                        AmountToHex() + // Monto de la venta 12 bytes
                        InvoiceToHex() + // Numero de la factura 12bytes
                        "30 31 " + // Cantidad de cuotas: 01
                        StringToHex(paymentMethod) + // Codigo de tarjeta: VVI: 30 56 49 (este permite todas las tarjetas) VI: 
                        "30 " + // Codigo del plan
                        "30 30 30 30 30 30 30 30 30 30 30 30 " + // Monto de propina
                        LocalCodeToHex() + // Codigo del comercio
                        LocalNameToHex() + // Nomber del comercio
                        CuitToHex() + // CUIT del comercio
                        "01 " + // Si es online o offline
                        "03";
                    return "02 " + /*STX*/ codeHex + " " + SumarXOR(codeHex);
                }
            }
            else if (_numStep == 2)
            {
                _numStep++;
                if (hex.CompareTo("15 ") == 0)
                {
                    return "06";
                }
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
            // Respuesta Comercion Invalido
            // 02 56 45 4E 30 30 30 70 00 30 33 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 20 30 30 30 30 30 30 30 30 30 30 30 30 33 30 30 31 43 41 4D 49 4C 45 54 54 49 2F 4D 41 54 49 41 53 20 4E 4F 20 20 20 20 20 20 20 39 38 30 30 34 35 34 36 34 30 30 39 2F 30 36 2F 32 30 31 39 31 34 3A 31 38 3A 31 39 31 34 31 32 37 30 34 33 03 75
            // Respuesta cancelacion por el usuario
            if (data.CompareTo("02 56 45 4E 32 30 31 00 00 03 6D ") == 0)
            {
                _listenerSell.onCompleteSell("99", "El usuario ha cancelado", "", "");
                return;
            }
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

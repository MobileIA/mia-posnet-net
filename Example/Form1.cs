using MobileiaPosnet.Services;
using MobileiaPosnet.Services.VX520;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Example
{
    public partial class Form1 : Form, MobileiaPosnet.Services.CompleteService
    {
        public Form1()
        {
            InitializeComponent();
            // Configurar parametros por default
            MobileiaPosnet.Vx520.Config("COM3", "19200", "None", "One", "8");
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            MobileiaPosnet.Vx520 device = new MobileiaPosnet.Vx520();
            device.ExecuteService(new TestService(listener: this));
        }

        void CompleteService.complete(bool success, string data)
        {
            Console.WriteLine("Se completo el servicio");
        }
    }
}

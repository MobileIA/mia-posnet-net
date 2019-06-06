﻿using MobileiaPosnet.Services;
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
    public partial class Form1 : Form, MobileiaPosnet.Services.CompleteService, CompleteVentaService
    {

        MobileiaPosnet.Vx520 device;
        public Form1()
        {
            InitializeComponent();
            // Configurar parametros por default
            MobileiaPosnet.Vx520.Config("COM9", "19200", "None", "One", "8");
            device = new MobileiaPosnet.Vx520();
            device.OpenPort();
        }

        public void onCompleteSell(string code, string message, string client, string card)
        {
            this.Invoke((MethodInvoker) delegate
            {
                label1.Text = "Resultado: " + code + " - " + message + " - " + client + " - " + card;
            });
            
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            double amount = 30;

            
            device.ExecuteService(new VentaService(listener: this, amount: amount));
            //device.ExecuteService(new TestService(listener: this));
        }

        void CompleteService.complete(bool success, string data)
        {
            Console.WriteLine("Se completo el servicio");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            device.ClosePort();
        }
    }
}

using MobileiaPosnetUWP;
using MobileiaPosnetUWP.Services;
using MobileiaPosnetUWP.Services.VX520;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0xc0a

namespace ExampleUWP
{
    /// <summary>
    /// Página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class MainPage : Page, CompleteVentaService
    {
        public MainPage()
        {
            this.InitializeComponent();
            // Configurar parametros por default
            Vx520.Config("COM3", 19200, Windows.Devices.SerialCommunication.SerialParity.None, Windows.Devices.SerialCommunication.SerialStopBitCount.One, 8);
        }

        public void onCompleteSell(string code, string message, string client, string card)
        {
            TextoTest.Text = "Se completo la tarea: " + code + " - " + message + " - " + client + " - " + card;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Vx520 device = new Vx520();
            VentaService service = new VentaService(this, 100);
            device.ExecuteService(service);
        }
    }
}

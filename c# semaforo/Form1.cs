using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;

namespace c__semaforo
{
    public partial class Form1 : Form
    {
        private SerialPort serialPort;
        private Dictionary<string, SensorInfo> sensores;

        public Form1()
        {
            InitializeComponent();
            json();

            serialPort = new SerialPort("COM14", 9600);
            serialPort.DataReceived += SerialPort_DataReceived;

            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Sin abrir puerto: {ex.Message}");
            }
        }

        private void mostrarInfo(string data)
        {
            if (data.Length >= 3)
            {
                string clave = data.Substring(0, 3);
                string info = "";

                if (sensores.ContainsKey(clave))
                {
                    string descripcion = sensores[clave].Descripcion;
                    string unidades = sensores[clave].Unidad;

                    string indice = data.Substring(3, 2);
                    string valor = data.Substring(6);

                    if (clave.StartsWith("LE"))
                    {
                        string estado = valor == "1" ? "Encendido" : "Apagado";
                        info = $"{descripcion} {indice}: {estado}";
                    }
                    else
                    {
                        info = $"{descripcion} {indice}: {valor} {unidades}";
                    }

                    this.Invoke(new Action(() =>
                    {
                        listBox1.Items.Insert(0, info);
                        label1.Text = info;
                    }));
                }
            }
        }

        private void json()
        {
            try
            {
                string json = File.ReadAllText("datos.json");
                sensores = JsonConvert.DeserializeObject<Dictionary<string, SensorInfo>>(json);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el archivo JSON: {ex.Message}");
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadLine().Trim();
            mostrarInfo(data);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }

    public class SensorInfo
    {
        public string Descripcion { get; set; }
        public string Unidad { get; set; }
    }
}
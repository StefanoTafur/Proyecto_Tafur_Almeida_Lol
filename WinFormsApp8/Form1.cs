using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ServerApp
{
    public partial class Form1 : Form
    {
        private TcpListener listener;
        private bool isRunning = true;
        private List<TcpClient> clients = new List<TcpClient>();
        private Dictionary<TcpClient, string> clientNames = new Dictionary<TcpClient, string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Crear y agregar controles al formulario
            TextBox portTextBox = new TextBox() { Top = 10, Left = 10, Width = 100 };
            Button startButton = new Button() { Text = "Iniciar Servidor", Top = 40, Left = 10 };

            startButton.Click += (s, args) =>
            {
                StartServerFromInput(portTextBox, startButton);
            };

            portTextBox.KeyPress += (s, args) =>
            {
                if (args.KeyChar == (char)Keys.Enter)
                {
                    StartServerFromInput(portTextBox, startButton);
                    args.Handled = true;
                }
            };

            this.Controls.Add(portTextBox);
            this.Controls.Add(startButton);

            // Configurar anclajes para hacer el formulario responsivo
            this.listBox1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            portTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            startButton.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
        }

        private void StartServerFromInput(TextBox portTextBox, Button startButton)
        {
            if (int.TryParse(portTextBox.Text, out int port))
            {
                Thread serverThread = new Thread(() => StartServer(port));
                serverThread.Start();
                startButton.Enabled = false;
                portTextBox.Enabled = false;
            }
            else
            {
                MessageBox.Show("No seas burro, ingresa bien el puerto.");
            }
        }

        private void StartServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            UpdateStatus($"Servidor iniciado en el puerto {port}...");

            while (isRunning)
            {
                try
                {
                    var client = listener.AcceptTcpClient();
                    lock (clients)
                    {
                        clients.Add(client);
                    }
                    Thread clientThread = new Thread(HandleClient);
                    clientThread.Start(client);
                }
                catch (SocketException se)
                {
                    UpdateStatus("Error: " + se.Message);
                }
            }
        }

        private void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                string clientName = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                lock (clientNames)
                {
                    clientNames[client] = clientName;
                }
                UpdateStatus(clientName + " se ha conectado.");
                BroadcastMessage(client, clientName + " se ha conectado.");

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (message == "BUZZ")
                    {
                        UpdateStatus(clientName + " envió un zumbido.");
                        BroadcastMessage(client, "BUZZ");
                    }
                    else
                    {
                        var parts = message.Split('|');
                        if (parts.Length == 2)
                        {
                            string timeStamp = parts[0];
                            string clientMessage = parts[1];
                            string formattedMessage = $"({timeStamp}) {clientName}: {clientMessage}";
                            UpdateStatus(formattedMessage);
                            BroadcastMessage(client, formattedMessage);
                        }
                        else
                        {
                            UpdateStatus("Formato de mensaje incorrecto.");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UpdateStatus("Error: " + e.Message);
            }
            finally
            {
                lock (clients)
                {
                    clients.Remove(client);
                }
                string clientName;
                lock (clientNames)
                {
                    clientNames.TryGetValue(client, out clientName);
                    clientNames.Remove(client);
                }
                client.Close();
                UpdateStatus(clientName + " se ha desconectado.");
                BroadcastMessage(client, clientName + " se ha desconectado.");
            }
        }

        private void BroadcastMessage(TcpClient sender, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            lock (clients)
            {
                foreach (var client in clients)
                {
                    if (client != sender)
                    {
                        try
                        {
                            NetworkStream stream = client.GetStream();
                            stream.Write(data, 0, data.Length);
                        }
                        catch (Exception e)
                        {
                            UpdateStatus("Error al enviar mensaje: " + e.Message);
                        }
                    }
                }
            }
        }

        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateStatus), message);
                return;
            }
            listBox1.Items.Add(message);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isRunning = false;
            listener.Stop();
            lock (clients)
            {
                foreach (var client in clients)
                {
                    client.Close();
                }
            }
        }
    }
}

using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace ExamClientWPF
{
    public partial class MainWindow : Window
    {
        private const int Port = 8888;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartExam_Click(object sender, RoutedEventArgs e)
        {
            // Check if the name textbox is empty
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter your name to start the exam.");
                return;
            }

            // Attempt to connect to the server
            try
            {
                TcpClient client= new TcpClient(txtServerIP.Text, Port);

                // Get the network stream
                NetworkStream stream = client.GetStream();

                // Convert the name to bytes
                byte[] nameBytes = Encoding.UTF8.GetBytes(txtName.Text);

                // Send the name to the server
                stream.Write(nameBytes, 0, nameBytes.Length);

                // Start the exam
                ExamWindow examWindow = new ExamWindow(txtName.Text, client);
               
                this.Close();
            }
            catch (Exception ex)
            {
                // Notify the user if an error occurred while connecting
                MessageBox.Show($"An error occurred while connecting to the server: {ex.Message}");
            }
        }
    }
}

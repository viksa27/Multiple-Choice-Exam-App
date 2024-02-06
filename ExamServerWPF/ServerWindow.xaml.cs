using System.Collections.Generic;
using System.Net.Sockets;
using System;
using System.Windows;
using ExamLib;
using System.Net;
using System.Threading;
using System.Text;
using System.Linq;
using System.Text.Json;

namespace ExamServerWPF
{
    public partial class ServerWindow : Window
    {
        List<Question> allQuestions;
        private const int Port = 8888;
        Thread acceptThread;
        private TcpListener listener;

        private object clientLock;
        private List<ClientHandler> clients;
        private object resultsLock;
        private List<ExamResult> results;

        private Random random;
        int numQuestions;
        int totalTime;
        bool exiting;

        public ServerWindow(QuestionBankManager questionBankManager)
        {
            InitializeComponent();
            this.random = new Random();
            this.allQuestions = questionBankManager.QuestionBank.Questions;
            this.clients = new List<ClientHandler>();
            this.clientLock = new object();
            this.resultsLock = new object();
            this.results = new List<ExamResult>();
            this.btnStop.IsEnabled = false;
            this.exiting = false;
        }

        private void StartExam_Click(object sender, RoutedEventArgs e)
        {
            // Parse total time
            if (!int.TryParse(txtTotalTime.Text, out totalTime))
            {
                MessageBox.Show("Invalid total time. Please enter a valid number.");
                return;
            }

            if(totalTime <= 0)
            {
                totalTime = 0;
                MessageBox.Show("Invalid total time. The number should be a positive integer (minutes)");
                return;
            }

            // Parse number of questions
            if (!int.TryParse(txtNumQuestions.Text, out numQuestions))
            {
                MessageBox.Show("Invalid number of questions. Please enter a valid number.");
                return;
            }

            if (numQuestions <= 0)
            {
                numQuestions = 0;
                MessageBox.Show("Invalid number of questions. Please enter a positive integer.");
                return;
            }
            else if(numQuestions > allQuestions.Count)
            {
                numQuestions = 0;
                MessageBox.Show($"Not enough questions in the question bank. Question Count: {allQuestions.Count}");
                return;
            }

            btnStop.IsEnabled = true;
            btnStart.IsEnabled = false;

            StartListening();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            this.exiting = true;
            this.btnStop.IsEnabled = false;
            FreeUpResources();
            Environment.Exit(0);
        }

        private void RefreshResults_Click(object sender, RoutedEventArgs e)
        {
            this.lstResults.ItemsSource = null;
            this.lstResults.ItemsSource = this.results;
        }

        private void StartListening()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, Port);
                listener.Start();
                acceptThread = new Thread(AcceptClients);
                acceptThread.IsBackground = true;
                acceptThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting server: " + ex.Message);
            }
        }

        private void AcceptClients()
        {
            while (true)
            {
                if (exiting)
                {
                    break;
                }
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    List<Question> questionsToSend = GenerateQuestions();

                    ClientHandler handler = new ClientHandler(client, questionsToSend, this.totalTime);
                    lock (clientLock)
                    {
                        clients.Add(handler);
                    }
                    handler.thread = new Thread(handler.HandleClient);
                    handler.thread.IsBackground = true;
                    handler.thread.Start();
                    handler.ClientFinished += Handler_ClientFinished;
                }
                catch(Exception e)
                {
                    return;
                }
            }
        }

        private async void Handler_ClientFinished(object sender, ClientEventArgs args)
        {
            ClientHandler handler = args.Client;

            double score = CalculateScorePercentage(args.Client.answers, args.Client.allQuestions);
            ExamResult examResult = new ExamResult(args.Client.studentName, score);
            lock (resultsLock)
            {
                results.Add(examResult);
            }

            // Removing the client from the active clients list
            lock (clientLock)
            {
                clients.Remove(args.Client); 
            }

            // Serialize the score into bytes
            byte[] scoreBytes = BitConverter.GetBytes(score);

            // Get the network stream from the client's TcpClient
            NetworkStream stream = args.Client.client.GetStream();

            // Send the score bytes to the client
            await stream.WriteAsync(scoreBytes, 0, scoreBytes.Length);

            args.Client.client.GetStream().Close();
            args.Client.client.Dispose();
        }

        private static double CalculateScorePercentage(string[] answers, List<Question> questions)
        {
            int correctAnswers = 0;
            for(int i = 0; i < answers.Length; i++)
            {
                if (answers[i] == questions[i].CorrectAnswer)
                {
                    correctAnswers++;
                }
            }

            // Calculate the percentage of correct answers
            if(correctAnswers == 0)
            {
                return 0;
            }

            return (double)correctAnswers / answers.Length * 100;
        }

        private List<Question> GenerateQuestions()
        {
            // Shuffle all questions and store them in a new list
            List<Question> shuffledBank = allQuestions.OrderBy(q => random.Next()).ToList();

            // Select the specified number of questions from the shuffled bank
            List<Question> selectedQuestions = shuffledBank.Take(numQuestions).ToList();

            return selectedQuestions;
        }

        private void FreeUpResources()
        {
            // Abort the acceptThread if it's running
            if (acceptThread != null && acceptThread.IsAlive)
            {
                acceptThread.Join(2000);
            }

            // Stop accepting new clients
            listener.Stop();

            // Close and dispose all client connections
            lock (clientLock)
            {
                foreach (var clientHandler in clients)
                {
                    // Stop the client's thread
                    if (clientHandler.thread != null && clientHandler.thread.IsAlive)
                    {
                        clientHandler.thread.Join(); // Wait for the thread to finish
                    }

                    // Close and dispose the TcpClient
                    clientHandler.client.GetStream().Close();
                    clientHandler.client.Close();
                    clientHandler.client.Dispose();
                }
                // Clear the list of clients
                clients.Clear();
            }
        }

        class ClientEventArgs
        {
            public ClientHandler Client { get; }

            public ClientEventArgs(ClientHandler client)
            {
                Client = client;
            }
        }

        private class ClientHandler
        {
            public event EventHandler<ClientEventArgs> ClientFinished;

            public string studentName;
            public TcpClient client;
            public List<Question> allQuestions;
            public int timerMinutes;
            public string[] answers;
            public Thread thread;

            public ClientHandler(TcpClient client, List<Question> questions, int tMinutes)
            {
                this.client = client;
                this.allQuestions = questions;
                this.timerMinutes = tMinutes;
            }

            public async void HandleClient()
            {
                try
                {
                    NetworkStream stream = client.GetStream();

                    // Buffer to store the incoming data (Student Name)
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    // Read the data sent by the client
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    // Convert the received data to a string (Student Name)
                    studentName = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    ExamInfo examInfo = new ExamInfo();
                    examInfo.Questions = new List<Question>(allQuestions.ToArray());
                    examInfo.TimerMinutes = this.timerMinutes;

                    // Send the exam data to the client
                    byte[] examBuffer = SerializeExamInfo(examInfo);
                    await stream.WriteAsync(examBuffer, 0, examBuffer.Length);

                    // Buffer for the answer keys
                    byte[] resultBuffer = new byte[allQuestions.Count * 100];

                    // Wait for a result
                    bytesRead = await stream.ReadAsync(resultBuffer, 0, resultBuffer.Length);

                    // Deserialize the byte array into an array of strings (answers)
                    answers = DeserializeAnswers(resultBuffer, bytesRead);

                    if (answers == null || answers.Length != allQuestions.Count)
                    {
                        // No answers received
                        return;
                    }

                    ClientEventArgs args = new ClientEventArgs(this);
                    ClientFinished?.Invoke(this, args);
                }
                catch(Exception e)
                {
                    return;
                }
            }

            // Method to serialize ExamInfo object into bytes
            private byte[] SerializeExamInfo(ExamInfo examInfo)
            {
                // Serialize the ExamInfo object to a JSON string
                string jsonString = JsonSerializer.Serialize(examInfo);

                // Convert the JSON string to bytes
                return Encoding.UTF8.GetBytes(jsonString);
            }

            private string[] DeserializeAnswers(byte[] data, int bytesRead)
            {
                // Convert the byte array to a string
                string jsonString = Encoding.UTF8.GetString(data, 0, bytesRead);

                // Deserialize the JSON string into an array of strings
                return JsonSerializer.Deserialize<string[]>(jsonString);
            }
        }

        private class ExamInfo
        {
            public List<Question> Questions { get; set; }
            public int TimerMinutes { get; set; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using System.Text;
using System.Windows.Controls;
using System.Linq;
using ExamLib;

namespace ExamClientWPF
{
    public partial class ExamWindow : Window
    {
        private string studentName;

        private TcpClient client;
        List<Question> allQuestions;

        private DispatcherTimer timer;
        private TimeSpan examDuration;
        private DateTime examEndTime;

        int currentQuestionIndex;
        bool submitingAnswers;

        public ExamWindow(string name, TcpClient client)
        {
            InitializeComponent();

            // Set the client received from the constructor
            this.client = client;
            this.submitingAnswers = false;
            this.studentName = name;

            SetUp();
        }
        
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            UpdateAnswer();
            currentQuestionIndex++;
            SetQuestion();
        }

        private void Previous_Click(object sender, RoutedEventArgs e)
        {
            UpdateAnswer();
            currentQuestionIndex--;
            SetQuestion();
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            submitingAnswers = true;
            timer.Stop();
            UpdateAnswer();
            SubmitAnswers();
        }

        private async void SetUp()
        {
            try
            {
                NetworkStream stream = client.GetStream();

                // Buffer to store the incoming data (Questions and time)
                byte[] buffer = new byte[1024 * 1024 * 5];
                int bytesRead;

                // Read the data sent by the server
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                {
                    //fail
                    Environment.Exit(1);
                }

                // Deserialize the received byte array into the examInfo structure
                ExamInfo examInfo = DeserializeExamInfo(buffer, bytesRead);

                if (examInfo.Questions == null || examInfo.TimerMinutes <= 0)
                {
                    MessageBox.Show("Invalid data received from server. Exiting...");
                    Environment.Exit(1);
                }

                // Set up the timer
                timer = new DispatcherTimer();
                examDuration = TimeSpan.FromMinutes(examInfo.TimerMinutes);
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;

                // Set up the end time of the exam
                examEndTime = DateTime.Now + examDuration;


                // Setting the received questions
                this.allQuestions = examInfo.Questions;

                foreach(var question in allQuestions)
                {
                    question.CorrectAnswer = "";
                }
            }
            catch(Exception e)
            {
                MessageBox.Show($"Communication with the server failed: {e.Message}");
                Environment.Exit(2);
            }

            // Set up the first question
            currentQuestionIndex = 0;
            SetQuestion();

            // Start the timer
            timer.Start();

            this.Show();
        }

        private async void SubmitAnswers()
        {
            try
            {
                NetworkStream stream = client.GetStream();

                string[] answers = new string[allQuestions.Count];
                for(int i = 0; i < answers.Length; i++)
                {
                    answers[i] = allQuestions[i].CorrectAnswer;
                }

                // Serialize the array of answers into a byte array
                byte[] answerData = SerializeAnswers(allQuestions);

                // Send the byte array over the network stream
                await stream.WriteAsync(answerData, 0, answerData.Length);

                // Buffer to store the incoming score bytes
                byte[] scoreBytes = new byte[sizeof(double)];

                // Read the score bytes from the network stream
                await stream.ReadAsync(scoreBytes, 0, scoreBytes.Length);

                // Deserialize the score bytes back into a double
                double score = BitConverter.ToDouble(scoreBytes, 0);

                ExamResult result = new ExamResult(this.studentName, score);
                string hasPassedString = result.HasPassed ? "Congratulations! You passed" : "Unfortunately, you failed the exam";

                MessageBox.Show($"Your results:\nName: {studentName}\nScore: {score}%\nGrade: {result.Grade}\n" + hasPassedString);
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Communication with the server failed: {e.Message}");
                Environment.Exit(2);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the timer display
            TimeSpan remainingTime = examEndTime - DateTime.Now;

            if(submitingAnswers)
            {
                // The user submited his answers
                timer.Stop();
                return;
            }

            if (remainingTime.TotalSeconds <= 0 && !submitingAnswers)
            {
                submitingAnswers = true;
                timer.Stop();
                SubmitAnswers();
            }
            else
            {
                txtTimeLeft.Text = $"Time Left: {remainingTime:mm\\:ss}";
            }
        }

        private void SetQuestion()
        {
            Question q = allQuestions[currentQuestionIndex];

            txtQuestion.Content = q.Text;

            lblAnswer0.Content = q.Answers[0].Text;
            lblAnswer1.Content = q.Answers[1].Text;
            lblAnswer2.Content = q.Answers[2].Text;
            lblAnswer3.Content = q.Answers[3].Text;

            if(!SetCorrectAnswer())
            {
                radAnswer0.IsChecked = false;
                radAnswer1.IsChecked = false;
                radAnswer2.IsChecked = false;
                radAnswer3.IsChecked = false;
            }

            if (currentQuestionIndex == allQuestions.Count - 1)
            {
                btnNext.IsEnabled = false;
            }
            else
            {
                btnNext.IsEnabled = true;
            }

            if (currentQuestionIndex == 0)
            {
                btnPrev.IsEnabled = false;
            }
            else
            {
                btnPrev.IsEnabled = true;
            }
        }

        private void UpdateAnswer()
        {
            // Update the answers in the list if the user made a decision
            string? answer = GetCorrectAnswer();
            if (answer != null)
            {
                allQuestions[currentQuestionIndex].CorrectAnswer = answer;
            }
        }

        private string? GetCorrectAnswer()
        {
            if (radAnswer0.IsChecked == true)
            {
                return allQuestions[currentQuestionIndex].Answers[0].Key;
            }
            else if (radAnswer1.IsChecked == true)
            {
                return allQuestions[currentQuestionIndex].Answers[1].Key;
            }
            else if (radAnswer2.IsChecked == true)
            {
                return allQuestions[currentQuestionIndex].Answers[2].Key;
            }
            else if (radAnswer3.IsChecked == true)
            {
                return allQuestions[currentQuestionIndex].Answers[3].Key;
            }
            else
            {
                return null;
            }
        }

        private bool SetCorrectAnswer()
        {
            for (int i = 0; i < allQuestions[currentQuestionIndex].Answers.Count; i++)
            {
                if (allQuestions[currentQuestionIndex].CorrectAnswer == allQuestions[currentQuestionIndex].Answers[i].Key)
                {
                    RadioButton radioButton = FindName($"radAnswer{i}") as RadioButton;
                    if (radioButton == null)
                    {
                        return false;
                    }

                    radioButton.IsChecked = true;
                    return true;
                }
            }

            return false;
        }
 
        private ExamInfo? DeserializeExamInfo(byte[] data, int bytesRead)
        {
            ExamInfo? examInfo;
            // Convert the byte array to a string
            byte[] trimmedData = new byte[bytesRead];
            Array.Copy(data, trimmedData, bytesRead);

            try
            {
                // Deserialize the JSON string into an ExamInfo object
                examInfo = JsonSerializer.Deserialize<ExamInfo>(trimmedData);
            }
            catch(Exception e)
            {
                examInfo = null;                
            }

            return examInfo;
        }

        private byte[] SerializeAnswers(List<Question> questions)
        {
            // Create a JSON string representing the array of answers
            string jsonString = JsonSerializer.Serialize(questions.Select(q => q.CorrectAnswer).ToArray());

            // Convert the JSON string to a byte array
            return Encoding.UTF8.GetBytes(jsonString);
        }

        private class ExamInfo
        {
            public List<Question> Questions { get; set; }
            public int TimerMinutes { get; set; }
        }

        ~ExamWindow()
        {
            client.GetStream().Close();
            client.Close();
            client.Dispose();
        }
    }
}
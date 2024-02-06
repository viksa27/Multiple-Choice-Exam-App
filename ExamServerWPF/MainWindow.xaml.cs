using System;
using System.Windows;
using ExamLib;

namespace ExamServerWPF
{
    public partial class MainWindow : Window
    {
        QuestionBankManager? questionBankManager;
        public MainWindow()
        {
            questionBankManager = null;
            InitializeComponent();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            string filePath = txtFilePath.Text;

            this.questionBankManager = new QuestionBankManager(filePath);

            if (!questionBankManager.LoadQuestionsFromFile())
            {
                this.questionBankManager = null;
                MessageBox.Show("Couldn't load questions");
                return;
            }

            MessageBox.Show("Questions loaded successfully");
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if(this.questionBankManager == null)
            {
                MessageBox.Show("No questions loaded");
                return;
            }

            string filePath = txtFilePath.Text;
            if(filePath == "")
            {
                MessageBox.Show("Invalid file path");
                return;
            }

            this.questionBankManager.FilePath = filePath;
            bool result = this.questionBankManager.SaveQuestionsToFile();

            if (result)
            {
                MessageBox.Show("Sucessfully saved the questions to file");
            }
            else
            {
                MessageBox.Show("Error. Couldn't save the questions to file");
            }
        }

        private void Questions_Click(object sender, RoutedEventArgs e)
        {
            if(this.questionBankManager == null)
            {
                this.questionBankManager = new QuestionBankManager("");
            }

            QuestionsWindow questionsWindow = new QuestionsWindow(questionBankManager);

            questionsWindow.Show();
        }

        private void Start_Exam_Click(object sender, RoutedEventArgs e)
        {
            if (this.questionBankManager == null || this.questionBankManager.QuestionBank.Questions.Count == 0)
            {
                MessageBox.Show("No questions loaded");
                return;
            }

            ServerWindow serverWindow = new ServerWindow(questionBankManager);

            serverWindow.Show();

            this.Close();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
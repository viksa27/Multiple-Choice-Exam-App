using System;
using System.Windows;
using System.Windows.Controls;
using ExamLib;

namespace ExamServerWPF
{
    public partial class QuestionEditorWindow : Window
    {
        public event EventHandler<QuestionEventArgs> QuestionUpdated;

        private Question question;

        public QuestionEditorWindow(Question question)
        {
            InitializeComponent();

            if (question == null)
            {
                MessageBox.Show("UNEXPECTED ERROR: Null question selected");
                this.Close();
            }

            this.question = question;

            txtQuestion.Text = question.Text;

            // We can do this because we have made sure that every question has exactly 4 answers
            // This logic is very dependent on the QuestionBankManager class
            // See QuestionBankManager.LoadQuestionsFromFile()

            txtAnswer0.Text = question.Answers[0].Text;
            txtAnswer1.Text = question.Answers[1].Text;
            txtAnswer2.Text = question.Answers[2].Text;
            txtAnswer3.Text = question.Answers[3].Text;

            SetCorrectAnswer();
        }

        private string GetCorrectAnswer()
        {
            if (radAnswer0.IsChecked == true)
            {
                return question.Answers[0].Key;
            }
            else if (radAnswer1.IsChecked == true)
            {
                return question.Answers[1].Key;
            }
            else if (radAnswer2.IsChecked == true)
            {
                return question.Answers[2].Key;
            }
            else if (radAnswer3.IsChecked == true)
            {
                return question.Answers[3].Key;
            }
            else
            {
                return "";
            }
        }

        private void SetCorrectAnswer()
        {
            for (int i = 0; i < this.question.Answers.Count; i++)
            {
                if (question.CorrectAnswer == question.Answers[i].Key)
                {
                    RadioButton radioButton = FindName($"radAnswer{i}") as RadioButton;
                    if(radioButton == null)
                    {
                        return;
                    }

                    radioButton.IsChecked = true;
                    return;
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Update the question object with the data from the controls
            question.Text = txtQuestion.Text;
            question.Answers[0].Text = txtAnswer0.Text;
            question.Answers[1].Text = txtAnswer1.Text;
            question.Answers[2].Text = txtAnswer2.Text;
            question.Answers[3].Text = txtAnswer3.Text;

            question.CorrectAnswer = GetCorrectAnswer();

            // Update the Question window
            QuestionUpdated?.Invoke(this, new QuestionEventArgs(question));

            // Close the window
            this.Close();
        }
    }
}
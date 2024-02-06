using System.Windows;
using System.Collections.Generic;
using ExamLib;

namespace ExamServerWPF
{
    public partial class QuestionsWindow : Window
    {
        private List<Question> questions;

        public QuestionsWindow(QuestionBankManager questionBankManager)
        {
            InitializeComponent();
            questions = questionBankManager.QuestionBank.Questions;

            // Updating the GUI with a source of questions
            questionList.ItemsSource = questions;

            lblQuestionCount.Content += "" + questions.Count;

            // Disabling these buttons because they need a selected question
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
        }

        private void QuestionEditorWindow_QuestionUpdated(object sender, QuestionEventArgs e)
        {
            UpdateQuestion(e.UpdatedQuestion);            
        }

        private void UpdateQuestion(Question updatedQuestion) 
        {
            // Update the corresponding question in the sourceQuestions list
            int index = questions.FindIndex(q => q == updatedQuestion);
            if (index != -1)
            {
                questions[index] = updatedQuestion;
                UpdateUI();
            }
        }

        private void OpenEditorWindow(Question question)
        {
            QuestionEditorWindow questionEditorWindow = new QuestionEditorWindow(question);
            questionEditorWindow.QuestionUpdated += QuestionEditorWindow_QuestionUpdated;
            // Show the QuestionsEditorWindow
            questionEditorWindow.Show();
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            Question question = new Question();
            questions.Add(question);

            // Setting default keys for the answers
            string[] defaultKeys = { "a", "b", "c", "d" };
            for (int i = 0; i < 4; i++)
            {
                question.Answers.Add(new Answer());
                question.Answers[i].Key = defaultKeys[i];
            }

            // Setting a default correct answer
            question.CorrectAnswer = question.Answers[0].Key;

            OpenEditorWindow(question);
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            if (questionList.SelectedItem == null)
            {
                MessageBox.Show("Please select a question to edit.");
                return;
            }

            Question selectedQuestion = (Question)questionList.SelectedItem;
            OpenEditorWindow(selectedQuestion);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Delete selected question
            if (questionList.SelectedItem != null)
            {
                Question selectedQuestion = (Question)questionList.SelectedItem;
                questions.Remove(selectedQuestion);
                UpdateUI();
            }
        }

        private void QuestionList_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Update UI based on selected question
            if (questionList.SelectedItem != null)
            {
                // Enable buttons based on selection
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
            }
            else
            {
                // Disable buttons if no selection
                btnEdit.IsEnabled = false;
                btnDelete.IsEnabled = false;
            }
        }

        private void UpdateUI()
        {
            // Updating the window view
            questionList.ItemsSource = null;
            questionList.ItemsSource = questions;
            lblQuestionCount.Content = $"Questions count: {questions.Count}";
        }
    }
}

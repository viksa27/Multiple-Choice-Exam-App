using System;
using ExamLib;

namespace ExamServerWPF
{
    public class QuestionEventArgs : EventArgs
    {
        public Question UpdatedQuestion { get; }

        public QuestionEventArgs(Question updatedQuestion)
        {
            UpdatedQuestion = updatedQuestion;
        }
    }
}

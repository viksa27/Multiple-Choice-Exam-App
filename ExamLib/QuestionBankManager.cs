using System.Numerics;
using System.Xml.Serialization;

namespace ExamLib
{
    public class QuestionBankManager
    {
        private string filePath;
        private QuestionBank questionBank;
        
        public QuestionBankManager(string filePath)
        {
            if(filePath == null)
            {
                throw new ArgumentNullException("Questions file path can't be null!");
            }

            this.FilePath = filePath;
            questionBank = new QuestionBank();
        }

        public QuestionBank QuestionBank
        {
            get 
            {
                return this.questionBank;
            }
        }

        public string FilePath
        {
            set
            {
                if(value != null)
                {
                    filePath = value;
                }
            }
            get
            {
                return filePath;
            }
        }

        public void AddQuestion(Question newQuestion)
        {
            if(newQuestion == null)
            {
                return;
            }
            this.questionBank.Questions.Add(newQuestion);
        }

        public bool LoadQuestionsFromFile()
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File doesn't exist: {filePath}");
                return false;
            }

            try
            {
                var serializer = new XmlSerializer(typeof(QuestionBank));

                using (var reader = new StreamReader(filePath))
                {
                    var questionBank = (QuestionBank?)serializer.Deserialize(reader);

                    if(questionBank == null)
                    {
                        Console.WriteLine($"No questions loaded (null). Error reading questions from file \"{filePath}\"");
                        return false;
                    }

                    // Validating - every question should have 4 answers with keys (a,b,c,d) and one correct answer
                    foreach(var question in questionBank.Questions)
                    {
                        if(question.Text == "")
                        {
                            Console.WriteLine("Invalid question bank. Question with no text.");
                            return false;
                        }

                        if(question.Answers.Count != 4)
                        {
                            Console.WriteLine($"Invalid question bank ({question.Text}). Each question should have 4 possible answers.");
                            return false;
                        }
                        

                        // Checking for duplicate keys
                        for(int i = 0; i < question.Answers.Count; i++)
                        {
                            for(int j = i + 1; j < question.Answers.Count; j++)
                            {
                                if (question.Answers[i].Key == question.Answers[j].Key)
                                {
                                    Console.WriteLine($"Invalid answers for question: \"{question.Text}\". Duplicate key.");
                                    return false;
                                }
                            }
                        }

                        // The correct answer must be a valid existing key, we check that here
                        bool validCorrectAnswer = false;
                        foreach(var answer in question.Answers)
                        {
                            if(answer.Key == question.CorrectAnswer)
                            {
                                validCorrectAnswer = true;
                                break;
                            }
                        }

                        if(!validCorrectAnswer)
                        {
                            Console.WriteLine($"Invalid correct answer for question: \"{question.Text}\". Key doesn't exist.");
                            return false;
                        }
                    }


                    this.questionBank = questionBank;
                    return true;
                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error deserializing XML: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading questions from file (\"{filePath}\"): {ex.Message}");
            }
            

            return false;
        }

        public bool SaveQuestionsToFile()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(QuestionBank));

                using (var writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, this.QuestionBank);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving questions to file: {ex.Message}");
            }

            return false;
        }
    }
}
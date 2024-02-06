using System.Xml.Serialization;

namespace ExamLib
{
    [XmlRoot("QuestionBank")]
    public class QuestionBank
    {
        [XmlElement("Question")]
        public List<Question> Questions { get; set; }

        public QuestionBank()
        {
            this.Questions = new List<Question>();
        }
    }
}

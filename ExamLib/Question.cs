using System.Xml.Serialization;

namespace ExamLib
{
    public class Question
    {
        [XmlElement]
        public string Text { get; set; }

        [XmlArray("Answers")]
        [XmlArrayItem("Answer")]
        public List<Answer> Answers { get; set; }

        [XmlElement("CorrectAnswer")]
        public string CorrectAnswer { get; set; }

        public Question()
        {
            this.Answers = new List<Answer>();
            this.Text = "";
            this.CorrectAnswer = "";
        }
    }
}

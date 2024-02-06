using System.Xml.Serialization;

namespace ExamLib
{
    public class Answer
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlText]
        public string Text { get; set; }

        public Answer()
        {
            this.Key = "";
            this.Text = "";
        }    
    }
}

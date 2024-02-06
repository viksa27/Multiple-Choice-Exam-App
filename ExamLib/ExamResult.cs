namespace ExamLib
{
    public class ExamResult
    {
        public string StudentName{ get; }
        public double Score { get; }
        public int Grade { get; }
        public bool HasPassed { get; }

        public ExamResult(string studentName, double score)
        {
            this.StudentName = studentName;
            this.Score = score;
            this.Grade = CalculateGrade(score);
            this.HasPassed = Grade > 2;
        }

        public static int CalculateGrade(double score)
        {
            if(score < 55)
            {
                return 2;
            }
            else if(score < 65)
            {
                return 3;
            }
            else if(score < 75)
            {
                return 4;
            }
            else if(score < 85)
            {
                return 5;
            }
            else
            {
                return 6;
            }
        }

        public override string ToString()
        {
            return $"Name: {StudentName}, Grade: {Grade}, Score: {Score}%";
        }
    }
}

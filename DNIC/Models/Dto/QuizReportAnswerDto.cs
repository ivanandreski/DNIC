namespace DNIC.Models.Dto
{
    public class QuizReportAnswerDto
    {
        public string Text { get; set; }
        public Answer YourAnswer { get; set; }
        public Answer CorrectAnswer { get; set; }

        public bool Correct
        {
            get
            {
                if (YourAnswer == null)
                    return false;

                return YourAnswer.Id == CorrectAnswer.Id;
            }
        }
    }
}

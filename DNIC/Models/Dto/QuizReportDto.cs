using System.Collections.Generic;
using System.Linq;

namespace DNIC.Models.Dto
{
    public class QuizReportDto
    {
        public List<QuizReportAnswerDto> Answers { get; set; }

        public double Score
        {
            get
            {
                return ((double)Answers.Where(x => x.Correct).Count() / (double)Answers.Count()) * 100.0;
            }
        }
    }
}

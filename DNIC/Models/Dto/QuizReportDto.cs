using System.Collections.Generic;

namespace DNIC.Models.Dto
{
    public class QuizReportDto
    {
        public List<QuizReportAnswerDto> Answers { get; set; }

        public double Score { get; set; }
    }
}

using DNIC.Models.Dto;
using System;
using System.Collections.Generic;

namespace DNIC.Models
{
    public class UserQuiz
    {
        public User User { get; set; }

        public Quiz Quiz { get; set; }

        // KEY: QuestionId, VALUE: User selected answer ID
        public Dictionary<Guid, Answer> QuestionAnswer { get; set; }

        public UserQuiz(User user, Quiz quiz)
        {
            User = user;
            Quiz = quiz;

            QuestionAnswer = new Dictionary<Guid, Answer>();
        }
    }
}

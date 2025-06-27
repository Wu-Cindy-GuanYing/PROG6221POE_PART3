using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotWPF
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Choices { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public string Explanation { get; set; }
        public bool IsTrueFalse { get; set; }
        public string Category { get; set; }
    }
}

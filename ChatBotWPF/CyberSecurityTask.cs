using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotWPF
{
    public class CyberSecurityTask
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? ReminderTime { get; set; }
        public bool IsCompleted { get; set; }
    }
}

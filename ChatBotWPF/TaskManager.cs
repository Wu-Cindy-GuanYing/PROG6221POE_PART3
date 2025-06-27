using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotWPF
{
        public class TaskManager
    {
        private List<CyberSecurityTask> tasks = new List<CyberSecurityTask>();
        private string lastTaskTitle = null;

        public void AddTask(string title, string description, DateTime? reminderTime = null)
        {
            tasks.Add(new CyberSecurityTask
            {
                Title = title,
                Description = description,
                ReminderTime = reminderTime,
                IsCompleted = false
            });
        }

        public void SetReminder(string title, TimeSpan delay)
        {
            var task = tasks.FirstOrDefault(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (task != null)
            {
                task.ReminderTime = DateTime.Now.Add(delay);
            }
        }

        public void MarkComplete(string title)
        {
            var task = tasks.FirstOrDefault(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            if (task != null)
            {
                task.IsCompleted = true;
            }
        }

        public void DeleteTask(string title)
        {
            tasks.RemoveAll(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }

        public string HandleUserInput(string input)
        {
            var lowerInput = input.ToLower();

            // Enhanced task recognition
            if (lowerInput.Contains("add") || lowerInput.Contains("create") || lowerInput.Contains("new"))
            {
                var taskName = ExtractTaskName(input);
                if (!string.IsNullOrEmpty(taskName))
                {
                    AddTask(taskName, "Security task");
                    return $"Task '{taskName}' added successfully. Would you like to set a reminder?";
                }
            }

            // Enhanced reminder recognition
            if (lowerInput.Contains("remind") || lowerInput.Contains("remember") || lowerInput.Contains("alert"))
            {
                var reminderText = ExtractReminderText(input);
                if (!string.IsNullOrEmpty(reminderText))
                {
                    var time = ExtractReminderTime(input);
                    AddTask(reminderText, "Reminder", time);
                    return $"I'll remind you to '{reminderText}' at {time}";
                }
            }

            var (command, title, timeSpanText) = CommandParser.Parse(input);

            if (command == "add")
            {
                lastTaskTitle = title;
                AddTask(title, "Review account privacy settings to ensure your data is protected.");
                return $"Task added with the description \"Review account privacy settings to ensure your data is protected.\" Would you like a reminder?";
            }
            else if (command == "remind" && lastTaskTitle != null)
            {
                var timespan = ParseTimeSpan(timeSpanText);
                SetReminder(lastTaskTitle, timespan);
                return $"Got it! I'll remind you in {timeSpanText}.";
            }
            else if (command == "complete" || command =="completed" || command == "mark")
            {
                MarkComplete(title);
                return $"Task \"{title}\" marked as complete.";
            }
            else if (command == "delete" || command == "delete task")
            {
                DeleteTask(title);
                return $"Task \"{title}\" has been deleted.";
            }

            return "I didn't understand that. Please try again.";
        }

        private string ExtractTaskName(string input)
        {
            // Remove common task-related phrases
            var phrases = new[] { "add", "create", "new", "task", "to", "please", "can you" };
            var words = input.Split(' ');
            return string.Join(" ", words.Where(w => !phrases.Contains(w.ToLower())));
        }

        private string ExtractReminderText(string input)
        {
            // Remove reminder-related phrases
            var phrases = new[] { "remind", "me", "to", "about", "set", "a", "alert", "for" };
            var words = input.Split(' ');
            return string.Join(" ", words.Where(w => !phrases.Contains(w.ToLower())));
        }

        private DateTime? ExtractReminderTime(string input)
        {
            if (input.ToLower().Contains("tomorrow")) return DateTime.Now.AddDays(1);
            if (input.ToLower().Contains("next week")) return DateTime.Now.AddDays(7);
            if (input.ToLower().Contains("in an hour")) return DateTime.Now.AddHours(1);
            return DateTime.Now.AddHours(24); // Default to tomorrow same time
        }

        public List<CyberSecurityTask> GetPendingReminders(DateTime currentTime)
        {
            return tasks.Where(t =>
                t.ReminderTime.HasValue &&
                t.ReminderTime <= currentTime &&
                !t.IsCompleted).ToList();
        }

        public void MarkReminderCompleted(CyberSecurityTask task)
        {
            var existingTask = tasks.FirstOrDefault(t => t.Title == task.Title);
            if (existingTask != null)
            {
                existingTask.IsCompleted = true;
            }
        }

        public List<CyberSecurityTask> GetAllTasks()
        {
            return tasks;
        }

        private TimeSpan ParseTimeSpan(string input)
        {
            var number = int.Parse(new string(input.Where(char.IsDigit).ToArray()));
            if (input.Contains("day")) return TimeSpan.FromDays(number);
            if (input.Contains("hour")) return TimeSpan.FromHours(number);
            if (input.Contains("minute")) return TimeSpan.FromMinutes(number);
            return TimeSpan.Zero;
        }

        public class CommandParser
        {
            public static (string command, string title, string timeSpanText) Parse(string input)
            {
                if (input.StartsWith("Add task", StringComparison.OrdinalIgnoreCase))
                {
                    var title = input.Substring(8).Trim();
                    return ("add", title, null);
                }
                else if (input.StartsWith("Remind me in", StringComparison.OrdinalIgnoreCase))
                {
                    var timeSpanText = input.Replace("Remind me in", "").Trim();
                    return ("remind", null, timeSpanText);
                }
                else if (input.StartsWith("Mark as complete", StringComparison.OrdinalIgnoreCase))
                {
                    var title = input.Substring(17).Trim();
                    return ("complete", title, null);
                }
                else if (input.StartsWith("Delete task", StringComparison.OrdinalIgnoreCase))
                {
                    var title = input.Substring(11).Trim();
                    return ("delete", title, null);
                }

                return (null, null, null);
            }
        }
    }

}

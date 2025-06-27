using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotWPF
{
    public class NLPSimulator
    {
        // Keywords for intent detection
        private readonly string[] taskKeywords = { "task", "add", "create", "new", "todo" };
        private readonly string[] reminderKeywords = { "remind", "reminder", "alert", "notify", "remember" };
        private readonly string[] queryKeywords = { "what", "list", "show", "summary", "have", "done" };
        private readonly string[] securityKeywords = { "password", "2fa", "two-factor", "authentication", "phishing", "privacy", "security" };

        public (string intent, string entity) ProcessInput(string userInput)
        {
            string lowerInput = userInput.ToLower();

            // Check for task-related requests
            if (taskKeywords.Any(kw => lowerInput.Contains(kw)))
            {
                return ("task", ExtractEntity(userInput, taskKeywords));
            }

            // Check for reminder-related requests
            if (reminderKeywords.Any(kw => lowerInput.Contains(kw)))
            {
                return ("reminder", ExtractEntity(userInput, reminderKeywords));
            }

            // Check for query requests
            if (queryKeywords.Any(kw => lowerInput.Contains(kw)))
            {
                return ("query", "");
            }

            // Check for security-related content
            foreach (var securityWord in securityKeywords)
            {
                if (lowerInput.Contains(securityWord))
                {
                    return ("security", securityWord);
                }
            }

            return ("unknown", "");
        }

        private string ExtractEntity(string input, string[] keywords)
        {
            string lowerInput = input.ToLower();

            // Find the last keyword position to extract text after it
            int lastKeywordPos = keywords
                .Select(kw => lowerInput.LastIndexOf(kw))
                .Where(pos => pos >= 0)
                .DefaultIfEmpty(-1)
                .Max();

            if (lastKeywordPos < 0) return null;

            // Extract text after the last keyword
            string afterKeyword = input.Substring(lastKeywordPos + keywords.First(kw =>
                lowerInput.LastIndexOf(kw) == lastKeywordPos).Length);

            // Clean up the description
            string entity = afterKeyword
                .Replace("to", "").Replace("about", "").Replace("me", "")
                .Trim(new char[] { ' ', '.', '?', '!' });

            return entity;
        }
    }
}

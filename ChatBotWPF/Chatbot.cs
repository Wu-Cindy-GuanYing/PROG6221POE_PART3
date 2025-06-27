using ChatBotWPF;
using System.IO;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using static ChatBotWPF.ActivityLogger;

namespace ChatbotWPF
{
    public class ChatBot
    {
        
        public event Action<string, bool>? OnMessageReceived;

        private CybersecurityQuiz quizGame = new CybersecurityQuiz();
        private NLPSimulator nlpSimulator = new NLPSimulator();
        private TaskManager taskManager;

        // Colors for text
        private ConsoleColor defaultColor = ConsoleColor.White;
        private ConsoleColor botColor = ConsoleColor.Cyan;
        private ConsoleColor userColor = ConsoleColor.Green;
        private ConsoleColor warningColor = ConsoleColor.Yellow;
        private ConsoleColor errorColor = ConsoleColor.White;
        private ConsoleColor positiveColor = ConsoleColor.Magenta;

        private string userName = " ";
        private string userInterest = "";
        private string favoriteTopic = "";

        // For sentiment detection
        private bool isWorried = false;
        private bool isCurious = false;
        private bool isFrustrated = false;
        private bool isFavourite = false;


        // Arrays: Response collections for random selection
        private List<string> passwordResponses = new List<string>
        {
            "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.",
            "A good password should be long (12+ characters) and include a mix of letters, numbers, and symbols.",
            "Consider using a passphrase like 'BlueCoffee$Makes5Cups!' instead of a simple password.",
            "Never share your passwords with anyone, even if they claim to be from tech support.",
            "Prevent using your name, surname, adress or birthdays in the passwords as they can be easily guessed and encrypted.",
            "Avoid using the same password for different accounts. Try creating unique passords for each platform you use.",
            "Use long passwords (at least 10 characters)",
            "Include uppercase, lowercase, numbers, and special characters",
            "Don't use personal information like birthdays or names",
            "Use a unique password for each account",
            "Consider using a password manager to keep track",
            "Enable two-factor authentication where available"

        };

        private List<string> phishingResponses = new List<string>
        {
            "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
            "Always check the sender's email address - phishing emails often come from addresses that look similar but not identical to legitimate ones.",
            "If an email creates a sense of urgency or threatens consequences, it's likely a phishing attempt.",
            "Hover over links before clicking to see the actual URL. If it looks suspicious, don't click!",
            "Make sure you verify the sender's identity. Scammers may pretend to be a legitimate being in order to manipulate their targets.",
            "When possible, report suspicious links or entity to prevent having more phishing victims.",
            "Urgent or threatening language demanding immediate action",
            "Requests for personal information via email or message",
            "Suspicious sender addresses (hover to check before clicking)",
            "Poor spelling and grammar",
            "Links that don't match the supposed sender (hover to check) and check for unexpected attachments"

        };

        private List<string> browsingResponses = new List<string>
        {
            "Always look for HTTPS and the padlock icon in your browser when entering sensitive information.",
            "Keep your browser and all plugins updated to protect against known vulnerabilities.",
            "Use browser extensions that block malicious websites and trackers for safer browsing.",
            "Be especially careful when using public Wi-Fi - consider using a VPN for added security.",
            "Regularly update your browser/web configurations. Keeping your device updated can help protect you from evolving cyber-threats",
            "Look for 'https://' and the padlock icon in your browser",
            "Keep your browser and operating system updated",
            "Use reputable antivirus and anti-malware software",
            "Be cautious with downloads - only from trusted sources",
            "Use a VPN on public Wi-Fi networks",
            "Regularly clear cookies and cache",
            "Be careful of 'too good to be true' offers"

        };

        private List<string> socialEngineeringResponses = new List<string>
        {
            "Social engineering is when attackers manipulate people into giving up confidential information.",
            "Be wary of anyone asking for sensitive information, even if they seem authoritative.",
            "Common social engineering tactics include pretexting, baiting, and quid pro quo offers.",
            "Never feel pressured to provide information - legitimate organizations won't rush you.",
            "Verify the identity of anyone requesting sensitive data, even if they claim to be from IT support.",
            "Attackers often exploit human psychology rather than technical vulnerabilities.",
            "PRETEXTING: Creating a fake scenario to obtain information",
            "BAITING: Offering something enticing to deliver malware",
            "QUID PRO QUO: Offering a service in exchange for information",
            "TAILGATING: Following someone into a restricted area",
            "PHISHING: Fraudulent communications pretending to be someone else",
            "Always verify identities before sharing information",
            "Be suspicious of unsolicited requests for information",
            "Don't let urgency or authority pressure you into acting",
            "Educate yourself and others about these tactics"


        };

        private List<string> malwareResponses = new List<string>
        {
            "Malware includes viruses, ransomware, spyware - any malicious software designed to harm.",
            "Keep your antivirus software updated and run regular scans of your system.",
            "Be very careful with email attachments and downloads from untrusted sources.",
            "Ransomware can encrypt your files - maintain regular backups on separate devices.",
            "Signs of malware infection include slow performance, pop-ups, and unexpected behavior.",
            "Use application whitelisting to only allow approved programs to run on your devices.",
            "VIRUSES: Self-replicating programs that infect other files",
            "WORMS: Spread through networks without user interaction",
            "TROJANS: Malware disguised as legitimate software",
            "RANSOMWARE: Encrypts files and demands payment",
            "SPYWARE: Secretly monitors user activity",
            "ADWARE: Displays unwanted advertisements",
            "Keep all software updated with the latest patches",
            "Use reputable antivirus/anti-malware software",
            "Be extremely cautious with email attachments",
            "Regular backups can save you from ransomware"

        };

        private List<string> worriedResponses = new List<string>
        {
            "Don't let your worry overwhelm you! With good cyber practices you're already ahead of most threats!",
            "The internet is generally safe if you follow the right methods. No need to worry too much!",
            "While the internet contains many threats, following good practices greatly reduces your risk!",
            "Worrying means you're aware of these cyber threats! You're already one step forward in reducing internet danger!"
        };

        private List<string> frustratedResponses = new List<string>
        {
            "It's normal for anyone to feel upset about these online criminal activities. Let's help one another to reduce cyber threats like this!",
            "It's definitely upsetting to have such dangers in our internet environment. Let me help you to reduce these cyber threats!",
            "It's right for anyone to feel upset about unsafe internet environment. Let's help one another to reduce cyber threats like this!",
            "Exploitment of human minds is definitely upsetting. Thus, it's important to try each other safe online using good practices.",
            "Don't let this disappointment press you down. Help us all stay safe online by spreading the good cyber practice."
        };

        private List<string> curiousResponses = new List<string>
        {
            "It's good to be curious as we can learn more! Here's more tips for you on password safety!",
            "Your curiosity enhances learning!",
            "Curiosity definitely improves one's problem-solving skill!"
        };

        //instances
        public ChatBot()
        {
            this.taskManager = new TaskManager();
        }

        public TaskManager TaskManager => taskManager;
        public void PlayWelcomeAudio()
        {
            try
            {
                string path = "WAVChatbotWelcomeAudio.wav";
                if (File.Exists(path))
                {
                    using (SoundPlayer player = new SoundPlayer(path))
                    {
                        player.PlaySync();
                    }
                }
                else
                {
                    OnMessageReceived?.Invoke("Welcome sound file not found. Continuing without audio...", false);
                }
            }
            catch (Exception ex)
            {
                OnMessageReceived?.Invoke($"Error playing welcome sound: {ex.Message}", false);
            }
        }

        public void GetUserName()
        {
            OnMessageReceived?.Invoke("Hello! Before we begin, what's your name?", false);
            OnMessageReceived?.Invoke("You can ask about cybersecurity topics or say 'start quiz' to test your knowledge!", false);
        }

        public void ProcessInput(string input)
        {
            ActivityLogger.Log($"User input received: {input}");

            if (string.IsNullOrWhiteSpace(input))
            {
                OnMessageReceived?.Invoke("I didn't quite understand that...", false);
                return;
            }

            string lowerInput = input.ToLower();

            // First try to handle with NLP
            var (intent, entity) = nlpSimulator.ProcessInput(input);

            switch (intent)
            {
                case "task":
                    HandleTaskIntent(entity);
                    return;
                case "reminder":
                    HandleReminderIntent(entity);
                    return;
                case "query":
                    HandleQueryIntent();
                    return;
                case "security":
                    HandleSecurityIntent(entity);
                    return;
            }


            // Handle quiz commands
            if (lowerInput.Contains("start quiz") || lowerInput.Contains("take quiz") || lowerInput.Contains("cybersecurity quiz"))
            {
                StartQuiz();
                return;
            }

            // Handle quiz answers if quiz is active
            if (quizGame.IsGameActive && int.TryParse(lowerInput, out int answer) &&
                answer >= 1 && answer <= 4) // Adjust max based on current question
            {
                ProcessQuizAnswer(answer);
                return;
            }

            // If NLP didn't recognise, fall back to original processing
            lowerInput = input.ToLower();

            //handle activity log
            if (input.ToLower().Contains("show activity log") ||
                input.ToLower().Contains("what have you done") ||
                input.ToLower().Contains("activity history"))
            {

                return;
            }

            // Handle task-related commands
            if (lowerInput.Contains("show my tasks") || lowerInput.Contains("list tasks") || lowerInput.Contains("my reminders"))
            {
                ShowAllTasks();
                return;
            }

            // Handle username input if not set
            if (string.IsNullOrWhiteSpace(userName) || userName == " ")
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    OnMessageReceived?.Invoke("I didn't catch your name. Could you please tell me your name?", false);
                }
                else
                {
                    userName = input;
                    OnMessageReceived?.Invoke($"Nice to meet you, {userName}! I'm your Cybersecurity Awareness ChatBot.", false);
                    OnMessageReceived?.Invoke("I can help you with topics like PASSWORD SAFETY, PHISHING, SAFE BROWSING, SOCIAL ENGINEERING and MALWARE.", false);
                    OnMessageReceived?.Invoke("Reply with 'password' / 'scam' / 'browsing'/ 'social engineering'/'malware'", false);
                    OnMessageReceived?.Invoke("OR say 'bye' to exit :)", false);
                }
                return;
            }
            // Handle task-related commands first
            var taskResponse = taskManager.HandleUserInput(input);
            if (taskResponse != "I didn't understand that. Please try again.")
            {
                OnMessageReceived?.Invoke(taskResponse, false);
                return;
            }

            // Sentiment detection
            DetectSentiment(lowerInput);

            // Save favourite topic
            CheckForFavoriteTopic(lowerInput);

            // Keyword recognition with conversation flow
            if (lowerInput.Contains("hello") || lowerInput.Contains("hi") || lowerInput.Contains("hey"))
            {
                if (isWorried && (lowerInput.Contains("scam") || lowerInput.Contains("phishing")))
                {
                    OnMessageReceived?.Invoke("It's totally understandable to feel that way. Scammers can be convincing for those unfamiliar with cybersecurity. Let me help you stay safe.", false);
                }
                OnMessageReceived?.Invoke($"Hello {userName}! How can I help you today?", false);
            }
            else if (lowerInput.Contains("how are you"))
            {
                OnMessageReceived?.Invoke("Though I'm just a bot, I'm working perfectly! Ready to help you with cybersecurity.", false);
            }
            else if (lowerInput.Contains("purpose") || lowerInput.Contains("what do you do"))
            {
                OnMessageReceived?.Invoke("My purpose is to help you stay safe online by providing cybersecurity awareness.", false);
                OnMessageReceived?.Invoke("I can explain common threats and best practices for staying secure.", false);
            }
            else if (ContainsKeywords(lowerInput, new[] { "password", "login", "credential" }))
            {
                userInterest = "password safety";
                DisplayPasswordSafetyAnswers();
            }
            else if (ContainsKeywords(lowerInput, new[] { "phishing", "scam", "fraud", "email scam" }))
            {
                userInterest = "phishing protection";
                DisplayPhishingAnswers(input);
            }
            else if (ContainsKeywords(lowerInput, new[] { "browsing", "internet", "online", "privacy", "browser" }))
            {
                userInterest = "safe browsing";
                DisplaySafeBrowsingInfo(input);
            }
            else if (lowerInput.Contains("thank") || lowerInput.Contains("thanks"))
            {
                OnMessageReceived($"You're welcome, {userName}! Is there anything else you'd like to know about {(!string.IsNullOrEmpty(userInterest) ? userInterest : "cybersecurity")}?", false);
            }
            else if (lowerInput.Contains("more") || lowerInput.Contains("detail") || lowerInput.Contains("explain"))
            {
                ProvideMoreDetails();
            }
            else if (ContainsKeywords(lowerInput, new[] { "social engineering", "manipulation", "pretexting", "baiting" }))
            {
                userInterest = "social engineering";
                DisplaySocialEngineeringInfo(input);
            }
            else if (ContainsKeywords(lowerInput, new[] { "malware", "virus", "ransomware", "spyware", "antivirus" }))
            {
                userInterest = "malware protection";
                DisplayMalwareProtectionInfo(input);
            }
            else if (lowerInput.Contains("exit") || lowerInput.Contains("quit") || lowerInput.Contains("bye"))
            {
                OnMessageReceived?.Invoke($"Goodbye, {userName}! Stay safe online!", false);
                Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
            }
            else if (lowerInput.ToLower().Contains("what is my name") || lowerInput.ToLower().Contains("who am i") || lowerInput.ToLower().Contains("name"))
            {
                if (!string.IsNullOrEmpty(userName))
                {
                    OnMessageReceived?.Invoke($"You told me your name is {userName}.", false);
                }
                else
                {
                    OnMessageReceived?.Invoke("I don't know your name yet. Please tell me by saying 'My name is ...'", false);
                }
                return;
            }
            else
            {
                HandleUnknownInput();
            }
        }
        private string GenerateResponse(string input)
        {
            // Implement your response generation logic here
            // This should return a string response based on the input
            return "This is a sample response to: " + input;
        }


        private void HandleTaskIntent(string taskDescription)
        {
            if (string.IsNullOrEmpty(taskDescription))
            {
                OnMessageReceived?.Invoke("What task would you like me to add?", false);
                return;
            }

            taskManager.AddTask(taskDescription, "Security-related task");
            OnMessageReceived?.Invoke($"Task added: '{taskDescription}'.", false);
            
        }

        private void HandleReminderIntent(string reminderDescription)
        {
            if (string.IsNullOrEmpty(reminderDescription))
            {
                OnMessageReceived?.Invoke("What would you like me to remind you about?", false);
                return;
            }

            // Simple date extraction (would need enhancement for real dates)
            string dateReference = "soon";
            if (reminderDescription.ToLower().Contains("tomorrow"))
            {
                dateReference = "tomorrow";
            }
            else if (reminderDescription.ToLower().Contains("next week"))
            {
                dateReference = "next week";
            }

            taskManager.AddTask(reminderDescription, "Reminder", DateTime.Now.AddDays(1));
            OnMessageReceived?.Invoke($"Reminder set for '{reminderDescription}' on {dateReference}.", false);
        }

        private void HandleQueryIntent()
        {
            var tasks = taskManager.GetAllTasks();
            if (!tasks.Any())
            {
                OnMessageReceived?.Invoke("You don't have any active tasks or reminders.", false);
                return;
            }

            var response = new StringBuilder("Here are your current tasks and reminders:\n");
            foreach (var task in tasks)
            {
                response.AppendLine($"- {task.Title}{(task.ReminderTime.HasValue ? $" (due {task.ReminderTime})" : "")}");
            }
            OnMessageReceived?.Invoke(response.ToString(), false);
        }

        private void HandleSecurityIntent(string securityTopic)
        {
            switch (securityTopic.ToLower())
            {
                case "password":
                case "2fa":
                case "two-factor":
                    DisplayPasswordSafetyAnswers();
                    break;
                case "phishing":
                    DisplayPhishingAnswers(securityTopic);
                    break;
                case "privacy":
                    DisplaySafeBrowsingInfo(securityTopic);
                    break;
                case "security":
                    // General security advice
                    OnMessageReceived?.Invoke("Here are some general security tips:", false);
                    OnMessageReceived?.Invoke("- Use strong, unique passwords for each account", false);
                    OnMessageReceived?.Invoke("- Enable two-factor authentication where available", false);
                    OnMessageReceived?.Invoke("- Be cautious of suspicious emails and links", false);
                    break;
                default:
                    OnMessageReceived?.Invoke($"I can help with {securityTopic} security. What specifically would you like to know?", false);
                    break;
            }
        }
        private void ShowAllTasks()
        {
            var tasks = taskManager.GetAllTasks();
            if (tasks.Any())
            {
                var sb = new StringBuilder();
                sb.AppendLine("Your current tasks:");

                foreach (var task in tasks.OrderBy(t => t.ReminderTime))
                {
                    var status = task.IsCompleted ? "[✓]" : "[ ]";
                    var due = task.ReminderTime.HasValue
                        ? $" (Due: {task.ReminderTime.Value:yyyy-MM-dd HH:mm})"
                        : "";
                    sb.AppendLine($"{status} {task.Title}{due}");
                }

                OnMessageReceived?.Invoke(sb.ToString(), false);
            }
            else
            {
                OnMessageReceived?.Invoke("You have no tasks currently.", false);
            }
        }
        private bool ContainsKeywords(string input, string[] keywords)
        {
            return keywords.Any(keyword => input.Contains(keyword));
        }

        private void DetectSentiment(string input)//detect sentiment
        {
            isWorried = input.Contains("worried") || input.Contains("concern") ||
                        input.Contains("scared") || input.Contains("anxious");

            isCurious = input.Contains("curious") || input.Contains("interested");

            isFrustrated = input.Contains("frustrated") || input.Contains("angry") ||
                        input.Contains("upset") || input.Contains("disappointed");

            isFavourite = input.Contains("favourite") || input.Contains("like");

            if (isWorried)
            {
                OnMessageReceived(GetRandomResponse(worriedResponses), false);
            }
            else if (isCurious)
            {
                OnMessageReceived(GetRandomResponse(curiousResponses), false);
                OnMessageReceived("Which topic are you most curious about?", false);

                if (input.Contains("password"))
                {
                    OnMessageReceived("Here's more info for you: ", false);
                    OnMessageReceived(GetRandomResponse(passwordResponses), false);
                }

                if (input.Contains("scam") || input.Contains("phishing"))
                {
                    OnMessageReceived("Here's more info for you: ", false);
                    OnMessageReceived(GetRandomResponse(phishingResponses), false);
                }

                if (input.Contains("internet") || input.Contains("browsing"))
                {
                    OnMessageReceived("Here's more info for you: ", false);
                    OnMessageReceived(GetRandomResponse(browsingResponses), false);
                }

                if (input.Contains("social") || input.Contains("engineering"))
                {
                    OnMessageReceived("Here's more info for you: ", false);
                    OnMessageReceived(GetRandomResponse(socialEngineeringResponses), false);
                }

                if (input.Contains("malware"))
                {
                    OnMessageReceived("Here's more info for you: ", false);
                    OnMessageReceived(GetRandomResponse(malwareResponses), false);
                }
            }
            else if (isFrustrated)
            {
                OnMessageReceived(GetRandomResponse(frustratedResponses), false);
            }
        }

        private void CheckForFavoriteTopic(string input)
        {
            string lowerInput = input.ToLower();

            if (lowerInput.Contains("favorite") || lowerInput.Contains("favourite"))
            {
                if (lowerInput.Contains("password") || lowerInput.Contains("login") || lowerInput.Contains("credential"))
                {
                    favoriteTopic = "password safety";
                    OnMessageReceived?.Invoke($"I've noted that password safety is your favorite topic, {userName}!", false);
                }
                else if (lowerInput.Contains("phishing") || lowerInput.Contains("scam") || lowerInput.Contains("fraud"))
                {
                    favoriteTopic = "phishing protection";
                    OnMessageReceived?.Invoke($"I've noted that phishing protection is your favorite topic, {userName}!", false);
                }
                else if (lowerInput.Contains("browsing") || lowerInput.Contains("internet") || lowerInput.Contains("online") || lowerInput.Contains("privacy"))
                {
                    favoriteTopic = "safe browsing";
                    OnMessageReceived?.Invoke($"I've noted that safe browsing is your favorite topic, {userName}!", false);
                }
                else if (lowerInput.Contains("malware") || lowerInput.Contains("virus") || lowerInput.Contains("spyware") || lowerInput.Contains("privacy"))
                {
                    favoriteTopic = "malware protection";
                    OnMessageReceived?.Invoke($"I've noted that safe browsing is your favorite topic, {userName}!", false);
                }
            }
        }

        private void ProvideMoreDetails()
        {
            if (!string.IsNullOrEmpty(userInterest))
            {
                switch (userInterest.ToLower())
                {
                    case "password safety":
                        OnMessageReceived?.Invoke("Here's more about password safety:", false);
                        OnMessageReceived?.Invoke(GetRandomResponse(passwordResponses), false);
                        break;
                    case "phishing protection":
                        OnMessageReceived?.Invoke("Additional phishing information:", false);
                        OnMessageReceived?.Invoke(GetRandomResponse(phishingResponses), false);
                        break;
                    case "safe browsing":
                        OnMessageReceived?.Invoke("More safe browsing tips:", false);
                        OnMessageReceived?.Invoke(GetRandomResponse(browsingResponses), false);
                        break;
                    case "social engineering":
                        OnMessageReceived?.Invoke("More social engineering tips:", false);
                        OnMessageReceived?.Invoke(GetRandomResponse(socialEngineeringResponses), false);
                        break;
                    case "malware protection":
                        OnMessageReceived?.Invoke("More malware protection tips:", false);
                        OnMessageReceived?.Invoke(GetRandomResponse(malwareResponses), false);
                        break;
                    default:
                        OnMessageReceived?.Invoke("Which topic would you like more information about?", false);
                        break;
                }
            }
            else
            {
                OnMessageReceived("Which topic would you like me to explain in more detail?", false);
            }
        }

        private void HandleUnknownInput() //error handling
        {
            if (!string.IsNullOrEmpty(userInterest))
            {
                OnMessageReceived($"I'm not sure I understand. Would you like more information about {userInterest}?", false);
            }
            else
            {
                OnMessageReceived("I didn't quite understand that. I can help with:", false);
                OnMessageReceived("- PASSWORD SAFETY (try 'tell me about passwords')", false);
                OnMessageReceived("- PHISHING SCAMS (try 'how to spot scams')", false);
                OnMessageReceived("- SAFE BROWSING PRACTICES (try 'internet safety tips')", false);
                OnMessageReceived("- SOCIAL ENGINEERING (try 'about manipulation attacks')", false);
                OnMessageReceived("- MALWARE PROTECTION (try 'virus protection tips')", false);
                OnMessageReceived("- CYBERSECURITY QUIZ (try 'start quiz')", false);
                OnMessageReceived("OR say 'BYE' to exit :)", false);
            }
        }

        private string GetRandomResponse(List<string> responses) //Selects random responses from the Array list
        {
            Random rnd = new Random();
            return responses[rnd.Next(responses.Count)];
        }

        private void DisplayPasswordSafetyAnswers()
        {
            Console.ForegroundColor = botColor;
            Console.WriteLine("\n" + new string('=', 40) + " PASSWORD SAFETY " + new string('=', 40) + "\n");
            OnMessageReceived(GetRandomResponse(passwordResponses), false);

            if (isFavourite)
            {
                OnMessageReceived("Great! I'll remember that this is your favourite topic!", false);
            }

            Console.WriteLine("\n" + new string('=', 97) + "\n");
            Console.ForegroundColor = defaultColor;
        }

        private void DisplayPhishingAnswers(string input)
        {
            Console.ForegroundColor = botColor;
            Console.WriteLine("\n" + new string('=', 38) + " PHISHING PROTECTION " + new string('=', 38) + "\n");
            OnMessageReceived(GetRandomResponse(phishingResponses), false);

            string lowerInput = input.ToLower();

            if (isFavourite)
            {
                OnMessageReceived("Great! I'll remember that this is your favourite topic!", false);
            }

            Console.WriteLine("\n" + new string('=', 97) + "\n");
            Console.ForegroundColor = defaultColor;

        }

        private void DisplaySafeBrowsingInfo(string input)
        {
            Console.ForegroundColor = botColor;
            Console.WriteLine("\n" + new string('=', 37) + " SAFE BROWSING PRACTICES " + new string('=', 37) + "\n");

            OnMessageReceived(GetRandomResponse(browsingResponses), false);

            if (isFavourite)
            {
                OnMessageReceived("Great! I'll remember that this is your favourite topic!", false);
            }


            Console.WriteLine("\n" + new string('=', 97) + "\n");
            Console.ForegroundColor = defaultColor;

        }

        private void DisplaySocialEngineeringInfo(string input)
        {
            Console.ForegroundColor = botColor;
            Console.WriteLine("\n" + new string('=', 35) + " SOCIAL ENGINEERING PROTECTION " + new string('=', 35) + "\n");
            OnMessageReceived(GetRandomResponse(socialEngineeringResponses), false);

            if (isFavourite)
            {
                OnMessageReceived("Great! I'll remember that this is your favourite topic!", false);
            }

            Console.WriteLine("\n" + new string('=', 97) + "\n");
            Console.ForegroundColor = defaultColor;

        }

        private void DisplayMalwareProtectionInfo(string input)
        {
            Console.ForegroundColor = botColor;
            OnMessageReceived("\n" + new string('=', 37) + " MALWARE PROTECTION " + new string('=', 37) + "\n", false);
            OnMessageReceived(GetRandomResponse(malwareResponses), false);

            if (isFavourite)
            {
                OnMessageReceived("Great! I'll remember that this is your favourite topic!", false);
            }

            OnMessageReceived("\n" + new string('=', 97) + "\n", false);
            Console.ForegroundColor = defaultColor;

        }
        private void TypeWrite(string message, bool isUserMessage)
        {
            // Simulate typing effect (optional)
            Task.Run(() =>
            {
                foreach (char c in message)
                {
                    OnMessageReceived?.Invoke(c.ToString(), isUserMessage);
                    Thread.Sleep(20); // Adjust speed if needed
                }
            });
        }

        private void StartQuiz()
        {
            quizGame.StartGame();
            DisplayCurrentQuestion();
           
        }

        private void DisplayCurrentQuestion()
        {
            var question = quizGame.GetCurrentQuestion();
            if (question != null)
            {
                string questionText = $"Question {quizGame.CurrentQuestionNumber}/{quizGame.QuestionCount}: {question.Question}\n\n";

                for (int i = 0; i < question.Choices.Count; i++)
                {
                    questionText += $"{i + 1}. {question.Choices[i]}\n";
                }

                OnMessageReceived?.Invoke(questionText, false);
            }
            else
            {
                var results = quizGame.GetFinalResults();
                OnMessageReceived?.Invoke($"Quiz complete! Your score: {results.score}/{quizGame.QuestionCount}\n{results.message}", false);
            }
        }

        private void ProcessQuizAnswer(int answerNumber)
        {
            if (quizGame.IsGameActive)
            {
                int answerIndex = answerNumber - 1;
                var result = quizGame.SubmitAnswer(answerIndex);

                string response = result.isCorrect ? "Correct! " : "Incorrect. ";
                response += result.explanation;

                OnMessageReceived?.Invoke(response, false);

                if (quizGame.IsGameActive)
                {
                    DisplayCurrentQuestion();
                }
                else
                {
                    var results = quizGame.GetFinalResults();
                    OnMessageReceived?.Invoke($"Quiz complete! Your score: {results.score}/{quizGame.QuestionCount}\n{results.message}", false);
                }
            }
        }

               
    }
}

  
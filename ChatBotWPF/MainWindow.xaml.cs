using System;
using System.Collections.ObjectModel;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Threading;
using ChatBotWPF;
using System.Text;

namespace ChatbotWPF
{
    public partial class MainWindow : Window
    {
       
        private ChatBot _chatbot;
        private DispatcherTimer reminderTimer;
        public ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();

        

        // Colors for messages
        private SolidColorBrush botColor = new SolidColorBrush(Color.FromRgb(0, 183, 235)); // Cyan-like
        private SolidColorBrush userColor = new SolidColorBrush(Colors.Green);
        private SolidColorBrush warningColor = new SolidColorBrush(Colors.Yellow);
        private SolidColorBrush positiveColor = new SolidColorBrush(Colors.Magenta);
             
        //main
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            ChatHistory.ItemsSource = Messages;

            // Initialize your chatbot
            _chatbot = new ChatBot();
            _chatbot.OnMessageReceived += Chatbot_OnMessageReceived;
            SetupReminderTimer();
        }
        private void RefreshLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", $"activity_{DateTime.Now:yyyyMMdd}.log");

                if (File.Exists(logFilePath))
                {
                    LogTextBox.Text = File.ReadAllText(logFilePath);
                }
                else
                {
                    LogTextBox.Text = "No log file found for today.";
                }
            }
            catch (Exception ex)
            {
                LogTextBox.Text = $"Error loading logs: {ex.Message}";
            }
        }


        private void SetupReminderTimer()
        {
            reminderTimer = new DispatcherTimer();
            reminderTimer.Interval = TimeSpan.FromMinutes(1);
            reminderTimer.Tick += ReminderTimer_Tick;
            reminderTimer.Start();
        }

        private void ReminderTimer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var pendingTasks = _chatbot.TaskManager.GetPendingReminders(now);

            if (pendingTasks != null && pendingTasks.Any())
            {
                foreach (var task in pendingTasks)
                {
                    Messages.Add(new ChatMessage(
                        $"REMINDER: {task.Title}\n{task.Description}",
                        false));
                    ScrollToBottom();
                    _chatbot.TaskManager.MarkReminderCompleted(task);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Display ASCII art
            AsciiArt.Text = @"
  ____      _                                        _ _            
 / ___|   _| |__   ___ _ __ ___  ___  ___ _   _ _ __(_) |_ _   _    
| |  | | | | '_ \ / _ \ '__/ __|/ _ \/ __| | | | '__| | __| | | |   
| |__| |_| | |_) |  __/ |  \__ \  __/ (__| |_| | |  | | |_| |_| |   
 \____\__, |_.__/ \___|_|  |___/\___|\___|\__,_|_|  |_|\__|\__, |   
   / \|___/   ____ _ _ __ ___ _ __   ___  ___ ___  | __ )  |___/ |_ 
  / _ \ \ /\ / / _` | '__/ _ \ '_ \ / _ \/ __/ __| |  _ \ / _ \| __|
 / ___ \ V  V / (_| | | |  __/ | | |  __/\__ \__ \ | |_) | (_) | |_ 
/_/   \_\_/\_/ \__,_|_|  \___|_| |_|\___||___/___/ |____/ \___/ \__|
";

            // Play welcome audio (async to not block UI)
            Task.Run(() => _chatbot.PlayWelcomeAudio());

            // Start conversation
            _chatbot.GetUserName();

            
        }

        private void Chatbot_OnMessageReceived(string message, bool isUserMessage)
        {
            Dispatcher.Invoke(() =>
            {
                Messages.Add(new ChatMessage(message, isUserMessage));
                ScrollToBottom();
            });
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            string userInput = UserInput.Text.Trim();
            if (!string.IsNullOrEmpty(userInput))
            {
                // Add user message to chat
                Messages.Add(new ChatMessage(userInput, true));
                UserInput.Clear();
                ScrollToBottom();

                // Process the message with chatbot (now with NLP)
                Task.Run(() => _chatbot.ProcessInput(userInput));
            }
        }

        private void ScrollToBottom()
        {
            if (ChatHistory.Items.Count > 0)
            {
                ChatHistory.ScrollIntoView(ChatHistory.Items[ChatHistory.Items.Count - 1]);
            }
        }
    }


    public class ChatMessage
    {
        public string Message { get; set; }
        public bool IsUserMessage { get; set; }

        public ChatMessage(string message, bool isUserMessage)
        {
            Message = message;
            IsUserMessage = isUserMessage;
        }
    }

    public class MessageBackgroundConverter : IValueConverter
    {
        public Brush UserBrush { get; set; }
        public Brush BotBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? UserBrush : BotBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Message foreground converter
    public class MessageForegroundConverter : IValueConverter
    {
        public Brush UserBrush { get; set; }
        public Brush BotBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? UserBrush : BotBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}


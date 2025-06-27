using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBotWPF
{
    internal class CybersecurityQuiz
    {
        private List<QuizQuestion> questions;
        private int currentQuestionIndex;
        private int score;
        private bool gameActive;

        public CybersecurityQuiz()
        {
            InitializeQuestions();
            ResetGame();
            ActivityLogger.Log("CybersecurityQuiz initialized", ActivityLogger.LogLevel.Info);
        }

        private void InitializeQuestions()
        {
            try
            {
                questions = new List<QuizQuestion>
                {
                    new QuizQuestion
                    {
                        Question = "What should you do if you receive an email asking for your password?",
                        Choices = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                        CorrectAnswerIndex = 2,
                        Explanation = "Reporting phishing emails helps prevent scams and protects others.",
                        IsTrueFalse = false
                    },
                    new QuizQuestion
        {
            Question = "Which of these is the strongest password?",
            Choices = new List<string> { "password123", "John1985", "Tr0ub4dor&3", "CorrectHorseBatteryStaple" },
            CorrectAnswerIndex = 3,
            Explanation = "Long passphrases are stronger than complex but short passwords.",
            IsTrueFalse = false
        },
        new QuizQuestion
        {
            Question = "Two-factor authentication (2FA) provides extra security by:",
            Choices = new List<string> { "Requiring two passwords", "Using biometrics only", "Combining something you know with something you have", "Remembering your login details" },
            CorrectAnswerIndex = 2,
            Explanation = "2FA requires both knowledge (password) and possession (phone/device).",
            IsTrueFalse = false
        },
        new QuizQuestion
        {
            Question = "Public WiFi networks are generally safe for online banking.",
            Choices = new List<string> { "True", "False" },
            CorrectAnswerIndex = 1,
            Explanation = "Public WiFi is often unsecured. Use a VPN or mobile data for sensitive transactions.",
            IsTrueFalse = true
        },
        new QuizQuestion
        {
            Question = "What does HTTPS in a website URL indicate?",
            Choices = new List<string> { "The site is popular", "The connection is encrypted", "The site is government-approved", "The site loads faster" },
            CorrectAnswerIndex = 1,
            Explanation = "HTTPS means communications between you and the site are encrypted.",
            IsTrueFalse = false
        },
        new QuizQuestion
        {
            Question = "Which of these is NOT a common characteristic of phishing attempts?",
            Choices = new List<string> { "Urgent language", "Requests for personal information", "Poor grammar/spelling", "Links to official-looking websites", "Clear sender identification" },
            CorrectAnswerIndex = 4,
            Explanation = "Phishing often hides or spoofs sender information.",
            IsTrueFalse = false
        },
        new QuizQuestion
        {
            Question = "You should use the same password for multiple important accounts.",
            Choices = new List<string> { "True", "False" },
            CorrectAnswerIndex = 1,
            Explanation = "Unique passwords prevent one breach from compromising multiple accounts.",
            IsTrueFalse = true
        },
        new QuizQuestion
        {
            Question = "What is the purpose of a VPN?",
            Choices = new List<string> { "Increase internet speed", "Encrypt internet traffic", "Block all ads", "Increase storage space" },
            CorrectAnswerIndex = 1,
            Explanation = "VPNs encrypt your connection to protect your online activity.",
            IsTrueFalse = false
        },
        new QuizQuestion
        {
            Question = "Software updates should be installed:",
            Choices = new List<string> { "Only when you have time", "Immediately when available", "Never - they might break things", "Only for operating systems" },
            CorrectAnswerIndex = 1,
            Explanation = "Updates often contain critical security patches.",
            IsTrueFalse = false
        },
        new QuizQuestion
        {
            Question = "What is social engineering?",
            Choices = new List<string> { "A type of software", "Manipulating people to gain access to systems", "A new programming language", "A hardware component" },
            CorrectAnswerIndex = 1,
            Explanation = "Social engineering tricks people rather than hacking systems directly.",
            IsTrueFalse = false
        }
    };
                ActivityLogger.Log($"Initialized {questions.Count} quiz questions", ActivityLogger.LogLevel.Info);
            }
            catch (Exception ex)
            {
                ActivityLogger.Log($"Error initializing questions: {ex.Message}", ActivityLogger.LogLevel.Error);
                throw;
            }
        }

        public void StartGame()
        {
            try
            {
                gameActive = true;
                currentQuestionIndex = 0;
                score = 0;
                ActivityLogger.Log("Quiz game started", ActivityLogger.LogLevel.Info);
            }
            catch (Exception ex)
            {
                ActivityLogger.Log($"Error starting quiz game: {ex.Message}", ActivityLogger.LogLevel.Error);
                throw;
            }
        }

        public void ResetGame()
        {
            gameActive = false;
            currentQuestionIndex = 0;
            score = 0;
            ActivityLogger.Log("Quiz game reset", ActivityLogger.LogLevel.Info);
        }

        public QuizQuestion GetCurrentQuestion()
        {
            try
            {
                if (gameActive && currentQuestionIndex < questions.Count)
                {
                    ActivityLogger.Log($"Retrieved question {CurrentQuestionNumber}/{QuestionCount}",
                                     ActivityLogger.LogLevel.Debug);
                    return questions[currentQuestionIndex];
                }
                return null;
            }
            catch (Exception ex)
            {
                ActivityLogger.Log($"Error getting current question: {ex.Message}", ActivityLogger.LogLevel.Error);
                return null;
            }
        }

        public (bool isCorrect, string explanation) SubmitAnswer(int answerIndex)
        {
            if (!gameActive || currentQuestionIndex >= questions.Count)
            {
                ActivityLogger.Log("Attempted to submit answer when game not active", ActivityLogger.LogLevel.Warning);
                return (false, "Game is not active or has ended.");
            }

            try
            {
                var currentQuestion = questions[currentQuestionIndex];
                bool isCorrect = answerIndex == currentQuestion.CorrectAnswerIndex;

                ActivityLogger.Log($"Question {CurrentQuestionNumber}: Submitted answer {answerIndex} " +
                                 $"(Correct: {currentQuestion.CorrectAnswerIndex}), " +
                                 $"Result: {(isCorrect ? "Correct" : "Incorrect")}",
                                 ActivityLogger.LogLevel.Info);

                if (isCorrect)
                {
                    score++;
                }

                string explanation = currentQuestion.Explanation;
                currentQuestionIndex++;

                if (currentQuestionIndex >= questions.Count)
                {
                    gameActive = false;
                    ActivityLogger.Log($"Quiz completed. Final score: {score}/{questions.Count}",
                                     ActivityLogger.LogLevel.Info);
                }

                return (isCorrect, explanation);
            }
            catch (Exception ex)
            {
                ActivityLogger.Log($"Error submitting answer: {ex.Message}", ActivityLogger.LogLevel.Error);
                return (false, "An error occurred while processing your answer.");
            }
        }

        public (int score, string message) GetFinalResults()
        {
            try
            {
                string message;
                double percentage = (double)score / questions.Count;

                if (percentage >= 0.9)
                {
                    message = "Great job! You're a cybersecurity pro!";
                }
                else if (percentage >= 0.7)
                {
                    message = "Good work! You know quite a bit about cybersecurity.";
                }
                else if (percentage >= 0.5)
                {
                    message = "Not bad! Consider learning more about cybersecurity best practices.";
                }
                else
                {
                    message = "Keep learning to stay safe online! Cybersecurity is important for everyone.";
                }

                ActivityLogger.Log($"Final results: Score {score}/{questions.Count} - {message}",
                                 ActivityLogger.LogLevel.Info);

                return (score, message);
            }
            catch (Exception ex)
            {
                ActivityLogger.Log($"Error getting final results: {ex.Message}", ActivityLogger.LogLevel.Error);
                return (0, "Error calculating results");
            }
        }

        public bool IsGameActive => gameActive;
        public int QuestionCount => questions?.Count ?? 0;
        public int CurrentQuestionNumber => currentQuestionIndex + 1;
    }

}


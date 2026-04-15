using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using DiplomProject.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DiplomProject.ViewModels
{
    public partial class PracticeViewModel : ViewModelBase
    {
        private readonly IPracticeService _practiceService;

        public ObservableCollection<PracticeTaskDto> Tasks { get; } = new();

        [ObservableProperty]
        private PracticeTaskDto? currentTask;

        [ObservableProperty]
        private string userAnswer = string.Empty;

        [ObservableProperty]
        private int score;

        [ObservableProperty]
        private int attempts;

        [ObservableProperty]
        private string feedbackState = "None";

        [ObservableProperty]
        private int currentTaskIndex;

        public IAsyncRelayCommand LoadTaskCommand { get; }
        public IRelayCommand CheckAnswerCommand { get; }
        public IRelayCommand NextTaskCommand { get; }
        public IRelayCommand NavigateBackCommand { get; }

        public string ScoreText => $"{Score} / {Attempts}";
        public string OperationSymbol => MapOperation(CurrentTask?.Operation, CurrentTask?.Subtype ?? 0);
        public string FeedbackText =>
            FeedbackState == "Correct"
                ? "Верно! Отличная работа."
                : FeedbackState == "Incorrect"
                    ? $"Неверно. Правильный ответ: {CurrentTask?.CorrectAnswer}"
                    : string.Empty;

        public bool IsCorrectFeedback => FeedbackState == "Correct";
        public bool IsIncorrectFeedback => FeedbackState == "Incorrect";
        public bool CanCheck => !string.IsNullOrWhiteSpace(UserAnswer) && FeedbackState == "None" && CurrentTask is not null;
        public bool CanShowNext => FeedbackState != "None";
        public string CurrentTaskProgress => Tasks.Count == 0 ? "Задача 0 из 0" : $"Задача {CurrentTaskIndex + 1} из {Tasks.Count}";

        public PracticeViewModel()
        {
            _practiceService = new PracticeService(db);

            LoadTaskCommand = new AsyncRelayCommand(LoadTasksAsync);
            CheckAnswerCommand = new RelayCommand(CheckAnswer, () => CanCheck);
            NextTaskCommand = new RelayCommand(NextTask);
            NavigateBackCommand = new RelayCommand(NavigateBack);

            _ = LoadTaskCommand.ExecuteAsync(null);
        }

        private async System.Threading.Tasks.Task LoadTasksAsync()
        {
            var loadedTasks = await _practiceService.GetPracticeTasksAsync();

            Tasks.Clear();
            foreach (var task in loadedTasks)
            {
                Tasks.Add(task);
            }

            CurrentTaskIndex = 0;
            CurrentTask = Tasks.FirstOrDefault();
            ResetAnswerState();

            OnPropertyChanged(nameof(CurrentTaskProgress));
            OnPropertyChanged(nameof(OperationSymbol));
        }

        private void CheckAnswer()
        {
            if (!CanCheck || CurrentTask is null)
            {
                return;
            }

            Attempts++;

            var userValues = ParseSetValues(UserAnswer);
            var correctValues = ParseSetValues(CurrentTask.CorrectAnswer);

            var isCorrect = userValues.SequenceEqual(correctValues);
            FeedbackState = isCorrect ? "Correct" : "Incorrect";

            if (isCorrect)
            {
                Score++;
            }

            OnPropertyChanged(nameof(ScoreText));
            OnPropertyChanged(nameof(FeedbackText));
            OnPropertyChanged(nameof(IsCorrectFeedback));
            OnPropertyChanged(nameof(IsIncorrectFeedback));
            OnPropertyChanged(nameof(CanShowNext));
            CheckAnswerCommand.NotifyCanExecuteChanged();
        }

        private void NextTask()
        {
            if (Tasks.Count == 0)
            {
                return;
            }

            var nextIndex = CurrentTaskIndex + 1;
            if (nextIndex >= Tasks.Count)
            {
                nextIndex = 0;
            }

            CurrentTaskIndex = nextIndex;
            CurrentTask = Tasks[nextIndex];
            ResetAnswerState();

            OnPropertyChanged(nameof(CurrentTaskProgress));
            OnPropertyChanged(nameof(OperationSymbol));
        }

        private void ResetAnswerState()
        {
            UserAnswer = string.Empty;
            FeedbackState = "None";

            OnPropertyChanged(nameof(FeedbackText));
            OnPropertyChanged(nameof(IsCorrectFeedback));
            OnPropertyChanged(nameof(IsIncorrectFeedback));
            OnPropertyChanged(nameof(CanShowNext));
            CheckAnswerCommand.NotifyCanExecuteChanged();
        }

        private void NavigateBack()
        {
            var user = MainWindowViewModel.Instance?.CurrentUser;
            if (user is null)
            {
                MainWindowViewModel.Instance!.CurrentViewModel = new AuthViewModel();
                return;
            }

            MainWindowViewModel.Instance!.CurrentViewModel = new MainViewModel(user);
        }

        private static List<int> ParseSetValues(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return new List<int>();
            }

            return source
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(token => int.TryParse(token, out var value) ? (int?)value : null)
                .Where(value => value.HasValue)
                .Select(value => value!.Value)
                .Distinct()
                .OrderBy(value => value)
                .ToList();
        }

        private static string MapOperation(string? operation, int subtype)
        {
            if (!string.IsNullOrWhiteSpace(operation))
            {
                return operation.ToLowerInvariant() switch
                {
                    "union" => "A ∪ B",
                    "intersection" => "A ∩ B",
                    "difference" => "A − B",
                    _ => operation
                };
            }

            return subtype switch
            {
                1 => "A ∪ B",
                2 => "A ∩ B",
                3 => "A − B",
                _ => "A ? B"
            };
        }

        partial void OnUserAnswerChanged(string value)
        {
            CheckAnswerCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanCheck));
        }

        partial void OnFeedbackStateChanged(string value)
        {
            CheckAnswerCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(CanCheck));
            OnPropertyChanged(nameof(CanShowNext));
            OnPropertyChanged(nameof(FeedbackText));
            OnPropertyChanged(nameof(IsCorrectFeedback));
            OnPropertyChanged(nameof(IsIncorrectFeedback));
        }

        partial void OnCurrentTaskChanged(PracticeTaskDto? value)
        {
            OnPropertyChanged(nameof(OperationSymbol));
        }
    }
}

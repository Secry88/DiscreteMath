using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscreteMath.Models;
using DiscreteMath.Services;
using System.Collections.Generic;
using System.Linq;

namespace DiscreteMath.ViewModels
{
    public partial class PracticeViewModel : ViewModelBase
    {
        private static readonly PracticeGenerator Generator = new();
        private const int SessionSize = 10;

        private readonly List<GeneratedPracticeTask> _tasks = new();

        [ObservableProperty] private GeneratedPracticeTask? currentTask;
        [ObservableProperty] private string userAnswer = string.Empty;
        [ObservableProperty] private int score;
        [ObservableProperty] private int attempts;
        [ObservableProperty] private string feedbackState = "None";
        [ObservableProperty] private int currentTaskIndex;
        [ObservableProperty] private bool isSessionFinished;

        public IRelayCommand CheckAnswerCommand { get; }
        public IRelayCommand NextTaskCommand { get; }
        public IRelayCommand ResetCommand { get; }
        public IRelayCommand NavigateBackCommand { get; }
        public IRelayCommand InsertEmptySetCommand { get; }

        public string ScoreText => $"{Score} / {Attempts}";
        public string ProgressText => $"Задача {CurrentTaskIndex + 1} из {_tasks.Count}";

        public string FeedbackText =>
            FeedbackState == "Correct"   ? "✓ Верно! Отличная работа." :
            FeedbackState == "Incorrect" ? $"✗ Неверно. Правильный ответ: {CurrentTask?.CorrectAnswer}" :
            string.Empty;

        public bool IsCorrectFeedback  => FeedbackState == "Correct";
        public bool IsIncorrectFeedback => FeedbackState == "Incorrect";
        public bool CanCheck   => !string.IsNullOrWhiteSpace(UserAnswer) && FeedbackState == "None" && CurrentTask is not null;
        public bool CanShowNext => FeedbackState != "None";

        // Task display helpers
        public string ExpressionDisplay => CurrentTask?.Expression ?? string.Empty;
        public string SetADisplay        => string.IsNullOrEmpty(CurrentTask?.SetADisplay) ? string.Empty : $"A = {CurrentTask.SetADisplay}";
        public string SetBDisplay        => string.IsNullOrEmpty(CurrentTask?.SetBDisplay) ? string.Empty : $"B = {CurrentTask.SetBDisplay}";
        public string SetCDisplay        => CurrentTask?.UsesC == true && CurrentTask.SetCDisplay is not null ? $"C = {CurrentTask.SetCDisplay}" : string.Empty;
        public string UniversalDisplay   => string.IsNullOrEmpty(CurrentTask?.UniversalDisplay) ? string.Empty : $"U = {CurrentTask.UniversalDisplay}";
        public bool HasSetC => CurrentTask?.UsesC == true;

        public PracticeViewModel()
        {
            CheckAnswerCommand  = new RelayCommand(CheckAnswer, () => CanCheck);
            NextTaskCommand     = new RelayCommand(NextTask);
            ResetCommand        = new RelayCommand(Reset);
            NavigateBackCommand = new RelayCommand(NavigateBack);
            InsertEmptySetCommand = new RelayCommand(() => UserAnswer = "∅");

            GenerateSession();
        }

        private void GenerateSession()
        {
            _tasks.Clear();
            for (int i = 0; i < SessionSize; i++)
                _tasks.Add(Generator.Generate());

            CurrentTaskIndex = 0;
            CurrentTask = _tasks[0];
            ResetAnswerState();
            OnPropertyChanged(nameof(ProgressText));
        }

        private void CheckAnswer()
        {
            if (!CanCheck || CurrentTask is null) return;
            Attempts++;

            var userSet    = ParseSet(UserAnswer);
            var correctSet = ParseSet(CurrentTask.CorrectAnswer);

            bool isCorrect = userSet.SetEquals(correctSet);
            FeedbackState = isCorrect ? "Correct" : "Incorrect";

            if (isCorrect) Score++;

            OnPropertyChanged(nameof(ScoreText));
            OnPropertyChanged(nameof(FeedbackText));
            OnPropertyChanged(nameof(CanShowNext));
            CheckAnswerCommand.NotifyCanExecuteChanged();
        }

        private void NextTask()
        {
            var next = CurrentTaskIndex + 1;
            if (next >= _tasks.Count) { IsSessionFinished = true; return; }
            CurrentTaskIndex = next;
            CurrentTask = _tasks[next];
            ResetAnswerState();
            OnPropertyChanged(nameof(ProgressText));
        }

        private void Reset()
        {
            Score = 0;
            Attempts = 0;
            IsSessionFinished = false;
            GenerateSession();
            OnPropertyChanged(nameof(ScoreText));
        }

        private void ResetAnswerState()
        {
            UserAnswer = string.Empty;
            FeedbackState = "None";
            OnPropertyChanged(nameof(FeedbackText));
            OnPropertyChanged(nameof(IsCorrectFeedback));
            OnPropertyChanged(nameof(IsIncorrectFeedback));
            OnPropertyChanged(nameof(CanShowNext));
            OnPropertyChanged(nameof(ExpressionDisplay));
            OnPropertyChanged(nameof(SetADisplay));
            OnPropertyChanged(nameof(SetBDisplay));
            OnPropertyChanged(nameof(SetCDisplay));
            OnPropertyChanged(nameof(UniversalDisplay));
            OnPropertyChanged(nameof(HasSetC));
            CheckAnswerCommand.NotifyCanExecuteChanged();
        }

        private void NavigateBack()
        {
            var user = MainWindowViewModel.Instance?.CurrentUser;
            if (user is null) MainWindowViewModel.Instance!.CurrentViewModel = new AuthViewModel();
            else MainWindowViewModel.Instance!.CurrentViewModel = new MainViewModel(user);
        }

        private static HashSet<int> ParseSet(string source)
        {
            if (string.IsNullOrWhiteSpace(source) || source.Trim() == "∅")
                return new HashSet<int>();
            return source
                .Trim('{', '}', ' ')
                .Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries)
                .Where(t => int.TryParse(t, out _))
                .Select(int.Parse)
                .ToHashSet();
        }

        partial void OnUserAnswerChanged(string value)
        {
            OnPropertyChanged(nameof(CanCheck));
            CheckAnswerCommand.NotifyCanExecuteChanged();
        }

        partial void OnFeedbackStateChanged(string value)
        {
            OnPropertyChanged(nameof(IsCorrectFeedback));
            OnPropertyChanged(nameof(IsIncorrectFeedback));
            OnPropertyChanged(nameof(FeedbackText));
            OnPropertyChanged(nameof(CanCheck));
            OnPropertyChanged(nameof(CanShowNext));
            CheckAnswerCommand.NotifyCanExecuteChanged();
        }

        partial void OnCurrentTaskChanged(GeneratedPracticeTask? value)
        {
            OnPropertyChanged(nameof(ExpressionDisplay));
            OnPropertyChanged(nameof(SetADisplay));
            OnPropertyChanged(nameof(SetBDisplay));
            OnPropertyChanged(nameof(SetCDisplay));
            OnPropertyChanged(nameof(UniversalDisplay));
            OnPropertyChanged(nameof(HasSetC));
        }
    }
}

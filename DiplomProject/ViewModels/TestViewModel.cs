using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscreteMath.Models;
using DiscreteMath.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DiscreteMath.ViewModels
{
    public partial class TestViewModel : ViewModelBase
    {
        private readonly ITestService _testService;
        private readonly int _testId;
        private readonly int _userId;
        private readonly Dictionary<int, int> _selectedAnswersByQuestionId = new();

        public ObservableCollection<TestQuestionDto> Questions { get; } = new();

        [ObservableProperty]
        private string testTitle = string.Empty;

        [ObservableProperty]
        private int currentQuestionIndex;

        [ObservableProperty]
        private TestQuestionDto? currentQuestion;

        [ObservableProperty]
        private bool isFinished;

        [ObservableProperty]
        private int score;

        [ObservableProperty]
        private int resultPercent;

        public IAsyncRelayCommand LoadTestCommand { get; }
        public IRelayCommand<TestAnswerDto> SelectAnswerCommand { get; }
        public IRelayCommand NextQuestionCommand { get; }
        public IRelayCommand PreviousQuestionCommand { get; }
        public IAsyncRelayCommand FinishTestCommand { get; }
        public IRelayCommand BackToTestsCommand { get; }
        public IRelayCommand RetakeCommand { get; }

        public string QuestionProgress => Questions.Count == 0 ? "Question: 0/0" : $"Question: {CurrentQuestionIndex + 1}/{Questions.Count}";
        public double ProgressValue => Questions.Count == 0 ? 0 : ((double)(CurrentQuestionIndex + 1) / Questions.Count) * 100.0;
        public bool CanGoPrevious => !IsFinished && CurrentQuestionIndex > 0;
        public bool CanGoNext => !IsFinished && CurrentQuestionIndex < Questions.Count - 1;
        public bool CanFinish => !IsFinished && Questions.Count > 0;
        public bool IsTakingTest => !IsFinished;
        public string ResultSummary => $"{Score} out {Questions.Count} correct";
        public string ResultAssessment =>
            ResultPercent >= 90 ? "Excellent!" :
            ResultPercent >= 70 ? "Good result" :
            "Try again!";

        public TestViewModel(int testId)
        {
            _testService = new TestService(db);
            _testId = testId;
            _userId = MainWindowViewModel.Instance?.CurrentUser?.Id ?? 0;

            LoadTestCommand = new AsyncRelayCommand(LoadTestAsync);
            SelectAnswerCommand = new RelayCommand<TestAnswerDto>(SelectAnswer);
            NextQuestionCommand = new RelayCommand(NextQuestion);
            PreviousQuestionCommand = new RelayCommand(PreviousQuestion);
            FinishTestCommand = new AsyncRelayCommand(FinishTestAsync);
            BackToTestsCommand = new RelayCommand(BackToTests);
            RetakeCommand = new RelayCommand(Retake);

            _ = LoadTestCommand.ExecuteAsync(null);
        }

        private async Task LoadTestAsync()
        {
            var detail = await _testService.GetTestDetailAsync(_testId);
            if (detail is null)
            {
                return;
            }

            TestTitle = detail.Title;

            Questions.Clear();
            foreach (var question in detail.Questions)
            {
                Questions.Add(question);
            }

            CurrentQuestionIndex = 0;
            CurrentQuestion = Questions.FirstOrDefault();
            IsFinished = false;
            Score = 0;
            ResultPercent = 0;
            _selectedAnswersByQuestionId.Clear();

            UpdateComputedState();
        }

        private void SelectAnswer(TestAnswerDto? answer)
        {
            if (answer is null || CurrentQuestion is null || IsFinished)
            {
                return;
            }

            foreach (var item in CurrentQuestion.Answers)
            {
                item.IsSelected = false;
            }

            answer.IsSelected = true;
            _selectedAnswersByQuestionId[CurrentQuestion.Id] = answer.Id;
        }

        private void NextQuestion()
        {
            if (!CanGoNext)
            {
                return;
            }

            CurrentQuestionIndex++;
            CurrentQuestion = Questions[CurrentQuestionIndex];
            RestoreSelectedState();
            UpdateComputedState();
        }

        private void PreviousQuestion()
        {
            if (!CanGoPrevious)
            {
                return;
            }

            CurrentQuestionIndex--;
            CurrentQuestion = Questions[CurrentQuestionIndex];
            RestoreSelectedState();
            UpdateComputedState();
        }

        private async Task FinishTestAsync()
        {
            if (!CanFinish)
            {
                return;
            }

            Score = Questions.Count(question =>
            {
                if (!_selectedAnswersByQuestionId.TryGetValue(question.Id, out var selectedAnswerId))
                {
                    return false;
                }

                return question.Answers.Any(answer => answer.Id == selectedAnswerId && answer.IsCorrect);
            });

            ResultPercent = Questions.Count == 0 ? 0 : (int)((double)Score / Questions.Count * 100);
            IsFinished = true;

            await _testService.SaveTestResultAsync(_userId, _testId, Score, Questions.Count);

            UpdateComputedState();
        }

        private void BackToTests()
        {
            MainWindowViewModel.Instance!.CurrentViewModel = new TestSelectionViewModel();
        }

        private void Retake()
        {
            foreach (var question in Questions)
            {
                foreach (var answer in question.Answers)
                {
                    answer.IsSelected = false;
                }
            }

            _selectedAnswersByQuestionId.Clear();
            CurrentQuestionIndex = 0;
            CurrentQuestion = Questions.FirstOrDefault();
            IsFinished = false;
            Score = 0;
            ResultPercent = 0;

            UpdateComputedState();
        }

        private void RestoreSelectedState()
        {
            if (CurrentQuestion is null)
            {
                return;
            }

            _selectedAnswersByQuestionId.TryGetValue(CurrentQuestion.Id, out var selectedAnswerId);

            foreach (var answer in CurrentQuestion.Answers)
            {
                answer.IsSelected = answer.Id == selectedAnswerId;
            }
        }

        private void UpdateComputedState()
        {
            OnPropertyChanged(nameof(QuestionProgress));
            OnPropertyChanged(nameof(ProgressValue));
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
            OnPropertyChanged(nameof(CanFinish));
            OnPropertyChanged(nameof(ResultSummary));
            OnPropertyChanged(nameof(ResultAssessment));
        }

        partial void OnCurrentQuestionIndexChanged(int value)
        {
            UpdateComputedState();
        }

        partial void OnIsFinishedChanged(bool value)
        {
            OnPropertyChanged(nameof(IsTakingTest));
            UpdateComputedState();
        }

        partial void OnScoreChanged(int value)
        {
            OnPropertyChanged(nameof(ResultSummary));
        }

        partial void OnResultPercentChanged(int value)
        {
            OnPropertyChanged(nameof(ResultAssessment));
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using DiplomProject.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiplomProject.ViewModels
{
    public partial class EulerViewModel : ViewModelBase
    {
        private readonly EulerService _eulerService;

        public ObservableCollection<EulerProblemDto> Problems { get; }

        [ObservableProperty]
        private EulerProblemDto? currentProblem;

        [ObservableProperty]
        private int currentIndex;

        [ObservableProperty]
        private string? selectedRegion;

        [ObservableProperty]
        private string feedbackState = "None";

        [ObservableProperty]
        private int score;

        [ObservableProperty]
        private bool isSessionFinished;

        private bool _scoredForCurrent;

        public IRelayCommand<string> SelectRegionCommand { get; }
        public IRelayCommand NextCommand { get; }
        public IRelayCommand ResetCommand { get; }
        public IRelayCommand NavigateBackCommand { get; }

        public string ScoreText => $"Score: {Score}/{Problems.Count}";
        public string ProgressText => Problems.Count == 0 ? "Problem 0 of 0" : $"Problem {CurrentIndex + 1} of {Problems.Count}";
        public bool IsCorrectFeedback => FeedbackState == "Correct";
        public bool IsIncorrectFeedback => FeedbackState == "Incorrect";
        public string CurrentProblemDescription => CurrentProblem?.Description ?? "No problem available.";
        public bool CanGoNext => !IsSessionFinished && Problems.Count > 0;
        public bool IsAOnlySelected => SelectedRegion == "a-only";
        public bool IsAbSelected => SelectedRegion == "ab";
        public bool IsBOnlySelected => SelectedRegion == "b-only";

        public EulerViewModel()
        {
            _eulerService = new EulerService(db);
            Problems = new ObservableCollection<EulerProblemDto>(_eulerService.GetProblems());

            SelectRegionCommand = new RelayCommand<string>(SelectRegion);
            NextCommand = new RelayCommand(GoNext);
            ResetCommand = new RelayCommand(Reset);
            NavigateBackCommand = new RelayCommand(NavigateBack);

            CurrentIndex = 0;
            CurrentProblem = Problems.FirstOrDefault();
        }

        private void SelectRegion(string? regionCode)
        {
            if (CurrentProblem is null || string.IsNullOrWhiteSpace(regionCode) || IsSessionFinished)
                return;

            SelectedRegion = regionCode;

            var selected = CurrentProblem.Regions.FirstOrDefault(x => x.RegionCode == regionCode);
            var isCorrect = selected is not null && selected.IsCorrect;
            FeedbackState = isCorrect ? "Correct" : "Incorrect";

            // Очко начисляется только один раз за вопрос (при первом правильном ответе)
            if (isCorrect && !_scoredForCurrent)
            {
                _scoredForCurrent = true;
                Score++;
                OnPropertyChanged(nameof(ScoreText));
            }
        }

        private void GoNext()
        {
            if (Problems.Count == 0)
            {
                return;
            }

            var nextIndex = CurrentIndex + 1;
            if (nextIndex >= Problems.Count)
            {
                IsSessionFinished = true;
                OnPropertyChanged(nameof(CanGoNext));
                return;
            }

            CurrentIndex = nextIndex;
            CurrentProblem = Problems[CurrentIndex];
            SelectedRegion = null;
            FeedbackState = "None";
            _scoredForCurrent = false;

            OnPropertyChanged(nameof(CurrentProblemDescription));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(IsCorrectFeedback));
            OnPropertyChanged(nameof(IsIncorrectFeedback));
        }

        private void Reset()
        {
            Score = 0;
            CurrentIndex = 0;
            CurrentProblem = Problems.FirstOrDefault();
            SelectedRegion = null;
            FeedbackState = "None";
            IsSessionFinished = false;
            _scoredForCurrent = false;

            OnPropertyChanged(nameof(ScoreText));
            OnPropertyChanged(nameof(ProgressText));
            OnPropertyChanged(nameof(CurrentProblemDescription));
            OnPropertyChanged(nameof(IsCorrectFeedback));
            OnPropertyChanged(nameof(IsIncorrectFeedback));
            OnPropertyChanged(nameof(CanGoNext));
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

        partial void OnFeedbackStateChanged(string value)
        {
            OnPropertyChanged(nameof(IsCorrectFeedback));
            OnPropertyChanged(nameof(IsIncorrectFeedback));
        }

        partial void OnSelectedRegionChanged(string? value)
        {
            OnPropertyChanged(nameof(IsAOnlySelected));
            OnPropertyChanged(nameof(IsAbSelected));
            OnPropertyChanged(nameof(IsBOnlySelected));
        }

        partial void OnScoreChanged(int value)
        {
            OnPropertyChanged(nameof(ScoreText));
        }

        partial void OnCurrentIndexChanged(int value)
        {
            OnPropertyChanged(nameof(ProgressText));
        }

        partial void OnCurrentProblemChanged(EulerProblemDto? value)
        {
            OnPropertyChanged(nameof(CurrentProblemDescription));
        }

        partial void OnIsSessionFinishedChanged(bool value)
        {
            OnPropertyChanged(nameof(CanGoNext));
        }
    }
}

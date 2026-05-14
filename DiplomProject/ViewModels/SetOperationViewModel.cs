using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using DiplomProject.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiplomProject.ViewModels
{
    public partial class SetOperationViewModel : ViewModelBase
    {
        private readonly ISetOperationService _service;
        private readonly HashSet<string> _selectedRegions = new();
        private bool _scoredForCurrent;
        private string _prevStepResult = string.Empty;

        public ObservableCollection<SetOperationTaskDto> Tasks { get; } = new();

        [ObservableProperty] private SetOperationTaskDto? currentTask;
        [ObservableProperty] private int currentTaskIndex;
        [ObservableProperty] private int currentStepIndex;
        [ObservableProperty] private string stepUserAnswer = string.Empty;
        [ObservableProperty] private string stepFeedbackState = "None";
        [ObservableProperty] private string diagramFeedbackState = "None";
        [ObservableProperty] private bool isStepPhase;
        [ObservableProperty] private bool isDiagramPhase;
        [ObservableProperty] private bool isSessionFinished;
        [ObservableProperty] private int score;

        public IRelayCommand NavigateBackCommand { get; }
        public IRelayCommand CheckStepCommand { get; }
        public IRelayCommand NextStepCommand { get; }
        public IRelayCommand<string> ToggleRegionCommand { get; }
        public IRelayCommand CheckDiagramCommand { get; }
        public IRelayCommand NextTaskCommand { get; }
        public IRelayCommand ResetCommand { get; }

        public SetOperationStepDto? CurrentStep =>
            CurrentTask?.Steps.ElementAtOrDefault(CurrentStepIndex);

        public string FullExpression => CurrentTask?.Expression ?? string.Empty;
        public string CurrentStepExpression => CurrentStep?.Expression ?? string.Empty;
        public string CurrentStepDescription => CurrentStep?.Description ?? string.Empty;
        public string TaskProgressText => Tasks.Count == 0 ? "" : $"Задача {CurrentTaskIndex + 1} из {Tasks.Count}";
        public string StepProgressText => (CurrentTask?.Steps.Count ?? 0) > 0
            ? $"Шаг {CurrentStepIndex + 1} из {CurrentTask!.Steps.Count}" : string.Empty;
        public string ScoreText => $"Счёт: {Score}/{Tasks.Count}";

        public bool CanCheckStep => !string.IsNullOrWhiteSpace(StepUserAnswer)
            && StepFeedbackState != "Correct" && CurrentStep is not null;
        public bool CanShowNextStep => StepFeedbackState == "Correct";
        public bool IsStepCorrect => StepFeedbackState == "Correct";
        public bool IsStepIncorrect => StepFeedbackState == "Incorrect";
        public bool IsDiagramCorrect => DiagramFeedbackState == "Correct";
        public bool IsDiagramIncorrect => DiagramFeedbackState == "Incorrect";

        public string PrevStepResultText => string.IsNullOrEmpty(_prevStepResult) ? string.Empty
            : $"Результат предыдущего шага: {{{_prevStepResult}}}";
        public bool HasPrevStepResult => CurrentStepIndex > 0 && !string.IsNullOrEmpty(_prevStepResult);

        public bool IsTwoSetDiagram => CurrentTask?.DiagramType == 2;
        public bool IsThreeSetDiagram => CurrentTask?.DiagramType == 3;

        public bool IsAOnlySelected => _selectedRegions.Contains("a-only");
        public bool IsAbSelected => _selectedRegions.Contains("ab");
        public bool IsBOnlySelected => _selectedRegions.Contains("b-only");
        public bool IsOutsideSelected => _selectedRegions.Contains("outside");
        public bool IsCOnlySelected => _selectedRegions.Contains("c-only");
        public bool IsAcSelected => _selectedRegions.Contains("ac");
        public bool IsBcSelected => _selectedRegions.Contains("bc");
        public bool IsAbcSelected => _selectedRegions.Contains("abc");

        public string SetADisplay => string.IsNullOrEmpty(CurrentTask?.SetA) ? string.Empty : $"A = {{{CurrentTask.SetA}}}";
        public string SetBDisplay => string.IsNullOrEmpty(CurrentTask?.SetB) ? string.Empty : $"B = {{{CurrentTask.SetB}}}";
        public string SetCDisplay => (CurrentTask?.DiagramType == 3 && !string.IsNullOrEmpty(CurrentTask?.SetC))
            ? $"C = {{{CurrentTask.SetC}}}" : string.Empty;
        public string UniversalSetDisplay => string.IsNullOrEmpty(CurrentTask?.UniversalSet) ? string.Empty : $"U = {{{CurrentTask.UniversalSet}}}";
        public bool HasSetC => CurrentTask?.DiagramType == 3 && !string.IsNullOrEmpty(CurrentTask?.SetC);

        public SetOperationViewModel()
        {
            _service = new SetOperationService(db);

            NavigateBackCommand = new RelayCommand(NavigateBack);
            CheckStepCommand = new RelayCommand(CheckStep, () => CanCheckStep);
            NextStepCommand = new RelayCommand(GoToNextStep, () => CanShowNextStep);
            ToggleRegionCommand = new RelayCommand<string>(ToggleRegion);
            CheckDiagramCommand = new RelayCommand(CheckDiagram);
            NextTaskCommand = new RelayCommand(NextTask);
            ResetCommand = new RelayCommand(Reset);

            LoadTasks();
        }

        private void LoadTasks()
        {
            var tasks = _service.GetTasks();
            Tasks.Clear();
            foreach (var t in tasks) Tasks.Add(t);
            CurrentTaskIndex = 0;
            CurrentTask = Tasks.FirstOrDefault();
            ResetTaskState();
        }

        private void ResetTaskState()
        {
            CurrentStepIndex = 0;
            StepUserAnswer = string.Empty;
            StepFeedbackState = "None";
            DiagramFeedbackState = "None";
            _selectedRegions.Clear();
            _scoredForCurrent = false;
            _prevStepResult = string.Empty;

            bool hasSteps = (CurrentTask?.Steps.Count ?? 0) > 0;
            IsStepPhase = hasSteps;
            IsDiagramPhase = !hasSteps;

            NotifyAllRegions();
            NotifyTaskProperties();
            CheckStepCommand.NotifyCanExecuteChanged();
            NextStepCommand.NotifyCanExecuteChanged();
        }

        private void CheckStep()
        {
            if (!CanCheckStep || CurrentStep is null) return;
            var userSet = ParseSet(StepUserAnswer);
            var correctSet = ParseSet(CurrentStep.CorrectAnswer);
            StepFeedbackState = userSet.SetEquals(correctSet) ? "Correct" : "Incorrect";
            CheckStepCommand.NotifyCanExecuteChanged();
            NextStepCommand.NotifyCanExecuteChanged();
        }

        private void GoToNextStep()
        {
            _prevStepResult = StepUserAnswer.Trim('{', '}', ' ');
            var nextIdx = CurrentStepIndex + 1;
            if (nextIdx >= (CurrentTask?.Steps.Count ?? 0))
            {
                IsStepPhase = false;
                IsDiagramPhase = true;
            }
            else
            {
                CurrentStepIndex = nextIdx;
                StepUserAnswer = string.Empty;
                StepFeedbackState = "None";
                OnPropertyChanged(nameof(CurrentStep));
                OnPropertyChanged(nameof(CurrentStepExpression));
                OnPropertyChanged(nameof(CurrentStepDescription));
                OnPropertyChanged(nameof(StepProgressText));
                OnPropertyChanged(nameof(PrevStepResultText));
                OnPropertyChanged(nameof(HasPrevStepResult));
                CheckStepCommand.NotifyCanExecuteChanged();
                NextStepCommand.NotifyCanExecuteChanged();
            }
        }

        private void ToggleRegion(string? code)
        {
            if (string.IsNullOrEmpty(code) || !IsDiagramPhase) return;
            if (!_selectedRegions.Add(code)) _selectedRegions.Remove(code);
            NotifyAllRegions();
        }

        private void CheckDiagram()
        {
            if (CurrentTask is null) return;
            var selected = new HashSet<string>(_selectedRegions);
            var correct = new HashSet<string>(CurrentTask.CorrectRegions);
            bool isCorrect = selected.SetEquals(correct);
            DiagramFeedbackState = isCorrect ? "Correct" : "Incorrect";
            if (isCorrect && !_scoredForCurrent)
            {
                _scoredForCurrent = true;
                Score++;
                OnPropertyChanged(nameof(ScoreText));
            }
            OnPropertyChanged(nameof(IsDiagramCorrect));
            OnPropertyChanged(nameof(IsDiagramIncorrect));
        }

        private void NextTask()
        {
            if (Tasks.Count == 0) return;
            var next = CurrentTaskIndex + 1;
            if (next >= Tasks.Count)
            {
                IsSessionFinished = true;
                return;
            }
            CurrentTaskIndex = next;
            CurrentTask = Tasks[next];
            ResetTaskState();
            OnPropertyChanged(nameof(TaskProgressText));
        }

        private void Reset()
        {
            Score = 0;
            IsSessionFinished = false;
            CurrentTaskIndex = 0;
            CurrentTask = Tasks.FirstOrDefault();
            ResetTaskState();
            OnPropertyChanged(nameof(ScoreText));
            OnPropertyChanged(nameof(TaskProgressText));
        }

        private void NavigateBack()
        {
            var user = MainWindowViewModel.Instance?.CurrentUser;
            if (user is null) MainWindowViewModel.Instance!.CurrentViewModel = new AuthViewModel();
            else MainWindowViewModel.Instance!.CurrentViewModel = new MainViewModel(user);
        }

        private void NotifyAllRegions()
        {
            OnPropertyChanged(nameof(IsAOnlySelected));
            OnPropertyChanged(nameof(IsAbSelected));
            OnPropertyChanged(nameof(IsBOnlySelected));
            OnPropertyChanged(nameof(IsOutsideSelected));
            OnPropertyChanged(nameof(IsCOnlySelected));
            OnPropertyChanged(nameof(IsAcSelected));
            OnPropertyChanged(nameof(IsBcSelected));
            OnPropertyChanged(nameof(IsAbcSelected));
        }

        private void NotifyTaskProperties()
        {
            OnPropertyChanged(nameof(FullExpression));
            OnPropertyChanged(nameof(CurrentStep));
            OnPropertyChanged(nameof(CurrentStepExpression));
            OnPropertyChanged(nameof(CurrentStepDescription));
            OnPropertyChanged(nameof(TaskProgressText));
            OnPropertyChanged(nameof(StepProgressText));
            OnPropertyChanged(nameof(SetADisplay));
            OnPropertyChanged(nameof(SetBDisplay));
            OnPropertyChanged(nameof(SetCDisplay));
            OnPropertyChanged(nameof(UniversalSetDisplay));
            OnPropertyChanged(nameof(HasSetC));
            OnPropertyChanged(nameof(IsTwoSetDiagram));
            OnPropertyChanged(nameof(IsThreeSetDiagram));
            OnPropertyChanged(nameof(CanCheckStep));
            OnPropertyChanged(nameof(HasPrevStepResult));
            OnPropertyChanged(nameof(PrevStepResultText));
        }

        private static HashSet<int> ParseSet(string source)
        {
            if (string.IsNullOrWhiteSpace(source)) return new HashSet<int>();
            return source
                .Trim('{', '}', ' ')
                .Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries)
                .Where(t => int.TryParse(t, out _))
                .Select(int.Parse)
                .ToHashSet();
        }

        partial void OnStepUserAnswerChanged(string value)
        {
            if (StepFeedbackState == "Incorrect")
                StepFeedbackState = "None";
            OnPropertyChanged(nameof(CanCheckStep));
            CheckStepCommand.NotifyCanExecuteChanged();
        }

        partial void OnStepFeedbackStateChanged(string value)
        {
            OnPropertyChanged(nameof(IsStepCorrect));
            OnPropertyChanged(nameof(IsStepIncorrect));
            OnPropertyChanged(nameof(CanShowNextStep));
            OnPropertyChanged(nameof(CanCheckStep));
            CheckStepCommand.NotifyCanExecuteChanged();
            NextStepCommand.NotifyCanExecuteChanged();
        }

        partial void OnCurrentTaskChanged(SetOperationTaskDto? value) => NotifyTaskProperties();
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using DiplomProject.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiplomProject.ViewModels
{
    public partial class RegionIdentificationViewModel : ViewModelBase
    {
        private readonly IRegionIdentificationService _service;

        private static readonly List<int> Numbers2 = new() { 1, 2, 3, 4 };
        private static readonly List<int> Numbers3 = new() { 1, 2, 3, 4, 5, 6, 7, 8 };

        public ObservableCollection<RegionIdentificationTaskDto> Tasks { get; } = new();
        public ObservableCollection<RegionIdentificationElementDto> CurrentElements { get; } = new();

        [ObservableProperty] private RegionIdentificationTaskDto? currentTask;
        [ObservableProperty] private int currentTaskIndex;
        [ObservableProperty] private bool isChecked;
        [ObservableProperty] private int score;
        [ObservableProperty] private bool isSessionFinished;
        [ObservableProperty] private string statusText = string.Empty;

        public IRelayCommand NavigateBackCommand { get; }
        public IRelayCommand CheckAnswersCommand { get; }
        public IRelayCommand NextTaskCommand { get; }
        public IRelayCommand ResetCommand { get; }

        public List<int> AvailableRegionNumbers { get; private set; } = Numbers2;

        public string TaskProgressText => Tasks.Count == 0 ? "" : $"Задача {CurrentTaskIndex + 1} из {Tasks.Count}";
        public string ScoreText => $"Счёт: {Score}/{Tasks.Count}";

        public bool IsTwoSetDiagram  => CurrentTask?.DiagramType == 2;
        public bool IsThreeSetDiagram => CurrentTask?.DiagramType == 3;
        public bool HasCustomImage   => !string.IsNullOrEmpty(CurrentTask?.DiagramImageBase64);
        public string? DiagramImageBase64 => CurrentTask?.DiagramImageBase64;
        public bool ShowFallback2Set => IsTwoSetDiagram && !HasCustomImage;
        public bool ShowFallback3Set => IsThreeSetDiagram && !HasCustomImage;

        public string SetADisplay      => string.IsNullOrEmpty(CurrentTask?.SetA)  ? string.Empty : $"A = {{{CurrentTask.SetA}}}";
        public string SetBDisplay      => string.IsNullOrEmpty(CurrentTask?.SetB)  ? string.Empty : $"B = {{{CurrentTask.SetB}}}";
        public string SetCDisplay      => (CurrentTask?.DiagramType == 3 && !string.IsNullOrEmpty(CurrentTask?.SetC)) ? $"C = {{{CurrentTask.SetC}}}" : string.Empty;
        public string UniversalSetDisplay => string.IsNullOrEmpty(CurrentTask?.UniversalSet) ? string.Empty : $"U = {{{CurrentTask.UniversalSet}}}";
        public bool HasSetC            => CurrentTask?.DiagramType == 3 && !string.IsNullOrEmpty(CurrentTask?.SetC);

        public RegionIdentificationViewModel()
        {
            _service = new RegionIdentificationService(db);

            NavigateBackCommand  = new RelayCommand(NavigateBack);
            CheckAnswersCommand  = new RelayCommand(CheckAnswers);
            NextTaskCommand      = new RelayCommand(NextTask);
            ResetCommand         = new RelayCommand(Reset);

            LoadTasks();
        }

        private void LoadTasks()
        {
            var tasks = _service.GetTasks();
            Tasks.Clear();
            foreach (var t in tasks) Tasks.Add(t);
            CurrentTaskIndex = 0;
            CurrentTask = Tasks.FirstOrDefault();
            LoadCurrentTaskElements();
        }

        private void LoadCurrentTaskElements()
        {
            CurrentElements.Clear();
            if (CurrentTask is null) return;

            foreach (var elem in CurrentTask.Elements)
            {
                CurrentElements.Add(new RegionIdentificationElementDto
                {
                    Id = elem.Id,
                    ElementValue = elem.ElementValue,
                    CorrectRegionNumber = elem.CorrectRegionNumber
                });
            }

            AvailableRegionNumbers = CurrentTask.DiagramType == 3 ? Numbers3 : Numbers2;

            IsChecked = false;
            StatusText = string.Empty;

            OnPropertyChanged(nameof(AvailableRegionNumbers));
            OnPropertyChanged(nameof(TaskProgressText));
            OnPropertyChanged(nameof(SetADisplay));
            OnPropertyChanged(nameof(SetBDisplay));
            OnPropertyChanged(nameof(SetCDisplay));
            OnPropertyChanged(nameof(UniversalSetDisplay));
            OnPropertyChanged(nameof(HasSetC));
            OnPropertyChanged(nameof(IsTwoSetDiagram));
            OnPropertyChanged(nameof(IsThreeSetDiagram));
            OnPropertyChanged(nameof(HasCustomImage));
            OnPropertyChanged(nameof(DiagramImageBase64));
            OnPropertyChanged(nameof(ShowFallback2Set));
            OnPropertyChanged(nameof(ShowFallback3Set));
        }

        private void CheckAnswers()
        {
            if (CurrentTask is null) return;

            int correct = 0;
            foreach (var elem in CurrentElements)
            {
                bool isCorrect = elem.SelectedRegionNumber == elem.CorrectRegionNumber;
                elem.FeedbackState = isCorrect ? "Correct" : "Incorrect";
                if (isCorrect) correct++;
            }

            IsChecked = true;
            StatusText = $"Правильно: {correct} из {CurrentElements.Count}";

            if (correct == CurrentElements.Count && CurrentElements.Count > 0)
            {
                Score++;
                OnPropertyChanged(nameof(ScoreText));
            }
        }

        private void NextTask()
        {
            if (Tasks.Count == 0) return;
            var next = CurrentTaskIndex + 1;
            if (next >= Tasks.Count) { IsSessionFinished = true; return; }
            CurrentTaskIndex = next;
            CurrentTask = Tasks[next];
            LoadCurrentTaskElements();
        }

        private void Reset()
        {
            Score = 0;
            IsSessionFinished = false;
            CurrentTaskIndex = 0;
            CurrentTask = Tasks.FirstOrDefault();
            LoadCurrentTaskElements();
            OnPropertyChanged(nameof(ScoreText));
        }

        private void NavigateBack()
        {
            var user = MainWindowViewModel.Instance?.CurrentUser;
            if (user is null) MainWindowViewModel.Instance!.CurrentViewModel = new AuthViewModel();
            else MainWindowViewModel.Instance!.CurrentViewModel = new MainViewModel(user);
        }

        partial void OnCurrentTaskChanged(RegionIdentificationTaskDto? value)
        {
            OnPropertyChanged(nameof(IsTwoSetDiagram));
            OnPropertyChanged(nameof(IsThreeSetDiagram));
            OnPropertyChanged(nameof(HasCustomImage));
            OnPropertyChanged(nameof(ShowFallback2Set));
            OnPropertyChanged(nameof(ShowFallback3Set));
        }
    }
}

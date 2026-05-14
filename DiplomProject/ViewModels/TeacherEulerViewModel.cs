using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscreteMath.Models;
using DiscreteMath.Services;
using System.Collections.ObjectModel;

namespace DiscreteMath.ViewModels
{
    public partial class TeacherEulerViewModel : ViewModelBase
    {
        private readonly EulerService _eulerService;

        public ObservableCollection<EulerProblemDto> Problems { get; } = new();

        // ── Edit form ────────────────────────────────────────────────────────────
        [ObservableProperty] private bool   isFormVisible;
        [ObservableProperty] private string editTitle       = string.Empty;
        [ObservableProperty] private string editDescription = string.Empty;
        [ObservableProperty] private int    editDifficulty  = 1;
        [ObservableProperty] private bool   editAOnlyCorrect;
        [ObservableProperty] private bool   editAbCorrect;
        [ObservableProperty] private bool   editBOnlyCorrect;
        [ObservableProperty] private string statusMessage = string.Empty;

        public bool HasStatus => !string.IsNullOrEmpty(StatusMessage);

        public int[] Difficulties { get; } = { 1, 2, 3 };

        private bool _isEditing;
        private int  _editingId;

        // ── Commands ─────────────────────────────────────────────────────────────
        public IRelayCommand ShowAddFormCommand { get; }
        public IRelayCommand<EulerProblemDto> ShowEditFormCommand { get; }
        public IRelayCommand SaveCommand { get; }
        public IRelayCommand CancelCommand { get; }
        public IRelayCommand<EulerProblemDto> DeleteCommand { get; }
        public IRelayCommand NavigateBackCommand { get; }

        public TeacherEulerViewModel()
        {
            _eulerService = new EulerService(db);

            ShowAddFormCommand  = new RelayCommand(ShowAddForm);
            ShowEditFormCommand = new RelayCommand<EulerProblemDto>(ShowEditForm);
            SaveCommand         = new RelayCommand(Save);
            CancelCommand       = new RelayCommand(() => { IsFormVisible = false; StatusMessage = string.Empty; });
            DeleteCommand       = new RelayCommand<EulerProblemDto>(Delete);
            NavigateBackCommand = new RelayCommand(NavigateBack);

            LoadProblems();
        }

        private void LoadProblems()
        {
            var items = _eulerService.GetProblems();
            Problems.Clear();
            foreach (var p in items) Problems.Add(p);
        }

        private void ShowAddForm()
        {
            _isEditing       = false;
            _editingId       = 0;
            EditTitle        = string.Empty;
            EditDescription  = string.Empty;
            EditDifficulty   = 1;
            EditAOnlyCorrect = false;
            EditAbCorrect    = false;
            EditBOnlyCorrect = false;
            IsFormVisible    = true;
        }

        private void ShowEditForm(EulerProblemDto? problem)
        {
            if (problem is null) return;
            _isEditing      = true;
            _editingId      = problem.Id;
            EditTitle       = problem.Title;
            EditDescription = problem.Description;
            EditDifficulty  = problem.Difficulty;

            EditAOnlyCorrect = false;
            EditAbCorrect    = false;
            EditBOnlyCorrect = false;
            foreach (var r in problem.Regions)
            {
                if (r.RegionCode == "a-only") EditAOnlyCorrect = r.IsCorrect;
                else if (r.RegionCode == "ab") EditAbCorrect   = r.IsCorrect;
                else if (r.RegionCode == "b-only") EditBOnlyCorrect = r.IsCorrect;
            }

            IsFormVisible = true;
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(EditDescription)) return;

            if (_isEditing)
            {
                _eulerService.UpdateProblem(_editingId,
                    EditTitle, EditDescription, EditDifficulty,
                    EditAOnlyCorrect, EditAbCorrect, EditBOnlyCorrect);
                StatusMessage = "Задача обновлена";
            }
            else
            {
                _eulerService.AddProblem(
                    EditTitle, EditDescription, 2, EditDifficulty,
                    EditAOnlyCorrect, EditAbCorrect, EditBOnlyCorrect);
                StatusMessage = "Задача добавлена";
            }

            IsFormVisible = false;
            OnPropertyChanged(nameof(HasStatus));
            LoadProblems();
        }

        private void Delete(EulerProblemDto? problem)
        {
            if (problem is null) return;
            _eulerService.DeleteProblem(problem.Id);
            StatusMessage = "Задача удалена";
            OnPropertyChanged(nameof(HasStatus));
            LoadProblems();
        }

        private void NavigateBack() =>
            MainWindowViewModel.Instance!.CurrentViewModel = new TeacherDashboardViewModel();
    }
}

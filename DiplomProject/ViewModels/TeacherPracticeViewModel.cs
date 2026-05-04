using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using DiplomProject.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DiplomProject.ViewModels
{
    public partial class TeacherPracticeViewModel : ViewModelBase
    {
        private readonly PracticeService _practiceService;

        public ObservableCollection<PracticeTaskDto> Tasks { get; } = new();

        // ── Edit form ────────────────────────────────────────────────────────────
        [ObservableProperty] private bool   isFormVisible;
        [ObservableProperty] private string editSetA          = string.Empty;
        [ObservableProperty] private string editSetB          = string.Empty;
        [ObservableProperty] private string editOperation     = "union";
        [ObservableProperty] private string editCondition     = string.Empty;
        [ObservableProperty] private string editCorrectAnswer = string.Empty;
        [ObservableProperty] private string statusMessage     = string.Empty;
        [ObservableProperty] private bool   isLoading;

        public bool HasStatus => !string.IsNullOrEmpty(StatusMessage);

        private bool _isEditing;
        private int  _editingId;

        // ── Operations list for picker ───────────────────────────────────────────
        public string[] Operations { get; } = { "union", "intersection", "difference" };

        // ── Commands ─────────────────────────────────────────────────────────────
        public IAsyncRelayCommand LoadCommand { get; }
        public IRelayCommand ShowAddFormCommand { get; }
        public IRelayCommand<PracticeTaskDto> ShowEditFormCommand { get; }
        public IAsyncRelayCommand SaveCommand { get; }
        public IRelayCommand CancelCommand { get; }
        public IAsyncRelayCommand<PracticeTaskDto> DeleteCommand { get; }
        public IRelayCommand NavigateBackCommand { get; }

        public TeacherPracticeViewModel()
        {
            _practiceService = new PracticeService(db);

            LoadCommand       = new AsyncRelayCommand(LoadAsync);
            ShowAddFormCommand = new RelayCommand(ShowAddForm);
            ShowEditFormCommand = new RelayCommand<PracticeTaskDto>(ShowEditForm);
            SaveCommand       = new AsyncRelayCommand(SaveAsync);
            CancelCommand     = new RelayCommand(() => { IsFormVisible = false; StatusMessage = string.Empty; });
            DeleteCommand     = new AsyncRelayCommand<PracticeTaskDto>(DeleteAsync);
            NavigateBackCommand = new RelayCommand(NavigateBack);

            _ = LoadCommand.ExecuteAsync(null);
        }

        private async Task LoadAsync()
        {
            IsLoading = true;
            var items = await _practiceService.GetPracticeTasksAsync();
            Tasks.Clear();
            foreach (var t in items) Tasks.Add(t);
            IsLoading = false;
        }

        private void ShowAddForm()
        {
            _isEditing      = false;
            _editingId      = 0;
            EditSetA          = string.Empty;
            EditSetB          = string.Empty;
            EditOperation     = "union";
            EditCondition     = string.Empty;
            EditCorrectAnswer = string.Empty;
            IsFormVisible   = true;
        }

        private void ShowEditForm(PracticeTaskDto? task)
        {
            if (task is null) return;
            _isEditing        = true;
            _editingId        = task.Id;
            EditSetA          = task.SetA;
            EditSetB          = task.SetB;
            EditOperation     = task.Operation;
            EditCondition     = task.Condition;
            EditCorrectAnswer = task.CorrectAnswer;
            IsFormVisible     = true;
        }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(EditCondition)) return;

            var subtype = EditOperation switch
            {
                "intersection" => 2,
                "difference"   => 3,
                _              => 1
            };

            if (_isEditing)
            {
                await _practiceService.UpdateTaskAsync(_editingId,
                    EditSetA, EditSetB, EditOperation,
                    EditCondition, EditCorrectAnswer);
                StatusMessage = "Задача обновлена";
            }
            else
            {
                await _practiceService.AddTaskAsync(
                    EditSetA, EditSetB, EditOperation,
                    EditCondition, EditCorrectAnswer, subtype);
                StatusMessage = "Задача добавлена";
            }

            IsFormVisible = false;
            OnPropertyChanged(nameof(HasStatus));
            await LoadAsync();
        }

        private async Task DeleteAsync(PracticeTaskDto? task)
        {
            if (task is null) return;
            await _practiceService.DeleteTaskAsync(task.Id);
            StatusMessage = "Задача удалена";
            OnPropertyChanged(nameof(HasStatus));
            await LoadAsync();
        }

        private void NavigateBack() =>
            MainWindowViewModel.Instance!.CurrentViewModel = new TeacherDashboardViewModel();
    }
}

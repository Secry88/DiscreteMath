using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using DiplomProject.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiplomProject.ViewModels
{
    public partial class TeacherSetOperationViewModel : ViewModelBase
    {
        private readonly ISetOperationService _service;

        public ObservableCollection<SetOperationTaskDto> Tasks { get; } = new();

        [ObservableProperty] private bool isFormVisible;
        [ObservableProperty] private string editTitle = string.Empty;
        [ObservableProperty] private string editDescription = string.Empty;
        [ObservableProperty] private string editExpression = string.Empty;
        [ObservableProperty] private string editSetA = string.Empty;
        [ObservableProperty] private string editSetB = string.Empty;
        [ObservableProperty] private string editSetC = string.Empty;
        [ObservableProperty] private string editUniversalSet = string.Empty;
        [ObservableProperty] private int editDiagramType = 2;
        [ObservableProperty] private int editDifficulty = 1;
        [ObservableProperty] private string statusMessage = string.Empty;

        // Correct region checkboxes — 2-set
        [ObservableProperty] private bool editAOnlyCorrect;
        [ObservableProperty] private bool editAbCorrect;
        [ObservableProperty] private bool editBOnlyCorrect;
        [ObservableProperty] private bool editOutsideCorrect;
        // 3-set additional
        [ObservableProperty] private bool editCOnlyCorrect;
        [ObservableProperty] private bool editAcCorrect;
        [ObservableProperty] private bool editBcCorrect;
        [ObservableProperty] private bool editAbcCorrect;

        public ObservableCollection<EditSetStepVm> EditSteps { get; } = new();

        public bool HasStatus => !string.IsNullOrEmpty(StatusMessage);
        public bool IsEditingThreeSet => EditDiagramType == 3;
        public int[] Difficulties { get; } = { 1, 2, 3 };
        public int[] DiagramTypes { get; } = { 2, 3 };

        private bool _isEditing;
        private int _editingId;

        public IRelayCommand ShowAddFormCommand { get; }
        public IRelayCommand<SetOperationTaskDto> ShowEditFormCommand { get; }
        public IRelayCommand SaveCommand { get; }
        public IRelayCommand CancelCommand { get; }
        public IRelayCommand<SetOperationTaskDto> DeleteCommand { get; }
        public IRelayCommand AddStepCommand { get; }
        public IRelayCommand<EditSetStepVm> RemoveStepCommand { get; }
        public IRelayCommand<EditSetStepVm> MoveStepUpCommand { get; }
        public IRelayCommand<EditSetStepVm> MoveStepDownCommand { get; }
        public IRelayCommand NavigateBackCommand { get; }

        public TeacherSetOperationViewModel()
        {
            _service = new SetOperationService(db);

            ShowAddFormCommand    = new RelayCommand(ShowAddForm);
            ShowEditFormCommand   = new RelayCommand<SetOperationTaskDto>(ShowEditForm);
            SaveCommand           = new RelayCommand(Save);
            CancelCommand         = new RelayCommand(() => { IsFormVisible = false; StatusMessage = string.Empty; });
            DeleteCommand         = new RelayCommand<SetOperationTaskDto>(Delete);
            AddStepCommand        = new RelayCommand(() => EditSteps.Add(new EditSetStepVm()));
            RemoveStepCommand     = new RelayCommand<EditSetStepVm>(s => { if (s is not null) EditSteps.Remove(s); });
            MoveStepUpCommand     = new RelayCommand<EditSetStepVm>(MoveStepUp);
            MoveStepDownCommand   = new RelayCommand<EditSetStepVm>(MoveStepDown);
            NavigateBackCommand   = new RelayCommand(() => MainWindowViewModel.Instance!.CurrentViewModel = new TeacherDashboardViewModel());

            LoadTasks();
        }

        private void LoadTasks()
        {
            var items = _service.GetTasks();
            Tasks.Clear();
            foreach (var t in items) Tasks.Add(t);
        }

        private void ShowAddForm()
        {
            _isEditing = false; _editingId = 0;
            ClearEditForm();
            IsFormVisible = true;
        }

        private void ShowEditForm(SetOperationTaskDto? task)
        {
            if (task is null) return;
            _isEditing = true; _editingId = task.Id;

            EditTitle = task.Title;
            EditDescription = task.Description;
            EditExpression = task.Expression;
            EditSetA = task.SetA;
            EditSetB = task.SetB;
            EditSetC = task.SetC;
            EditUniversalSet = task.UniversalSet;
            EditDiagramType = task.DiagramType;
            EditDifficulty = task.Difficulty;

            EditSteps.Clear();
            foreach (var s in task.Steps.OrderBy(x => x.StepNumber))
                EditSteps.Add(new EditSetStepVm { Description = s.Description, Expression = s.Expression, CorrectAnswer = s.CorrectAnswer });

            EditAOnlyCorrect  = task.CorrectRegions.Contains("a-only");
            EditAbCorrect     = task.CorrectRegions.Contains("ab");
            EditBOnlyCorrect  = task.CorrectRegions.Contains("b-only");
            EditOutsideCorrect = task.CorrectRegions.Contains("outside");
            EditCOnlyCorrect  = task.CorrectRegions.Contains("c-only");
            EditAcCorrect     = task.CorrectRegions.Contains("ac");
            EditBcCorrect     = task.CorrectRegions.Contains("bc");
            EditAbcCorrect    = task.CorrectRegions.Contains("abc");

            IsFormVisible = true;
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(EditExpression)) return;
            var dto = BuildDto();

            if (_isEditing) { dto.Id = _editingId; _service.UpdateTask(dto); StatusMessage = "Задание обновлено"; }
            else { _service.AddTask(dto); StatusMessage = "Задание добавлено"; }

            IsFormVisible = false;
            OnPropertyChanged(nameof(HasStatus));
            LoadTasks();
        }

        private SetOperationTaskDto BuildDto()
        {
            var dto = new SetOperationTaskDto
            {
                Title = EditTitle, Description = EditDescription, Expression = EditExpression,
                SetA = EditSetA, SetB = EditSetB, SetC = EditSetC,
                UniversalSet = EditUniversalSet, DiagramType = EditDiagramType, Difficulty = EditDifficulty,
                Steps = EditSteps.Select((s, i) => new SetOperationStepDto
                {
                    StepNumber = i, Description = s.Description,
                    Expression = s.Expression, CorrectAnswer = s.CorrectAnswer
                }).ToList()
            };

            if (EditAOnlyCorrect) dto.CorrectRegions.Add("a-only");
            if (EditAbCorrect) dto.CorrectRegions.Add("ab");
            if (EditBOnlyCorrect) dto.CorrectRegions.Add("b-only");
            if (EditOutsideCorrect) dto.CorrectRegions.Add("outside");
            if (EditDiagramType == 3)
            {
                if (EditCOnlyCorrect) dto.CorrectRegions.Add("c-only");
                if (EditAcCorrect) dto.CorrectRegions.Add("ac");
                if (EditBcCorrect) dto.CorrectRegions.Add("bc");
                if (EditAbcCorrect) dto.CorrectRegions.Add("abc");
            }
            return dto;
        }

        private void Delete(SetOperationTaskDto? task)
        {
            if (task is null) return;
            _service.DeleteTask(task.Id);
            StatusMessage = "Задание удалено";
            OnPropertyChanged(nameof(HasStatus));
            LoadTasks();
        }

        private void MoveStepUp(EditSetStepVm? s)
        {
            if (s is null) return;
            var i = EditSteps.IndexOf(s);
            if (i > 0) EditSteps.Move(i, i - 1);
        }

        private void MoveStepDown(EditSetStepVm? s)
        {
            if (s is null) return;
            var i = EditSteps.IndexOf(s);
            if (i < EditSteps.Count - 1) EditSteps.Move(i, i + 1);
        }

        private void ClearEditForm()
        {
            EditTitle = EditDescription = EditExpression = string.Empty;
            EditSetA = EditSetB = EditSetC = EditUniversalSet = string.Empty;
            EditDiagramType = 2; EditDifficulty = 1;
            EditSteps.Clear();
            EditAOnlyCorrect = EditAbCorrect = EditBOnlyCorrect = EditOutsideCorrect = false;
            EditCOnlyCorrect = EditAcCorrect = EditBcCorrect = EditAbcCorrect = false;
        }

        partial void OnEditDiagramTypeChanged(int value) => OnPropertyChanged(nameof(IsEditingThreeSet));
    }

    public partial class EditSetStepVm : ObservableObject
    {
        [ObservableProperty] private string description = string.Empty;
        [ObservableProperty] private string expression = string.Empty;
        [ObservableProperty] private string correctAnswer = string.Empty;
    }
}

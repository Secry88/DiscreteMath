using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using DiplomProject.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiplomProject.ViewModels
{
    public partial class TeacherRegionIdentificationViewModel : ViewModelBase
    {
        private readonly IRegionIdentificationService _service;

        private static readonly List<int> Numbers2 = new() { 1, 2, 3, 4 };
        private static readonly List<int> Numbers3 = new() { 1, 2, 3, 4, 5, 6, 7, 8 };

        public ObservableCollection<RegionIdentificationTaskDto> Tasks { get; } = new();

        [ObservableProperty] private bool isFormVisible;
        [ObservableProperty] private string editTitle = string.Empty;
        [ObservableProperty] private string editDescription = string.Empty;
        [ObservableProperty] private string editSetA = string.Empty;
        [ObservableProperty] private string editSetB = string.Empty;
        [ObservableProperty] private string editSetC = string.Empty;
        [ObservableProperty] private string editUniversalSet = string.Empty;
        [ObservableProperty] private int editDiagramType = 2;
        [ObservableProperty] private int editDifficulty = 1;
        [ObservableProperty] private string statusMessage = string.Empty;
        [ObservableProperty] private string? editDiagramImageBase64;

        public ObservableCollection<EditRegionElementVm> EditElements { get; } = new();

        public bool HasStatus        => !string.IsNullOrEmpty(StatusMessage);
        public bool HasEditImage     => !string.IsNullOrEmpty(EditDiagramImageBase64);
        public bool IsEditingThreeSet => EditDiagramType == 3;
        public int[] Difficulties    { get; } = { 1, 2, 3 };
        public int[] DiagramTypes    { get; } = { 2, 3 };
        public List<int> AvailableRegionNumbers { get; private set; } = Numbers2;

        private bool _isEditing;
        private int  _editingId;

        public IRelayCommand ShowAddFormCommand { get; }
        public IRelayCommand<RegionIdentificationTaskDto> ShowEditFormCommand { get; }
        public IRelayCommand SaveCommand { get; }
        public IRelayCommand CancelCommand { get; }
        public IRelayCommand<RegionIdentificationTaskDto> DeleteCommand { get; }
        public IRelayCommand AddElementCommand { get; }
        public IRelayCommand<EditRegionElementVm> RemoveElementCommand { get; }
        public IRelayCommand NavigateBackCommand { get; }

        public TeacherRegionIdentificationViewModel()
        {
            _service = new RegionIdentificationService(db);

            ShowAddFormCommand   = new RelayCommand(ShowAddForm);
            ShowEditFormCommand  = new RelayCommand<RegionIdentificationTaskDto>(ShowEditForm);
            SaveCommand          = new RelayCommand(Save);
            CancelCommand        = new RelayCommand(() => { IsFormVisible = false; StatusMessage = string.Empty; });
            DeleteCommand        = new RelayCommand<RegionIdentificationTaskDto>(Delete);
            AddElementCommand    = new RelayCommand(() => EditElements.Add(new EditRegionElementVm()));
            RemoveElementCommand = new RelayCommand<EditRegionElementVm>(e => { if (e is not null) EditElements.Remove(e); });
            NavigateBackCommand  = new RelayCommand(() => MainWindowViewModel.Instance!.CurrentViewModel = new TeacherDashboardViewModel());

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

        public void SetImageData(string base64)
        {
            EditDiagramImageBase64 = base64;
            OnPropertyChanged(nameof(HasEditImage));
        }

        public void ClearImage()
        {
            EditDiagramImageBase64 = null;
            OnPropertyChanged(nameof(HasEditImage));
        }

        private void ShowEditForm(RegionIdentificationTaskDto? task)
        {
            if (task is null) return;
            _isEditing = true; _editingId = task.Id;

            EditTitle         = task.Title;
            EditDescription   = task.Description;
            EditSetA          = task.SetA;
            EditSetB          = task.SetB;
            EditSetC          = task.SetC;
            EditUniversalSet  = task.UniversalSet;
            EditDiagramType   = task.DiagramType;
            EditDifficulty    = task.Difficulty;
            EditDiagramImageBase64 = task.DiagramImageBase64;
            OnPropertyChanged(nameof(HasEditImage));

            UpdateNumbers();

            EditElements.Clear();
            foreach (var e in task.Elements)
            {
                EditElements.Add(new EditRegionElementVm
                {
                    ElementValueText    = e.ElementValue.ToString(),
                    SelectedRegionNumber = e.CorrectRegionNumber
                });
            }

            IsFormVisible = true;
        }

        private void Save()
        {
            var dto = new RegionIdentificationTaskDto
            {
                Title          = EditTitle,
                Description    = EditDescription,
                SetA           = EditSetA,
                SetB           = EditSetB,
                SetC           = EditSetC,
                UniversalSet   = EditUniversalSet,
                DiagramType    = EditDiagramType,
                Difficulty     = EditDifficulty,
                DiagramImageBase64 = EditDiagramImageBase64,
                Elements       = EditElements
                    .Where(e => int.TryParse(e.ElementValueText, out _) && e.SelectedRegionNumber.HasValue)
                    .Select(e => new RegionIdentificationElementDto
                    {
                        ElementValue       = int.Parse(e.ElementValueText),
                        CorrectRegionNumber = e.SelectedRegionNumber!.Value
                    }).ToList()
            };

            if (_isEditing) { dto.Id = _editingId; _service.UpdateTask(dto); StatusMessage = "Задание обновлено"; }
            else { _service.AddTask(dto); StatusMessage = "Задание добавлено"; }

            IsFormVisible = false;
            OnPropertyChanged(nameof(HasStatus));
            LoadTasks();
        }

        private void Delete(RegionIdentificationTaskDto? task)
        {
            if (task is null) return;
            _service.DeleteTask(task.Id);
            StatusMessage = "Задание удалено";
            OnPropertyChanged(nameof(HasStatus));
            LoadTasks();
        }

        private void ClearEditForm()
        {
            EditTitle = EditDescription = EditSetA = EditSetB = EditSetC = EditUniversalSet = string.Empty;
            EditDiagramType   = 2;
            EditDifficulty    = 1;
            EditDiagramImageBase64 = null;
            OnPropertyChanged(nameof(HasEditImage));
            EditElements.Clear();
            UpdateNumbers();
        }

        private void UpdateNumbers()
        {
            AvailableRegionNumbers = EditDiagramType == 3 ? Numbers3 : Numbers2;
            OnPropertyChanged(nameof(AvailableRegionNumbers));
            OnPropertyChanged(nameof(IsEditingThreeSet));
        }

        partial void OnEditDiagramTypeChanged(int value) => UpdateNumbers();
    }

    public partial class EditRegionElementVm : ObservableObject
    {
        [ObservableProperty] private string elementValueText = string.Empty;
        [ObservableProperty] private int? selectedRegionNumber;
    }
}

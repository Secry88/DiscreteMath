using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using DiplomProject.Services;
using System.Collections.ObjectModel;

namespace DiplomProject.ViewModels
{
    public partial class TeacherTheoryViewModel : ViewModelBase
    {
        private readonly TheoryService _theoryService;

        // ── Collections ──────────────────────────────────────────────────────────
        public ObservableCollection<TheoryCategoryDto> Categories { get; } = new();
        public ObservableCollection<TheoryTopicDto>    Topics     { get; } = new();
        public ObservableCollection<TheoryContentDto>  Contents   { get; } = new();

        // ── Selection ────────────────────────────────────────────────────────────
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedCategory))]
        private TheoryCategoryDto? selectedCategory;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedTopic))]
        private TheoryTopicDto? selectedTopic;

        public bool HasSelectedCategory => SelectedCategory != null;
        public bool HasSelectedTopic    => SelectedTopic    != null;

        // ── Category form ────────────────────────────────────────────────────────
        [ObservableProperty] private bool   isCategoryFormVisible;
        [ObservableProperty] private string editCategoryTitle       = string.Empty;
        [ObservableProperty] private string editCategoryDescription = string.Empty;
        private bool _isEditingCategory;
        private int  _editingCategoryId;

        // ── Topic form ───────────────────────────────────────────────────────────
        [ObservableProperty] private bool   isTopicFormVisible;
        [ObservableProperty] private string editTopicTitle = string.Empty;
        private bool _isEditingTopic;
        private int  _editingTopicId;

        // ── Content form ─────────────────────────────────────────────────────────
        [ObservableProperty] private bool   isContentFormVisible;
        [ObservableProperty] private string editContent = string.Empty;
        private bool _isEditingContent;
        private int  _editingContentId;

        // ── Status ───────────────────────────────────────────────────────────────
        [ObservableProperty] private string statusMessage = string.Empty;
        public bool HasStatus => !string.IsNullOrEmpty(StatusMessage);

        // ── Commands ─────────────────────────────────────────────────────────────
        public IRelayCommand<TheoryCategoryDto> SelectCategoryCommand      { get; }
        public IRelayCommand<TheoryTopicDto>    SelectTopicCommand         { get; }

        public IRelayCommand                    ShowAddCategoryFormCommand  { get; }
        public IRelayCommand<TheoryCategoryDto> ShowEditCategoryFormCommand { get; }
        public IRelayCommand                    SaveCategoryCommand         { get; }
        public IRelayCommand                    CancelCategoryCommand       { get; }
        public IRelayCommand<TheoryCategoryDto> DeleteCategoryCommand       { get; }

        public IRelayCommand                 ShowAddTopicFormCommand  { get; }
        public IRelayCommand<TheoryTopicDto> ShowEditTopicFormCommand { get; }
        public IRelayCommand                 SaveTopicCommand         { get; }
        public IRelayCommand                 CancelTopicCommand       { get; }
        public IRelayCommand<TheoryTopicDto> DeleteTopicCommand       { get; }

        public IRelayCommand                   ShowAddContentFormCommand  { get; }
        public IRelayCommand<TheoryContentDto> ShowEditContentFormCommand { get; }
        public IRelayCommand                   SaveContentCommand         { get; }
        public IRelayCommand                   CancelContentCommand       { get; }
        public IRelayCommand<TheoryContentDto> DeleteContentCommand       { get; }

        public IRelayCommand NavigateBackCommand { get; }

        public TeacherTheoryViewModel()
        {
            _theoryService = new TheoryService(db);

            SelectCategoryCommand      = new RelayCommand<TheoryCategoryDto>(SelectCategory);
            SelectTopicCommand         = new RelayCommand<TheoryTopicDto>(SelectTopic);

            ShowAddCategoryFormCommand  = new RelayCommand(ShowAddCategoryForm);
            ShowEditCategoryFormCommand = new RelayCommand<TheoryCategoryDto>(ShowEditCategoryForm);
            SaveCategoryCommand         = new RelayCommand(SaveCategory);
            CancelCategoryCommand       = new RelayCommand(() => { IsCategoryFormVisible = false; StatusMessage = string.Empty; });
            DeleteCategoryCommand       = new RelayCommand<TheoryCategoryDto>(DeleteCategory);

            ShowAddTopicFormCommand  = new RelayCommand(ShowAddTopicForm);
            ShowEditTopicFormCommand = new RelayCommand<TheoryTopicDto>(ShowEditTopicForm);
            SaveTopicCommand         = new RelayCommand(SaveTopic);
            CancelTopicCommand       = new RelayCommand(() => { IsTopicFormVisible = false; StatusMessage = string.Empty; });
            DeleteTopicCommand       = new RelayCommand<TheoryTopicDto>(DeleteTopic);

            ShowAddContentFormCommand  = new RelayCommand(ShowAddContentForm);
            ShowEditContentFormCommand = new RelayCommand<TheoryContentDto>(ShowEditContentForm);
            SaveContentCommand         = new RelayCommand(SaveContent);
            CancelContentCommand       = new RelayCommand(() => { IsContentFormVisible = false; StatusMessage = string.Empty; });
            DeleteContentCommand       = new RelayCommand<TheoryContentDto>(DeleteContent);

            NavigateBackCommand = new RelayCommand(NavigateBack);

            LoadCategories();
        }

        // ── Load ─────────────────────────────────────────────────────────────────

        private void LoadCategories()
        {
            var items = _theoryService.GetCategories();
            Categories.Clear();
            foreach (var c in items) Categories.Add(c);
        }

        private void SelectCategory(TheoryCategoryDto? category)
        {
            SelectedCategory = category;
            SelectedTopic    = null;
            Topics.Clear();
            Contents.Clear();
            IsTopicFormVisible   = false;
            IsContentFormVisible = false;

            if (category is null) return;

            var full = _theoryService.GetCategory(category.Id);
            if (full is null) return;
            foreach (var t in full.Topics) Topics.Add(t);
        }

        private void SelectTopic(TheoryTopicDto? topic)
        {
            SelectedTopic = topic;
            Contents.Clear();
            IsContentFormVisible = false;

            if (topic is null) return;

            var contents = _theoryService.GetContentsForTopic(topic.Id);
            foreach (var c in contents) Contents.Add(c);
        }

        // ── Category CRUD ────────────────────────────────────────────────────────

        private void ShowAddCategoryForm()
        {
            _isEditingCategory    = false;
            _editingCategoryId    = 0;
            EditCategoryTitle       = string.Empty;
            EditCategoryDescription = string.Empty;
            IsCategoryFormVisible   = true;
        }

        private void ShowEditCategoryForm(TheoryCategoryDto? cat)
        {
            if (cat is null) return;
            _isEditingCategory      = true;
            _editingCategoryId      = cat.Id;
            EditCategoryTitle       = cat.Title;
            EditCategoryDescription = cat.Description;
            IsCategoryFormVisible   = true;
        }

        private void SaveCategory()
        {
            if (string.IsNullOrWhiteSpace(EditCategoryTitle)) return;

            if (_isEditingCategory)
                _theoryService.UpdateCategory(_editingCategoryId, EditCategoryTitle, EditCategoryDescription);
            else
                _theoryService.AddCategory(EditCategoryTitle, EditCategoryDescription);

            IsCategoryFormVisible = false;
            StatusMessage = _isEditingCategory ? "Категория обновлена" : "Категория добавлена";
            OnPropertyChanged(nameof(HasStatus));
            LoadCategories();
        }

        private void DeleteCategory(TheoryCategoryDto? cat)
        {
            if (cat is null) return;
            _theoryService.DeleteCategory(cat.Id);
            if (SelectedCategory?.Id == cat.Id)
            {
                SelectedCategory = null;
                Topics.Clear();
                Contents.Clear();
            }
            StatusMessage = "Категория удалена";
            OnPropertyChanged(nameof(HasStatus));
            LoadCategories();
        }

        // ── Topic CRUD ───────────────────────────────────────────────────────────

        private void ShowAddTopicForm()
        {
            if (SelectedCategory is null) return;
            _isEditingTopic = false;
            _editingTopicId = 0;
            EditTopicTitle  = string.Empty;
            IsTopicFormVisible = true;
        }

        private void ShowEditTopicForm(TheoryTopicDto? topic)
        {
            if (topic is null) return;
            _isEditingTopic = true;
            _editingTopicId = topic.Id;
            EditTopicTitle  = topic.Title;
            IsTopicFormVisible = true;
        }

        private void SaveTopic()
        {
            if (string.IsNullOrWhiteSpace(EditTopicTitle) || SelectedCategory is null) return;

            if (_isEditingTopic)
                _theoryService.UpdateTopic(_editingTopicId, EditTopicTitle);
            else
                _theoryService.AddTopic(SelectedCategory.Id, EditTopicTitle);

            IsTopicFormVisible = false;
            StatusMessage = _isEditingTopic ? "Тема обновлена" : "Тема добавлена";
            OnPropertyChanged(nameof(HasStatus));
            SelectCategory(SelectedCategory);
        }

        private void DeleteTopic(TheoryTopicDto? topic)
        {
            if (topic is null) return;
            _theoryService.DeleteTopic(topic.Id);
            if (SelectedTopic?.Id == topic.Id)
            {
                SelectedTopic = null;
                Contents.Clear();
            }
            StatusMessage = "Тема удалена";
            OnPropertyChanged(nameof(HasStatus));
            SelectCategory(SelectedCategory);
        }

        // ── Content CRUD ─────────────────────────────────────────────────────────

        private void ShowAddContentForm()
        {
            if (SelectedTopic is null) return;
            _isEditingContent = false;
            _editingContentId = 0;
            EditContent       = string.Empty;
            IsContentFormVisible = true;
        }

        private void ShowEditContentForm(TheoryContentDto? content)
        {
            if (content is null) return;
            _isEditingContent = true;
            _editingContentId = content.Id;
            EditContent       = content.Content;
            IsContentFormVisible = true;
        }

        private void SaveContent()
        {
            if (string.IsNullOrWhiteSpace(EditContent) || SelectedTopic is null) return;

            if (_isEditingContent)
                _theoryService.UpdateContent(_editingContentId, EditContent);
            else
                _theoryService.AddContent(SelectedTopic.Id, EditContent);

            IsContentFormVisible = false;
            StatusMessage = _isEditingContent ? "Блок обновлён" : "Блок добавлен";
            OnPropertyChanged(nameof(HasStatus));
            SelectTopic(SelectedTopic);
        }

        private void DeleteContent(TheoryContentDto? content)
        {
            if (content is null) return;
            _theoryService.DeleteContent(content.Id);
            StatusMessage = "Блок удалён";
            OnPropertyChanged(nameof(HasStatus));
            SelectTopic(SelectedTopic);
        }

        // ── Navigation ───────────────────────────────────────────────────────────

        private void NavigateBack() =>
            MainWindowViewModel.Instance!.CurrentViewModel = new TeacherDashboardViewModel();
    }
}

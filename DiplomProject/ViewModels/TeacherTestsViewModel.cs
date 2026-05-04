using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using DiplomProject.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DiplomProject.ViewModels
{
    public partial class TeacherTestsViewModel : ViewModelBase
    {
        private readonly TestService _testService;

        // ── Collections ──────────────────────────────────────────────────────────
        public ObservableCollection<TestSelectionItemDto> Tests     { get; } = new();
        public ObservableCollection<TestQuestionDto>      Questions { get; } = new();
        public ObservableCollection<TestAnswerDto>        Answers   { get; } = new();

        // ── Selection ────────────────────────────────────────────────────────────
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedTest))]
        private TestSelectionItemDto? selectedTest;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasSelectedQuestion))]
        private TestQuestionDto? selectedQuestion;

        public bool HasSelectedTest     => SelectedTest     != null;
        public bool HasSelectedQuestion => SelectedQuestion != null;

        // ── Test form ────────────────────────────────────────────────────────────
        [ObservableProperty] private bool   isTestFormVisible;
        [ObservableProperty] private string editTestTitle       = string.Empty;
        [ObservableProperty] private string editTestDescription = string.Empty;
        [ObservableProperty] private string editTestTopic       = string.Empty;
        [ObservableProperty] private int    editTestDifficulty  = 1;
        [ObservableProperty] private int    editTestDuration    = 15;
        private bool _isEditingTest;
        private int  _editingTestId;

        // ── Question form ────────────────────────────────────────────────────────
        [ObservableProperty] private bool   isQuestionFormVisible;
        [ObservableProperty] private string editQuestionText = string.Empty;
        private bool _isEditingQuestion;
        private int  _editingQuestionId;

        // ── Answer form ──────────────────────────────────────────────────────────
        [ObservableProperty] private bool   isAnswerFormVisible;
        [ObservableProperty] private string editAnswerText      = string.Empty;
        [ObservableProperty] private bool   editAnswerIsCorrect;
        private bool _isEditingAnswer;
        private int  _editingAnswerId;

        // ── Status ───────────────────────────────────────────────────────────────
        [ObservableProperty] private string statusMessage = string.Empty;
        [ObservableProperty] private bool   isLoading;
        public bool HasStatus => !string.IsNullOrEmpty(StatusMessage);

        public int[] Difficulties { get; } = { 1, 2, 3 };

        // ── Commands: Tests ──────────────────────────────────────────────────────
        public IRelayCommand                    ShowAddTestFormCommand    { get; }
        public IRelayCommand<TestSelectionItemDto> ShowEditTestFormCommand  { get; }
        public IAsyncRelayCommand               SaveTestCommand           { get; }
        public IRelayCommand                    CancelTestCommand         { get; }
        public IAsyncRelayCommand<TestSelectionItemDto> DeleteTestCommand  { get; }
        public IAsyncRelayCommand<TestSelectionItemDto> SelectTestCommand  { get; }

        // ── Commands: Questions ───────────────────────────────────────────────────
        public IRelayCommand                  ShowAddQuestionFormCommand    { get; }
        public IRelayCommand<TestQuestionDto> ShowEditQuestionFormCommand   { get; }
        public IAsyncRelayCommand             SaveQuestionCommand           { get; }
        public IRelayCommand                  CancelQuestionCommand         { get; }
        public IAsyncRelayCommand<TestQuestionDto> DeleteQuestionCommand    { get; }
        public IAsyncRelayCommand<TestQuestionDto> SelectQuestionCommand    { get; }

        // ── Commands: Answers ─────────────────────────────────────────────────────
        public IRelayCommand                ShowAddAnswerFormCommand   { get; }
        public IRelayCommand<TestAnswerDto> ShowEditAnswerFormCommand  { get; }
        public IAsyncRelayCommand           SaveAnswerCommand          { get; }
        public IRelayCommand                CancelAnswerCommand        { get; }
        public IAsyncRelayCommand<TestAnswerDto> DeleteAnswerCommand   { get; }

        public IRelayCommand NavigateBackCommand { get; }

        public TeacherTestsViewModel()
        {
            _testService = new TestService(db);

            ShowAddTestFormCommand   = new RelayCommand(ShowAddTestForm);
            ShowEditTestFormCommand  = new RelayCommand<TestSelectionItemDto>(ShowEditTestForm);
            SaveTestCommand          = new AsyncRelayCommand(SaveTestAsync);
            CancelTestCommand        = new RelayCommand(() => { IsTestFormVisible = false; });
            DeleteTestCommand        = new AsyncRelayCommand<TestSelectionItemDto>(DeleteTestAsync);
            SelectTestCommand        = new AsyncRelayCommand<TestSelectionItemDto>(SelectTestAsync);

            ShowAddQuestionFormCommand  = new RelayCommand(ShowAddQuestionForm);
            ShowEditQuestionFormCommand = new RelayCommand<TestQuestionDto>(ShowEditQuestionForm);
            SaveQuestionCommand         = new AsyncRelayCommand(SaveQuestionAsync);
            CancelQuestionCommand       = new RelayCommand(() => { IsQuestionFormVisible = false; });
            DeleteQuestionCommand       = new AsyncRelayCommand<TestQuestionDto>(DeleteQuestionAsync);
            SelectQuestionCommand       = new AsyncRelayCommand<TestQuestionDto>(SelectQuestionAsync);

            ShowAddAnswerFormCommand  = new RelayCommand(ShowAddAnswerForm);
            ShowEditAnswerFormCommand = new RelayCommand<TestAnswerDto>(ShowEditAnswerForm);
            SaveAnswerCommand         = new AsyncRelayCommand(SaveAnswerAsync);
            CancelAnswerCommand       = new RelayCommand(() => { IsAnswerFormVisible = false; });
            DeleteAnswerCommand       = new AsyncRelayCommand<TestAnswerDto>(DeleteAnswerAsync);

            NavigateBackCommand = new RelayCommand(NavigateBack);

            _ = LoadTestsAsync();
        }

        // ── Load ─────────────────────────────────────────────────────────────────

        private async Task LoadTestsAsync()
        {
            IsLoading = true;
            var items = await _testService.GetAllTestsForTeacherAsync();
            Tests.Clear();
            foreach (var t in items) Tests.Add(t);
            IsLoading = false;
        }

        private async Task SelectTestAsync(TestSelectionItemDto? test)
        {
            SelectedTest     = test;
            SelectedQuestion = null;
            Questions.Clear();
            Answers.Clear();
            IsQuestionFormVisible = false;
            IsAnswerFormVisible   = false;

            if (test is null) return;

            var questions = await _testService.GetQuestionsAsync(test.Id);
            foreach (var q in questions) Questions.Add(q);
        }

        private async Task SelectQuestionAsync(TestQuestionDto? question)
        {
            SelectedQuestion  = question;
            Answers.Clear();
            IsAnswerFormVisible = false;

            if (question is null) return;

            var answers = await _testService.GetQuestionsAsync(SelectedTest!.Id);
            var target  = answers.Find(q => q.Id == question.Id);
            if (target is null) return;
            foreach (var a in target.Answers) Answers.Add(a);
        }

        // ── Tests CRUD ───────────────────────────────────────────────────────────

        private void ShowAddTestForm()
        {
            _isEditingTest       = false;
            _editingTestId       = 0;
            EditTestTitle        = string.Empty;
            EditTestDescription  = string.Empty;
            EditTestTopic        = string.Empty;
            EditTestDifficulty   = 1;
            EditTestDuration     = 15;
            IsTestFormVisible    = true;
        }

        private void ShowEditTestForm(TestSelectionItemDto? test)
        {
            if (test is null) return;
            _isEditingTest      = true;
            _editingTestId      = test.Id;
            EditTestTitle       = test.Title;
            EditTestDescription = test.Description;
            EditTestTopic       = test.Topic;
            EditTestDifficulty  = test.Difficulty;
            EditTestDuration    = test.DurationMinutes;
            IsTestFormVisible   = true;
        }

        private async Task SaveTestAsync()
        {
            if (string.IsNullOrWhiteSpace(EditTestTitle)) return;

            if (_isEditingTest)
            {
                await _testService.UpdateTestAsync(_editingTestId,
                    EditTestTitle, EditTestDescription, EditTestTopic,
                    EditTestDifficulty, EditTestDuration);
                StatusMessage = "Тест обновлён";
            }
            else
            {
                await _testService.AddTestAsync(
                    EditTestTitle, EditTestDescription, EditTestTopic,
                    EditTestDifficulty, EditTestDuration);
                StatusMessage = "Тест добавлен";
            }

            IsTestFormVisible = false;
            OnPropertyChanged(nameof(HasStatus));
            await LoadTestsAsync();
        }

        private async Task DeleteTestAsync(TestSelectionItemDto? test)
        {
            if (test is null) return;
            await _testService.DeleteTestAsync(test.Id);
            if (SelectedTest?.Id == test.Id)
            {
                SelectedTest = null;
                Questions.Clear();
                Answers.Clear();
            }
            StatusMessage = "Тест удалён";
            OnPropertyChanged(nameof(HasStatus));
            await LoadTestsAsync();
        }

        // ── Questions CRUD ────────────────────────────────────────────────────────

        private void ShowAddQuestionForm()
        {
            if (SelectedTest is null) return;
            _isEditingQuestion = false;
            _editingQuestionId = 0;
            EditQuestionText   = string.Empty;
            IsQuestionFormVisible = true;
        }

        private void ShowEditQuestionForm(TestQuestionDto? question)
        {
            if (question is null) return;
            _isEditingQuestion = true;
            _editingQuestionId = question.Id;
            EditQuestionText   = question.QuestionText;
            IsQuestionFormVisible = true;
        }

        private async Task SaveQuestionAsync()
        {
            if (string.IsNullOrWhiteSpace(EditQuestionText) || SelectedTest is null) return;

            if (_isEditingQuestion)
            {
                await _testService.UpdateQuestionAsync(_editingQuestionId, EditQuestionText);
                StatusMessage = "Вопрос обновлён";
            }
            else
            {
                await _testService.AddQuestionAsync(SelectedTest.Id, EditQuestionText);
                StatusMessage = "Вопрос добавлен";
            }

            IsQuestionFormVisible = false;
            OnPropertyChanged(nameof(HasStatus));
            await SelectTestAsync(SelectedTest);
        }

        private async Task DeleteQuestionAsync(TestQuestionDto? question)
        {
            if (question is null) return;
            await _testService.DeleteQuestionAsync(question.Id);
            if (SelectedQuestion?.Id == question.Id)
            {
                SelectedQuestion = null;
                Answers.Clear();
            }
            StatusMessage = "Вопрос удалён";
            OnPropertyChanged(nameof(HasStatus));
            await SelectTestAsync(SelectedTest);
        }

        // ── Answers CRUD ──────────────────────────────────────────────────────────

        private void ShowAddAnswerForm()
        {
            if (SelectedQuestion is null) return;
            _isEditingAnswer    = false;
            _editingAnswerId    = 0;
            EditAnswerText      = string.Empty;
            EditAnswerIsCorrect = false;
            IsAnswerFormVisible = true;
        }

        private void ShowEditAnswerForm(TestAnswerDto? answer)
        {
            if (answer is null) return;
            _isEditingAnswer    = true;
            _editingAnswerId    = answer.Id;
            EditAnswerText      = answer.AnswerText;
            EditAnswerIsCorrect = answer.IsCorrect;
            IsAnswerFormVisible = true;
        }

        private async Task SaveAnswerAsync()
        {
            if (string.IsNullOrWhiteSpace(EditAnswerText) || SelectedQuestion is null) return;

            if (_isEditingAnswer)
            {
                await _testService.UpdateAnswerAsync(_editingAnswerId, EditAnswerText, EditAnswerIsCorrect);
                StatusMessage = "Ответ обновлён";
            }
            else
            {
                await _testService.AddAnswerAsync(SelectedQuestion.Id, EditAnswerText, EditAnswerIsCorrect);
                StatusMessage = "Ответ добавлен";
            }

            IsAnswerFormVisible = false;
            OnPropertyChanged(nameof(HasStatus));
            await SelectQuestionAsync(SelectedQuestion);
        }

        private async Task DeleteAnswerAsync(TestAnswerDto? answer)
        {
            if (answer is null) return;
            await _testService.DeleteAnswerAsync(answer.Id);
            StatusMessage = "Ответ удалён";
            OnPropertyChanged(nameof(HasStatus));
            await SelectQuestionAsync(SelectedQuestion);
        }

        private void NavigateBack() =>
            MainWindowViewModel.Instance!.CurrentViewModel = new TeacherDashboardViewModel();
    }
}

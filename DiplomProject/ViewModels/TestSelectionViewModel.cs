using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscreteMath.Models;
using DiscreteMath.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DiscreteMath.ViewModels
{
    public partial class TestSelectionViewModel : ViewModelBase
    {
        private readonly ITestService _testService;
        private readonly int _userId;

        public ObservableCollection<TestSelectionItemDto> Tests { get; } = new();

        [ObservableProperty]
        private bool isLoading;

        public IAsyncRelayCommand LoadTestsCommand { get; }
        public IRelayCommand<TestSelectionItemDto> StartTestCommand { get; }
        public IRelayCommand NavigateBackCommand { get; }

        public int AvailableTestsCount => Tests.Count;
        public int CompletedCount => Tests.Count(x => x.IsCompleted);
        public string AverageScoreText =>
            CompletedCount == 0
                ? "0%"
                : $"{(int)Tests.Where(x => x.LastPercentage.HasValue).Average(x => x.LastPercentage!.Value)}%";

        public TestSelectionViewModel()
        {
            _testService = new TestService(db);
            _userId = MainWindowViewModel.Instance?.CurrentUser?.Id ?? 0;

            LoadTestsCommand = new AsyncRelayCommand(LoadTestsAsync);
            StartTestCommand = new RelayCommand<TestSelectionItemDto>(StartTest);
            NavigateBackCommand = new RelayCommand(NavigateBack);

            _ = LoadTestsCommand.ExecuteAsync(null);
        }

        private async Task LoadTestsAsync()
        {
            IsLoading = true;

            var items = await _testService.GetTestsForSelectionAsync(_userId);

            Tests.Clear();
            foreach (var item in items)
            {
                Tests.Add(item);
            }

            IsLoading = false;

            OnPropertyChanged(nameof(AvailableTestsCount));
            OnPropertyChanged(nameof(CompletedCount));
            OnPropertyChanged(nameof(AverageScoreText));
        }

        private void StartTest(TestSelectionItemDto? item)
        {
            if (item is null)
            {
                return;
            }

            MainWindowViewModel.Instance!.CurrentViewModel = new TestViewModel(item.Id);
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
    }
}

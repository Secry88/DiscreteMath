using CommunityToolkit.Mvvm.Input;
using DiscreteMath.Models;
using DiscreteMath.Services;
using System.Collections.ObjectModel;

namespace DiscreteMath.ViewModels
{
    public partial class TheoryCategoryViewModel : ViewModelBase
    {
        private readonly TheoryService _theoryService;

        public ObservableCollection<TheoryCategoryDto> Categories { get; } = new();

        public IRelayCommand<int> OpenCategoryCommand { get; }
        public IRelayCommand NavigateBackCommand { get; }

        public TheoryCategoryViewModel()
        {
            _theoryService = new TheoryService(db);

            OpenCategoryCommand = new RelayCommand<int>(OpenCategory);
            NavigateBackCommand = new RelayCommand(NavigateBack);

            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = _theoryService.GetCategories();

            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        private void OpenCategory(int id)
        {
            var category = _theoryService.GetCategory(id);
            if (category is null)
            {
                return;
            }

            MainWindowViewModel.Instance!.CurrentViewModel = new TheoryDetailViewModel(category);
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

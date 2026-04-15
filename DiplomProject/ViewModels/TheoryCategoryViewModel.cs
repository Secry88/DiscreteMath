using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using DiplomProject.Services;
using System.Collections.ObjectModel;

namespace DiplomProject.ViewModels
{
    public partial class TheoryCategoryViewModel : ViewModelBase
    {
        private readonly TheoryService _theoryService;

        public ObservableCollection<TheoryCategoryDto> Categories { get; }

        public IRelayCommand<int> OpenCategoryCommand { get; }
        public IRelayCommand NavigateBackCommand { get; }

        public TheoryCategoryViewModel()
        {
            _theoryService = new TheoryService(db);

            Categories = new ObservableCollection<TheoryCategoryDto>
    {
        new() { Id = 1, Title = "Множества", Description = "Базовые понятия" },
        new() { Id = 2, Title = "Операции", Description = "Объединение и пересечение" },
        new() { Id = 3, Title = "Диаграммы Эйлера", Description = "Визуализация" }
    };

            OpenCategoryCommand = new RelayCommand<int>(OpenCategory);
            NavigateBackCommand = new RelayCommand(NavigateBack);
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

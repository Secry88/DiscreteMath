using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using System.Collections.ObjectModel;

namespace DiplomProject.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly User _user;

        public string UserName { get; }

        public ObservableCollection<MenuItem> MenuItems { get; }
        public IRelayCommand OpenProfileCommand { get; }
        public IRelayCommand LogoutCommand { get; }

        public MainViewModel(User user)
        {
            _user = user;
            UserName = user.FullName;
            OpenProfileCommand = new RelayCommand(OpenProfile);
            LogoutCommand = new RelayCommand(Logout);

            MenuItems = new ObservableCollection<MenuItem>
        {
            new MenuItem
            {
                Title = "Теория",
                Description = "Изучение теории множеств",
                Icon = "📘",
                Command = new RelayCommand(OpenTheory)
            },
            new MenuItem
            {
                Title = "Практика",
                Description = "Решение задач",
                Icon = "✏️",
                Command = new RelayCommand(OpenPractice)
            },
            new MenuItem
            {
                Title = "Круги Эйлера",
                Description = "Визуальные задания",
                Icon = "🔵",
                Command = new RelayCommand(OpenEuler)
            },
            new MenuItem
            {
                Title = "Тесты",
                Description = "Проверка знаний",
                Icon = "🧪",
                Command = new RelayCommand(OpenTests)
            },
            new MenuItem
            {
                Title = "Результаты",
                Description = "Ваш прогресс",
                Icon = "🏆",
                Command = new RelayCommand(OpenResults)
            }
        };
        }

        // методы-заглушки
        private void OpenTheory() 
        {
            MainWindowViewModel.Instance!.CurrentViewModel = new TheoryCategoryViewModel();
        }
        private void OpenPractice()
        {
            MainWindowViewModel.Instance!.CurrentViewModel = new PracticeViewModel();
        }
        private void OpenEuler()
        {
            MainWindowViewModel.Instance!.CurrentViewModel = new EulerViewModel();
        }
        private void OpenTests()
        {
            MainWindowViewModel.Instance!.CurrentViewModel = new TestSelectionViewModel();
        }
        private void OpenResults() { }

        private void OpenProfile()
        {
            MainWindowViewModel.Instance!.CurrentViewModel = new ProfileViewModel();
        }

        private void Logout()
        {
            MainWindowViewModel.Instance!.CurrentUser = null;
            MainWindowViewModel.Instance!.CurrentViewModel = new AuthViewModel();
        }
    }
}

using CommunityToolkit.Mvvm.Input;
using DiscreteMath.Models;
using System;
using System.Collections.ObjectModel;

namespace DiscreteMath.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly User _user;

        public string UserName { get; }
        // Используем RoleId напрямую, чтобы не зависеть от загрузки навигационного свойства Role
        public bool IsTeacher { get; }

        // Верхняя строка: теория и оценивание
        public ObservableCollection<MenuItem> TopMenuItems { get; }
        // Нижняя строка: практика
        public ObservableCollection<MenuItem> BottomMenuItems { get; }

        public IRelayCommand OpenProfileCommand { get; }
        public IRelayCommand LogoutCommand { get; }
        public IRelayCommand OpenTeacherPanelCommand { get; }

        public MainViewModel(User user)
        {
            _user = user;
            UserName = user.FullName;
            IsTeacher = user.RoleId == 2;

            OpenProfileCommand      = new RelayCommand(OpenProfile);
            LogoutCommand           = new RelayCommand(Logout);
            OpenTeacherPanelCommand = new RelayCommand(OpenTeacherDashboard);

            TopMenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Title = "Теория",    Description = "Изучение теории множеств", Icon = "📘", Command = new RelayCommand(OpenTheory) },
                new MenuItem { Title = "Тесты",     Description = "Проверка знаний",          Icon = "🧪", Command = new RelayCommand(OpenTests) },
                new MenuItem { Title = "Результаты",Description = "Ваш прогресс",             Icon = "🏆", Command = new RelayCommand(OpenResults) },
            };

            BottomMenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Title = "Практика",          Description = "Решение задач с генерацией", Icon = "✏️", Command = new RelayCommand(OpenPractice) },
                new MenuItem { Title = "Круги Эйлера",      Description = "Визуальные задания",         Icon = "🔵", Command = new RelayCommand(OpenEuler) },
                new MenuItem { Title = "Пошаговые задания", Description = "Операции над множествами",   Icon = "🔢", Command = new RelayCommand(OpenSetOperation) },
                new MenuItem { Title = "Области диаграмм",  Description = "Определи область элемента", Icon = "📍", Command = new RelayCommand(OpenRegionIdentification) },
            };

            if (IsTeacher)
                BottomMenuItems.Add(new MenuItem { Title = "Управление", Description = "Редактирование материалов", Icon = "⚙️", Command = new RelayCommand(OpenTeacherDashboard) });
        }

        private void OpenTheory()              => Navigate(new TheoryCategoryViewModel());
        private void OpenPractice()            => Navigate(new PracticeViewModel());
        private void OpenEuler()               => Navigate(new EulerViewModel());
        private void OpenTests()               => Navigate(new TestSelectionViewModel());
        private void OpenSetOperation()        => Navigate(new SetOperationViewModel());
        private void OpenRegionIdentification() => Navigate(new RegionIdentificationViewModel());
        private void OpenResults()             => Navigate(new ResultsViewModel());
        private void OpenTeacherDashboard()    => Navigate(new TeacherDashboardViewModel());

        private void OpenProfile()
        {
            MainWindowViewModel.Instance!.CurrentViewModel = new ProfileViewModel();
        }

        private void Logout()
        {
            MainWindowViewModel.Instance!.CurrentUser = null;
            MainWindowViewModel.Instance!.CurrentViewModel = new AuthViewModel();
        }

        private static void Navigate(ViewModelBase vm) =>
            MainWindowViewModel.Instance!.CurrentViewModel = vm;
    }
}

using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using System.Collections.ObjectModel;

namespace DiplomProject.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public string UserName { get; }

        public ObservableCollection<MenuItem> MenuItems { get; }

        public MainViewModel(User user)
        {
            UserName = user.FullName;

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
        private void OpenTheory() { }
        private void OpenPractice() { }
        private void OpenEuler() { }
        private void OpenTests() { }
        private void OpenResults() { }
    }
}

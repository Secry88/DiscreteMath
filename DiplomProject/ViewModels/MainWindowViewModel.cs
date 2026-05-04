using CommunityToolkit.Mvvm.ComponentModel;
using DiplomProject.Models;
using System;

namespace DiplomProject.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public static MainWindowViewModel? Instance { get; set; }

        [ObservableProperty]
        private ViewModelBase? _currentViewModel;

        [ObservableProperty]
        private User? _currentUser = null;

        public bool IsTeacher => IsTeacherRole(CurrentUser?.Role?.Name);

        private static bool IsTeacherRole(string? roleName) =>
            roleName != null &&
            (roleName.Equals("Teacher",       StringComparison.OrdinalIgnoreCase) ||
             roleName.Equals("Преподаватель", StringComparison.OrdinalIgnoreCase));

        public MainWindowViewModel()
        {
            Instance = this;
            CurrentViewModel = new AuthViewModel();
        }

        partial void OnCurrentUserChanged(User? oldValue, User? newValue)
        {
            OnPropertyChanged(nameof(IsTeacher));
        }
    }
}

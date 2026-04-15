using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiplomProject.ViewModels
{
    public partial class ProfileViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string fullName = string.Empty;

        [ObservableProperty]
        private string login = string.Empty;

        [ObservableProperty]
        private string groupName = "Computer Science Student";

        [ObservableProperty]
        private string roleName = "Student";

        public ObservableCollection<ProfileRecentTestDto> RecentTests { get; } = new();
        public IRelayCommand NavigateBackCommand { get; }

        public int TestsCompleted { get; private set; }
        public string AverageScoreText { get; private set; } = "0%";
        public string LessonsCompletedText => "0 / 0";
        public bool HasRecentTests => RecentTests.Count > 0;

        public ProfileViewModel()
        {
            NavigateBackCommand = new RelayCommand(NavigateBack);
            LoadProfile();
        }

        private void LoadProfile()
        {
            var currentUserId = MainWindowViewModel.Instance?.CurrentUser?.Id;
            if (currentUserId is null)
            {
                return;
            }

            var user = db.Users
                .Include(x => x.Group)
                .Include(x => x.Role)
                .Include(x => x.TestResults)
                .ThenInclude(x => x.Test)
                .FirstOrDefault(x => x.Id == currentUserId.Value);

            if (user is null)
            {
                return;
            }

            MainWindowViewModel.Instance!.CurrentUser = user;

            FullName = user.FullName;
            Login = user.Login;
            GroupName = user.Group?.Name ?? "Computer Science Student";
            RoleName = user.Role?.Name ?? "Student";

            TestsCompleted = user.TestResults.Count;
            AverageScoreText = TestsCompleted == 0
                ? "0%"
                : $"{(int)user.TestResults.Average(x => x.Percentage ?? 0)}%";

            RecentTests.Clear();
            foreach (var result in user.TestResults
                         .OrderByDescending(x => x.CompletedAt ?? DateTime.MinValue)
                         .Take(4))
            {
                RecentTests.Add(new ProfileRecentTestDto
                {
                    Title = result.Test?.Title ?? "Test",
                    ResultText = $"{result.Score}/{result.TotalQuestions} ({result.Percentage ?? 0}%)",
                    DateText = (result.CompletedAt ?? DateTime.Now).ToString("dd MMM yyyy")
                });
            }

            OnPropertyChanged(nameof(TestsCompleted));
            OnPropertyChanged(nameof(AverageScoreText));
            OnPropertyChanged(nameof(HasRecentTests));
            OnPropertyChanged(nameof(LessonsCompletedText));
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

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
        [ObservableProperty] private string fullName = string.Empty;
        [ObservableProperty] private string login = string.Empty;
        [ObservableProperty] private string groupName = string.Empty;
        [ObservableProperty] private string roleName = string.Empty;
        [ObservableProperty] private string? profileImageBase64;

        public ObservableCollection<ProfileRecentTestDto> RecentTests { get; } = new();
        public IRelayCommand NavigateBackCommand { get; }
        public IRelayCommand ChangePhotoCommand { get; }

        public int TestsCompleted { get; private set; }
        public string AverageScoreText { get; private set; } = "—";
        public int TotalTopics { get; private set; }
        public string LessonsCompletedText => $"0 / {TotalTopics}";
        public bool HasRecentTests => RecentTests.Count > 0;
        public bool HasPhoto => !string.IsNullOrEmpty(ProfileImageBase64);

        public ProfileViewModel()
        {
            NavigateBackCommand = new RelayCommand(NavigateBack);
            ChangePhotoCommand = new RelayCommand(ChangePhoto);
            LoadProfile();
        }

        private void LoadProfile()
        {
            var currentUserId = MainWindowViewModel.Instance?.CurrentUser?.Id;
            if (currentUserId is null) return;

            var user = db.Users
                .Include(x => x.Group)
                .Include(x => x.Role)
                .Include(x => x.TestResults)
                    .ThenInclude(x => x.Test)
                .FirstOrDefault(x => x.Id == currentUserId.Value);

            if (user is null) return;

            MainWindowViewModel.Instance!.CurrentUser = user;

            FullName = user.FullName;
            Login = user.Login;
            GroupName = user.Group?.Name ?? "—";
            RoleName = user.Role?.Name ?? "—";
            ProfileImageBase64 = user.ProfileImage;

            TestsCompleted = user.TestResults.Count;
            AverageScoreText = TestsCompleted == 0
                ? "—"
                : $"{(int)user.TestResults.Average(x => x.Percentage ?? 0)}%";

            TotalTopics = db.TheoryTopics.AsNoTracking().Count();

            RecentTests.Clear();
            foreach (var result in user.TestResults
                .OrderByDescending(x => x.CompletedAt ?? DateTime.MinValue)
                .Take(4))
            {
                RecentTests.Add(new ProfileRecentTestDto
                {
                    Title = result.Test?.Title ?? "Тест",
                    ResultText = $"{result.Score}/{result.TotalQuestions} ({result.Percentage ?? 0}%)",
                    DateText = (result.CompletedAt ?? DateTime.Now).ToString("dd.MM.yyyy")
                });
            }

            OnPropertyChanged(nameof(TestsCompleted));
            OnPropertyChanged(nameof(AverageScoreText));
            OnPropertyChanged(nameof(LessonsCompletedText));
            OnPropertyChanged(nameof(HasRecentTests));
            OnPropertyChanged(nameof(HasPhoto));
        }

        public void SetPhoto(string base64)
        {
            var userId = MainWindowViewModel.Instance?.CurrentUser?.Id;
            if (userId is null) return;

            var user = db.Users.Find(userId.Value);
            if (user is null) return;

            user.ProfileImage = base64;
            db.SaveChanges();

            if (MainWindowViewModel.Instance!.CurrentUser is not null)
                MainWindowViewModel.Instance.CurrentUser.ProfileImage = base64;

            ProfileImageBase64 = base64;
            OnPropertyChanged(nameof(HasPhoto));
        }

        private void ChangePhoto() { }

        private void NavigateBack()
        {
            var user = MainWindowViewModel.Instance?.CurrentUser;
            if (user is null) MainWindowViewModel.Instance!.CurrentViewModel = new AuthViewModel();
            else MainWindowViewModel.Instance!.CurrentViewModel = new MainViewModel(user);
        }
    }
}

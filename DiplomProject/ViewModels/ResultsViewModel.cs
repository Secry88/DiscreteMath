using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiplomProject.ViewModels
{
    public partial class ResultsViewModel : ViewModelBase
    {
        public ObservableCollection<TestResultItemDto> Results { get; } = new();

        public IRelayCommand NavigateBackCommand { get; }

        public int TotalTests { get; private set; }
        public string AverageScoreText { get; private set; } = "—";
        public string BestScoreText { get; private set; } = "—";
        public bool HasResults => Results.Count > 0;

        public ResultsViewModel()
        {
            NavigateBackCommand = new RelayCommand(NavigateBack);
            LoadResults();
        }

        private void LoadResults()
        {
            var userId = MainWindowViewModel.Instance?.CurrentUser?.Id;
            if (userId is null) return;

            var results = db.TestResults
                .Include(r => r.Test)
                .AsNoTracking()
                .Where(r => r.UserId == userId.Value)
                .OrderByDescending(r => r.CompletedAt ?? DateTime.MinValue)
                .ToList();

            Results.Clear();
            foreach (var r in results)
            {
                Results.Add(new TestResultItemDto
                {
                    TestTitle = r.Test?.Title ?? "Без названия",
                    Score = r.Score,
                    TotalQuestions = r.TotalQuestions,
                    Percentage = r.Percentage ?? 0,
                    DateText = (r.CompletedAt ?? DateTime.Now).ToString("dd.MM.yyyy HH:mm")
                });
            }

            TotalTests = results.Count;
            AverageScoreText = TotalTests == 0 ? "—" : $"{(int)results.Average(r => r.Percentage ?? 0)}%";
            BestScoreText = TotalTests == 0 ? "—" : $"{results.Max(r => r.Percentage ?? 0)}%";

            OnPropertyChanged(nameof(TotalTests));
            OnPropertyChanged(nameof(AverageScoreText));
            OnPropertyChanged(nameof(BestScoreText));
            OnPropertyChanged(nameof(HasResults));
        }

        private void NavigateBack()
        {
            var user = MainWindowViewModel.Instance?.CurrentUser;
            if (user is null) MainWindowViewModel.Instance!.CurrentViewModel = new AuthViewModel();
            else MainWindowViewModel.Instance!.CurrentViewModel = new MainViewModel(user);
        }
    }
}

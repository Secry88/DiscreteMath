using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscreteMath.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiscreteMath.ViewModels;

public partial class TheoryDetailViewModel : ViewModelBase
{
    public string Title { get; }
    public string Description { get; }

    public ObservableCollection<TheoryTopicDto> Topics { get; }

    [ObservableProperty] private TheoryTopicDto? selectedTopic;
    [ObservableProperty] private bool isCompleted;
    [ObservableProperty] private string completionMessage = string.Empty;

    public IRelayCommand<TheoryTopicDto> SelectTopicCommand { get; }
    public IRelayCommand NavigateBackCommand { get; }
    public IRelayCommand MarkCompletedCommand { get; }

    public string SelectedTopicTitle => SelectedTopic?.Title ?? string.Empty;

    public string SelectedTopicContent =>
        SelectedTopic is null ? string.Empty
        : string.Join("\n\n", SelectedTopic.Contents.Select(c => c.Content));

    public bool IsLastTopicSelected =>
        Topics.Count > 0 && SelectedTopic?.Id == Topics[^1].Id;

    public bool ShowCompletionMessage => !string.IsNullOrEmpty(CompletionMessage);

    public TheoryDetailViewModel(TheoryCategoryDto category)
    {
        Title = category.Title;
        Description = category.Description;

        Topics = new ObservableCollection<TheoryTopicDto>(category.Topics);
        SelectTopicCommand = new RelayCommand<TheoryTopicDto>(SelectTopic);
        NavigateBackCommand = new RelayCommand(NavigateBack);
        MarkCompletedCommand = new RelayCommand(MarkCompleted);

        SelectedTopic = Topics.FirstOrDefault();
        UpdateSelectedTopicState();
    }

    private void SelectTopic(TheoryTopicDto? topic)
    {
        if (topic is null) return;
        SelectedTopic = topic;
        UpdateSelectedTopicState();
    }

    partial void OnSelectedTopicChanged(TheoryTopicDto? value)
    {
        UpdateSelectedTopicState();
        OnPropertyChanged(nameof(SelectedTopicTitle));
        OnPropertyChanged(nameof(SelectedTopicContent));
        OnPropertyChanged(nameof(IsLastTopicSelected));
        CompletionMessage = string.Empty;
        OnPropertyChanged(nameof(ShowCompletionMessage));
    }

    private void UpdateSelectedTopicState()
    {
        foreach (var topic in Topics)
            topic.IsSelected = SelectedTopic?.Id == topic.Id;
    }

    private void MarkCompleted()
    {
        var userId = MainWindowViewModel.Instance?.CurrentUser?.Id;
        if (userId is null || SelectedTopic is null) return;

        var existing = db.UserTheoryProgresses
            .FirstOrDefault(p => p.UserId == userId.Value && p.TopicId == SelectedTopic.Id);

        if (existing is null)
        {
            db.UserTheoryProgresses.Add(new UserTheoryProgress
            {
                UserId = userId.Value,
                TopicId = SelectedTopic.Id,
                CompletedAt = DateTime.Now
            });
            db.SaveChanges();
        }

        CompletionMessage = "✓ Тема отмечена как изученная!";
        OnPropertyChanged(nameof(ShowCompletionMessage));
    }

    private void NavigateBack()
    {
        var user = MainWindowViewModel.Instance?.CurrentUser;
        if (user is null) MainWindowViewModel.Instance!.CurrentViewModel = new AuthViewModel();
        else MainWindowViewModel.Instance!.CurrentViewModel = new MainViewModel(user);
    }
}

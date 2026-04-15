using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace DiplomProject.ViewModels;

public partial class TheoryDetailViewModel : ViewModelBase
{
    public string Title { get; }
    public string Description { get; }

    public ObservableCollection<TheoryTopicDto> Topics { get; }

    [ObservableProperty]
    private TheoryTopicDto? selectedTopic;

    public IRelayCommand<TheoryTopicDto> SelectTopicCommand { get; }
    public IRelayCommand NavigateBackCommand { get; }

    public string SelectedTopicTitle => SelectedTopic?.Title ?? "Topic not selected";

    public ObservableCollection<TheoryContentDto> SelectedTopicContents =>
        SelectedTopic is null
            ? new ObservableCollection<TheoryContentDto>()
            : new ObservableCollection<TheoryContentDto>(SelectedTopic.Contents);

    public TheoryDetailViewModel(TheoryCategoryDto category)
    {
        Title = category.Title;
        Description = category.Description;

        Topics = new ObservableCollection<TheoryTopicDto>(category.Topics);
        SelectTopicCommand = new RelayCommand<TheoryTopicDto>(SelectTopic);

        SelectedTopic = Topics.FirstOrDefault();
        UpdateSelectedTopicState();
    }

    private void SelectTopic(TheoryTopicDto? topic)
    {
        if (topic is null)
        {
            return;
        }

        SelectedTopic = topic;
        UpdateSelectedTopicState();
    }

    partial void OnSelectedTopicChanged(TheoryTopicDto? value)
    {
        UpdateSelectedTopicState();
        OnPropertyChanged(nameof(SelectedTopicTitle));
        OnPropertyChanged(nameof(SelectedTopicContents));
    }

    private void UpdateSelectedTopicState()
    {
        foreach (var topic in Topics)
        {
            topic.IsSelected = SelectedTopic?.Id == topic.Id;
        }
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
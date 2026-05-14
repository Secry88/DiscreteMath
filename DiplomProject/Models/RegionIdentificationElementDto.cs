using CommunityToolkit.Mvvm.ComponentModel;

namespace DiplomProject.Models;

public partial class RegionIdentificationElementDto : ObservableObject
{
    public int Id { get; set; }
    public int ElementValue { get; set; }
    public int CorrectRegionNumber { get; set; }

    [ObservableProperty]
    private int? selectedRegionNumber;

    [ObservableProperty]
    private string feedbackState = "None";

    public bool IsFeedbackCorrect  => FeedbackState == "Correct";
    public bool IsFeedbackIncorrect => FeedbackState == "Incorrect";
    public bool IsFeedbackNone     => FeedbackState == "None";

    partial void OnFeedbackStateChanged(string value)
    {
        OnPropertyChanged(nameof(IsFeedbackCorrect));
        OnPropertyChanged(nameof(IsFeedbackIncorrect));
        OnPropertyChanged(nameof(IsFeedbackNone));
    }
}

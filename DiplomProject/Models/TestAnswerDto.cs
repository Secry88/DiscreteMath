using CommunityToolkit.Mvvm.ComponentModel;

namespace DiplomProject.Models
{
    public partial class TestAnswerDto : ObservableObject
    {
        public int Id { get; set; }
        public string AnswerText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }

        [ObservableProperty]
        private bool isSelected;
    }
}

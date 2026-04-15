using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace DiplomProject.Models
{
    public partial class TheoryTopicDto : ObservableObject
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
        public List<TheoryContentDto> Contents { get; set; } = new();

        [ObservableProperty]
        private bool isSelected;
    }
}

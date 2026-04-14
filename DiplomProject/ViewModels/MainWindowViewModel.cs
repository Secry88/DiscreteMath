using CommunityToolkit.Mvvm.ComponentModel;
using DiplomProject.Models;

namespace DiplomProject.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        
        public static MainWindowViewModel? Instance { get; set; }

        [ObservableProperty]
        private ViewModelBase? _currentViewModel;

        [ObservableProperty]
        private User? _currentUser = null;


        public MainWindowViewModel() 
        {
            Instance = this;
            CurrentViewModel = new AuthViewModel();
        }
    }
}

using CommunityToolkit.Mvvm.Input;

namespace DiscreteMath.ViewModels
{
    public partial class TeacherDashboardViewModel : ViewModelBase
    {
        public IRelayCommand NavigateBackCommand { get; }
        public IRelayCommand ManageTheoryCommand { get; }
        public IRelayCommand ManagePracticeCommand { get; }
        public IRelayCommand ManageEulerCommand { get; }
        public IRelayCommand ManageTestsCommand { get; }
        public IRelayCommand ManageSetOperationCommand { get; }
        public IRelayCommand ManageRegionIdentificationCommand { get; }

        public TeacherDashboardViewModel()
        {
            NavigateBackCommand = new RelayCommand(NavigateBack);
            ManageTheoryCommand  = new RelayCommand(() => Navigate(new TeacherTheoryViewModel()));
            ManagePracticeCommand = new RelayCommand(() => Navigate(new TeacherPracticeViewModel()));
            ManageEulerCommand   = new RelayCommand(() => Navigate(new TeacherEulerViewModel()));
            ManageTestsCommand   = new RelayCommand(() => Navigate(new TeacherTestsViewModel()));
            ManageSetOperationCommand = new RelayCommand(() => Navigate(new TeacherSetOperationViewModel()));
            ManageRegionIdentificationCommand = new RelayCommand(() => Navigate(new TeacherRegionIdentificationViewModel()));
        }

        private static void Navigate(ViewModelBase vm) =>
            MainWindowViewModel.Instance!.CurrentViewModel = vm;

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

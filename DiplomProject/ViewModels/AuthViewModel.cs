using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomProject.ViewModels
{
    public partial class AuthViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? _password = "1234";

        [ObservableProperty]
        private string? _login = "student1";

        [ObservableProperty]
        private string? _messageError;

        public AuthViewModel()
        {
            AuthCommand = new RelayCommand(Auth, CanAuth);
        }

        public IRelayCommand AuthCommand { get; }

        private bool CanAuth()
        {
            return
                !string.IsNullOrWhiteSpace(Password) &&
                !string.IsNullOrWhiteSpace(Login);
        }

        private void ValidateLogin()
        {
            if (String.IsNullOrWhiteSpace(Login))
            {
                MessageError = "Логин не может быть пустым";
            }
            else MessageError = "";
        }

        private void ValidatePassword()
        {
            if (String.IsNullOrWhiteSpace(Password))
            {
                MessageError = "Пароль не может быть пустым";
            }
            else MessageError = "";
        }

        partial void OnLoginChanged(string? oldValue, string? newValue)
        {
            ValidateLogin();
            AuthCommand.NotifyCanExecuteChanged();
        }

        partial void OnPasswordChanged(string? oldValue, string? newValue)
        {
            ValidatePassword();
            AuthCommand.NotifyCanExecuteChanged();
        }

        private void AuthProcess()
        {
            User? user = db.Users.Include(x => x.Role)
                .Include(x => x.Group)                
                .FirstOrDefault(x => x.Login == Login);

            if (user == null)
            {
                MessageError = "Пользователь с таким логином не найден";
                return;
            }

            if (user!.Password != Password)
            {
                MessageError = "Неверный пароль";
            }
            else
            {
                MainWindowViewModel.Instance!.CurrentUser = user;
                MainWindowViewModel.Instance!.CurrentViewModel = new MainViewModel(user);
            }
        }

        private void Auth()
        {
            if (!CanAuth()) return;
            AuthProcess();
        }

        [RelayCommand]
        private void NavigateToRegister()
        {
            MainWindowViewModel.Instance!.CurrentViewModel = new RegisterViewModel();
        }
    }
}

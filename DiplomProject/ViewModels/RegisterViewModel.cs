using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscreteMath.Models;
using System;
using System.Linq;

namespace DiscreteMath.ViewModels
{
    public partial class RegisterViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string? _login;

        [ObservableProperty]
        private string? _password;

        [ObservableProperty]
        private string? _fullName;

        [ObservableProperty]
        private string? _messageToUser;

        public RegisterViewModel()
        {
            RegisterCommand = new RelayCommand(Register, CanRegister);
        }

        public IRelayCommand RegisterCommand { get; }

        private bool CanRegister()
        {
            return !string.IsNullOrWhiteSpace(Login) &&
                   IsValidEmail(Login) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   Password.Length >= 4 &&
                   IsLoginUnique(Login);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsLoginUnique(string? login)
        {
            if (string.IsNullOrWhiteSpace(login)) return false;
            return db.Users.FirstOrDefault(x => x.Login == login) == null;
        }

        partial void OnLoginChanged(string? oldValue, string? newValue)
        {
            if (!string.IsNullOrWhiteSpace(newValue) && !IsLoginUnique(newValue))
            {
                MessageToUser = "Пользователь с таким логином уже существует";
            }
            else if (!string.IsNullOrWhiteSpace(newValue) && !IsValidEmail(newValue))
            {
                MessageToUser = "Введите корректный email адрес";
            }
            else
            {
                MessageToUser = string.Empty;
            }

            RegisterCommand.NotifyCanExecuteChanged();
        }

        partial void OnPasswordChanged(string? oldValue, string? newValue)
        {
            if (string.IsNullOrWhiteSpace(newValue))
            {
                MessageToUser = "Пароль не может быть пустым";
            }
            else if (newValue.Length < 4)
            {
                MessageToUser = "Пароль должен содержать минимум 4 символа";
            }
            else
            {
                MessageToUser = string.Empty;
            }

            RegisterCommand.NotifyCanExecuteChanged();
        }

        private void Register()
        {
            if (!CanRegister())
                return;

            User user = new User
            {
                Login = Login!,
                Password = Password,
                RoleId = 2,
                FullName = FullName
            };

            db.Users.Add(user);
            db.SaveChanges();

            Login = null;
            Password = null;
            FullName = null;

            MainWindowViewModel.Instance!.CurrentViewModel = new AuthViewModel();
        }

        [RelayCommand]
        private void NavigateToLogin()
        {
            MainWindowViewModel.Instance!.CurrentViewModel = new AuthViewModel();
        }
    }
}
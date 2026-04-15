using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiplomProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiplomProject.ViewModels
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
        private string? _secondPassword;

        [ObservableProperty]
        private string? _messageToUser;

        [ObservableProperty]
        private bool _isVerifyFieldVisible;

       

        public RegisterViewModel()
        {

            RegisterCommand = new RelayCommand(Register, CanRegister);
        }

        public IRelayCommand RegisterCommand { get; }

        private bool CanRegister()
        {
            return !IsVerifyFieldVisible &&
                   !string.IsNullOrWhiteSpace(Login) &&
                   IsValidEmail(Login) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(SecondPassword) &&
                   IsPasswordStrong(Password) &&
                   Password == SecondPassword &&
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

        private bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = Regex.IsMatch(password, @"[!@#$%^&*(),.?"":{}|<>]");
            return hasDigit && hasSpecialChar;
        }

        private bool IsLoginUnique(string? login)
        {
            if (string.IsNullOrWhiteSpace(login)) return false;
            User? existingUser = db.Users.FirstOrDefault(x => x.Login == login);
            return existingUser == null;
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
            ValidatePasswords();
            RegisterCommand.NotifyCanExecuteChanged();
        }

        partial void OnSecondPasswordChanged(string? oldValue, string? newValue)
        {
            ValidatePasswords();
            RegisterCommand.NotifyCanExecuteChanged();
        }

        private void ValidatePasswords()
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                MessageToUser = "Пароль не может быть пустым";
            }
            else if (Password.Length < 8)
            {
                MessageToUser = "Пароль должен быть не меньше 8 символов";
            }
            else if (!IsPasswordStrong(Password))
            {
                MessageToUser = "Пароль должен содержать хотя бы одну цифру и один спецсимвол (!@#$%^&* и т.п.)";
            }
            else if (!string.IsNullOrWhiteSpace(SecondPassword) && SecondPassword != Password)
            {
                MessageToUser = "Пароли не совпадают";
            }
            else
            {
                MessageToUser = string.Empty;
            }
        }

        private void Register()
        {
            if (!CanRegister())
            {
                return;
            }

            IsVerifyFieldVisible = true;
            RegisterCommand.NotifyCanExecuteChanged();

            IsVerifyFieldVisible = false;

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
            SecondPassword = null;
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

    using System.Data.Entity;
    using System.Windows;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using ArtDesign.Database;
    using ArtDesign.Helpers;
    using ArtDesign.Services;

    namespace ArtDesign.ViewModel
    {
        public class LoginViewModel : ViewModelHelper
        {
            private readonly IAuthorizationService _authService;
            private readonly NavigationService _navService;
            private readonly ShellViewModel _shellVm;

            public LoginViewModel(IAuthorizationService authService, NavigationService navService)
            {
                _authService = authService;
                _navService = navService;
                _shellVm = navService.ShellViewModel;

                LoginCommand = new RelayCommand(async _ => await ExecuteLogin());
            }

            private string _login;
            public string Login
            {
                get => _login;
                set => SetField(ref _login, value);
            }

            private string _password;
            public string Password
            {
                get => _password;
                set => SetField(ref _password, value);
            }

            public ICommand LoginCommand { get; }

            private async Task ExecuteLogin()
            {
                var employee = await _authService.AuthorizeAsync(Login, Password);
                if (employee == null)
                {
                    MessageBox.Show("Неверный логин или пароль!");
                    return;
                }

                // Успешно - говорим ShellViewModel, что логин закончился
                _shellVm.InitNavigation(_navService);
                _shellVm.OnLoginSuccess(employee);
            }
        }
    }

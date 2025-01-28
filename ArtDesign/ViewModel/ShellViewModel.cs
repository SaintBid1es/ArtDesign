using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;
using ArtDesign.Services;

namespace ArtDesign.ViewModel
{
  public class ShellViewModel : ViewModelHelper
    {
        private INavigationService _navigationService;

        private Employee _currentUser;
        public Employee CurrentUser
        {
            get => _currentUser;
            set
            {
                if (SetField(ref _currentUser, value))
                {
                    OnPropertyChanged(nameof(IsLoggedIn));
                }
            }
        }
        DataManager _dataManager;
        public bool IsLoggedIn => CurrentUser != null;

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set => SetField(ref _currentViewModel, value);
        }

        // Коллекция пунктов меню (кнопок) для навигации
        public ObservableCollection<NavItem> NavItems { get; } 
            = new ObservableCollection<NavItem>();

        public ShellViewModel(DataManager dataManager)
        {
            _dataManager = dataManager;
        }
        public ShellViewModel()
        {
        }

        public void InitNavigation(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        /// <summary>
        /// Вызывается LoginViewModel, когда авторизация прошла успешно.
        /// Передаём авторизованного сотрудника (с ролью).
        /// </summary>
        public void OnLoginSuccess(Employee user)
        {
            CurrentUser = user;
            CurrentViewModel = null;
            SetupNavigation(); // формируем список кнопок меню
        }

        private void SetupNavigation()
        {
            NavItems.Clear();

            // Проекты доступны всем
            NavItems.Add(new NavItem
            {
                Title = "Проекты",
                Command = new RelayCommand(_ => NavigateToProjects())
            });

            // «Клиенты» - только менеджер
            if (IsManager())
            {
                NavItems.Add(new NavItem
                {
                    Title = "Клиенты",
                    Command = new RelayCommand(_ => NavigateToClients())
                });
            }

            // «Сотрудники» - только менеджер
            if (IsManager())
            {
                NavItems.Add(new NavItem
                {
                    Title = "Сотрудники",
                    Command = new RelayCommand(_ => NavigateToEmployees())
                });
            }

            // «Отчёты» - менеджер + руководитель
            if (IsManager() || IsBoss())
            {
                NavItems.Add(new NavItem
                {
                    Title = "Отчёты",
                    Command = new RelayCommand(_ => NavigateToReports())
                });
            }
        }

        private void NavigateToProjects()
        {
            _navigationService.NavigateTo<ProjectsViewModel>(CurrentUser, _dataManager); 
        }

        private void NavigateToClients()
        {
            _navigationService.NavigateTo<ClientsViewModel>(CurrentUser, _dataManager); 
        }

        private void NavigateToEmployees()
        {
            _navigationService.NavigateTo<EmployeesViewModel>(CurrentUser, _dataManager); 
        }

        private void NavigateToReports()
        {
            _navigationService.NavigateTo<ReportsViewModel>(CurrentUser, _dataManager); 
        }

        private bool IsManager() => CurrentUser?.Role?.RoleName?.ToLower() == "менеджер";
        private bool IsDesigner() => CurrentUser?.Role?.RoleName?.ToLower() == "дизайнер";
        private bool IsBoss() => CurrentUser?.Role?.RoleName?.ToLower() == "руководитель";
    }

    public class NavItem : ViewModelHelper
    {
        public string Title { get; set; }
        public ICommand Command { get; set; }
    }
}
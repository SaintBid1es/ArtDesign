using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ArtDesign.Database;
using ArtDesign.Services;
using ArtDesign.View;
using ArtDesign.ViewModel;

namespace ArtDesign
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var context = new ArtDesignDbContext();
            var dataManager = new DataManager(context);

            var shellVm = new ShellViewModel(dataManager);

            var navService = new NavigationService(shellVm);

            navService.Register<LoginViewModel, LoginView>();
            navService.Register<ClientsViewModel, ClientsView>();
            navService.Register<EmployeesViewModel, EmployeesView>();
            navService.Register<ProjectsViewModel, ProjectsView>();
            navService.Register<ReportsViewModel, ReportsView>();
            

            var authService = new AuthorizationService(dataManager);

            var loginVm = new LoginViewModel(authService, navService);
            shellVm.CurrentViewModel = loginVm;

            var shellWindow = new ShellView
            {
                DataContext = shellVm
            };
            shellWindow.Show();
        }
    } 
}


using System;
using System.Collections.Generic;
using System.Windows;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;
using ArtDesign.ViewModel;

namespace ArtDesign.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Dictionary<Type, Type> _mappings = new Dictionary<Type, Type>();

        internal readonly ShellViewModel ShellViewModel;

        public NavigationService(ShellViewModel shellViewModel)
        {
            ShellViewModel = shellViewModel;
        }

        public void Register<TViewModel, TView>()
            where TViewModel : class
            where TView : FrameworkElement
        {
            _mappings[typeof(TViewModel)] = typeof(TView);
        }

        public void NavigateTo<TViewModel>() where TViewModel : class
        {
            NavigateTo<TViewModel>(null);
        }

        public void NavigateTo<TViewModel>(object parameter) where TViewModel : class
        {
            // Создаем ViewModel
            var vmType = typeof(TViewModel);
            var vm = Activator.CreateInstance(vmType);

            // Если у VM есть метод Init(object parameter), можем вызвать
            var initMethod = vmType.GetMethod("Init");
            if (initMethod != null && parameter != null)
            {
                initMethod.Invoke(vm, new[] { parameter });
            }

            ShellViewModel.CurrentViewModel = vm as ViewModelHelper;
        }
        public void NavigateTo<TViewModel>(Employee user, DataManager dataManager) where TViewModel : class
        {
            var args = new NavigationArgs 
            {
                CurrentUser = user,
                DataManager = dataManager
            };
            NavigateTo<TViewModel>(args);
        }
    }
    public class NavigationArgs
    {
        public Employee CurrentUser { get; set; }
        public DataManager DataManager { get; set; }
        public object ExtraParameter { get; set; }
        // при необходимости добавьте другие поля
    }
}
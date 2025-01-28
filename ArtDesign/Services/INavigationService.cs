using System.Windows;
using ArtDesign.Database;
using ArtDesign.Model;

namespace ArtDesign.Services
{
    public interface INavigationService
    {
        /// <summary>
        /// Регистрирует связь: typeof(TViewModel) -> typeof(TView)
        /// </summary>
        void Register<TViewModel, TView>()
            where TViewModel : class
            where TView : FrameworkElement;

        /// <summary>
        /// Создаёт объект VM (через фабрику/DI) и открывает его во «фрейме».
        /// </summary>
        void NavigateTo<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Или перегрузка: NavigateTo c передачей параметра (role, ID, т.п.)
        /// </summary>
        void NavigateTo<TViewModel>(object parameter) where TViewModel : class;
        // (Необязательный) Добавляем ещё одну перегрузку для 2-х параметров
        // Но рекомендую использовать NavigationArgs, чтобы не плодить перегрузки
        void NavigateTo<TViewModel>(Employee user, DataManager dataManager) where TViewModel : class;
    }
}


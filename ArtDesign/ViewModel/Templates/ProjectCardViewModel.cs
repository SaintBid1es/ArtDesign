using System.Windows;
using System.Windows.Input;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;
using ArtDesign.View.Templates.Windows;
using ArtDesign.ViewModel.Templates.Windows;

namespace ArtDesign.ViewModel.Templates
{
    public class ProjectCardViewModel : ViewModelHelper
    {
        private readonly DataManager _dataManager;
        public Project Model { get; }

        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        // Новая команда "Подробнее"
        public ICommand MoreInfoCommand { get; }

        public ProjectCardViewModel(DataManager dataManager, Project project)
        {
            _dataManager = dataManager;
            Model = project;

            EditCommand = new RelayCommand(_ => EditProject());
            DeleteCommand = new RelayCommand(_ => DeleteProject());
            MoreInfoCommand = new RelayCommand(_ => ShowDetails());
        }

        private void EditProject()
        {
            var vm = new EditProjectWindowViewModel(_dataManager, Model, isNew: false);
            var wnd = new EditProjectWindow { DataContext = vm };
            bool? result = wnd.ShowDialog();
            if (result == true)
            {
                // Обновляемся
                OnPropertyChanged(nameof(Model));
            }
        }

        private async void DeleteProject()
        {
            var res = MessageBox.Show(
                "Вы уверены, что хотите удалить проект?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                await _dataManager.Projects.DeleteAsync(Model.ID);
            }
        }

        private void ShowDetails()
        {
            // Открываем окно подробной информации
            var detailsVm = new ProjectDetailsViewModel(_dataManager, Model);
            var detailsWindow = new ProjectDetailsWindow
            {
                DataContext = detailsVm
            };
            detailsWindow.Show();
        }
    }
}
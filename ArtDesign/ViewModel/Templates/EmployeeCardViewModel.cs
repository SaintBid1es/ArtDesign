using System.Windows;
using System.Windows.Input;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;
using ArtDesign.View.Templates.Windows;
using ArtDesign.ViewModel.Templates.Windows;

namespace ArtDesign.ViewModel.Templates
{
    public class EmployeeCardViewModel : ViewModelHelper
    {
        private readonly DataManager _dataManager;

        public Employee Model { get; }

        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        // Новые свойства
        public string FullName => $"{Model.Surname} {Model.Name} {Model.Patronymic}";
        public int WorkStageCount => Model.WorkStages?.Count ?? 0;

        public EmployeeCardViewModel(DataManager dataManager, Employee employee)
        {
            _dataManager = dataManager;
            Model = employee;

            EditCommand = new RelayCommand(_ => EditEmployee());
            DeleteCommand = new RelayCommand(_ => DeleteEmployee());
        }

        private void EditEmployee()
        {
            // Открываем окно редактирования
            var vm = new EditEmployeeWindowViewModel(_dataManager, Model, isNew: false);
            var window = new EditEmployeeWindow { DataContext = vm };
            bool? result = window.ShowDialog();

            if (result == true)
            {
                OnPropertyChanged(nameof(Model));
                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(WorkStageCount));
            }
        }

        private async void DeleteEmployee()
        {
            var res = MessageBox.Show(
                "Вы уверены, что хотите удалить сотрудника?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                await _dataManager.Employees.DeleteAsync(Model.ID);
            }
        }
    }
}
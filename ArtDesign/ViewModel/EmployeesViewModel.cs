using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;
using ArtDesign.Services;
using ArtDesign.View.Templates.Windows;
using ArtDesign.ViewModel.Templates;
using ArtDesign.ViewModel.Templates.Windows;

namespace ArtDesign.ViewModel
{
    public class EmployeesViewModel : ViewModelHelper
    {
        private DataManager _dataManager;
        private Employee _currentEmployee;

        public ObservableCollection<EmployeeCardViewModel> Employees { get; set; }
            = new ObservableCollection<EmployeeCardViewModel>();

        public RelayCommand AddEmployeeCommand { get; set; }

        public EmployeesViewModel() { }
        public void Init(object parameter)
        {
            if (parameter is NavigationArgs args)
            {
                _currentEmployee = args.CurrentUser;
                _dataManager = args.DataManager;
            }
            AddEmployeeCommand = new RelayCommand(_ => AddEmployee());

            _ = LoadEmployeesAsync();
        }

        private async Task LoadEmployeesAsync()
        {
            Employees.Clear();

            var all = await _dataManager.Employees.GetAllAsync();
            foreach(var emp in all)
            {
                var cardVm = new EmployeeCardViewModel(_dataManager, emp);
                Employees.Add(cardVm);
            }
        }

        private void AddEmployee()
        {
            // Создаём нового сотрудника
            var newEmp = new Employee();
            var vm = new EditEmployeeWindowViewModel(_dataManager, newEmp, isNew: true);

            var wnd = new EditEmployeeWindow { DataContext = vm };
            bool? result = wnd.ShowDialog();

            if (result == true)
            {
                // Обновить список
                _ = LoadEmployeesAsync();
            }
        }
    }
}
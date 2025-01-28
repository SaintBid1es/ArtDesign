using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;

namespace ArtDesign.ViewModel.Templates.Windows
{
    public class EditEmployeeWindowViewModel : ViewModelHelper, IDataErrorInfo
    {
        private readonly DataManager _dataManager;
        private readonly bool _isNew;

        public Employee Employee { get; set; }
        public ObservableCollection<Role> Roles { get; set; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public EditEmployeeWindowViewModel(DataManager dataManager, Employee employee, bool isNew)
        {
            _dataManager = dataManager;
            Employee = employee;
            _isNew = isNew;

            SaveCommand = new RelayCommand(_ => ExecuteSave());
            CancelCommand = new RelayCommand(_ => ExecuteCancel());

            LoadRoles();
        }

        private async void LoadRoles()
        {
            var roles = await _dataManager.Roles.GetAllAsync();
            Roles = new ObservableCollection<Role>(roles);
            OnPropertyChanged(nameof(Roles));
        }
        private async void ExecuteSave()
        {
            // Валидация
            if(!IsValid())
            {
                MessageBox.Show("Заполните обязательные поля (Фамилия, Имя, Логин, Пароль).");
                return;
            }

            if (_isNew)
            {
                var ok = await _dataManager.Employees.AddAsync(Employee);
                if(!ok) 
                {
                    MessageBox.Show("Ошибка при добавлении сотрудника");
                    return;
                }
            }
            else
            {
                var ok = await _dataManager.Employees.UpdateAsync(Employee);
                if(!ok)
                {
                    MessageBox.Show("Ошибка при обновлении сотрудника");
                    return;
                }
            }

            CloseWindow(true);
        }

        private void ExecuteCancel()
        {
            CloseWindow(false);
        }

        private void CloseWindow(bool result)
        {
            var wnd = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(x => x.DataContext == this);
            if (wnd != null)
            {
                wnd.DialogResult = result;
                wnd.Close();
            }
        }

        #region IDataErrorInfo


        public string this[string columnName]
        {
            get
            {
                switch(columnName)
                {
                    case nameof(Employee.Surname):
                        if (string.IsNullOrWhiteSpace(Employee.Surname))
                            return "Фамилия не может быть пустой";
                        break;
                    case nameof(Employee.Name):
                        if (string.IsNullOrWhiteSpace(Employee.Name))
                            return "Имя не может быть пустым";
                        break;
                    case nameof(Employee.Login):
                        if (string.IsNullOrWhiteSpace(Employee.Login))
                            return "Логин не может быть пустым";
                        break;
                    case nameof(Employee.Password):
                        if (string.IsNullOrWhiteSpace(Employee.Password))
                            return "Пароль не может быть пустым";
                        break;
                }
                return null;
            }
        }

        public string Error => null;

        private bool IsValid()
        {
            return string.IsNullOrEmpty(this[nameof(Employee.Surname)]) &&
                   string.IsNullOrEmpty(this[nameof(Employee.Name)]) &&
                   string.IsNullOrEmpty(this[nameof(Employee.Login)]) &&
                   string.IsNullOrEmpty(this[nameof(Employee.Password)]);
        }

        #endregion
    }
}
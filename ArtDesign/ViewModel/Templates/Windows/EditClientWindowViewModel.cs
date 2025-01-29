using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;

namespace ArtDesign.ViewModel.Templates.Windows
{
    public class EditClientWindowViewModel : ViewModelHelper, IDataErrorInfo
    {
        private readonly DataManager _dataManager;
        private readonly bool _isNew;
        private static readonly Regex onlyNumbers = new Regex("[^0-9.-]+");
        public Client Client { get; set; }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public EditClientWindowViewModel(DataManager dataManager, Client client, bool isNew)
        {
            _dataManager = dataManager;
            Client = client;
            _isNew = isNew;

            SaveCommand = new RelayCommand(_ => ExecuteSave());
            CancelCommand = new RelayCommand(_ => ExecuteCancel());
        }

        private async void ExecuteSave()
        {
            if (!IsValid())
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }


            if (Client.Surname.Length < 4 || Client.Surname.Length > 20)
            {
                MessageBox.Show("Фамилию от 4 до 20 символов");
                return;
            }
            if (Client.Phone.ToString().Length < 4 || Client.Phone.ToString().Length > 8)
            {
                MessageBox.Show("Телефон от 4 до 8 символов");
                return;
            }
            if (Client.Name.Length < 4 || Client.Name.Length > 20)
            {
                MessageBox.Show("Имя от 4 до 20 символов");
                return;
            }
            if (!Regex.Match(Client.Name, @"[а-яА-Я]|[a-zA-Z]").Success)
            {
                MessageBox.Show("Только буквы в имени");
                return;
            }
            if (!Regex.Match(Client.Surname, @"[а-яА-Я]|[a-zA-Z]").Success)
            {
                MessageBox.Show("Только буквы в фамилии");
                return;
            }
           
           

                if (_isNew)
            {
                var ok = await _dataManager.Clients.AddAsync(Client);
                if (!ok) 
                {
                    MessageBox.Show("Ошибка при добавлении клиента.");
                    return;
                }
            }
            else
            {
                var ok = await _dataManager.Clients.UpdateAsync(Client);
                if (!ok)
                {
                    MessageBox.Show("Ошибка при обновлении клиента.");
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

        #region Validation (IDataErrorInfo)

        // Обязательные поля: Surname, Name, Email, NameCompany
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Client.Surname):
                        if (string.IsNullOrWhiteSpace(Client.Surname))
                            return "Фамилия не может быть пустой.";
                        break;
                    case nameof(Client.Name):
                        if (string.IsNullOrWhiteSpace(Client.Name))
                            return "Имя не может быть пустым.";
                        break;
                    case nameof(Client.Email):
                        if (string.IsNullOrWhiteSpace(Client.Email))
                            return "Email не может быть пустым.";
                        break;
                    case nameof(Client.NameCompany):
                        if (string.IsNullOrWhiteSpace(Client.NameCompany))
                            return "Название компании не может быть пустым.";
                        break;
                }
                return null;
            }
        }

        public string Error => null;

        private bool IsValid()
        {
            return string.IsNullOrEmpty(this[nameof(Client.Surname)]) &&
                   string.IsNullOrEmpty(this[nameof(Client.Name)]) &&
                   string.IsNullOrEmpty(this[nameof(Client.Email)]) &&
                   string.IsNullOrEmpty(this[nameof(Client.NameCompany)]);
        }

        #endregion
    }
}
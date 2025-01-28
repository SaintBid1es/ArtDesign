using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;

namespace ArtDesign.ViewModel.Templates.Windows
{
    public class EditProjectWindowViewModel : ViewModelHelper, IDataErrorInfo
    {
        private readonly DataManager _dataManager;
        private readonly bool _isNew;

        public Project Project { get; set; }

        // Списки для Combobox
        public List<Client> AllClients { get; set; }
        public List<Status> AllStatuses { get; set; }

        // Выбранный клиент (ID)
        public int SelectedClientId
        {
            get => Project.ClientId;
            set
            {
                Project.ClientId = value;
                OnPropertyChanged();
            }
        }

        // Выбранный статус (ID)
        public int SelectedStatusId
        {
            get => Project.StatusId;
            set
            {
                Project.StatusId = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public EditProjectWindowViewModel(DataManager dataManager, Project project, bool isNew)
        {
            _dataManager = dataManager;
            _isNew = isNew;
            Project = project;

            SaveCommand = new RelayCommand(_ => ExecuteSave());
            CancelCommand = new RelayCommand(_ => ExecuteCancel());

            // Загрузим списки
            AllClients = _dataManager.Clients.GetQueryable().ToList();
            AllStatuses = _dataManager.Statuses.GetQueryable().ToList();
        }

        private async void ExecuteSave()
        {
            if (!IsValid())
            {
                MessageBox.Show("Заполните обязательные поля (Название, Описание).");
                return;
            }

            if (_isNew)
            {
                var ok = await _dataManager.Projects.AddAsync(Project);
                if(!ok)
                {
                    MessageBox.Show("Ошибка при добавлении проекта.");
                    return;
                }
            }
            else
            {
                var ok = await _dataManager.Projects.UpdateAsync(Project);
                if(!ok)
                {
                    MessageBox.Show("Ошибка при обновлении проекта.");
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
            if(wnd != null)
            {
                wnd.DialogResult = result;
                wnd.Close();
            }
        }

        #region IDataErrorInfo

        // Required: ProjectName, Description, ...
        public string this[string columnName]
        {
            get
            {
                switch(columnName)
                {
                    case nameof(Project.ProjectName):
                        if(string.IsNullOrWhiteSpace(Project.ProjectName))
                            return "Название не может быть пустым";
                        break;
                    case nameof(Project.Description):
                        if(string.IsNullOrWhiteSpace(Project.Description))
                            return "Описание не может быть пустым";
                        break;
                }
                return null;
            }
        }

        public string Error => null;

        private bool IsValid()
        {
            return string.IsNullOrEmpty(this[nameof(Project.ProjectName)]) &&
                   string.IsNullOrEmpty(this[nameof(Project.Description)]);
        }

        #endregion
    }
}
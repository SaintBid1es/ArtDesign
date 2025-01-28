using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;

namespace ArtDesign.ViewModel.Templates.Windows
{
    public class EditWorkStageWindowViewModel : ViewModelHelper, IDataErrorInfo
    {
        private readonly DataManager _dataManager;
        private readonly bool _isNew;

        public WorkStage WorkStage { get; set; }

        public List<Project> AllProjects { get; set; }
        public List<Employee> AllEmployees { get; set; }

        // ComboBox для выбора проекта
        public int SelectedProjectId
        {
            get => WorkStage.ProjectId;
            set
            {
                WorkStage.ProjectId = value;
                OnPropertyChanged();
            }
        }

        // ComboBox для выбора сотрудника
        public int SelectedEmployeeId
        {
            get => WorkStage.EmployeeId;
            set
            {
                WorkStage.EmployeeId = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public EditWorkStageWindowViewModel(DataManager dataManager, WorkStage workStage, bool isNew)
        {
            _dataManager = dataManager;
            _isNew = isNew;
            WorkStage = workStage;

            SaveCommand = new RelayCommand(_ => ExecuteSave());
            CancelCommand = new RelayCommand(_ => ExecuteCancel());

            AllProjects = _dataManager.Projects.GetQueryable().ToList();
            AllEmployees = _dataManager.Employees.GetQueryable().ToList();
        }

        private async void ExecuteSave()
        {
            if(!IsValid())
            {
                MessageBox.Show("Проверьте обязательные поля (Название этапа).");
                return;
            }

            if(_isNew)
            {
                var ok = await _dataManager.WorkStages.AddAsync(WorkStage);
                if(!ok)
                {
                    MessageBox.Show("Ошибка при добавлении этапа.");
                    return;
                }
            }
            else
            {
                var ok = await _dataManager.WorkStages.UpdateAsync(WorkStage);
                if(!ok)
                {
                    MessageBox.Show("Ошибка при обновлении этапа.");
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
                    case nameof(WorkStage.StageName):
                        if(string.IsNullOrWhiteSpace(WorkStage.StageName))
                            return "Название этапа не может быть пустым.";
                        break;
                }
                return null;
            }
        }

        public string Error => null;

        private bool IsValid()
        {
            return string.IsNullOrEmpty(this[nameof(WorkStage.StageName)]);
        }

        #endregion
    }
}
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ArtDesign.Database;
using ArtDesign.Model;
using ArtDesign.Services;
using ArtDesign.Helpers;

namespace ArtDesign.ViewModel
{
    public class ReportsViewModel : ViewModelHelper
    {
        private DataManager _dataManager;
        private Employee _currentUser;

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        private ObservableCollection<FileEntity> _reports = new ObservableCollection<FileEntity>();
        public ObservableCollection<FileEntity> Reports
        {
            get => _reports;
            set => SetField(ref _reports, value);
        }

        public AsyncRelayCommand AddReportCommand { get; set; }
        public RelayCommand OpenReportCommand { get; set; }
        public RelayCommand DeleteReportCommand { get; set; }

        private bool _isGeneratingReport;
        public bool IsGeneratingReport
        {
            get => _isGeneratingReport;
            set
            {
                if (SetField(ref _isGeneratingReport, value))
                {
                  
                    AddReportCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ReportsViewModel() { }

        public void Init(object parameter)
        {
            if (parameter is NavigationArgs args)
            {
                _currentUser = args.CurrentUser;
                _dataManager = args.DataManager;

                AddReportCommand = new AsyncRelayCommand(AddReportAsync, _ => !IsGeneratingReport);
                OpenReportCommand = new RelayCommand(obj => OpenReport(obj as FileEntity));
                DeleteReportCommand = new RelayCommand(obj => DeleteReport(obj as FileEntity));

                LoadReportsFromDb();
            }
        }

        private void LoadReportsFromDb()
        {
            Reports.Clear();
            var list = _dataManager.Files.GetQueryable()
                .Where(f => f.ProjectId == 0) 
                .ToList();
            foreach (var fileEntity in list)
            {
                Reports.Add(fileEntity);
            }
        }

        private async Task AddReportAsync(object parameter)
        {
            
            if (DateFrom.HasValue && DateTo.HasValue && DateFrom > DateTo)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            IsGeneratingReport = true;

            try
            {
                string folder = "C:\\MyReports";
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = $"report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string fullPath = Path.Combine(folder, fileName);

               
                var clients = (await _dataManager.Clients.GetAllAsync()).ToList();
                var projects = (await _dataManager.Projects.GetAllAsync()).ToList();
                var statuses = (await _dataManager.Statuses.GetAllAsync()).ToList();
                var employees = (await _dataManager.Employees.GetAllAsync()).ToList();
                var workStages = (await _dataManager.WorkStages.GetAllAsync()).ToList();
                var roles = (await _dataManager.Roles.GetAllAsync()).ToList();

                
                foreach (var project in projects)
                {
                    project.Status = statuses.FirstOrDefault(s => s.ID == project.StatusId);
                    project.Client = clients.FirstOrDefault(c => c.ID == project.ClientId);
                    project.WorkStages = workStages.Where(ws => ws.ProjectId == project.ID).ToList();
                    project.Files = _dataManager.Files.GetQueryable().Where(f => f.ProjectId == project.ID).ToList();
                }

                foreach (var employee in employees)
                {
                    employee.Role = roles.FirstOrDefault(r => r.ID == employee.RoleId);
                    employee.WorkStages = workStages.Where(ws => ws.EmployeeId == employee.ID).ToList();
                }

             
                var generator = new PdfReportGenerator();
                await Task.Run(() => generator.GenerateReport(
                    DateFrom,
                    DateTo,
                    projects,
                    employees,
                    clients,
                    statuses,
                    workStages,
                    fullPath
                ));

           
                var entity = new FileEntity
                {
                    FileName = fileName,
                    FilePath = fullPath,
                    ProjectId = 0 
                };

       
                bool addSuccess = true;
                if (addSuccess)
                {
                    Reports.Add(entity);
                    MessageBox.Show($"Отчет \"{fileName}\" создан в {folder}.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Не удалось добавить запись отчета в базу данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Ошибка при генерации отчёта: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\n\nВнутренняя ошибка: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsGeneratingReport = false;
            }
        }

        private void OpenReport(FileEntity file)
        {
            if (file == null) return;
            try
            {
                if (File.Exists(file.FilePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = file.FilePath,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("Файл отчета не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось открыть файл: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteReport(FileEntity file)
        {
            if (file == null) return;

            var res = MessageBox.Show(
                $"Удалить отчёт \"{file.FileName}\"?",
                "Удаление отчёта",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                bool ok = await _dataManager.Files.DeleteAsync(file.ID);
                if (ok)
                {
                    Reports.Remove(file);
                    if (File.Exists(file.FilePath)) File.Delete(file.FilePath);
                }
                else
                {
                    MessageBox.Show("Не удалось удалить запись из базы данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

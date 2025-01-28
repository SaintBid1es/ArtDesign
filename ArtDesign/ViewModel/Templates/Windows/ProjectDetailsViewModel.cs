using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;
using ArtDesign.View.Templates.Windows;
using Microsoft.Win32;

namespace ArtDesign.ViewModel.Templates.Windows
{
    public class ProjectDetailsViewModel : ViewModelHelper
    {
        private readonly DataManager _dataManager;
        private readonly Project _project;

        public string ProjectNameInfo => _project.ProjectName;
        public string DescriptionInfo => _project.Description;
        public string DateRangeInfo => $"с {_project.StartDate:dd.MM.yyyy} по {_project.EndDate:dd.MM.yyyy}";

        // Этапы
        public ObservableCollection<WorkStage> WorkStages { get; } 
            = new ObservableCollection<WorkStage>();

        // Файлы
        public ObservableCollection<FileEntity> Files { get; }
            = new ObservableCollection<FileEntity>();

        // Команды (добавление есть уже, ниже - редактировать/удалить)
        public RelayCommand AddWorkStageCommand { get; }
        public RelayCommand EditStageCommand { get; }
        public RelayCommand DeleteStageCommand { get; }

        // Команды (добавление есть уже, ниже - открыть/удалить)
        public RelayCommand AddFileCommand { get; }
        public RelayCommand OpenFileCommand { get; }
        public RelayCommand DeleteFileCommand { get; }

        public ProjectDetailsViewModel(DataManager dataManager, Project project)
        {
            _dataManager = dataManager;
            _project = project;

            AddWorkStageCommand = new RelayCommand(_ => AddWorkStage());
            EditStageCommand = new RelayCommand(obj => EditStage(obj as WorkStage));
            DeleteStageCommand = new RelayCommand(obj => DeleteStage(obj as WorkStage));

            AddFileCommand = new RelayCommand(_ => AddFile());
            OpenFileCommand = new RelayCommand(obj => OpenFile(obj as FileEntity));
            DeleteFileCommand = new RelayCommand(obj => DeleteFile(obj as FileEntity));

            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            // Загрузить этапы
            var stages = _dataManager.WorkStages.GetQueryable()
                .Where(ws => ws.ProjectId == _project.ID)
                .ToList();
            WorkStages.Clear();
            foreach(var st in stages) WorkStages.Add(st);

            // Загрузить файлы
            var files = _dataManager.Files.GetQueryable()
                .Where(f => f.ProjectId == _project.ID)
                .ToList();
            Files.Clear();
            foreach(var f in files) Files.Add(f);

            // Если нужно синхронизировать с папкой:
            // await SyncFilesFromFolder();
        }

        // ============== ЭТАПЫ ==============
        private void AddWorkStage()
        {
            var newStage = new WorkStage
            {
                ProjectId = _project.ID,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7)
            };
            var vm = new EditWorkStageWindowViewModel(_dataManager, newStage, isNew:true);
            var wnd = new EditWorkStageWindow { DataContext = vm };
            bool? result = wnd.ShowDialog();
            if(result == true)
            {
                WorkStages.Add(newStage);
            }
        }

        private void EditStage(WorkStage stage)
        {
            if (stage == null) return;

            var vm = new EditWorkStageWindowViewModel(_dataManager, stage, isNew:false);
            var wnd = new EditWorkStageWindow { DataContext = vm };
            bool? result = wnd.ShowDialog();
            if(result == true)
            {
                // Объект stage уже изменён, 
                // WorkStages указывает на те же объекты => 
                // при необходимости: OnPropertyChanged(nameof(WorkStages));
            }
        }

        private async void DeleteStage(WorkStage stage)
        {
            if (stage == null) return;

            var res = MessageBox.Show(
                "Удалить этап?",
                "Удаление",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                bool ok = await _dataManager.WorkStages.DeleteAsync(stage.ID);
                if (ok)
                {
                    WorkStages.Remove(stage);
                }
            }
        }

        // ============== ФАЙЛЫ ==============
        private void AddFile()
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                string selectedPath = ofd.FileName;
                string fileName = Path.GetFileName(selectedPath);

                var entity = new FileEntity
                {
                    FileName = fileName,
                    FilePath = selectedPath,
                    ProjectId = _project.ID
                };
                _ = _dataManager.Files.AddAsync(entity);
                Files.Add(entity);
            }
        }

        private void OpenFile(FileEntity file)
        {
            if (file == null) return;
            try
            {
                System.Diagnostics.Process.Start(file.FilePath);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Не удалось открыть файл: {ex.Message}");
            }
        }

        private async void DeleteFile(FileEntity file)
        {
            if (file == null) return;

            var res = MessageBox.Show(
                $"Удалить файл {file.FileName}?",
                "Удаление файла",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            if (res == MessageBoxResult.Yes)
            {
                bool ok = await _dataManager.Files.DeleteAsync(file.ID);
                if(ok)
                {
                    Files.Remove(file);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class ProjectsViewModel : ViewModelHelper
    {
        private  DataManager _dataManager;
        private Employee _currentEmployee;

        // Коллекция карточек
        public ObservableCollection<ProjectCardViewModel> Projects { get; set; }
            = new ObservableCollection<ProjectCardViewModel>();

        // Команда добавления проекта
        public RelayCommand AddProjectCommand { get; set; }

        // Поиск
        private string _searchTerm;
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if(SetField(ref _searchTerm, value))
                {
                    // Если нужно реагировать на изменение строки поиска сразу
                    // LoadProjectsAsync();
                }
            }
        }

        // Фильтры
        public int? SelectedStatusId { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
        public int? FilterMinStages { get; set; }
        public int? FilterMaxStages { get; set; }
        public string FilterClientName { get; set; }

        // Для ComboBox статусов
        public List<Status> AllStatuses { get; set; }

        // Команды для применения / сброса фильтра
        public RelayCommand ApplyFilterCommand { get; set; }
        public RelayCommand ResetFilterCommand { get; set; }

        public ProjectsViewModel() { }
        
        public void Init(object parameter)
        {
            if (parameter is NavigationArgs args)
            {
                _currentEmployee = args.CurrentUser;
                _dataManager = args.DataManager;
            }
            AddProjectCommand = new RelayCommand(_ => AddProject());
            ApplyFilterCommand = new RelayCommand(_ => LoadProjectsAsync());
            ResetFilterCommand = new RelayCommand(_ => ResetFilters());

            // Загрузим список статусов (для фильтра)
            AllStatuses = _dataManager.Statuses.GetQueryable().ToList();

            // При старте
            _ = LoadProjectsAsync();
        }

        private async Task LoadProjectsAsync()
        {
            Projects.Clear();

            var query = _dataManager.Projects.GetQueryable();

            // 1) Применим поиск (по строковым полям [Searchable], напр. ProjectName)
            query = SearchFilterSortHelper.Search(query, SearchTerm);

            // 2) Применим какую-то фильтрацию

            // Фильтр по статусу
            if (SelectedStatusId.HasValue)
            {
                query = query.Where(p => p.StatusId == SelectedStatusId.Value);
            }

            // Фильтр по дате
            if (FilterStartDate.HasValue)
            {
                query = query.Where(p => p.StartDate >= FilterStartDate.Value);
            }
            if (FilterEndDate.HasValue)
            {
                query = query.Where(p => p.EndDate <= FilterEndDate.Value);
            }

            // Фильтр по количеству этапов (от .. до ..)
            if (FilterMinStages.HasValue)
            {
                query = query.Where(p => p.WorkStages.Count >= FilterMinStages.Value);
            }
            if (FilterMaxStages.HasValue)
            {
                query = query.Where(p => p.WorkStages.Count <= FilterMaxStages.Value);
            }

            // Фильтр по названию компании клиента
            if (!string.IsNullOrWhiteSpace(FilterClientName))
            {
                // У нас Client.NameCompany. Нужно Include или Join
                // Проще сделать Eager Loading:
                // но в BaseRepo нет Include. Для примера: 
                query = query.Where(p => p.Client.NameCompany.Contains(FilterClientName));
            }

            // 3) Cортировка, если нужно (например, по StartDate):
            // query = SearchFilterSortHelper.Sort(query, "StartDate", ascending:true);

            var list = await Task.Run(() => query.ToList()); // подгрузим
            foreach(var p in list)
            {
                Projects.Add(new ProjectCardViewModel(_dataManager, p));
            }
        }

        private void ResetFilters()
        {
            SearchTerm = null;
            SelectedStatusId = null;
            FilterStartDate = null;
            FilterEndDate = null;
            FilterMinStages = null;
            FilterMaxStages = null;
            FilterClientName = null;

            _ = LoadProjectsAsync();
        }

        private void AddProject()
        {
            var newProj = new Project
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7)
            };
            var vm = new EditProjectWindowViewModel(_dataManager, newProj, isNew:true);
            var wnd = new EditProjectWindow { DataContext = vm };
            bool? result = wnd.ShowDialog();
            if(result == true)
            {
                _ = LoadProjectsAsync();
            }
        }
    }
}
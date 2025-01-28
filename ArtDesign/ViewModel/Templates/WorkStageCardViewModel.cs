using System.Windows;
using System.Windows.Input;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;
using ArtDesign.View.Templates.Windows;
using ArtDesign.ViewModel.Templates.Windows;

namespace ArtDesign.ViewModel.Templates
{
    public class WorkStageCardViewModel : ViewModelHelper
    {
        private readonly DataManager _dataManager;

        public WorkStage Model { get; }

        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public WorkStageCardViewModel(DataManager dataManager, WorkStage workStage)
        {
            _dataManager = dataManager;
            Model = workStage;

            EditCommand = new RelayCommand(_ => EditWorkStage());
            DeleteCommand = new RelayCommand(_ => DeleteWorkStage());
        }

        private void EditWorkStage()
        {
            var vm = new EditWorkStageWindowViewModel(_dataManager, Model, isNew: false);
            var window = new EditWorkStageWindow { DataContext = vm };
            bool? result = window.ShowDialog();

            if (result == true)
            {
                OnPropertyChanged(nameof(Model));
            }
        }

        private async void DeleteWorkStage()
        {
            var res = MessageBox.Show(
                "Вы уверены, что хотите удалить этап?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                await _dataManager.WorkStages.DeleteAsync(Model.ID);
            }
        }
    }
}
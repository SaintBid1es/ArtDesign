using System.Windows;
using System.Windows.Input;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;

namespace ArtDesign.ViewModel.Templates
{
    public class FileEntityCardViewModel : ViewModelHelper
    {
        private readonly DataManager _dataManager;

        public FileEntity Model { get; }

        public ICommand EditCommand { get; }    // Если нужно переименовать
        public ICommand DeleteCommand { get; }

        public FileEntityCardViewModel(DataManager dataManager, FileEntity fileEntity)
        {
            _dataManager = dataManager;
            Model = fileEntity;

            //EditCommand = new RelayCommand(_ => RenameFile());
            DeleteCommand = new RelayCommand(_ => DeleteFile());
        }
        
        private async void DeleteFile()
        {
            var res = MessageBox.Show(
                "Вы уверены, что хотите удалить файл?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                await _dataManager.Files.DeleteAsync(Model.ID);
            }
        }
    }
}
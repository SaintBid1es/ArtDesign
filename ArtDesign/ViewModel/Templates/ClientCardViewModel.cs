using System.Windows;
using System.Windows.Input;
using ArtDesign.Database;
using ArtDesign.Helpers;
using ArtDesign.Model;
using ArtDesign.View.Templates.Windows;
using ArtDesign.ViewModel.Templates.Windows;

namespace ArtDesign.ViewModel.Templates
{
    public class ClientCardViewModel : ViewModelHelper
    {
        private readonly DataManager _dataManager;

        public Client Model { get; }

        public ICommand MoreInfoCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public ClientCardViewModel(DataManager dataManager, Client client)
        {
            _dataManager = dataManager;
            Model = client;

            MoreInfoCommand = new RelayCommand(_ => ShowMoreInfo());
            EditCommand = new RelayCommand(_ => EditClient());
            DeleteCommand = new RelayCommand(_ => DeleteClient());
        }

        private void ShowMoreInfo()
        {
            // Показываем окно с дополнительной информацией
            // Например, простым MessageBox, либо отдельным окном
            MessageBox.Show(
                $"Информация о клиенте:\n" +
                $"Компания: {Model.NameCompany}\n" +
                $"Фамилия: {Model.Surname}\n" +
                $"Имя: {Model.Name}\n" +
                $"Email: {Model.Email}\n" +
                $"Телефон: {Model.Phone}\n",
                "Подробнее о клиенте",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void EditClient()
        {
            // Открываем окно редактирования (модальное).
            // Допустим, у нас есть EditClientWindow и соответствующий ViewModel (EditClientWindowViewModel)
            var vm = new EditClientWindowViewModel(_dataManager, Model, isNew: false);
            var window = new EditClientWindow { DataContext = vm };
            bool? result = window.ShowDialog();

            if (result == true)
            {
                // Если клиент был обновлён, уведомляем об изменениях
                OnPropertyChanged(nameof(Model));
            }
        }

        private async void DeleteClient()
        {
            var res = MessageBox.Show(
                "Вы уверены, что хотите удалить клиента?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                // Удаляем из БД
                await _dataManager.Clients.DeleteAsync(Model.ID);
            }
        }
    }
}
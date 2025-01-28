using System.Collections.ObjectModel;
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
    public class ClientsViewModel : ViewModelHelper
    {
        private  DataManager _dataManager;
        Employee _currentEmployee;

        public ObservableCollection<ClientCardViewModel> Clients { get; set; }
            = new ObservableCollection<ClientCardViewModel>();

        public RelayCommand AddClientCommand { get; set; }

        public ClientsViewModel() { }
        public void Init(object parameter)
        {
            if (parameter is NavigationArgs args)
            {
                _currentEmployee = args.CurrentUser;
                _dataManager = args.DataManager;
            }
            AddClientCommand = new RelayCommand(_ => AddClient());
            // При загрузке VM:
            _ = LoadClientsAsync();
        }

        private async Task LoadClientsAsync()
        {
            Clients.Clear();

            var allClients = await _dataManager.Clients.GetAllAsync(); 
            foreach(var c in allClients)
            {
                var cardVm = new ClientCardViewModel(_dataManager, c);
                Clients.Add(cardVm);
            }
        }

        private void AddClient()
        {
            // Создание нового клиента
            var newClient = new Client();
            var vm = new EditClientWindowViewModel(_dataManager, newClient, isNew: true);

            // Показываем окно (модально) 
            var wnd = new EditClientWindow { DataContext = vm };
            bool? result = wnd.ShowDialog();

            if (result == true)
            {
                // Успешно добавлено => обновляем список
                _ = LoadClientsAsync();
            }
        }
    }
}
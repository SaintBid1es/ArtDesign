using System.Windows;
using System.Windows.Controls;
using ArtDesign.ViewModel;

namespace ArtDesign.View
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void PasswordBoxControl_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Пробрасываем пароль в VM
            if (this.DataContext is LoginViewModel vm)
            {
                vm.Password = PasswordBoxControl.Password;
            }
        }
    }
}
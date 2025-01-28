using System.Windows;
using ArtDesign.ViewModel.Templates.Windows;

namespace ArtDesign.View.Templates.Windows
{
    public partial class EditEmployeeWindow : Window
    {
        public EditEmployeeWindow()
        {
            InitializeComponent();
        }

        private void PasswordBoxControl_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is EditEmployeeWindowViewModel vm)
            {
                vm.Employee.Password = PasswordBoxControl.Password;
            }
        }
    }
}
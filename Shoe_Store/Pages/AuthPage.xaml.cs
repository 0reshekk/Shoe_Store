using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace testDemoYP.pagesFr
{
    /// Логика взаимодействия для AuthPage.xaml
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            string password = PasswordBox.Password;

            var user = Entities.GetContext().User.FirstOrDefault(u => u.Login == login && u.Password == password);

            if (user != null)
            {
                var userRole = Entities.GetContext().Role.FirstOrDefault(r => r.ID == user.RoleID);


                if (userRole != null)
                {
                    string userName = $"{user.FIO} ";
                    string roleName = userRole.Name;

                    switch (userRole.Name)
                    {
                        case "Авторизированный клиент":
                            NavigationService.Navigate(new ProductsPage(false, false, false, false, userName, roleName));
                            break;
                        case "Менеджер":
                            NavigationService.Navigate(new ProductsPage(true, true, true, false, userName, roleName));
                            break;
                        case "Администратор":
                            NavigationService.Navigate(new AdminPage(userName, roleName));
                            break;
                        default:
                            MessageBox.Show($"Неизвестная роль пользователя: {userRole.Name}");
                            break;
                    }
                }
                else
                    MessageBox.Show("У пользователя не назначена роль");
            }
            else
                MessageBox.Show("Неверный логин или пароль");
        }

        private void GuestBtn_Click(object sender, RoutedEventArgs e)
        {
            string userName = "Гость";
            string roleName = "Гость";
            NavigationService.Navigate(new ProductsPage(false, false, false, false, userName, roleName));
        }
    }
}
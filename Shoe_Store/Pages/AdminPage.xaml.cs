using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace testDemoYP.pagesFr
{
    /// Логика взаимодействия для AdminPage.xaml
    public partial class AdminPage : Page
    {
        private string _userName;
        private string _userRole;

        public AdminPage(string userName = "", string userRole = "")
        {
            InitializeComponent();
            _userName = userName;
            _userRole = userRole;

            DisplayUserInfo();
            ProductsFrame.Navigate(new ProductsPage(true, true, true, true, userName, userRole));
            LoadOrders();
        }

        private void DisplayUserInfo()
        {
            UserNameText.Text = _userName;
            UserRoleText.Text = _userRole;
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void LogoutBtn_Click1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void LoadOrders()
        {
            var orders = Entities.GetContext().Order
                .Include("Status")
                .Include("Address")
                .Include("Sostav")
                .ToList();
            OrdersListView.ItemsSource = orders;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductsFrame.Navigate(new AddEditProductPage(null));
        }


        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var productsPage = ProductsFrame.Content as ProductsPage;
            if (productsPage != null)
            {
                Products selectedProduct = productsPage.GetSelectedProduct();
                if (selectedProduct != null)
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить товар '{selectedProduct.Name}'?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Entities.GetContext().Products.Remove(selectedProduct);
                            Entities.GetContext().SaveChanges();
                            // Обновляем страницу товаров
                            ProductsFrame.Navigate(new ProductsPage(true, true, true, true, _userName, _userRole));
                            MessageBox.Show("Товар успешно удален");
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка при удалении товара");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите товар для удаления");
                }
            }
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditOrderPage(null));
        }

        // Метод для обработки двойного клика по элементу списка заказов
        private void OrdersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrdersListView.SelectedItem != null)
            {
                Order selectedOrder = OrdersListView.SelectedItem as Order;
                if (selectedOrder != null)
                {
                    NavigationService.Navigate(new AddEditOrderPage(selectedOrder));
                }
            }
        }


        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersListView.SelectedItem != null)
            {
                Order selectedOrder = OrdersListView.SelectedItem as Order;
                if (selectedOrder != null)
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить заказ №{selectedOrder.ID}?",
                        "Подтверждение удаления", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Entities.GetContext().Order.Remove(selectedOrder);
                            Entities.GetContext().SaveChanges();
                            LoadOrders();
                            MessageBox.Show("Заказ успешно удален");
                        }
                        catch
                        {
                            MessageBox.Show("Ошибка при удалении заказа");
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для удаления");
            }
        }
    }
}
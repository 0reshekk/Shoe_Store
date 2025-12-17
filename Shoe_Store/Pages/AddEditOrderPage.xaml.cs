using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace testDemoYP.pagesFr
{
    /// Логика взаимодействия для AddEditOrderPage.xaml
    public partial class AddEditOrderPage : Page
    {
        private Order _currentOrder;

        public AddEditOrderPage(Order selectedOrder)
        {
            InitializeComponent();
            _currentOrder = selectedOrder ?? new Order();
            LoadComboBoxData();
            LoadOrderData();
        }


        private void LoadComboBoxData()
        {
            try
            {
                StatusComboBox.ItemsSource = Entities.GetContext().Status.ToList();
                AddressComboBox.ItemsSource = Entities.GetContext().PointAddress.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadOrderData()
        {
            if (_currentOrder.ID != 0)
            {
                OrderDatePicker.SelectedDate = _currentOrder.OrdersDate;
                DeliveryDatePicker.SelectedDate = _currentOrder.DeliverisDate;
                StatusComboBox.SelectedValue = _currentOrder.Status;
                AddressComboBox.SelectedValue = _currentOrder.PointAddress;
            }
            else
                OrderDatePicker.SelectedDate = DateTime.Now;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (OrderDatePicker.SelectedDate == null || StatusComboBox.SelectedItem == null || AddressComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Заполните обязательные поля");
                    return;
                }

                _currentOrder.OrdersDate = (DateTime)OrderDatePicker.SelectedDate;
                _currentOrder.DeliverisDate = (DateTime)DeliveryDatePicker.SelectedDate;
                _currentOrder.StatusID = ((Status)StatusComboBox.SelectedItem).ID;
                _currentOrder.PointAddressID = ((PointAddress)AddressComboBox.SelectedItem).ID;

                if (_currentOrder.ID == 0)
                {
                    Entities.GetContext().Order.Add(_currentOrder);
                }

                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены успешно");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
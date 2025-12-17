using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace testDemoYP.pagesFr
{
    /// Логика взаимодействия для AddEditProductPage.xaml
    public partial class AddEditProductPage : Page
    {
        private Products _currentProduct;

        public AddEditProductPage(Products selectedProduct)
        {
            InitializeComponent();
            _currentProduct = selectedProduct ?? new Products();
            LoadComboBoxData();
            LoadProductData();
        }

        private void LoadComboBoxData()
        {
            try
            {
                TitleComboBox.ItemsSource = Entities.GetContext().Products.ToList();
                SupplierComboBox.ItemsSource = Entities.GetContext().Provider.ToList();
                ManufacturerComboBox.ItemsSource = Entities.GetContext().Manufacturer.ToList();
                CategoryComboBox.ItemsSource = Entities.GetContext().Category.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            }
        }

        private void LoadProductData()
        {
            if (_currentProduct.ID != 0)
            {
                TitleComboBox.SelectedValue = _currentProduct.Name;
                UnitTextBox.Text = _currentProduct.Unit;
                PriceTextBox.Text = _currentProduct.Price.ToString();
                SupplierComboBox.SelectedValue = _currentProduct.Provider;
                ManufacturerComboBox.SelectedValue = _currentProduct.Manufacturer;
                CategoryComboBox.SelectedValue = _currentProduct.Category;
                SaleTextBox.Text = _currentProduct.Discount.ToString();
                CountTextBox.Text = _currentProduct.InStock.ToString();
                DescriptionTextBox.Text = _currentProduct.Description;
                PhotoTextBox.Text = _currentProduct.Photo;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TitleComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите название");
                    return;
                }
                if (string.IsNullOrWhiteSpace(UnitTextBox.Text))
                {
                    MessageBox.Show("Введите единицу измерения");
                    return;
                }
                if (string.IsNullOrWhiteSpace(PriceTextBox.Text) || !decimal.TryParse(PriceTextBox.Text, out decimal price))
                {
                    MessageBox.Show("Введите корректную цену");
                    return;
                }
                if (SupplierComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите поставщика");
                    return;
                }
                if (ManufacturerComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите производителя");
                    return;
                }
                if (CategoryComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите категорию");
                    return;
                }

                _currentProduct.ID = (int)TitleComboBox.SelectedValue;
                _currentProduct.Unit = UnitTextBox.Text;
                _currentProduct.Price = price;
                _currentProduct.ProviderID = (int)SupplierComboBox.SelectedValue;
                _currentProduct.ManufacturerID = (int)ManufacturerComboBox.SelectedValue;
                _currentProduct.CategoryID = (int)CategoryComboBox.SelectedValue;

                if (!string.IsNullOrWhiteSpace(SaleTextBox.Text) && int.TryParse(SaleTextBox.Text, out int sale))
                {
                    _currentProduct.Discount = sale;
                }
                else
                {
                    _currentProduct.Discount = 0;
                }

                if (!string.IsNullOrWhiteSpace(CountTextBox.Text) && int.TryParse(CountTextBox.Text, out int count))
                {
                    _currentProduct.InStock = count;
                }
                else
                {
                    _currentProduct.InStock = 0;
                }

                _currentProduct.Description = DescriptionTextBox.Text;
                _currentProduct.Photo = PhotoTextBox.Text;

                if (_currentProduct.ID == 0)
                {
                    Entities.GetContext().Products.Add(_currentProduct);
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
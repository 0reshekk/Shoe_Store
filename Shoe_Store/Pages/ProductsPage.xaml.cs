using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace testDemoYP.pagesFr
{
    /// Логика взаимодействия для ProductsPage.xaml
    public partial class ProductsPage : Page
    {
        private bool _enableSearch;
        private bool _enableSort;
        private bool _enableFilter;
        private bool _isAdminMode;
        private string _userName;
        private string _userRole;
        private List<Products> _allProducts;
        private List<Products> _currentProducts;

        public ProductsPage(bool enableSearch, bool enableSort, bool enableFilter, bool isAdminMode = false, string userName = "", string userRole = "")
        {
            InitializeComponent();
            _enableSearch = enableSearch;
            _enableSort = enableSort;
            _enableFilter = enableFilter;
            _isAdminMode = isAdminMode;
            _userName = userName;
            _userRole = userRole;

            InitializeControls();
            LoadProducts();
            DisplayUserInfo();

            Loaded += ProductsPage_Loaded;
        }

        private void DisplayUserInfo()
        {
            // Для администратора скрываем панель пользователя
            if (_isAdminMode)
                UserInfoPanel.Visibility = Visibility.Collapsed;
            else
            {
                UserNameText.Text = _userName;
                UserRoleText.Text = _userRole;
            }
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthPage());
        }

        private void InitializeControls()
        {
            if (!_enableSearch && !_enableSort && !_enableFilter)
            {
                SearchTextBox.IsEnabled = false;
                SortComboBox.IsEnabled = false;
                FilterComboBox.IsEnabled = false;
                DiscountFilterComboBox.IsEnabled = false;


                SearchTextBox.Opacity = 0.5;
                SortComboBox.Opacity = 0.5;
                FilterComboBox.Opacity = 0.5;
                DiscountFilterComboBox.Opacity = 0.5;

                SearchTextBox.ToolTip = "Функция поиска недоступна для вашей роли";
                SortComboBox.ToolTip = "Функция сортировки недоступна для вашей роли";
                FilterComboBox.ToolTip = "Функция фильтрации недоступна для вашей роли";
                DiscountFilterComboBox.ToolTip = "Функция фильтрации по скидке недоступна для вашей роли";

                return;
            }

            if (!_enableSearch)
            {
                SearchTextBox.IsEnabled = false;
                SearchTextBox.Opacity = 0.5;
                SearchTextBox.ToolTip = "Функция поиска недоступна";
            }

            if (!_enableSort)
            {
                SortComboBox.IsEnabled = false;
                SortComboBox.Opacity = 0.5;
                SortComboBox.ToolTip = "Функция сортировки недоступна";
            }

            if (!_enableFilter)
            {
                FilterComboBox.IsEnabled = false;
                FilterComboBox.Opacity = 0.5;
                FilterComboBox.ToolTip = "Функция фильтрации недоступна";
                DiscountFilterComboBox.IsEnabled = false;
                DiscountFilterComboBox.Opacity = 0.5;
                DiscountFilterComboBox.ToolTip = "Функция фильтрации по скидке недоступна";
            }

            if (_enableFilter)
            {
                var categories = Entities.GetContext().Category.ToList();
                FilterComboBox.Items.Clear();
                FilterComboBox.Items.Add("Все категории");
                foreach (var category in categories)
                    FilterComboBox.Items.Add(category.Name);
                FilterComboBox.SelectedIndex = 0;
            }
            else
            {
                FilterComboBox.Items.Clear();
                FilterComboBox.Items.Add("Все категории");
                FilterComboBox.SelectedIndex = 0;
            }
        }
        private void ProductsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProducts();
        }

        private void LoadProducts()
        {
            _allProducts = Entities.GetContext().Products.ToList();
            _currentProducts = _allProducts.ToList();
            UpdateProducts();
        }


        private void UpdateProducts()
        {
            if (!IsInitialized)
            {
                return;
            }

            try
            {
                List<Products> products = _allProducts.ToList();

                if (_enableSearch && !string.IsNullOrWhiteSpace(SearchTextBox.Text))
                    products = products.Where(x => x.Name.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();

                if (_enableFilter && FilterComboBox.SelectedIndex > 0)
                {
                    string selectedCategory = FilterComboBox.SelectedItem.ToString();
                    products = products.Where(x => x.Category.Name == selectedCategory).ToList();
                }

                if (_enableFilter && DiscountFilterComboBox.SelectedIndex > 0)
                {
                    if (DiscountFilterComboBox.SelectedItem.ToString() == "Скидка > 15%")
                        products = products.Where(x => x.Discount != 0 && x.Discount > 14).ToList();
                }

                _currentProducts = products;

                if (_enableSort)
                {
                    switch (SortComboBox.SelectedIndex)
                    {
                        case 1:
                            products = products.OrderBy(x => x.InStock).ToList();
                            break;

                        case 2:
                            products = products.OrderByDescending(x => x.InStock).ToList();
                            break;
                    }
                }

                var productsWithColor = products.Select(p => new
                {
                    Article = p.Article,
                    Name = p.Name,
                    Category = p.Category,
                    Manufacturer = p.ManufacturerID,
                    ProviderID = p.ProviderID,
                    Price = p.Price,
                    Discount = p.Discount,
                    Unit = p.Unit,
                    InStock = p.InStock,
                    Description = p.Description,
                    Photo = p.Photo,
                    OriginalProduct = p,

                    //OriginalPriceValue = p.OriginalPriceValue,
                    //HasDiscount = p.HasDiscount,
                    //DiscountedPriceValue = p.DiscountedPriceValue,

                    BackgroundColor = GetProductBackgroundColor(p)
                }).ToList();

                ProductsListView.ItemsSource = productsWithColor;
            }
            catch (Exception)
            {
            }
        }
        private Brush GetProductBackgroundColor(Products product)
        {
            if (product.InStock == 0 || product.InStock == 0)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ADD8E6"));
            if (product.Discount != 0 && product.Discount > 14)
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E8B57"));

            return new SolidColorBrush(Colors.White);
        }

        public Products GetSelectedProduct()
        {
            if (ProductsListView.SelectedItem != null)
            {
                dynamic selectedItem = ProductsListView.SelectedItem;
                return selectedItem.OriginalProduct;
            }
            return null;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_enableSearch) UpdateProducts();
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_enableSort) UpdateProducts();
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_enableFilter) UpdateProducts();
        }

        private void DiscountFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_enableFilter) UpdateProducts();
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            SortComboBox.SelectedIndex = 0;
            FilterComboBox.SelectedIndex = 0;
            DiscountFilterComboBox.SelectedIndex = 0;
            UpdateProducts();
        }

        private void ProductsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!_isAdminMode)
            {
                MessageBox.Show("Недостаточно прав");
                return;
            }

            Products selectedProduct = GetSelectedProduct();
            if (selectedProduct != null)
            {
                DependencyObject parent = this;
                Frame frame = null;

                while (parent != null)
                {
                    if (parent is Frame)
                    {
                        frame = parent as Frame;
                        break;
                    }
                    parent = VisualTreeHelper.GetParent(parent);
                }

                if (frame != null)
                    frame.Navigate(new AddEditProductPage(selectedProduct));
            }
        }

    }
}
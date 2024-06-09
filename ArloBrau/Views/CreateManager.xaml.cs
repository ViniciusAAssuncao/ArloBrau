using ArloBrau.Config;
using ArloBrau.Models;
using ArloBrau.Services;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ArloBrau.Views
{
    public partial class CreateManager : Window
    {
        private readonly PlayerManagerService _playerManagerService;
        private readonly SaveService _saveService;
        private readonly string _connectionString;

        public CreateManager()
        {
            InitializeComponent();

            DatabaseConfig dbConfig = new DatabaseConfig();
            _connectionString = dbConfig.ConnectionString;
            _playerManagerService = new PlayerManagerService(_connectionString);
            _saveService = new SaveService(_connectionString);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadContentAsync();

            MainContentGrid.Visibility = Visibility.Visible;
            LoadingGrid.Visibility = Visibility.Collapsed;
        }

        public async Task LoadContentAsync()
        {
            await Task.Delay(500);
        }

        private void onHomeHandler(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            fadeOutAnimation.Completed += (s, a) =>
            {
                this.Hide();
                mainWindow.Show();
                this.Close();
            };

            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }

        private void Certification_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == vitaCheckbox)
            {
                ccslCheckbox.IsEnabled = false;
                foreach (ComboBoxItem item in technicalFormationComboBox.Items)
                {
                    if (item.Content.ToString() == "Licença Genérica Mundial")
                        item.IsEnabled = false;
                }
            }
            else if (sender == ccslCheckbox)
            {
                vitaCheckbox.IsEnabled = false;
                foreach (ComboBoxItem item in technicalFormationComboBox.Items)
                {
                    if (item.Content.ToString() == "Licença VITA Mundial")
                        item.IsEnabled = false;
                }
            }
        }

        private void Certification_Unchecked(object sender, RoutedEventArgs e)
        {
            vitaCheckbox.IsEnabled = true;
            ccslCheckbox.IsEnabled = true;
            foreach (ComboBoxItem item in technicalFormationComboBox.Items)
            {
                item.IsEnabled = true;
            }
        }

        private void BirthDatePicker_LostFocus(object sender, RoutedEventArgs e)
        {
            FormatBirthDate();
        }

        private void BirthDatePicker_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FormatBirthDate();
            }
        }

        private void BirthDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FormatBirthDate();
        }

        private void FormatBirthDate()
        {
            if (birthDatePicker.SelectedDate.HasValue)
            {
                DateTime parsedDate = birthDatePicker.SelectedDate.Value;
                birthDatePicker.Text = parsedDate.ToString("dd/MM/yyyy");

                int age = DateTime.Now.Year - parsedDate.Year;
                if (parsedDate > DateTime.Now.AddYears(-age)) age--;

                ageTextBlock.Text = $"Idade: {age} anos";
                if (age < 35)
                {
                    birthDateError.Visibility = Visibility.Visible;
                    birthDateError.Text = "Treinador deve ter pelo menos 35 anos.";
                }
                else
                {
                    birthDateError.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SkinColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (skinColorDisplay != null)
            {
                switch (((ComboBoxItem)((ComboBox)sender).SelectedItem).Content.ToString())
                {
                    case "Branco 1":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDDC1"));
                        break;
                    case "Branco 2":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5CBA7"));
                        break;
                    case "Branco 3":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E59866"));
                        break;
                    case "Branco 4":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5A478"));
                        break;
                    case "Pardo 1":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D5A478"));
                        break;
                    case "Pardo 2":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B18357"));
                        break;
                    case "Pardo 3":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8C6A52"));
                        break;
                    case "Pardo 4":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#603A18"));
                        break;
                    case "Negro 1":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#603A18"));
                        break;
                    case "Negro 2":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4A2512"));
                        break;
                    case "Negro 3":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#361F10"));
                        break;
                    case "Negro 4":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#241510"));
                        break;
                    case "Mediterrâneo":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8C6A52"));
                        break;
                    case "Mestiço":
                        skinColorDisplay.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B18357"));
                        break;
                    default:
                        skinColorDisplay.Fill = new SolidColorBrush(Colors.Transparent);
                        break;
                }
            }
        }

        private async void CreateTrainer_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                nameError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                nameError.Visibility = Visibility.Collapsed;
            }

            if (!birthDatePicker.SelectedDate.HasValue || DateTime.Now.Year - birthDatePicker.SelectedDate.Value.Year < 35)
            {
                birthDateError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                birthDateError.Visibility = Visibility.Collapsed;
            }

            if (nationalityComboBox.SelectedItem == null)
            {
                nationalityError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                nationalityError.Visibility = Visibility.Collapsed;
            }

            if ((vitaCheckbox.IsChecked == true || ccslCheckbox.IsChecked == true) && technicalFormationComboBox.SelectedItem != null && technicalFormationComboBox.SelectedItem.ToString() == "--")
            {
                technicalFormationError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                technicalFormationError.Visibility = Visibility.Collapsed;
            }

            if (playerHistoryComboBox.SelectedItem == null)
            {
                playerHistoryError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                playerHistoryError.Visibility = Visibility.Collapsed;
            }

            if (!int.TryParse(heightTextBox.Text, out int height) || height < 120 || height > 205)
            {
                heightError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                heightError.Visibility = Visibility.Collapsed;
            }

            if (skinColorComboBox.SelectedItem == null)
            {
                skinColorError.Visibility = Visibility.Visible;
                isValid = false;
            }
            else
            {
                skinColorError.Visibility = Visibility.Collapsed;
            }

            if (isValid)
            {
                var birthDate = birthDatePicker.SelectedDate ?? DateTime.MinValue;
                var nationality = ((ComboBoxItem)nationalityComboBox.SelectedItem)?.Content?.ToString() ?? string.Empty;
                var technicalFormation = ((ComboBoxItem)technicalFormationComboBox.SelectedItem)?.Content?.ToString() ?? string.Empty;
                var playerHistory = ((ComboBoxItem)playerHistoryComboBox.SelectedItem)?.Content?.ToString() ?? string.Empty;
                var skinColor = ((ComboBoxItem)skinColorComboBox.SelectedItem)?.Content?.ToString() ?? string.Empty;

                PlayerManager playerManager = new PlayerManager
                {
                    Name = nameTextBox.Text ?? string.Empty,
                    BirthDate = birthDate,
                    Nationality = nationality,
                    TechnicalFormation = technicalFormation ?? string.Empty,
                    PlayerHistory = playerHistory,
                    Height = height,
                    SkinColor = skinColor,
                    VitaCertified = vitaCheckbox.IsChecked == true,
                    CcslCertified = ccslCheckbox.IsChecked == true
                };

                LoadingScreen loadingScreen = new LoadingScreen();
                loadingScreen.Show();

                try
                {
                    string playerId = await Task.Run(() => _playerManagerService.InsertPlayerManager(playerManager));

                    Dispatcher.Invoke(() =>
                    {
                        loadingScreen.Close();

                        Views.Office OfficeWindow = new Views.Office(playerId, _connectionString);
                        OfficeWindow.Show();

                        DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
                        fadeOutAnimation.Completed += (s, a) =>
                        {
                            this.Hide();
                            this.Close();
                        };

                        this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
                    });
                }
                catch (Exception ex)
                {
                    loadingScreen.Close();
                    MessageBox.Show("Ocorreu um erro ao criar o treinador: " + ex.Message);
                }
            }
        }
    }
}

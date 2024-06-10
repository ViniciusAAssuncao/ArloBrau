using ArloBrau.Config;
using ArloBrau.Models;
using ArloBrau.Services;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ArloBrau.Views
{
    public partial class CreateManager : Window
    {
        private readonly PlayerManagerService _playerManagerService;
        private readonly SaveService _saveService;
        private readonly GameDateService _gameDateService;
        private readonly string _connectionString;
        private DateTime _baseGameDate;

        public CreateManager()
        {
            InitializeComponent();

            DatabaseConfig dbConfig = new DatabaseConfig();
            _connectionString = dbConfig.ConnectionString;
            _playerManagerService = new PlayerManagerService(_connectionString);
            _saveService = new SaveService(_connectionString);
            _gameDateService = new GameDateService(_connectionString);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadContentAsync();
            _baseGameDate = new DateTime(2125, 1, 1);

            PopulateDateSelectors();

            MainContentGrid.Visibility = Visibility.Visible;
            LoadingGrid.Visibility = Visibility.Collapsed;
        }

        private void PopulateDateSelectors()
        {
            for (int i = 1; i <= 31; i++)
            {
                birthDayComboBox.Items.Add(new ComboBoxItem { Content = i.ToString("D2") });
            }

            for (int i = 1; i <= 12; i++)
            {
                birthMonthComboBox.Items.Add(new ComboBoxItem { Content = i.ToString("D2") });
            }

            for (int i = 2090; i >= 2025; i--)
            {
                birthYearComboBox.Items.Add(new ComboBoxItem { Content = i.ToString() });
            }
        }

        public async Task LoadContentAsync()
        {
            await Task.Delay(500);
        }

        private void BirthDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FormatBirthDate();
        }

        private void FormatBirthDate()
        {
            if (birthDayComboBox.SelectedItem != null && birthMonthComboBox.SelectedItem != null && birthYearComboBox.SelectedItem != null)
            {
                string day = ((ComboBoxItem)birthDayComboBox.SelectedItem).Content.ToString();
                string month = ((ComboBoxItem)birthMonthComboBox.SelectedItem).Content.ToString();
                string year = ((ComboBoxItem)birthYearComboBox.SelectedItem).Content.ToString();

                if (DateTime.TryParseExact($"{day}/{month}/{year}", "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    int age = 2125 - parsedDate.Year;
                    if (new DateTime(2125, 1, 1).DayOfYear < parsedDate.DayOfYear) age--;

                    age = Math.Max(0, age);

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
                else
                {
                    birthDateError.Visibility = Visibility.Visible;
                    birthDateError.Text = "Data de nascimento inválida";
                }
            }
        }

        private void StartDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (startDateComboBox.SelectedItem != null)
            {
                string selectedSeason = ((ComboBoxItem)startDateComboBox.SelectedItem).Content.ToString();
                switch (selectedSeason)
                {
                    case "Temporada 2124/25 AH":
                        startDateTextBlock.Text = "Seu jogo iniciará na temporada 24/25 AH (01/05/2124 AH)";
                        break;
                    case "Temporada 2125/26 AH":
                        startDateTextBlock.Text = "Seu jogo iniciará na temporada 25/26 AH (01/05/2125 AH)";
                        break;
                    default:
                        startDateTextBlock.Text = string.Empty;
                        break;
                }
            }
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

            DateTime birthDate = DateTime.MinValue;

            if (birthDayComboBox.SelectedItem == null || birthMonthComboBox.SelectedItem == null || birthYearComboBox.SelectedItem == null ||
                !DateTime.TryParseExact($"{((ComboBoxItem)birthDayComboBox.SelectedItem).Content.ToString()}/{((ComboBoxItem)birthMonthComboBox.SelectedItem).Content.ToString()}/{((ComboBoxItem)birthYearComboBox.SelectedItem).Content.ToString()}", "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate) || 2125 - birthDate.Year < 35)
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

            if (startDateComboBox.SelectedItem == null)
            {
                startDateTextBlock.Text = "Por favor, selecione uma data de início.";
                isValid = false;
            }

            if (isValid)
            {
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

                    DateTime startDate = GetSelectedStartDate();

                    await Task.Run(() => _gameDateService.InsertGameDate(startDate));

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

        private DateTime GetSelectedStartDate()
        {
            if (startDateComboBox.SelectedItem != null)
            {
                string selectedSeason = ((ComboBoxItem)startDateComboBox.SelectedItem).Content.ToString();
                switch (selectedSeason)
                {
                    case "Temporada 2124/25 AH":
                        return new DateTime(2124, 5, 1);
                    case "Temporada 2125/26 AH":
                        return new DateTime(2125, 5, 1);
                    default:
                        throw new InvalidOperationException("Data de início inválida selecionada.");
                }
            }
            throw new InvalidOperationException("Data de início não selecionada.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using ArloBrau.Config;
using ArloBrau.Models;
using ArloBrau.Services;

namespace ArloBrau
{
    public partial class MainWindow : Window
    {
        private readonly SaveService _saveService;
        private readonly string _connectionString;

        public MainWindow()
        {
            InitializeComponent();

            DatabaseConfig dbConfig = new DatabaseConfig();
            _connectionString = dbConfig.ConnectionString;
            _saveService = new SaveService(_connectionString);

            CheckSaves();
        }

        private async void CheckSaves()
        {
            Views.LoadingScreen loadingScreen = new Views.LoadingScreen();
            loadingScreen.Show();

            List<Save> saves = await Task.Run(() => _saveService.GetAllSavesAsync());
            if (saves.Count == 0)
            {
                loadingScreen.Close();
                LoadSaveButton.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                loadingScreen.Close();
                LoadSaveButton.Visibility = Visibility.Visible;
            }
        }

        private async void onLoadSaveHandler(object sender, RoutedEventArgs e)
        {
            var loadingScreen = new Views.LoadingScreen();
            loadingScreen.Show();


            List<Save> saves = await Task.Run(() => _saveService.GetAllSavesAsync());

            loadingScreen.Close();

            if (saves.Count > 0)
            {
                var saveListWindow = new Views.SaveListWindow(saves, _saveService);

                saveListWindow.SavesUpdated += OnSavesUpdated;

                saveListWindow.Owner = this;
                saveListWindow.ShowDialog();

                if (saveListWindow.SelectedSave != null)
                {
                    Views.Office officeWindow = new Views.Office(saveListWindow.SelectedSave.SaveId.ToString(), _connectionString, false);
                    officeWindow.Show();
                    this.Close();
                }
            }
        }

        private void OnSavesUpdated()
        {
            CheckSaves();
        }

        private void onPlayHandler(object sender, RoutedEventArgs e)
        {
            Views.CreateManager createManagerWindow = new Views.CreateManager();
            DoubleAnimation fadeOutAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.5)));
            fadeOutAnimation.Completed += async (s, a) =>
            {
                this.Hide();
                createManagerWindow.Show();

                await createManagerWindow.LoadContentAsync();

                this.Close();
            };

            this.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }
    }
}

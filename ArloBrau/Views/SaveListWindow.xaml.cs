using ArloBrau.Models;
using ArloBrau.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ArloBrau.Views
{
    public partial class SaveListWindow : Window
    {
        private readonly SaveService _saveService;

        public event Action SavesUpdated;

        public Save SelectedSave { get; private set; }

        public SaveListWindow(List<Save> saves, SaveService saveService)
        {
            InitializeComponent();
            SaveListBox.ItemsSource = saves;
            _saveService = saveService;
        }

        private void OnSelectSaveButtonClick(object sender, RoutedEventArgs e)
        {
            SelectedSave = SaveListBox.SelectedItem as Save;
            this.DialogResult = true;
            this.Close();
        }

        private async void OnDeleteSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int saveId)
            {
                var result = MessageBox.Show("Você tem certeza que deseja deletar este save?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _saveService.DeleteSaveAsync(saveId);
                        var saves = await _saveService.GetAllSavesAsync();
                        SaveListBox.ItemsSource = saves;

                        SavesUpdated?.Invoke();

                        if (saves.Count == 0)
                        {
                            this.DialogResult = false;
                            this.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao deletar o save: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}

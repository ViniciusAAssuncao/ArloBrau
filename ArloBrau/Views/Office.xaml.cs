using System;
using System.Data;
using System.Windows;
using ArloBrau.Services;

namespace ArloBrau.Views
{
    public partial class Office : Window
    {
        private readonly SaveService _saveService;
        private readonly string _playerId;

        public Office(string playerId, string connectionString)
        {
            InitializeComponent();
            _playerId = playerId;
            _saveService = new SaveService(connectionString);

            Loaded += Office_Loaded;
        }

        private void Office_Loaded(object sender, RoutedEventArgs e)
        {
            ShowSaveDialog();
        }

        private void ShowSaveDialog()
        {
            SaveDialog saveDialog = new SaveDialog
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            bool? dialogResult = saveDialog.ShowDialog();

            if (dialogResult == true && saveDialog.IsSaveConfirmed)
            {
                string saveName = saveDialog.SaveName;
                try
                {
                    CreateSaveAndAssignToPlayer(saveName);
                }
                catch (DuplicateNameException ex)
                {
                    MessageBox.Show(ex.Message, "Nome do Save Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                    ShowSaveDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao salvar o jogo: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Você pode salvar o jogo futuramente pelo menu.", "Save Cancelado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void CreateSaveAndAssignToPlayer(string saveName)
        {
            try
            {
                int newSaveId = _saveService.CreateSave(saveName);
                _saveService.AssignSaveToPlayer(_playerId, newSaveId);
                MessageBox.Show("Jogo salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DuplicateNameException ex)
            {
                throw;
            }
        }
    }
}

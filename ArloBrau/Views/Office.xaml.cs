using System;
using System.Data;
using System.Windows;
using ArloBrau.Models;
using ArloBrau.Services;

namespace ArloBrau.Views
{
    public partial class Office : Window
    {
        private readonly SaveService _saveService;
        private readonly GameDateService _gameDateService;
        private readonly string _playerId;
        private readonly string _connectionString;
        private readonly bool _isNewPlayer;

        public Office(string playerId, string connectionString, bool isNewPlayer = true)
        {
            InitializeComponent();
            _playerId = playerId;
            _connectionString = connectionString;
            _isNewPlayer = isNewPlayer;
            _saveService = new SaveService(connectionString);
            _gameDateService = new GameDateService(connectionString);

            Loaded += Office_Loaded;
        }

        private void Office_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isNewPlayer)
            {
                ShowSaveDialog();
            }
            else
            {
                LoadExistingSave();
            }
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

                _gameDateService.UpdateGameDateWithSaveId(newSaveId);

                MessageBox.Show("Jogo salvo com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (DuplicateNameException ex)
            {
                throw;
            }
        }

        private void LoadExistingSave()
        {
            // Adicione aqui o código para carregar o save existente
            // Carregue as informações do save e inicialize os componentes necessários
        }
    }
}

using System;
using System.Windows;

namespace ArloBrau.Views
{
    public partial class SaveDialog : Window
    {
        public string SaveName { get; private set; } = string.Empty;
        public bool IsSaveConfirmed { get; private set; } = false;

        public SaveDialog()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveName = SaveNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(SaveName))
            {
                SaveName = "Save Automático " + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            IsSaveConfirmed = true;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmCancelTextBlock.Visibility == Visibility.Collapsed)
            {
                ConfirmCancelTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                this.DialogResult = false;
            }
        }
    }
}

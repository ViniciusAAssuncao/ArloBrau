using System.Windows;
using System.Windows.Media.Animation;

namespace ArloBrau
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

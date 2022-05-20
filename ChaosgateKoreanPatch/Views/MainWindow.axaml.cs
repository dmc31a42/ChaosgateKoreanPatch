using Avalonia.Controls;
using Avalonia.Interactivity;
using ChaosgateKoreanPatch.ViewModels;

namespace ChaosgateKoreanPatch.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public async void onOpenFolderButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog();
            var result = await dialog.ShowAsync(this);
            
            if (result != null && DataContext != null)
            {
                ((MainWindowViewModel)DataContext).GameDirectory = result;
            }
        }
    }
}

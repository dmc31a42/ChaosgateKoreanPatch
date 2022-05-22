using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ChaosgateKoreanPatch.ViewModels;
using ReactiveUI;

namespace ChaosgateKoreanPatch.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.LogTextBox = this.FindControl<TextBox>("LogTextBox");

            this.DataContextChanged += (object? sender, EventArgs e) =>
            {
                ((MainWindowViewModel)this.DataContext)?.WhenAnyValue(x => x.Log)
                .Subscribe(x => Dispatcher.UIThread.Post(() => this.LogTextBox.CaretIndex = int.MaxValue));
            };
            
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

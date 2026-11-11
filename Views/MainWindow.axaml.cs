using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MultiMediaPlayer.Lab.ViewModels;
using System;

namespace MultiMediaPlayer.Lab.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            if (DataContext is MainWindowViewModel vm)
            {
                VideoView.MediaPlayer = vm.MediaPlayer;
            }
        }
    }
}
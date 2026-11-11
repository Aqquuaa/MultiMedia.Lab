using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using MultiMediaPlayer.Lab.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiMediaPlayer.Lab.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public PlaybackEngine Engine { get; } = new PlaybackEngine();
        public MediaPlayer MediaPlayer => Engine.Player;

        [ObservableProperty]
        private string currentTime = "00:00:00";

        [ObservableProperty]
        private string totalTime = "00:00:00";

        [ObservableProperty]
        private int volume = 50;

        [ObservableProperty]
        private string mediaUrl = "";
        [ObservableProperty] 
        private bool hasVideo = true;
        [ObservableProperty] 
        private bool isAudioOnly = false;

        private double playbackPosition;

        public double PlaybackPosition
        {
            get => playbackPosition;
            set
            {
                if (SetProperty(ref playbackPosition, value))
                {
                    if (MediaPlayer != null && Math.Abs(MediaPlayer.Position - value) > 0.01)
                    {
                        MediaPlayer.Position = (float)value;
                        Engine.ResetTracking();
                    }
                }
            }
        }

        public MainWindowViewModel()
        {
            Engine.Player.Volume = this.Volume;

            Engine.Player.TimeChanged += (s, e) => Dispatcher.UIThread.Post(() =>
                CurrentTime = TimeSpan.FromMilliseconds(e.Time).ToString(@"hh\:mm\:ss"));

            Engine.Player.PositionChanged += (s, e) => Dispatcher.UIThread.Post(() =>
            {
                playbackPosition = e.Position;
                OnPropertyChanged(nameof(PlaybackPosition));
            });

            Engine.Player.LengthChanged += (s, e) => Dispatcher.UIThread.Post(() =>
                TotalTime = TimeSpan.FromMilliseconds(e.Length).ToString(@"hh\:mm\:ss"));

            Engine.Player.Playing += (s, e) => Dispatcher.UIThread.Post(() =>
            {
                var tracks = Engine.Player.Media?.Tracks;
                if (tracks != null)
                {
                    bool hasVideoTrack = tracks.Any(t => t.TrackType == TrackType.Video);
                    HasVideo = hasVideoTrack;
                    IsAudioOnly = !hasVideoTrack;
                }
            });

        }
        partial void OnVolumeChanged(int value) => Engine.Player.Volume = value;

        [RelayCommand]
        private async Task OpenFile(Window window)
        {
            var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Media File",
                AllowMultiple = false
            });

            if (files.Count > 0)
                Engine.LoadLocalFile(files[0].Path.LocalPath);
        }

        [RelayCommand]
        private void OpenUrl()
        {
            if (!string.IsNullOrWhiteSpace(MediaUrl))
            {
                HasVideo = true;
                IsAudioOnly = false;
                Engine.LoadUrl(MediaUrl);
            }
        }
        [RelayCommand]
        private void Play()
        {
            if (!Avalonia.Controls.Design.IsDesignMode)
                Engine.Play();
        }

        [RelayCommand] private void Pause() => Engine.Pause();
        [RelayCommand] private void Stop() => Engine.Stop();

        [RelayCommand] private void SkipForward() => Engine.Skip(10000);
        [RelayCommand] private void SkipBackward() => Engine.Skip(-10000);

        [RelayCommand] private void StepFrameForward() => Engine.StepForward();
        [RelayCommand] private void StepFrameBackward() => Engine.StepBackward();

        public void Dispose() => Engine.Dispose();
    }
}
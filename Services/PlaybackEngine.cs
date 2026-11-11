using LibVLCSharp.Shared;
using System;

namespace MultiMediaPlayer.Lab.Services
{
    public class PlaybackEngine : IDisposable
    {
        private readonly LibVLC libVlc;
        public MediaPlayer Player { get; }
        private Media? currentMedia;

        private double frameDurationMs = 10; 
        private long targetTimeMs = -1;

        public PlaybackEngine()
        {
            libVlc = new LibVLC();
            Player = new MediaPlayer(libVlc);
        }

        public void LoadLocalFile(string path)
        {
            currentMedia?.Dispose();
            currentMedia = new Media(libVlc, path, FromType.FromPath);
            Player.Play(currentMedia);
            targetTimeMs = -1;
        }

        public void LoadUrl(string url)
        {
            currentMedia?.Dispose();
            currentMedia = new Media(libVlc, url, FromType.FromLocation);
            Player.Play(currentMedia);
            targetTimeMs = -1;
        }

        public void Play()
        {
            Player.Play();
            targetTimeMs = -1;
        }

        public void Pause() => Player.SetPause(true);

        public void Stop() => Player.Stop();

        public void Skip(long milliseconds)
        {
            if (Player.Time + milliseconds > 0 && Player.Time + milliseconds < Player.Length)
                Player.Time += milliseconds;
            else if (Player.Time + milliseconds <= 0)
                Player.Time = 0;

            targetTimeMs = -1;
        }

        public void StepForward()
        {
            if (Player.IsPlaying) Player.SetPause(true);
            Player.NextFrame();
            targetTimeMs = -1;
        }

        public void StepBackward()
        {
            if (!Player.IsSeekable) return;
            if (Player.IsPlaying) Player.SetPause(true);

            if (targetTimeMs < 0 || Math.Abs(Player.Time - targetTimeMs) > frameDurationMs * 3)
                targetTimeMs = Player.Time;

            targetTimeMs = Math.Max(0, targetTimeMs - (long)Math.Round(frameDurationMs));
            Player.Time = targetTimeMs;
        }
        public void ResetTracking() => targetTimeMs = -1;

        public void Dispose()
        {
            currentMedia?.Dispose();
            Player.Dispose();
            libVlc.Dispose();
        }
    }
}

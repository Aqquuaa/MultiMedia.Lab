using System;

namespace MultiMediaPlayer.Lab.Models
{
    public class MediaItem
    {
        public required string Title { get; init; }
        public required string FilePathOrUrl { get; init; }
        public MediaFormat Format { get; init; }
        public MediaType Type { get; init; }
        public TimeSpan Duration { get; init; }

    }
}

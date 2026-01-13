using System.Collections.Frozen;

namespace WowVideoConverter;

static class Constants
{
    public static class VideoExtensions
    {
        public const string Mp4 = ".mp4";
        public const string Mkv = ".mkv";
        public const string Mov = ".mov";
        public const string Avi = ".avi";
        public const string Wmv = ".wmv";
        public const string Webm = ".webm";

        public static FrozenSet<string> All { get; } = new[]
        {
            Mp4, Mkv, Mov, Avi, Wmv, Webm,
        }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
    }
}

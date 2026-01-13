namespace WowVideoConverter.FFmpeg;

static class FFmpegConstants
{
    public static class VideoCodecs
    {
        /// <summary> NVIDIA GPU hardware encoder. </summary>
        public const string H264Nvenc = "h264_nvenc";

        /// <summary> AMD GPU hardware encoder. </summary>
        public const string H264Amf = "h264_amf";

        /// <summary> Intel GPU (QuickSync) hardware encoder. </summary>
        public const string H264Qsv = "h264_qsv";

        /// <summary> Mac (Apple Silicon/VideoToolbox) hardware encoder. </summary>
        public const string H264VideoToolbox = "h264_videotoolbox";

        /// <summary> Software encoder (CPU). Slower but high quality and widely supported. </summary>
        public const string Libx264 = "libx264";

        /// <summary> Default H.264 encoder. </summary>
        public const string H264 = "h264";
    }

    public static class Presets
    {
        /// <summary> Adjusts encoding speed vs quality trade-off. Higher quality. </summary>
        public const string Slow = "slow";
    }

    public static class RateControls
    {
        /// <summary> Variable Bitrate control method. </summary>
        public const string Vbr = "vbr";
    }

    public static class Tunes
    {
        /// <summary> Optimizes encoding for high quality content. </summary>
        public const string Hq = "hq";
    }
}

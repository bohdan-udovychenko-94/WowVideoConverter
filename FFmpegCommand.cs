namespace WowVideoConverter;

class FFmpegCommand
{
    public string? StartOffset { get; init; }
    public string InputPath { get; init; } = string.Empty;
    public bool DisableAudio { get; init; }
    public string VideoFilter { get; init; } = string.Empty;
    public string VideoCodec { get; init; } = string.Empty;
    public string Preset { get; init; } = string.Empty;
    public string RateControl { get; init; } = string.Empty;
    public string Bitrate { get; init; } = string.Empty;
    public string Tune { get; init; } = string.Empty;
    public string Multipass { get; init; } = string.Empty;
    public string OutputPath { get; init; } = string.Empty;
}

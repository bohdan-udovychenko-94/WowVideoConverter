using WowVideoConverter.FFmpeg;

class AvailableEncodersFFmpegCommand : IFFmpegCommand
{
    [FFmpegArg(1, "-encoders")]
    public bool Encoders { get; } = true;
}
namespace WowVideoConverter.FFmpeg;

class EncodeVideoFFmpegCommand : IFFmpegCommand
{
    /// <summary>
    /// Start offset relative to EOF (e.g., "-60").
    /// Must appear before input path.
    /// </summary>
    [FFmpegArg(1, "-sseof")]
    public string? StartOffset { get; init; }

    /// <summary>
    /// Path to the input media file. Required. Maps to <c>-i</c>.
    /// </summary>
    [FFmpegArg(2, "-i")]
    public required string InputPath { get; init; }

    /// <summary>
    /// Disables audio in the output. Maps to <c>-an</c>.
    /// </summary>
    [FFmpegArg(3, "-an")]
    public bool DisableAudio { get; init; }

    /// <summary>
    /// Video bitrate (e.g., "10M"). Maps to <c>-b:v</c>.
    /// </summary>
    [FFmpegArg(4, "-b:v")]
    public string? Bitrate { get; init; }

    /// <summary>
    /// Video codec (e.g., "h264_nvenc"). Maps to <c>-c:v</c>.
    /// </summary>
    [FFmpegArg(5, "-c:v")]
    public string? VideoCodec { get; init; }

    /// <summary>
    /// Multi-pass encoding (e.g., "2"). Maps to <c>-multipass</c>.
    /// </summary>
    [FFmpegArg(6, "-multipass")]
    public string? Multipass { get; init; }

    /// <summary>
    /// Preset for encoding speed/quality. Maps to <c>-preset</c>.
    /// </summary>
    [FFmpegArg(7, "-preset")]
    public string? Preset { get; init; }

    /// <summary>
    /// Rate control method (e.g., "vbr"). Maps to <c>-rc</c>.
    /// </summary>
    [FFmpegArg(8, "-rc")]
    public string? RateControl { get; init; }

    /// <summary>
    /// Tune parameter (e.g., "hq"). Maps to <c>-tune</c>.
    /// </summary>
    [FFmpegArg(9, "-tune")]
    public string? Tune { get; init; }

    /// <summary>
    /// Video filter chain (e.g., "scale=1280:720"). Maps to <c>-vf</c>.
    /// </summary>
    [FFmpegArg(10, "-vf")]
    public string? VideoFilter { get; init; }

    /// <summary>
    /// Path for the output media file. Required. Positional argument. Always should go last.
    /// </summary>
    [FFmpegArg(int.MaxValue)]
    public required string OutputPath { get; init; }
}

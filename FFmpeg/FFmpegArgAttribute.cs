namespace WowVideoConverter.FFmpeg;

[AttributeUsage(AttributeTargets.Property)]
sealed class FFmpegArgAttribute(int order, string? name = null) : Attribute
{
    public int Order { get; } = order;
    public string? Name { get; } = name;
}

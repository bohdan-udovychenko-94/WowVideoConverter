using System.Diagnostics;
using System.Reflection;

namespace WowVideoConverter.FFmpeg;

static class FFmpegRunner
{
    public static string Run(IFFmpegCommand command, bool redirectStandardOutput = false)
    {
        var args = BuildArguments(command);

        using var process = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = string.Join(" ", args),
            // UseShellExecute = false -> c# program will handle input/output in code, just run the executable
            UseShellExecute = false,
            CreateNoWindow = false,
            RedirectStandardOutput = redirectStandardOutput,
        }) ?? throw new InvalidOperationException("Failed to start ffmpeg process");

        string output = redirectStandardOutput ? process.StandardOutput.ReadToEnd() : string.Empty;
        process.WaitForExit();
        return output;
    }

    private static IEnumerable<string> BuildArguments(IFFmpegCommand command)
    {
        var props = command.GetType().GetProperties()
            .Select(x => new
            {
                Prop = x,
                Attr = x.GetCustomAttribute<FFmpegArgAttribute>(),
                Value = x.CanRead ? x.GetValue(command) : null,
            })
            .Where(x => x.Attr is not null && x.Value is not null)
            .OrderBy(x => x.Attr!.Order);

        foreach (var prop in props)
        {
            if (prop.Value is bool flag && flag && prop.Attr.Name is not null)
            {
                yield return $"\"{prop.Attr.Name}\"";
            }

            if (prop.Value is string str)
            {
                if (prop.Attr.Name is not null)
                {
                    yield return $"\"{prop.Attr.Name}\"";
                }

                yield return $"\"{str}\"";
            }
        }
    }
}

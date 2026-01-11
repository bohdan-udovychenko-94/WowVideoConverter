using System.Diagnostics;

namespace WowVideoConverter;

static class FFmpegRunner
{
    public static void Run(FFmpegCommand command)
    {
        var args = BuildArguments(command);
        var quotedArgs = args.Select(arg => $"\"{arg}\"");
        
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = string.Join(" ", quotedArgs),
            UseShellExecute = false,
            CreateNoWindow = false
        });

        if (process == null)
        {
            Console.Error.WriteLine("Error: Failed to start ffmpeg process.");
            return;
        }

        process.WaitForExit();
    }

    private static List<string> BuildArguments(FFmpegCommand command)
    {
        var args = new List<string>();

        if (!string.IsNullOrEmpty(command.StartOffset))
        {
            args.Add("-sseof");
            args.Add(command.StartOffset);
        }

        args.Add("-i");
        args.Add(command.InputPath);

        if (command.DisableAudio)
        {
            args.Add("-an");
        }

        if (!string.IsNullOrEmpty(command.VideoFilter))
        {
            args.Add("-vf");
            args.Add(command.VideoFilter);
        }

        if (!string.IsNullOrEmpty(command.VideoCodec))
        {
            args.Add("-c:v");
            args.Add(command.VideoCodec);
        }

        if (!string.IsNullOrEmpty(command.Preset))
        {
            args.Add("-preset");
            args.Add(command.Preset);
        }

        if (!string.IsNullOrEmpty(command.RateControl))
        {
            args.Add("-rc");
            args.Add(command.RateControl);
        }

        if (!string.IsNullOrEmpty(command.Bitrate))
        {
            args.Add("-b:v");
            args.Add(command.Bitrate);
        }

        if (!string.IsNullOrEmpty(command.Tune))
        {
            args.Add("-tune");
            args.Add(command.Tune);
        }

        if (!string.IsNullOrEmpty(command.Multipass))
        {
            args.Add("-multipass");
            args.Add(command.Multipass);
        }

        args.Add("-movflags");
        args.Add("+faststart");

        args.Add(command.OutputPath);

        return args;
    }
}

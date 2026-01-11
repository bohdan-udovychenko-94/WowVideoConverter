namespace WowVideoConverter;

class Program
{
    public static void Main()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("USERPROFILE")))
            {
                Console.Error.WriteLine("Environment variable USERPROFILE not found");
                return;
            }

            string inputFolder = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE")!, "Videos", "NVIDIA", "World Of Warcraft");
            string videosFolder = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE")!, "Videos");

            Console.WriteLine($"Input folder: {inputFolder}");
            Console.WriteLine($"Output folder: {videosFolder}");
            Console.WriteLine();

            Console.WriteLine($"Script started at: {DateTime.Now:HH:mm:ss}");
            Console.WriteLine();

            if (!Directory.Exists(inputFolder))
            {
                Console.Error.WriteLine("Input folder not found.");
                return;
            }

            foreach (var path in Directory.EnumerateFiles(inputFolder, "*.mp4", SearchOption.TopDirectoryOnly))
            {
                Console.WriteLine($"Processing: {Path.GetFileName(path)}");
                var startTime = DateTime.Now;

                CreateShortVersion(path, Path.Combine(videosFolder, "short_" + Path.GetFileName(path)));
                CreateFullVersion(path, Path.Combine(videosFolder, "full_" + Path.GetFileName(path)));

                Console.WriteLine($"Time taken: {(DateTime.Now - startTime).TotalSeconds:F0} seconds\n");
            }

            Console.WriteLine($"Script finished at: {DateTime.Now:HH:mm:ss}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
        }
    }

    static void CreateShortVersion(string inputPath, string outputPath)
    {
        if (File.Exists(outputPath))
        {
            Console.WriteLine("Short version already exists, skipping.");
            return;
        }

        Console.WriteLine($"Creating short version: {outputPath}");

        FFmpegRunner.Run(new FFmpegCommand
        {
            StartOffset = "-60",
            InputPath = inputPath,
            DisableAudio = true,
            VideoFilter = "scale=-2:1920,crop=1080:1920",
            VideoCodec = "h264_nvenc",
            Preset = "slow",
            RateControl = "vbr",
            Bitrate = "10M",
            Tune = "hq",
            Multipass = "2",
            OutputPath = outputPath
        });
    }

    static void CreateFullVersion(string inputPath, string outputPath)
    {
        if (File.Exists(outputPath))
        {
            Console.WriteLine("  Full version already exists, skipping.");
            return;
        }

        Console.WriteLine($"  Creating full version: {outputPath}");

        FFmpegRunner.Run(new FFmpegCommand
        {
            InputPath = inputPath,
            DisableAudio = true,
            VideoFilter = "scale=-2:'min(1440,ih)'",
            VideoCodec = "h264_nvenc",
            Preset = "slow",
            RateControl = "vbr",
            Bitrate = "16M",
            Tune = "hq",
            Multipass = "2",
            OutputPath = outputPath
        });
    }
}
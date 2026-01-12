using WowVideoConverter.FFmpeg;

namespace WowVideoConverter;

class Program
{
    public static void Main()
    {
        try
        {
            string? userProfileFolder = Environment.GetEnvironmentVariable("USERPROFILE");
            if (string.IsNullOrWhiteSpace(userProfileFolder))
            {
                Console.Error.WriteLine("Environment variable USERPROFILE is not found");
                Console.ReadLine();
                return;
            }

            string? baseInputFolder = Environment.GetEnvironmentVariable("WOW_VIDEO_CONVERTER_BASE_INPUT_FOLDER");
            if (string.IsNullOrWhiteSpace(baseInputFolder))
            {
                Console.Error.WriteLine("Environment variable WOW_VIDEO_CONVERTER_BASE_INPUT_FOLDER is not found");
                Console.ReadLine();
                return;
            }

            string videosFolder = Path.Combine(userProfileFolder, "Videos");
            Console.WriteLine($"Input folder: {baseInputFolder}");
            string baseOutputFolder = Path.Combine(videosFolder, "WowVideoConverter");
            Console.WriteLine($"Base output folder: {baseOutputFolder}");
            Console.WriteLine();

            Console.WriteLine($"Script started at: {DateTime.Now:HH:mm:ss}");
            Console.WriteLine();

            if (!Directory.Exists(baseInputFolder))
            {
                Console.Error.WriteLine("Input folder not found.");
                Console.ReadLine();
                return;
            }

            string codec = GetCodec();
            Console.WriteLine($"Selected codec: {codec}");

            foreach (var inputPath in Directory.EnumerateFiles(baseInputFolder, "*.mp4", SearchOption.AllDirectories))
            {
                Console.WriteLine($"Processing: {Path.GetFileName(inputPath)}");
                var startTime = DateTime.Now;
                string inputFileName = Path.GetFileName(inputPath);

                string PreparePath(string fileNamePrefix)
                {
                    string outputPath = inputPath
                        .Replace(baseInputFolder, baseOutputFolder)
                        .Replace(inputFileName, fileNamePrefix + inputFileName);
                    
                    string outputFolder = Path.GetDirectoryName(outputPath)!;
                    if(!Directory.Exists(outputFolder))
                    {
                        Directory.CreateDirectory(outputFolder);  
                    }

                    return outputPath;
                }

                string shortVideOutputPath = PreparePath("short_");
                string fullVideOutputPath = PreparePath("full_");

                CreateShortVersion(inputPath, shortVideOutputPath, codec);
                CreateFullVersion(inputPath, fullVideOutputPath, codec);
                Console.WriteLine($"Time taken: {(DateTime.Now - startTime).TotalSeconds:F0} seconds\n");
            }

            Console.WriteLine($"Script finished at: {DateTime.Now:HH:mm:ss}");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            Console.ReadLine();
        }
    }

    static string GetCodec()
    {
        string output = FFmpegRunner.Run(new AvailableEncodersFFmpegCommand(), redirectStandardOutput: true);

        if(output.Contains("h264_nvenc", StringComparison.OrdinalIgnoreCase))
        {
            return "h264_nvenc"; // for NVIDIA GPU
        }

        if(output.Contains("h264_amf", StringComparison.OrdinalIgnoreCase))
        {
            return "h264_amf"; // for AMD GPU
        }

        if(output.Contains("h264_qsv", StringComparison.OrdinalIgnoreCase))
        {
            return "h264_qsv"; // for // INTEL GPU
        }

        // Software encoder, slower but widely supported, full feature set. (use CPU)
        if (output.Contains("libx264", StringComparison.OrdinalIgnoreCase))
        {
            return "libx264";
        }

        return "h264";
    }

    static void CreateShortVersion(string inputPath, string outputPath, string codec)
    {
        if (File.Exists(outputPath))
        {
            Console.WriteLine("Short version already exists, skipping.");
            return;
        }

        Console.WriteLine($"Creating short version: {outputPath}");

        FFmpegRunner.Run(new EncodeVideoFFmpegCommand
        {
            InputPath = inputPath,
            OutputPath = outputPath,
            Bitrate = "10M",
            DisableAudio = true,
            // Two-pass encoding improves quality/bitrate distribution, especially at constrained bitrates
            Multipass = "2",
            // Adjusts encoding speed vs quality trade-off. "slow" -> higher quality
            Preset = "slow",
            RateControl = "vbr",
            StartOffset = "-60",
            // Optimizes encoding for a specific quality or type of content. "hq" = high quality
            Tune = "hq",
            VideoCodec = codec,
            // Video filter chain applied during encoding:
            // - scale=-2:1920 -> scale height to 1920px,
            // - width adjusted automatically to maintain aspect ratio (-2 rounds to nearest multiple of 2).
            // - crop=1080:1920 -> crop width/height to exactly 1080x1920.
            VideoFilter = "scale=-2:1920,crop=1080:1920",
        });
    }

    static void CreateFullVersion(string inputPath, string outputPath, string codec)
    {
        if (File.Exists(outputPath))
        {
            Console.WriteLine("Full version already exists, skipping.");
            return;
        }

        Console.WriteLine($"Creating full version: {outputPath}");

        FFmpegRunner.Run(new EncodeVideoFFmpegCommand
        {
            InputPath = inputPath,
            OutputPath = outputPath,
            Bitrate = "16M",
            DisableAudio = true,
            // Two-pass encoding improves quality/bitrate distribution, especially at constrained bitrates
            Multipass = "2",
            // Adjusts encoding speed vs quality trade-off. "slow" -> higher quality
            Preset = "slow",
            RateControl = "vbr",
            // Optimizes encoding for a specific quality or type of content. "hq" = high quality.
            Tune = "hq",
            VideoCodec = codec,
            // Video filter chain applied during encoding:
            // scale=-2:'min(1080,ih)' -> scales height to 1080px, width adjusted to preserve aspect ratio
            // (the -2 ensures width is divisible by 2, as required by many codecs)
            VideoFilter = "scale=-2:'min(1080,ih)'",
        });
    }
}
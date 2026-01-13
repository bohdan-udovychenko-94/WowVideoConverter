using WowVideoConverter.FFmpeg;

namespace WowVideoConverter;

class Program
{
    public static void Main()
    {
        try
        {
            string? inputFolder = Environment.GetEnvironmentVariable("WOW_VIDEO_CONVERTER_INPUT_FOLDER");
            if (string.IsNullOrWhiteSpace(inputFolder))
            {
                throw new InvalidOperationException("Environment variable WOW_VIDEO_CONVERTER_INPUT_FOLDER is not found");
            }
            Console.WriteLine($"Input folder: {inputFolder}");

            string? outputFolder = Environment.GetEnvironmentVariable("WOW_VIDEO_CONVERTER_OUTPUT_FOLDER");
            if (string.IsNullOrWhiteSpace(outputFolder))
            {
                throw new InvalidOperationException("Environment variable WOW_VIDEO_CONVERTER_OUTPUT_FOLDER is not found");
            }
            Console.WriteLine($"Output folder: {outputFolder}");
            Console.WriteLine();

            Console.WriteLine($"Script started at: {DateTime.Now:HH:mm:ss}");
            Console.WriteLine();

            if (!Directory.Exists(inputFolder))
            {
                throw new DirectoryNotFoundException($"Input folder not found: {inputFolder}");
            }

            string codec = GetCodec();
            Console.WriteLine($"Selected codec: {codec}");

            var inputPathes = Directory.EnumerateFiles(inputFolder, "*.*", SearchOption.AllDirectories)
                .Where(file => Constants.VideoExtensions.All.Contains(Path.GetExtension(file)));

            foreach (var inputPath in inputPathes)
            {
                Console.WriteLine($"Processing: {Path.GetFileName(inputPath)}");
                var startTime = DateTime.Now;
                string inputFileName = Path.GetFileName(inputPath);
                string inputExtension = Path.GetExtension(inputPath);

                string PreparePath(string fileNamePrefix)
                {
                    // Ensure the output is always .mp4 for consistency, even if input is .mkv etc.
                    string outputFileName = fileNamePrefix + Path.GetFileNameWithoutExtension(inputFileName) + Constants.VideoExtensions.Mp4;
                    string relativePath = Path.GetDirectoryName(Path.GetRelativePath(inputFolder, inputPath)) ?? string.Empty;
                    string outputSubFolder = Path.Combine(outputFolder, relativePath);

                    if (!Directory.Exists(outputSubFolder))
                    {
                        Directory.CreateDirectory(outputSubFolder);
                    }

                    return Path.Combine(outputSubFolder, outputFileName);
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

        if (output.Contains(FFmpegConstants.VideoCodecs.H264Nvenc, StringComparison.OrdinalIgnoreCase))
        {
            return FFmpegConstants.VideoCodecs.H264Nvenc; // for NVIDIA GPU
        }

        if (output.Contains(FFmpegConstants.VideoCodecs.H264Amf, StringComparison.OrdinalIgnoreCase))
        {
            return FFmpegConstants.VideoCodecs.H264Amf; // for AMD GPU
        }

        if (output.Contains(FFmpegConstants.VideoCodecs.H264Qsv, StringComparison.OrdinalIgnoreCase))
        {
            return FFmpegConstants.VideoCodecs.H264Qsv; // for INTEL GPU
        }

        if (output.Contains(FFmpegConstants.VideoCodecs.H264VideoToolbox, StringComparison.OrdinalIgnoreCase))
        {
            return FFmpegConstants.VideoCodecs.H264VideoToolbox; // for Mac M GPU
        }

        // Software encoder, slower but widely supported, full feature set. (use CPU)
        if (output.Contains(FFmpegConstants.VideoCodecs.Libx264, StringComparison.OrdinalIgnoreCase))
        {
            return FFmpegConstants.VideoCodecs.Libx264;
        }

        return FFmpegConstants.VideoCodecs.H264;
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
            Preset = FFmpegConstants.Presets.Slow,
            RateControl = FFmpegConstants.RateControls.Vbr,
            StartOffset = "-60",
            // Optimizes encoding for a specific quality or type of content. "hq" = high quality
            Tune = FFmpegConstants.Tunes.Hq,
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
            Preset = FFmpegConstants.Presets.Slow,
            RateControl = FFmpegConstants.RateControls.Vbr,
            // Optimizes encoding for a specific quality or type of content. "hq" = high quality.
            Tune = FFmpegConstants.Tunes.Hq,
            VideoCodec = codec,
            // Video filter chain applied during encoding:
            // scale=-2:'min(1080,ih)' -> scales height to 1080px, width adjusted to preserve aspect ratio
            // (the -2 ensures width is divisible by 2, as required by many codecs)
            VideoFilter = "scale=-2:'min(1080,ih)'",
        });
    }
}

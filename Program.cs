using System.Diagnostics;

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

        Console.WriteLine($"  Time taken: {(DateTime.Now - startTime).TotalSeconds:F0} seconds\n");
    }

    Console.WriteLine($"Script finished at: {DateTime.Now:HH:mm:ss}");
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex);
}

static void CreateShortVersion(string inputPath, string outputPath)
{
    if (File.Exists(outputPath))
    {
        Console.WriteLine("  Short version already exists, skipping.");
        return;
    }

    Console.WriteLine($"  Creating short version: {outputPath}");
    RunFFmpeg(
        "-sseof", "-60",
        "-i", inputPath,
        "-an",
        "-vf", "scale=-2:1920,crop=1080:1920",
        "-c:v", "h264_nvenc",
        "-preset", "slow",
        "-rc", "vbr",
        "-b:v", "10M",
        "-tune", "hq",
        "-multipass", "2",
        "-movflags", "+faststart",
        outputPath
    );
}

static void CreateFullVersion(string inputPath, string outputPath)
{
    if (File.Exists(outputPath))
    {
        Console.WriteLine("  Full version already exists, skipping.");
        return;
    }

    Console.WriteLine($"  Creating full version: {outputPath}");
    RunFFmpeg(
        "-i", inputPath,
        "-an",
        "-vf", "scale=-2:'min(1440,ih)'",
        "-c:v", "h264_nvenc",
        "-preset", "slow",
        "-rc", "vbr",
        "-b:v", "16M",
        "-tune", "hq",
        "-multipass", "2",
        "-movflags", "+faststart",
        outputPath
    );
}

static void RunFFmpeg(params string[] args)
{
    var process = Process.Start(new ProcessStartInfo
    {
        FileName = "ffmpeg",
        Arguments = string.Join(" ", args),
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

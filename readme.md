# WoW Video Encoder

This C# console application processes `.mp4` videos located in your `Videos\NVIDIA\World Of Warcraft` folder and creates two versions for each video:

- **Short version:** last 60 seconds, vertical 1080x1920 resolution  
- **Full version:** full length, scaled to max height 1440, keeping aspect ratio  

Both versions are saved into your `Videos` folder with prefixes `short_` and `full_`.

---

## Prerequisites

- Windows OS  
- [FFmpeg](https://ffmpeg.org/download.html) installed and accessible via system `PATH`  
- NVIDIA GPU with NVENC support (optional but recommended for hardware acceleration)  

---

## Build

To build the standalone executable, run:

```bash
dotnet publish -c Release --self-contained -p:PublishSingleFile=true
```

The executable will be created in `release/WowVideoConverter.exe` - a single file containing all dependencies, no DLLs needed.

---

## Usage

Run the executable:

```bash
WowVideoConverter.exe
```

It will automatically:
1. Scan for `.mp4` files in `Videos\NVIDIA\World Of Warcraft`
2. Create short and full versions if they don't already exist
3. Save processed videos to your `Videos` folder
4. Display progress and timing information

# WoW Video Encoder

This C# console application processes `.mp4` videos located in your `{WOW_VIDEO_CONVERTER_BASE_INPUT_FOLDER}` folder and subfolders and creates two versions for each video:

- **Short version:** last 60 seconds, vertical 1080x1920 resolution  
- **Full version:** full length, 1080p

Both versions are saved into `{USERPROFILE}\Videos\WowVideoConverter` folder with prefixes `short_` and `full_`.

---

## Prerequisites

- Windows OS  
- [FFmpeg](https://ffmpeg.org/download.html) installed and accessible via system `PATH`
- `{WOW_VIDEO_CONVERTER_BASE_INPUT_FOLDER}` env variable contanis path to folder with source videos

---

## Build

To build the standalone executable, run:

```bash
dotnet publish -c Release --self-contained -p:PublishSingleFile=true
```

The executable will be created in `bin/Release/<net-version>/<cpu-architecture>/publish/WowVideoConverter.exe` - a single file containing all dependencies, no DLLs needed.

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

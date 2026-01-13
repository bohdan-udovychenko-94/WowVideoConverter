# WoW Video Encoder

This C# console application processes videos located in your source folder (defined by `WOW_VIDEO_CONVERTER_INPUT_FOLDER`) and its subfolders, and creates two versions for each video in the destination folder (defined by `WOW_VIDEO_CONVERTER_OUTPUT_FOLDER`):

- **Short version:** last 60 seconds, vertical 1080x1920 resolution  
- **Full version:** full length, 1080p

Both versions are saved into the `{WOW_VIDEO_CONVERTER_OUTPUT_FOLDER}` folder, mirroring the input structure, with prefixes `short_` and `full_`.

---

## Prerequisites

- Windows, macOS, or Linux  
- [FFmpeg](https://ffmpeg.org/download.html) installed and accessible via system `PATH`
- `WOW_VIDEO_CONVERTER_INPUT_FOLDER` env variable contains the path to the folder with source videos.
- `WOW_VIDEO_CONVERTER_OUTPUT_FOLDER` env variable contains the path where processed videos will be saved.

---

## Build

### Standard Build (Cross-platform)

To build the project:

```bash
dotnet build -c Release
```

The application can then be run using `dotnet WowVideoConverter.dll` from the output directory.

### Build Standalone Executable (Platform-specific)

Since `<OutputType>Exe</OutputType>` has been removed from the project file to support cross-platform builds, you **must** specify a Runtime Identifier (`-r`) when publishing to create a standalone executable.

**Windows (x64):**
```bash
dotnet publish -c Release -r win-x64
```

**macOS (Apple Silicon):**
```bash
dotnet publish -c Release -r osx-arm64
```

**Linux (x64):**
```bash
dotnet publish -c Release -r linux-x64
```

The executable (e.g., `WowVideoConverter.exe` on Windows) will be created in the `bin/Release/<target-framework>/<runtime-identifier>/publish/` folder. All publishing settings like `PublishSingleFile` and `PublishTrimmed` are already configured in the `.csproj` file.

---

## Usage

Run the executable `WowVideoConverter.exe`

It will automatically:
1. Scan for video files in the `WOW_VIDEO_CONVERTER_INPUT_FOLDER` folder
2. Create short and full versions if they don't already exist
3. Save processed videos to the `WOW_VIDEO_CONVERTER_OUTPUT_FOLDER` folder, mirroring the input subfolder structure
4. Display progress and timing information

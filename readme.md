# WoW Video Encoder

This Go program processes `.mp4` videos located in your `Videos\NVIDIA\World Of Warcraft` folder and creates two versions for each video:

- **Short version:** last 60 seconds, vertical 1080x1920 resolution  
- **Full version:** full length, scaled to max height 720, keeping aspect ratio  

Both versions are saved into your `Videos` folder with prefixes `short_` and `full_`.

---

## Prerequisites

- Windows OS  
- [FFmpeg](https://ffmpeg.org/download.html) installed and accessible via system `PATH`  
- NVIDIA GPU with NVENC support (optional but recommended for hardware acceleration)  
- Go installed (for building the program)  

---

## Build

To build the executable with minimized binary size, run:

```bash
go build -ldflags="-s -w" -o bin/WowVideoConverter.exe

package main

import (
	"fmt"
	"log"
	"os"
	"os/exec"
	"path/filepath"
	"strings"
	"time"
)

func main() {
	userProfile := os.Getenv("USERPROFILE")
	if userProfile == "" {
		log.Fatal("Environment variable USERPROFILE not found")
	}

	videosFolder := filepath.Join(userProfile, "Videos")
	inputFolder := filepath.Join(videosFolder, "NVIDIA", "World Of Warcraft")

	fmt.Println("Input folder:", inputFolder)
	fmt.Println("Output folder:", videosFolder)
	fmt.Println()

	scriptStartedAt := time.Now()
	fmt.Println("Script started at:", scriptStartedAt.Format("15:04:05"))
	fmt.Println()

	err := filepath.Walk(inputFolder, func(path string, fileInfo os.FileInfo, err error) error {
		if err != nil {
			return err
		}
		if fileInfo.IsDir() || strings.ToLower(filepath.Ext(path)) != ".mp4" {
			return nil
		}

		fileName := fileInfo.Name()
		shortVideo := filepath.Join(videosFolder, "short_"+fileName)
		fullVideo := filepath.Join(videosFolder, "full_"+fileName)

		fmt.Println("Processing:", fileName)
		currentVideoProcessStartedAt := time.Now()

		createShortVersion(path, shortVideo)
		createFullVersion(path, fullVideo)

		fmt.Printf("  Time taken: %.0f seconds\n\n", time.Since(currentVideoProcessStartedAt).Seconds())
		return nil
	})
	if err != nil {
		log.Fatal(err)
	}

	fmt.Println("Script finished at:", time.Now().Format("15:04:05"))
}

func createShortVersion(inputPath, outputPath string) {
	if _, err := os.Stat(outputPath); os.IsNotExist(err) {
		fmt.Println("  Creating short version:", outputPath)
		err := runFFmpeg(
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
			outputPath,
		)
		if err != nil {
			log.Println("Error creating short version:", err)
		}
	} else {
		fmt.Println("  Short version already exists, skipping.")
	}
}

func createFullVersion(inputPath, outputPath string) {
	if _, err := os.Stat(outputPath); os.IsNotExist(err) {
		fmt.Println("  Creating full version:", outputPath)
		err := runFFmpeg(
			"-i", inputPath,
			"-an",
			"-vf", "scale=-2:'min(720,ih)'",
			"-c:v", "h264_nvenc",
			"-preset", "slow",
			"-rc", "vbr",
			"-b:v", "3M",
			"-tune", "hq",
			"-multipass", "2",
			"-movflags", "+faststart",
			outputPath,
		)
		if err != nil {
			log.Println("Error creating full version:", err)
		}
	} else {
		fmt.Println("  Full version already exists, skipping.")
	}
}

func runFFmpeg(args ...string) error {
	cmd := exec.Command("ffmpeg", args...)
	cmd.Stdout = os.Stdout
	cmd.Stderr = os.Stderr
	return cmd.Run()
}

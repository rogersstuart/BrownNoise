# BrownNoise

BrownNoise is a C# application designed to generate audio files containing brown noise. It provides both a command-line interface (CLI) and a graphical user interface (GUI) for generating WAV files with customizable parameters such as duration, bitrate, bit depth, and stereo/mono output.

## Features
- Generate brown noise audio files in WAV format.
- Customize audio properties such as duration, bitrate, bit depth, and stereo/mono output.
- GUI for easy interaction and CLI for advanced users.
- Supports 16-bit, 24-bit, and 32-bit floating-point audio formats.
- Adjustable "leakiness" parameter for fine-tuning the noise generation.

## Command-Line Usage
The CLI version of BrownNoise allows you to generate brown noise audio files with the following options:

### Syntax
brownnoise [options]
### Options
| Option                | Alias | Default Value | Description                                      |
|-----------------------|-------|---------------|--------------------------------------------------|
| `--file <FileInfo>`   | `-f`  | `out.wav`     | Specifies the output file path for the WAV file.|
| `--seconds <int>`     | `-s`  | `60`          | Sets the duration of the audio clip in seconds. |
| `--bitrate <int>`     | `-b`  | `44100`       | Sets the audio bitrate (e.g., 44100 Hz).        |
| `--bitdepth <int>`    | `-d`  | `16`          | Sets the audio bit depth (16, 24, or 32).       |
| `--stereo <bool>`     | `-m`  | `false`       | Enables stereo audio if set to `true`.          |
| `--leakiness <int>`   | `-l`  | `60`          | Adjusts the "leakiness" of the integrator.      |

### Example
Generate a 10-second stereo WAV file with 24-bit depth and 48000 Hz bitrate:brownnoise --file output.wav --seconds 10 -b 48000 -d 24 -m true

## Graphical User Interface (GUI)
The GUI version of BrownNoise provides an intuitive interface for generating brown noise audio files. Users can:
- Specify the output file path.
- Adjust the duration, sample rate, bit depth, and channels (mono/stereo).
- Modify the "leakiness" parameter using a slider.

### How to Use
1. Launch the GUI application.
2. Set the desired parameters using the provided input fields and dropdowns.
3. Click "Generate" to create the WAV file.
4. The generated file will open automatically upon completion.

## Requirements
- .NET Core 3.1 for the CLI version.
- .NET 9 for the GUI version.
- Windows operating system.

## Installation
1. Clone the repository:
2. Build the solution using Visual Studio 2022 or later.
3. Run the desired version (CLI or GUI).

## License
This project is licensed under the MIT License.
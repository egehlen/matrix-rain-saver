# Matrix Rain Screensaver

![Matrix Rain Preview](preview.gif)

A Matrix-style falling characters screensaver written in C# using .NET 9+ and WinForms.

## Features

- Full-screen Matrix rain effect with falling characters
- Katakana, Latin letters, and numbers character set
- White head characters with green trailing tail effect
- Smooth 20 FPS animation with double buffering
- Exits on any key press or mouse movement
- Multi-monitor support
- Self-contained executable

## Building

### Prerequisites

- .NET 9 SDK or later
- Windows 10/11 (x64)

### Build Commands

```bash
# Build in Release mode
dotnet build -c Release

# Publish as self-contained executable
dotnet publish -c Release -r win-x64 --self-contained
```

## Installation

1. Build the project using the commands above
2. Navigate to the output directory: `bin\Release\net9.0-windows\win-x64\publish\`
3. Rename the executable from `MatrixRainSaver.exe` to `MatrixRainSaver.scr`
4. Right-click on the `.scr` file and select "Install" to install as a screensaver
5. Configure through Windows Settings > Personalization > Lock screen > Screen saver settings

### Quick Installation Commands

```bash
# Build and prepare screensaver
dotnet publish -c Release -r win-x64 --self-contained
cd bin\Release\net9.0-windows\win-x64\publish\
ren MatrixRainSaver.exe MatrixRainSaver.scr
```

## Usage

### Command Line Arguments

- `/s` - Start screensaver mode (full screen)
- `/p` - Preview mode (shows preview dialog)
- `/c` - Configuration mode (shows configuration dialog)
- No arguments - Runs in windowed mode for testing

### Customization

The following parameters can be modified in `MatrixRainForm.cs`:

#### Animation Speed
```csharp
timer.Interval = 50; // 50ms = 20 FPS (lower = faster)
```

#### Colors
```csharp
greenBrush = new SolidBrush(Color.FromArgb(0, 255, 0)); // Tail color
whiteBrush = new SolidBrush(Color.White); // Head color
```

#### Font
```csharp
matrixFont = new Font("Consolas", 12, FontStyle.Bold); // Font family and size
```

#### Drop Probability
```csharp
if (random.NextDouble() < 0.02) // 2% chance to start new drop
```

#### Drop Length
```csharp
dropLengths[col] = random.Next(5, rows / 2); // Min 5, max half screen height
```

## Technical Details

- Target Framework: .NET 9+ (Windows)
- UI Framework: WinForms
- Graphics: BufferedGraphics for flicker-free rendering
- Character Set: Unicode Katakana (0x30A0-0x30FF), ASCII A-Z, 0-9
- Performance: Optimized for high resolutions using double buffering

## Project Structure

- `MatrixRainSaver.sln` - Solution file
- `MatrixRainSaver.csproj` - Project file with .NET 9+ configuration
- `Program.cs` - Entry point with command line argument handling
- `MatrixRainForm.cs` - Main form with Matrix rain implementation
- `MatrixRainForm.Designer.cs` - Designer file for the form
- `app.manifest` - Application manifest for Windows compatibility
- `README.md` - This documentation file

## License

This project is provided as-is for educational and entertainment purposes.
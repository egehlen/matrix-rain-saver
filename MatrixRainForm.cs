using System;
using System.Drawing;
using System.Windows.Forms;

namespace MatrixRainSaver
{
    public partial class MatrixRainForm : Form
    {
        private readonly System.Windows.Forms.Timer timer;
        private readonly int[] dropYPositions;
        private readonly int[] dropLengths;
        private readonly char[,] droppedChars;
        private readonly Random random;
        private readonly char[] charSet;
        private readonly SolidBrush[] fadeBrushes;
        private readonly SolidBrush whiteBrush;
        private readonly SolidBrush blackBrush;
        private readonly Font matrixFont;
        private readonly int columnWidth;
        private readonly int rowHeight;
        private readonly int columns;
        private readonly int rows;
        private Point lastMousePosition;
        private BufferedGraphicsContext bufferContext;
        private BufferedGraphics bufferGraphics;

        public MatrixRainForm(Screen screen)
        {
            InitializeComponent();
            
            random = new Random();
            
            // Character set: Katakana, Latin, numbers
            var katakana = new char[96];
            for (int i = 0; i < 96; i++)
            {
                katakana[i] = (char)(0x30A0 + i);
            }
            
            var latin = new char[26];
            for (int i = 0; i < 26; i++)
            {
                latin[i] = (char)('A' + i);
            }
            
            var numbers = new char[10];
            for (int i = 0; i < 10; i++)
            {
                numbers[i] = (char)('0' + i);
            }
            
            charSet = new char[katakana.Length + latin.Length + numbers.Length];
            Array.Copy(katakana, 0, charSet, 0, katakana.Length);
            Array.Copy(latin, 0, charSet, katakana.Length, latin.Length);
            Array.Copy(numbers, 0, charSet, katakana.Length + latin.Length, numbers.Length);
            
            // Colors (configurable)
            fadeBrushes = new SolidBrush[20];
            for (int i = 0; i < 20; i++)
            {
                int intensity = 255 - (i * 12);
                if (intensity < 0) intensity = 0;
                fadeBrushes[i] = new SolidBrush(Color.FromArgb(0, intensity, 0));
            }
            whiteBrush = new SolidBrush(Color.White);
            blackBrush = new SolidBrush(Color.Black);
            
            // Font (configurable)
            matrixFont = new Font("Consolas", 12, FontStyle.Bold);
            
            // Calculate dimensions
            using (var g = CreateGraphics())
            {
                var charSize = g.MeasureString("A", matrixFont);
                columnWidth = (int)Math.Ceiling(charSize.Width);
                rowHeight = (int)Math.Ceiling(charSize.Height);
            }
            
            // Setup form
            SetupForm(screen);
            
            columns = Width / columnWidth;
            rows = Height / rowHeight;
            
            // Initialize drop arrays
            dropYPositions = new int[columns];
            dropLengths = new int[columns];
            droppedChars = new char[columns, rows];
            
            // Initialize buffer
            bufferContext = BufferedGraphicsManager.Current;
            bufferGraphics = bufferContext.Allocate(CreateGraphics(), new Rectangle(0, 0, Width, Height));
            
            // Timer for animation (configurable speed)
            timer = new System.Windows.Forms.Timer
            {
                Interval = 50 // 50ms = 20 FPS (adjust for speed)
            };
            timer.Tick += Timer_Tick;
            
            lastMousePosition = Cursor.Position;
            timer.Start();
        }

        private void SetupForm(Screen screen)
        {
            // Full screen, borderless
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Normal;
            TopMost = true;
            BackColor = Color.Black;
            
            // Position on specified screen and make truly full screen
            Location = screen.Bounds.Location;
            Size = screen.Bounds.Size;
            StartPosition = FormStartPosition.Manual;
            
            // Hide cursor
            Cursor.Hide();
            
            // Enable double buffering
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Check for mouse movement
            if (Cursor.Position != lastMousePosition)
            {
                ExitScreensaver();
                return;
            }
            
            // Clear buffer
            bufferGraphics.Graphics.Clear(Color.Black);
            
            // Draw matrix rain
            DrawMatrixRain();
            
            // Render buffer to screen
            bufferGraphics.Render(CreateGraphics());
        }

        private void DrawMatrixRain()
        {
            for (int col = 0; col < columns; col++)
            {
                // 2% chance to start a new drop
                if (dropYPositions[col] == 0 && random.NextDouble() < 0.02)
                {
                    dropYPositions[col] = 1;
                    dropLengths[col] = random.Next(5, rows / 2);
                }
                
                if (dropYPositions[col] > 0)
                {
                    int headY = dropYPositions[col] - 1;
                    
                    // Place new character at head position
                    if (headY < rows && headY >= 0)
                    {
                        char randomChar = charSet[random.Next(charSet.Length)];
                        droppedChars[col, headY] = randomChar;
                        
                        // Draw white head
                        bufferGraphics.Graphics.DrawString(
                            randomChar.ToString(),
                            matrixFont,
                            whiteBrush,
                            col * columnWidth,
                            headY * rowHeight);
                    }
                    
                    // Draw fading tail
                    for (int i = 1; i < dropLengths[col]; i++)
                    {
                        int y = headY - i;
                        if (y >= 0 && y < rows && droppedChars[col, y] != '\0')
                        {
                            int fadeIndex = Math.Min(i - 1, fadeBrushes.Length - 1);
                            bufferGraphics.Graphics.DrawString(
                                droppedChars[col, y].ToString(),
                                matrixFont,
                                fadeBrushes[fadeIndex],
                                col * columnWidth,
                                y * rowHeight);
                        }
                    }
                    
                    // Clear character at tail end
                    int tailY = headY - dropLengths[col];
                    if (tailY >= 0 && tailY < rows)
                    {
                        droppedChars[col, tailY] = '\0';
                    }
                    
                    // Move drop down
                    dropYPositions[col]++;
                    
                    // Reset when drop goes off screen
                    if (dropYPositions[col] > rows + dropLengths[col])
                    {
                        dropYPositions[col] = 0;
                    }
                }
            }
        }

        private void ExitScreensaver()
        {
            timer.Stop();
            Cursor.Show();
            Application.Exit();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            ExitScreensaver();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Math.Abs(e.X - lastMousePosition.X) > 5 || Math.Abs(e.Y - lastMousePosition.Y) > 5)
            {
                ExitScreensaver();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            ExitScreensaver();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timer?.Dispose();
                if (fadeBrushes != null)
                {
                    foreach (var brush in fadeBrushes)
                    {
                        brush?.Dispose();
                    }
                }
                whiteBrush?.Dispose();
                blackBrush?.Dispose();
                matrixFont?.Dispose();
                bufferGraphics?.Dispose();
                bufferContext?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
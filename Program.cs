using System;
using System.Drawing;
using System.Windows.Forms;

namespace MatrixRainSaver
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MatrixRainForm mainForm = null;
            
            if (args.Length > 0)
            {
                string arg = args[0].ToLower();
                
                if (arg == "/c")
                {
                    MessageBox.Show("Matrix Rain Screensaver Configuration", "Configuration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (arg == "/p")
                {
                    MessageBox.Show("Matrix Rain Screensaver Preview", "Preview", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (arg.StartsWith("/s"))
                {
                    mainForm = new MatrixRainForm(Screen.PrimaryScreen);
                    foreach (Screen screen in Screen.AllScreens)
                    {
                        if (screen != Screen.PrimaryScreen)
                        {
                            new MatrixRainForm(screen).Show();
                        }
                    }
                }
            }
            else
            {
                mainForm = new MatrixRainForm(Screen.PrimaryScreen);
            }

            if (mainForm != null)
            {
                Application.Run(mainForm);
            }
        }
    }
}
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DLA
{
    internal class Program
    {
        public static int height = 300;
        public static int width = 300;
        public static int loops = 10_000;

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        public static void Main(string[] args)
        {
            Console.WriteLine("Working...");

            string paintPath = GetPaintPath();
            string path = GetSavePath();
            bool[,] cords = DLA();

            using Bitmap map = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (cords[x, y])
                        map.SetPixel(x, y, Color.Black);
                }
            }
            map.Save(path);

            using Process process = Process.Start(paintPath, path);
            IntPtr console = GetConsoleWindow();
            ShowWindow(console, SW_HIDE);

            process.WaitForExit();
        }

        private static string GetSavePath()
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            return desktop + @"\DLAbitmap.bmp";
        }

        private static string GetPaintPath()
        {
            string[] userFiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\AppData\Local\Microsoft\WindowsApps\", "*.exe", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            });

            foreach(string file in userFiles)
            {
                if (Path.GetFileNameWithoutExtension(file) == "mspaint")
                    return file;
            }

            return "";
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static bool[,] DLA()
        {
            bool[,] cords = new bool[height, width];

            Random random = new Random();
            int xCord = random.Next(width / 4, width / 4 * 3);
            int yCord = random.Next(height / 4, height / 4 * 3);

            int startX = xCord;
            int startY = yCord;

            cords[xCord, yCord] = true;
            for (int i = 0; i < loops; i++)
            {
                do
                {
                    xCord = random.Next(0, width);
                    yCord = random.Next(0, height);
                } while (cords[xCord, yCord] && !ArrayFull(cords));

                if (HasNeighbours(xCord, yCord, cords))
                    cords[xCord, yCord] = true;
                else
                {
                    do
                    {
                        int dir = random.Next(0, 4);
                        switch (dir)
                        {
                            case 0:
                                xCord = Math.Clamp(xCord + 1, 0, width - 1);
                                break;
                            case 1:
                                xCord = Math.Clamp(xCord - 1, 0, width - 1);
                                break;
                            case 2:
                                yCord = Math.Clamp(yCord + 1, 0, height - 1);
                                break;
                            case 3:
                                yCord = Math.Clamp(yCord - 1, 0, height - 1);
                                break;
                        }
                    } while (!HasNeighbours(xCord, yCord, cords));

                    cords[xCord, yCord] = true;
                }
            }

            return cords;
        }

        public static bool HasNeighbours(int x, int y, bool[,] cords)
        {
            return cords[Math.Clamp(x + 1, 0, width - 1), y] ||
                   cords[Math.Clamp(x - 1, 0, width - 1), y] ||
                   cords[x, Math.Clamp(y + 1, 0, height - 1)] ||
                   cords[x, Math.Clamp(y - 1, 0, height - 1)];
        }

        public static bool ArrayFull(bool[,] cords)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!cords[x, y])
                        return false;
                }
            }

            return true;
        }
    }
}

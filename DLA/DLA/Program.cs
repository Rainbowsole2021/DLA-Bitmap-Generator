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
        public static string paintPath = "YOUR_MS_PAINT_PATH";
        public static string path = "YOUR_DLA_IMAGE_PATH";

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        public static void Main(string[] args)
        {
            Console.WriteLine("Working...");

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
                        int xDir = random.Next(-1, 2);
                        int yDir = random.Next(-1, 2);

                        xCord = Math.Clamp(xCord + xDir, 0, width - 1);
                        yCord = Math.Clamp(yCord + yDir, 0, height - 1);
                    } while (!HasNeighbours(xCord, yCord, cords));

                    cords[xCord, yCord] = true;
                }
            }

            using Bitmap map = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (cords[x, y])
                        map.SetPixel(x, y, Color.Black);
                }
            }

            map.SetPixel(startX, startY, Color.Red);
            map.Save(path);

            using Process process = Process.Start(paintPath, path);
            IntPtr console = GetConsoleWindow();
            ShowWindow(console, SW_HIDE);

            process.WaitForExit();
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static bool HasNeighbours(int x, int y, bool[,] cords)
        {
            return cords[Math.Clamp(x + 1, 0, width - 1), y] ||
                   cords[Math.Clamp(x - 1, 0, width - 1), y] ||
                   cords[x, Math.Clamp(y + 1, 0, height - 1)] ||
                   cords[x, Math.Clamp(y - 1, 0, height - 1)] ||
                   cords[Math.Clamp(x + 1, 0, width - 1), Math.Clamp(y + 1, 0, height - 1)] ||
                   cords[Math.Clamp(x - 1, 0, width - 1), Math.Clamp(y - 1, 0, height - 1)] ||
                   cords[Math.Clamp(x + 1, 0, width - 1), Math.Clamp(y - 1, 0, height - 1)] ||
                   cords[Math.Clamp(x - 1, 0, width - 1), Math.Clamp(y + 1, 0, height - 1)];
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ColorClicker
{
    public class veriler
    {
        public double mesafe { get; set; }
        public Point point { get; set; }
    }

    public class Program
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;

        public static int colorinfoR;
        public static int colorinfoG;
        public static int colorinfoB;
        public static int colorinfoA;

        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("F1 - START \nF12 - QUİT ");
                var keyInfo = Console.ReadKey();
                if (keyInfo.Key.ToString() == "F1")
                {
                    Console.WriteLine("İşlem Başlıyor:\nRenk Degerini Giriniz R: ");
                   colorinfoR = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Renk Degerini Giriniz G: ");
                    colorinfoG = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Renk Degerini Giriniz B: ");
                    colorinfoB = Convert.ToInt32(Console.ReadLine());
                    Console.WriteLine("Parlaklık Degerini Giriniz A: ");
                    colorinfoA = Convert.ToInt32(Console.ReadLine());
                    Color clr = Color.FromArgb(colorinfoA, colorinfoR, colorinfoG, colorinfoB); ;
                    start(clr);
                }
                else if (keyInfo.Key.ToString() == "F12")
                {
                    break;
                }
            }
        }
        public static void start(Color color)
        {
            int screenWidth = Screen.PrimaryScreen.Bounds.Width / 2;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height / 2;
            Point centerscr = new Point(screenWidth, screenHeight);
            Console.WriteLine("Center of Screen: " + centerscr.X + " - " + centerscr.Y);
            Cursor.Position = centerscr;

            List<veriler> verisınıfı = new List<veriler>();
            Point[] points = FindColor(color);
            foreach (var x in points)
            {
                var distance = Math.Sqrt((Math.Pow(x.X - centerscr.X, 2) + Math.Pow(x.Y - centerscr.Y, 2)));
                verisınıfı.Add(new veriler { mesafe = distance, point = x });
                Console.WriteLine("Metin " + x.X + " - " + x.Y + " Mesafesi: " + distance);
            }




            veriler lowestRates = null;
            veriler z = null;

            if (verisınıfı.Count == 0)
            {             
                lowestRates = new veriler{mesafe = 0, point = new Point(0,0)};
            }
            else if(verisınıfı.Count == 1)
            {
                lowestRates = verisınıfı.FirstOrDefault();
            }
            else
            {
                foreach (var x in verisınıfı)
                {
                    if (z != null)
                    {
                        if (x.mesafe < z.mesafe)
                            lowestRates = x;
                    }
                    else
                        lowestRates = x;
                    z = x;
                }
            }

            Console.WriteLine("Gidilen Metin: " + lowestRates.point.X.ToString() +
                    " - " + lowestRates.point.Y.ToString() +
                    "  " + lowestRates.mesafe.ToString());
            Cursor.Position = lowestRates.point;
            mouse_event(MOUSEEVENTF_LEFTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        static Bitmap GetScreenShot()
        {
            Bitmap result = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
            {
                using (Graphics gfx = Graphics.FromImage(result))
                {
                    gfx.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
                }
            }
            return result;
        }

        static Point[] FindColor(Color color)
        {
            int searchValue = color.ToArgb();
            List<Point> result = new List<Point>();
            using (Bitmap bmp = GetScreenShot())
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        if (bmp.GetPixel(x, y) == color)
                        {
                            result.Add(new Point(x, y));
                        }
                        else
                        {
                            if (ColorAreCloseByPrimaryColor(bmp.GetPixel(x, y),color) == true)
                            {
                                result.Add(new Point(x, y));
                            }
                            else if(ColorsAreClose(bmp.GetPixel(x, y), color) == true)
                            {
                                result.Add(new Point(x, y));
                            }
                        }
                    }
                }
            }
            return result.ToArray();
        }

        public static bool ColorAreCloseByPrimaryColor(Color a, Color z)
        {
            char primarycolor = 'N';
            if (z.R >= z.B)
            {
                primarycolor = 'R';
                if (z.G > z.R)
                    primarycolor = 'G';
            }
            else
            {
                primarycolor = 'B';
                if (z.G > z.B)
                    primarycolor = 'G';
            }

            if(primarycolor != 'N')
            {
                if(primarycolor == 'R')
                {
                    int sayac = 0;
                    if (ColorTest(a.R, z.R, 70) == true)
                        sayac++;
                    if (ColorTest(a.G, z.G, 10) == true)
                        sayac++;
                    if (ColorTest(a.B, z.B, 10) == true)
                        sayac++;
                    if (a.A <= z.A + 30 && a.A >= z.A - 30)
                        sayac++;
                    if (sayac >= 3)
                    {
                        return true;
                    }
                    return false;
                }
                else if(primarycolor == 'B')
                {
                    int sayac = 0;
                    if (ColorTest(a.R, z.R, 10) == true)
                        sayac++;
                    if (ColorTest(a.G, z.G, 10) == true)
                        sayac++;
                    if (ColorTest(a.B, z.B, 70) == true)
                        sayac++;
                    if (a.A <= z.A + 30 && a.A >= z.A - 30)
                        sayac++;
                    if (sayac >= 3)
                    {
                        return true;
                    }
                    return false;
                }
                else
                {
                    int sayac = 0;
                    if (ColorTest(a.R, z.R, 10) == true)
                        sayac++;
                    if (ColorTest(a.G, z.G, 70) == true)
                        sayac++;
                    if (ColorTest(a.B, z.B, 10) == true)
                        sayac++;
                    if (a.A <= z.A + 30 && a.A >= z.A - 30)
                        sayac++;
                    if (sayac >= 3)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }


        public static bool ColorTest(int a, int z, int deger)
        {
            if (a <= z + deger && a >= z - deger)
            {
                return true;
            }
            return false;
        }

        public static bool ColorsAreClose(Color a, Color z, int deger = 10)
        {
            int sayac = 0;
            if (ColorTest(a.R,z.R,deger) == true)
                sayac++;
            if (ColorTest(a.G, z.G, deger) == true)
                sayac++;
            if (ColorTest(a.B, z.B, deger) == true)
                sayac++;
            if (a.A <= z.A + 30 && a.A >= z.A - 30)
                sayac++;
            if(sayac == 4)
            {
                return true;
            }
            return false;
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.MachineLearning;
using OpenCvSharp.Blob;
using OpenCvSharp.UserInterface;
using OpenCvSharp.CPlusPlus;
using System.Diagnostics;

namespace YüzTanıma
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap resim = (Bitmap)Bitmap.FromFile(@"C:\Users\ABRA\Desktop\Masaüstü\Kendim.jpg");

            IplImage resimipl = BitmapConverter.ToIplImage(resim);

            pictureBox1.Image = BitmapConverter.ToBitmap(resimipl);
        }
        private IplImage yüzalgila(IplImage resim, string tespitsuresi)  
        {
            CvColor surat = new CvColor(0, 0, 255); //mavi

            const double olcek = 3.0;   //hizli islem icin resmin kac kat kucultulecegi
            const double ScaleFactor = 2.5;
            const int MinNeighbors = 1;

            using (IplImage kucukresim = new IplImage(new CvSize(Cv.Round(resim.Width / olcek), Cv.Round(resim.Height / olcek)), BitDepth.U8, 1))
            {
                using (IplImage gri = new IplImage(resim.Size, BitDepth.U8, 1))   //renksizlestir


                {
                    Cv.CvtColor(resim, gri, ColorConversion.BgrToGray);
                    Cv.Resize(gri, kucukresim, Interpolation.Linear);
                    Cv.EqualizeHist(kucukresim, kucukresim);
                }

                IplImage outImage = Cv.Clone(resim);

                using (CvHaarClassifierCascade cascade = CvHaarClassifierCascade.FromFile(Environment.CurrentDirectory + "\\haarcascades\\haarcascade_frontalface_alt2.xml"))


                using (CvMemStorage storage = new CvMemStorage())
                {
                    storage.Clear();

                    Stopwatch watch = Stopwatch.StartNew();
                    CvSeq<CvAvgComp> faces = Cv.HaarDetectObjects(kucukresim, cascade, storage, ScaleFactor, MinNeighbors, 0, new CvSize(30, 30));
                    watch.Stop();
                    Console.WriteLine("tespit suresi = {0} milisaniye", watch.ElapsedMilliseconds);
                    tespitsuresi = (watch.ElapsedMilliseconds).ToString();

                    for (int i = 0; i < faces.Total; i++)


                    {


                        CvRect r = faces[i].Value.Rect;


                        CvPoint center = new CvPoint


                        {


                            X = Cv.Round((r.X + r.Width * 0.5) * olcek),


                            Y = Cv.Round((r.Y + r.Height * 0.5) * olcek)


                        };
                        int radius = Cv.Round((r.Width + r.Height) * 0.25 * olcek);


                        resim.Circle(center, radius, surat, 3, LineType.AntiAlias, 0);
                    }

                }

                return resim;
            }
        }
    }
}

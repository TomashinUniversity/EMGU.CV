using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu;
using Emgu.Util;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;

namespace ProjectEmguCv
{
    public partial class Form1 : Form
    {
        private VideoCapture videoCapture = null;
        private double frames;
        private double currentFrame;
        private double fps;
        private bool play = false;

        private static CascadeClassifier classifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");

        public Form1()
        {
            InitializeComponent();
        }

        private Image<Bgr, byte> Find(Image<Bgr, byte> image)
        {
            MCvObjectDetection[] regions;

            using (HOGDescriptor descriptor = new HOGDescriptor())
            {
                descriptor.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());
                regions = descriptor.DetectMultiScale(image);
            }

            foreach (MCvObjectDetection region in regions)
            {
                image.Draw(region.Rect, new Bgr(Color.Red), 3);
                CvInvoke.PutText(image, "Пешеход", new Point(region.Rect.X, region.Rect.Y), Emgu.CV.CvEnum.FontFace.HersheyPlain, 1, new MCvScalar(255, 255, 255), 2);
            }
            return image;
        }

        private Image<Bgr, byte> Detect(Image<Bgr, byte> image)
        {
            Rectangle[] faces = classifier.DetectMultiScale(image, 1.1, 0);
            if (faces.Length > 1)
            {
                for (int i = 0; i < faces.Length; i++)
                {
                    using (Graphics graphics = Graphics.FromImage(image.Bitmap))
                    {
                        int nextIndex = i + 1;
                        if (nextIndex >= faces.Length) nextIndex = 0;

                        Image newFace = CropImage(image.Bitmap, faces[nextIndex]);

                        graphics.DrawImage(newFace, faces[i]);
                    }
                }
            }
            /*
            foreach (Rectangle face in faces)
            {
                using (Graphics graphics = Graphics.FromImage(image.Bitmap))
                {
                    using (Pen pen = new Pen(Color.Yellow, 3))
                    {
                        graphics.DrawRectangle(pen, face);
                    }
                }
            }
            */
            return image;
        }
        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        private async void ReadFrames()
        {
            Mat m = new Mat();
            while(play && currentFrame < frames)
            {
                currentFrame++;
                videoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, currentFrame);
                videoCapture.Read(m);
                pictureBox1.Image = m.Bitmap;
                pictureBox2.Image = Detect(m.ToImage<Bgr, byte>()).Bitmap;

                await Task.Delay(100 / Convert.ToInt16(fps));
            }
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = openFileDialog1.ShowDialog();

                if (dialogResult == DialogResult.OK)
                {
                    videoCapture = new VideoCapture(openFileDialog1.FileName);
                    Mat material = new Mat();
                    videoCapture.Read(material);
                    pictureBox1.Image = material.Bitmap;

                    fps = videoCapture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps);
                    frames = videoCapture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount);
                    currentFrame = 1;

                }
                else
                {
                    ShowMessage("Видео не выбрано!");
                }
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }

        private void распознатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (videoCapture == null)
                    throw new Exception("Видео не выбрано");

                play = true;
                ReadFrames();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message);
            }
        }
        
        private void ShowMessage(string error)
        {
            MessageBox.Show(error, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

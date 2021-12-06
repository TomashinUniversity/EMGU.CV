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
using DirectShowLib;

namespace Laba2_FaceRecFromCamera
{
    public partial class Form1 : Form
    {
        private static CascadeClassifier classifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");
        private VideoCapture videoCapture = null;
        private DsDevice webCamera = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            webCamera = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice)[0];
            ShowVideo();
        }

        private void ShowVideo()
        {
            try
            {
                if (videoCapture != null)
                {
                    videoCapture.Start();
                }
                else
                {
                    videoCapture = new VideoCapture(0);
                    videoCapture.ImageGrabbed += VideoCapture_ImageGrabbed;
                    videoCapture.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void VideoCapture_ImageGrabbed(object sender, EventArgs e)
        {
            try
            {
                Mat material = new Mat();
                videoCapture.Retrieve(material);
                Image videoImage = material.ToImage<Bgr, byte>().Flip(Emgu.CV.CvEnum.FlipType.Horizontal).Bitmap;
                pictureBox1.Image = GetBitMapImage(videoImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Image GetBitMapImage(Image image)
        {
            Bitmap bitmap = new Bitmap(image);
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
            Rectangle[] faces = classifier.DetectMultiScale(grayImage, 1.4, 0);
            foreach (Rectangle face in faces)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Yellow, 3))
                    {
                        graphics.DrawRectangle(pen, face);
                    }
                }
            }
            return bitmap;
        }
    }
}

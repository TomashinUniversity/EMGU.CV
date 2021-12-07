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
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace Laba4_GeometryRecognition
{
    public partial class Form1 : Form
    {
        private Image<Bgr, byte> inputImage = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();

            if (dialogResult == DialogResult.OK)
            {
                inputImage = new Image<Bgr, byte>(openFileDialog1.FileName);
                pictureBox1.Image = inputImage.Bitmap;
            }
            else
            {
                MessageBox.Show("Изображение не выбрано!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void найтиФигурыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image<Gray, byte> grayImage = inputImage.SmoothGaussian(5).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(230), new Gray(255));
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(grayImage, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            for (int i = 0; i < contours.Size; i++)
            {
                double perimeter = CvInvoke.ArcLength(contours[i], true);
                VectorOfPoint approximation = new VectorOfPoint();
                CvInvoke.ApproxPolyDP(contours[i], approximation, 0.04 * perimeter, true);
                CvInvoke.DrawContours(inputImage, contours, i, new MCvScalar(0, 0, 255), 2);
                pictureBox2.Image = inputImage.Bitmap;
            }
        }
    }
}

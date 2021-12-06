using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Laba1_EMGUCV_FaceRecognitionFromImage
{
    public partial class Form1 : Form
    {
        private static CascadeClassifier classifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult res = openFileDialog1.ShowDialog();
                if (res == DialogResult.OK)
                {
                    string path = openFileDialog1.FileName;
                    pictureBox1.Image = Image.FromFile(path);
                    Bitmap bitmap = new Bitmap(pictureBox1.Image);
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
                    pictureBox1.Image = bitmap;
                }
                else
                {
                    MessageBox.Show("Изображение не выбрано!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

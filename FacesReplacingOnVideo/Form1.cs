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

namespace FacesReplacingOnVideo
{
    public partial class FacesReplacingOnVideo : Form
    {
        private static CascadeClassifier classifier = new CascadeClassifier("haarcascade_frontalface_alt_tree.xml");

        private VideoCapture videoCapture = null; // Объект, содержащий в себе видео.
        private double totalFrames = 0; // Общее количество кадров видео
        private double currentFrame = 0; // Номер текущего кадра
        private double fps = 0; // Текущий FPS видео
        private bool play = false; // Состояние видео - воспроизведение/пауза

        public FacesReplacingOnVideo()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = openFileDialog1.ShowDialog(); // Открытие проводника

                if (dialogResult == DialogResult.OK) // Если файл выбран
                {
                    videoCapture = new VideoCapture(openFileDialog1.FileName); // Установка видео
                    Mat material = new Mat(); // Материал кадра
                    videoCapture.Read(material); // Чтение первого кадра из видео в материал
                    pictureBox1.Image = material.Bitmap; // Извлечение картинки из материала и установка в pictureBox

                    totalFrames = videoCapture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameCount); // Количество кадров
                    currentFrame = 1; // Установка первого кадра
                    fps = videoCapture.GetCaptureProperty(Emgu.CV.CvEnum.CapProp.Fps); // FPS

                }
                else // Если не выбрали файл
                {
                    ShowErrorMessage("Видео не выбрано!");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void запуститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (videoCapture == null)
                    throw new Exception("Видео не выбрано");

                play = true;
                PlayVideo(true);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private async void PlayVideo(bool withReplacing)
        {
            Mat m = new Mat();
            while(play && currentFrame < totalFrames)
                // Цикл будет идти пока активен показ и текущий кадр меньше общего количества
            {
                currentFrame++; // Включение следующего кадра
                videoCapture.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.PosFrames, currentFrame);
                videoCapture.Read(m);
                // Получение кадра по индексу из видео и извлечение из него картинки
                pictureBox1.Image = m.Bitmap;
                pictureBox2.Image = withReplacing ? ReplaceFaces(m.ToImage<Bgr, byte>()).Bitmap : m.Bitmap;
                // Назначение оргинального кадра в первый pictureBox и изменение кадра во втором

                await Task.Delay(100 / Convert.ToInt16(fps)); // Задержка в переключении кадров
            }
        }

        private Image<Bgr, byte> ReplaceFaces(Image<Bgr, byte> image)
        {
            Rectangle[] faces = classifier.DetectMultiScale(image, 1.1, 0);
            // Получение массива прямоугольников, содержащих лица
            if (faces.Length > 1) // Если лиц больше 1, то делаем перестановку
            {
                for (int i = 0; i < faces.Length; i++)
                {
                    // Проход по всем лицам
                    using (Graphics graphics = Graphics.FromImage(image.Bitmap))
                    {
                        int nextIndex = i + 1;
                        if (nextIndex >= faces.Length) nextIndex = 0;
                        // Получение индекса следующего лица

                        Image newFace = CropImage(image.Bitmap, faces[nextIndex]);
                        // Получение картинки следующего лица по индексу

                        graphics.DrawImage(newFace, faces[i]);
                        // Замена старого лица новым поверх
                    }
                }
            }
            return image;
        }

        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // Вырезание картинки по прямоугольнику с оригинала
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        private void ShowErrorMessage(string error)
        {
            MessageBox.Show(error, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

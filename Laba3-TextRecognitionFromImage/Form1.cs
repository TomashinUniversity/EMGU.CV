﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.Util;

namespace Laba3_TextRecognitionFromImage
{
    public partial class Form1 : Form
    {
        private string filePath = string.Empty;
        private string lang = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(filePath);
            }
            else
            {
                MessageBox.Show("Картинка не выбрана!", "Необходимо выбрать картинку!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(filePath) || String.IsNullOrWhiteSpace(filePath))
                {
                    throw new Exception("Картинка не выбрана!");
                }
                else if (toolStripComboBox1.SelectedItem == null)
                {
                    throw new Exception("Язык не выбран!");
                }
                else
                {
                    Tesseract tesseract = new Tesseract(@"D:\Projects\University\EMGU.CV\Materials\TessData", lang, OcrEngineMode.TesseractLstmCombined);
                    tesseract.SetImage(new Image<Bgr, byte>(filePath));
                    tesseract.Recognize();
                    richTextBox1.Text = tesseract.GetUTF8Text();
                    tesseract.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (toolStripComboBox1.SelectedIndex == 0)
                lang = "rus";
            else if (toolStripComboBox1.SelectedIndex == 1)
                lang = "eng";
        }
    }
}

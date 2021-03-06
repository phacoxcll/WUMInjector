﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WUMInjector.Properties;

namespace WUMInjector
{
    public partial class WUMInjectorGUI : Form
    {
        private MediaInjector Injector;
        private string ResourcesPath;
        private BootImage BootTvImg;
        private BootImage BootDrcImg;
        private MenuIconImage MenuIconImg;

        public WUMInjectorGUI()
        {
            Cll.Log.SaveIn("WUMInjector.log");
            Cll.Log.WriteLine("WUM Injector " + MediaInjector.Release);
            Cll.Log.WriteLine(DateTime.Now.ToString());

            InitializeComponent();

            BootTvImg = new BootImage();
            BootDrcImg = new BootImage();
            MenuIconImg = new MenuIconImage();

            this.Text = "WUM Injector " + MediaInjector.Release;

            ResourcesPath = Path.Combine(MediaInjector.DataPath, "resources");
            string imagesPath = Path.Combine(ResourcesPath, "images");
            string NUSConverterDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NUSConverter");
            string packPath = Path.Combine(NUSConverterDataPath, "pack");
            string unpackPath = Path.Combine(NUSConverterDataPath, "unpack");
            string cnuspackerPath = Path.Combine(packPath, "CNUSPacker.exe");
            string cdecryptPath = Path.Combine(unpackPath, "CDecrypt.exe");

            if (!Directory.Exists(NUSConverterDataPath))
            {
                Directory.CreateDirectory(NUSConverterDataPath);
                Directory.CreateDirectory(packPath);
                Directory.CreateDirectory(unpackPath);
                FileStream fs = File.Create(cnuspackerPath);
                fs.Write(Resources.CNUSPacker, 0, Resources.CNUSPacker.Length);
                fs.Close();
                fs = File.Create(cdecryptPath);
                fs.Write(Resources.CDecrypt, 0, Resources.CDecrypt.Length);
                fs.Close();
                fs = File.Create(Path.Combine(unpackPath, "libeay32.dll"));
                fs.Write(Resources.libeay32, 0, Resources.libeay32.Length);
                fs.Close();
            }

            if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WUMMInjector")))
                Directory.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WUMMInjector"), true);

            if (!Directory.Exists(MediaInjector.DataPath))
                Directory.CreateDirectory(MediaInjector.DataPath);

            if (!Directory.Exists(ResourcesPath))
            {
                Directory.CreateDirectory(ResourcesPath);
                StreamWriter sw = File.CreateText(Path.Combine(ResourcesPath, "version"));
                sw.Write(MediaInjector.Release);
                sw.Close();
            }

            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
                Resources.icon_multimedia.Save(Path.Combine(imagesPath, "icon.png"), ImageFormat.Png);
            }

            StringBuilder sb = new StringBuilder();
            bool warning = false;
            if (!File.Exists(cnuspackerPath))
            {
                sb.AppendLine("Warning! \"" + cnuspackerPath + "\" not found! NUSPacker allows you to encrypt NUS Content for WUP Installer.");
                sb.AppendLine("");
                warning = true;
            }
            if (!File.Exists(cdecryptPath))
            {
                sb.AppendLine("Warning! \"" + cdecryptPath + "\" not found! CDecrypt allows you to decrypt NUS Content for Loadiine.");
                warning = true;
            }
            if (warning)
            {
                MessageBox.Show(sb.ToString(), "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (NUSContent.CheckCommonKeyFiles())
            {
                textBoxCommonKey.Enabled = false;
                panelCommonKey.BackgroundImage = Properties.Resources.checkmark_16;
            }
            else
            {
                textBoxCommonKey.Enabled = true;
                panelCommonKey.BackgroundImage = Properties.Resources.x_mark_16;
            }

            Injector = new MediaInjector();
            if (Injector.BaseIsLoaded)
            {
                panelLoadedBase.BackgroundImage = Properties.Resources.checkmark_16;
                labelBase.Text = Injector.LoadedBase;
                buttonLoadBase.Enabled = false;
            }
            else
            {
                panelLoadedBase.BackgroundImage = Properties.Resources.x_mark_16;
                labelBase.Text = "Base invalid!";
            }

            if (File.Exists(Path.Combine(imagesPath, "icon.png")))
                MenuIconImg.Frame = new Bitmap(Path.Combine(imagesPath, "icon.png"));
            else
                MenuIconImg.Frame = null;

            UpdateMenuIconPictureBox();
            UpdateBootTvPictureBox();
            UpdateBootDrcPictureBox();
        }

        private void LoadLogFile()
        {
            try
            {
                richTextBoxLog.Clear();
                StreamReader sr = File.OpenText(Cll.Log.Filename);
                richTextBoxLog.AppendText(sr.ReadToEnd());
                sr.Close();
            }
            catch
            {
                Cll.Log.WriteLine("Error reading log file.");
            }
        }

        private void WUMInjectorGUI_Load(object sender, EventArgs e)
        {
            if (!Converter.CheckFFmpeg())
            {
                if (MessageBox.Show(
                    "Do you want to allow WUM Injector to unzip FFmpeg on your computer?\n\n" +
                    "This will take a few moments depending on your system.",
                    "FFmpeg not found!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    DownloadFFmpeg();
                }
                else
                    this.Close();
            }
        }

        private async void DownloadFFmpeg()
        {
            await DownloadFFmpegAsync();
        }

        private async Task DownloadFFmpegAsync()
        {
            FFmpegWait ffmpegWait = new FFmpegWait();
            ffmpegWait.Show();
            await Converter.DownloadFFmpeg();
            ffmpegWait.Close();
        }

        private void UpdateMenuIconPictureBox()
        {
            if (pictureBoxMenuIcon.Image != null)
                pictureBoxMenuIcon.Image.Dispose();

            pictureBoxMenuIcon.Image = MenuIconImg.Create();
        }

        private void UpdateBootTvPictureBox()
        {
            if (pictureBoxBootTv.Image != null)
                pictureBoxBootTv.Image.Dispose();

            pictureBoxBootTv.Image = BootTvImg.Create();

        }

        private void UpdateBootDrcPictureBox()
        {
            if (pictureBoxBootDrc.Image != null)
                pictureBoxBootDrc.Image.Dispose();

            pictureBoxBootDrc.Image = BootDrcImg.Create();
        }

        private void buttonChoose_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = "";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                Injector.SetMultimedia(folderBrowserDialog.SelectedPath);
                textBoxShortName.Text = Injector.MultimediaTitle;
                labelSize.Text = "Size: " + Useful.GetFileSizeWithUnit(Injector.MultimediaSize);
                labelId.Text = "Title ID: " + Injector.TitleId;
            }
        }

        private void buttonInjectNotPack_Click(object sender, EventArgs e)
        {
            if (textBoxShortName.Text.Length > 0 &&
                Directory.Exists(Injector.MultimediaPath))
                Inject(false);
        }

        private void buttonInjectPack_Click(object sender, EventArgs e)
        {
            if (textBoxShortName.Text.Length > 0 &&
                Directory.Exists(Injector.MultimediaPath))
                Inject(true);
        }

        private void Inject(bool encrypt)
        {
            folderBrowserDialog.SelectedPath = "";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap menuIconImg = MenuIconImg.Create();
                    Bitmap bootTvImg = BootTvImg.Create();
                    Bitmap bootDrcImg = BootDrcImg.Create();

                    Injector.Inject(encrypt, folderBrowserDialog.SelectedPath,
                        textBoxShortName.Text, textBoxShortName.Text,
                        menuIconImg, bootTvImg, bootDrcImg);

                    menuIconImg.Dispose();
                    bootTvImg.Dispose();
                    bootDrcImg.Dispose();

                    Cll.Log.WriteLine("Injection success!");
                    LoadLogFile();
                    MessageBox.Show("Successfully", "WUM Injector", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Cll.Log.WriteLine(ex.ToString());
                    Cll.Log.WriteLine("Injection failed.");
                    LoadLogFile();
                    MessageBox.Show("Failed", "WUM Injector", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void textBoxCommonKey_TextChanged(object sender, EventArgs e)
        {
            if (NUSContent.LoadKey(textBoxCommonKey.Text))
            {
                textBoxCommonKey.Text = "";
                textBoxCommonKey.BackColor = Color.FromArgb(33, 33, 33);
                textBoxCommonKey.Enabled = false;
                panelCommonKey.BackgroundImage = Properties.Resources.checkmark_16;
            }
            else
            {
                textBoxCommonKey.BackColor = Color.FromArgb(51, 51, 51);
                textBoxCommonKey.Enabled = true;
                panelCommonKey.BackgroundImage = Properties.Resources.x_mark_16;
            }
        }

        private void buttonLoadBase_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = "";
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Injector.LoadBase(folderBrowserDialog.SelectedPath);
                    labelBase.Text = Injector.LoadedBase;
                    panelLoadedBase.BackgroundImage = Properties.Resources.checkmark_16;
                    buttonLoadBase.Enabled = false;
                }
                catch (Exception ex)
                {
                    labelBase.Text = "Base invalid!";
                    richTextBoxLog.AppendText(ex.ToString());
                    panelLoadedBase.BackgroundImage = Properties.Resources.x_mark_16;
                }
            }
        }

        private void buttonIconBackground_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.FilterIndex = 0;
            openFileDialog.Filter = "Image file|*.png;*.jpg;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MenuIconImg.Background = new Bitmap(openFileDialog.FileName);
                UpdateMenuIconPictureBox();
            }
        }

        private void buttonIconPreview_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.FilterIndex = 0;
            openFileDialog.Filter = "Image file|*.png;*.jpg;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MenuIconImg.Preview = new Bitmap(openFileDialog.FileName);
                UpdateMenuIconPictureBox();
            }
        }

        private void buttonIconFrame_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.FilterIndex = 0;
            openFileDialog.Filter = "Image file|*.png;*.jpg;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MenuIconImg.Frame = new Bitmap(openFileDialog.FileName);
                UpdateMenuIconPictureBox();
            }
        }

        private void buttonTvBackground_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.FilterIndex = 0;
            openFileDialog.Filter = "Image file|*.png;*.jpg;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                BootTvImg.Background = new Bitmap(openFileDialog.FileName);
                UpdateBootTvPictureBox();
            }
        }

        private void buttonTvPreview_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.FilterIndex = 0;
            openFileDialog.Filter = "Image file|*.png;*.jpg;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                BootTvImg.Preview = new Bitmap(openFileDialog.FileName);
                UpdateBootTvPictureBox();
            }
        }

        private void buttonDrcBackground_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.FilterIndex = 0;
            openFileDialog.Filter = "Image file|*.png;*.jpg;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                BootDrcImg.Background = new Bitmap(openFileDialog.FileName);
                UpdateBootDrcPictureBox();
            }
        }

        private void buttonDrcPreview_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = "";
            openFileDialog.FilterIndex = 0;
            openFileDialog.Filter = "Image file|*.png;*.jpg;*.bmp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                BootDrcImg.Preview = new Bitmap(openFileDialog.FileName);
                UpdateBootDrcPictureBox();
            }
        }
    }
}


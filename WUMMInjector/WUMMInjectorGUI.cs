using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WUMMInjector.Properties;

namespace WUMMInjector
{
    public partial class WUMMInjectorGUI : Form
    {
        private MultimediaInjector Injector;

        private BootImage BootTvImg;
        private BootImage BootDrcImg;
        private MenuIconImage MenuIconImg;

        public WUMMInjectorGUI()
        {
            Cll.Log.SaveIn("WUMMInjector.log");
            Cll.Log.WriteLine("WUMM Injector " + MultimediaInjector.Release);
            Cll.Log.WriteLine(DateTime.Now.ToString());

            InitializeComponent();

            BootTvImg = new BootImage();
            BootDrcImg = new BootImage();
            MenuIconImg = new MenuIconImage();

            this.Text = "WUMM Injector " + MultimediaInjector.Release;

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

            Injector = new MultimediaInjector();
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

        private void WUMMInjectorGUI_Load(object sender, EventArgs e)
        {
            if (!WUMMConverter.CheckFFmpeg())
            {
                if (MessageBox.Show("FFmpeg not found!\n\n" +
                    "Do you want to allow WUMM Injector to download FFmpeg from https://ffmpeg.zeranoe.com/builds/ ?\n\n" +
                    "This will take a few moments depending on your Internet connection.\n\n" +
                    "Please wait.", "Warning!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    WUMMConverter.DownloadFFmpeg();
                else
                    this.Close();
            }
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
                    MessageBox.Show("Successfully", "WUMM Injector", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    Cll.Log.WriteLine(ex.ToString());
                    Cll.Log.WriteLine("Injection failed.");
                    LoadLogFile();
                    MessageBox.Show("Failed", "WUMM Injector", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

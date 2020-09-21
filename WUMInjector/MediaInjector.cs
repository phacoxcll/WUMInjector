using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Xml;
using WUMInjector.Properties;

namespace WUMInjector
{
    public class MediaInjector
    {
        public const string Release = "debug 3 rev 1"; //CllVersionReplace "stability major rev revision"           

        public string MultimediaPath { private set; get; }
        public static string DataPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "WUMInjector");
            }
        }
        public string BasePath { private set; get; }
        protected WiiUNWF Base;

        public long MultimediaSize { private set; get; }
        public string MultimediaTitle { private set; get; }

        public bool BaseIsLoaded
        {
            get { return Base != null; }
        }
        public string LoadedBase
        {
            get
            {
                if (Base != null)
                    return Base.ToString();
                else
                    return "";
            }
        }
        private uint Id;

        public string TitleId
        {
            get
            {
                if (BaseIsLoaded)
                    return "000500024D" + (Id >> 8).ToString("X6");
                else
                    return "";
            }
        }

        public MediaInjector()
        {
            MultimediaPath = null;
            BasePath = Path.Combine(DataPath, "base");
            Base = GetLoadedBase();
            Id = 0;
        }

        public void SetMultimedia(string multimediaPath)
        {
            if (Directory.Exists(multimediaPath))
            {
                byte[] namesStruct = Encoding.UTF8.GetBytes(Useful.GetFolderNamesStruct(multimediaPath));
                Id = Cll.Security.ComputeCRC32(namesStruct, 0, namesStruct.Length);
                MultimediaPath = multimediaPath;
                MultimediaSize = Useful.GetFolderSize(multimediaPath);
                MultimediaTitle = Path.GetFileName(multimediaPath);
            }
            else
            {
                MultimediaPath = null;
                MultimediaSize = 0;
                MultimediaTitle = "";
            }
        }

        protected WiiUNWF GetLoadedBase()
        {
            return GetBase(BasePath);
        }

        public WiiUNWF GetBase(string path)
        {
            try
            {
                ValidateBase(path);
                return new WiiUNWF();
            }
            catch
            {
                return null;
            }
        }

        public void Inject(bool encrypt, string outputPath, string shortName, string longName,
            Bitmap menuIconImg, Bitmap bootTvImg, Bitmap bootDrcImg)
        {
            string outPath = encrypt ?
               Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "decrypted") :
               GetValidOutputPath(outputPath, shortName);

            if (!encrypt && Directory.Exists(outPath) &&
               (Directory.GetDirectories(outPath).Length != 0 || Directory.GetFiles(outPath).Length != 0))
                throw new Exception("The output path \"" + outPath + "\"exists and is not empty.");

            if (encrypt && Directory.Exists(outPath))
                Directory.Delete(outPath, true);

            CopyBase(outPath);
            BasePath = outPath;
            Base = GetLoadedBase();
            if (!BaseIsLoaded)
                throw new Exception("The base is not ready.");
            InjectImages(menuIconImg, bootTvImg, bootDrcImg);
            InjectMeta(shortName, longName);
            BasePath = Path.Combine(DataPath, "base");

            InjectMultimedia(Path.Combine(outPath, "content", "app"));

            if (encrypt)
            {
                if (Directory.Exists(GetValidOutputPath(outputPath, shortName)))
                    Directory.Delete(GetValidOutputPath(outputPath, shortName), true);
                Directory.CreateDirectory(GetValidOutputPath(outputPath, shortName));
                NUSContent.Encrypt(outPath, GetValidOutputPath(outputPath, shortName));
            }
        }

        protected void CopyBase(string outPath)
        {
            Useful.DirectoryCopy(BasePath, outPath, true);

            if (Directory.Exists(Path.Combine(outPath, "content", "app", ".tern-defs")))
                Directory.Delete(Path.Combine(outPath, "content", "app", ".tern-defs"), true);
            if (Directory.Exists(Path.Combine(outPath, "content", "app", "plugins")))
                Directory.Delete(Path.Combine(outPath, "content", "app", "plugins"), true);
            if (Directory.Exists(Path.Combine(outPath, "content", "app", "www")))
                Directory.Delete(Path.Combine(outPath, "content", "app", "www"), true);

            if (File.Exists(Path.Combine(outPath, "content", "app", ".gitignore")))
                File.Delete(Path.Combine(outPath, "content", "app", ".gitignore"));
            if (File.Exists(Path.Combine(outPath, "content", "app", ".jscodehints")))
                File.Delete(Path.Combine(outPath, "content", "app", ".jscodehints"));
            if (File.Exists(Path.Combine(outPath, "content", "app", "appinfo.xml")))
                File.Delete(Path.Combine(outPath, "content", "app", "appinfo.xml"));
            if (File.Exists(Path.Combine(outPath, "content", "app", "intelxdk.config.additions.xml")))
                File.Delete(Path.Combine(outPath, "content", "app", "intelxdk.config.additions.xml"));
            if (File.Exists(Path.Combine(outPath, "content", "app", "intelxdk.config.android.xml")))
                File.Delete(Path.Combine(outPath, "content", "app", "intelxdk.config.android.xml"));
            if (File.Exists(Path.Combine(outPath, "content", "app", "intelxdk.config.chrome.xml")))
                File.Delete(Path.Combine(outPath, "content", "app", "intelxdk.config.chrome.xml"));
            if (File.Exists(Path.Combine(outPath, "content", "app", "intelxdk.config.ios.xml")))
                File.Delete(Path.Combine(outPath, "content", "app", "intelxdk.config.ios.xml"));
            if (File.Exists(Path.Combine(outPath, "content", "app", "intelxdk.config.windows.xml")))
                File.Delete(Path.Combine(outPath, "content", "app", "intelxdk.config.windows.xml"));
            if (File.Exists(Path.Combine(outPath, "content", "app", "intelxdk.config.windows8.xml")))
                File.Delete(Path.Combine(outPath, "content", "app", "intelxdk.config.windows8.xml"));
            if (File.Exists(Path.Combine(outPath, "content", "app", "intelxdk.config.wp81.xml")))
                File.Delete(Path.Combine(outPath, "content", "app", "intelxdk.config.wp81.xml"));
            if (File.Exists(Path.Combine(outPath, "content", "app", "xdk-publish-meta-data.json")))
                File.Delete(Path.Combine(outPath, "content", "app", "xdk-publish-meta-data.json"));
        }

        protected void InjectImages(Bitmap menuIconImg, Bitmap bootTvImg, Bitmap bootDrcImg)
        {
            Graphics g;

            if (menuIconImg != null)
            {
                Bitmap tmpMenuIconImg;
                tmpMenuIconImg = new Bitmap(128, 128, PixelFormat.Format32bppArgb);
                g = Graphics.FromImage(tmpMenuIconImg);
                g.DrawImage(menuIconImg, new Rectangle(0, 0, 128, 128));
                g.Dispose();
                if (!NUSContent.SaveTGA(tmpMenuIconImg, BasePath + "\\meta\\iconTex.tga"))
                    throw new Exception("Error creating \"iconTex.tga\" file.");
            }

            if (bootTvImg != null)
            {
                Bitmap tmpBootTvImg;
                tmpBootTvImg = new Bitmap(1280, 720, PixelFormat.Format24bppRgb);
                g = Graphics.FromImage(tmpBootTvImg);
                g.DrawImage(bootTvImg, new Rectangle(0, 0, 1280, 720));
                g.Dispose();
                if (!NUSContent.SaveTGA(tmpBootTvImg, BasePath + "\\meta\\bootTvTex.tga"))
                    throw new Exception("Error creating \"bootTvTex.tga\" file.");
            }

            if (bootDrcImg != null)
            {
                Bitmap tmpBootDrcImg;
                tmpBootDrcImg = new Bitmap(854, 480, PixelFormat.Format24bppRgb);
                g = Graphics.FromImage(tmpBootDrcImg);
                g.DrawImage(bootDrcImg, new Rectangle(0, 0, 854, 480));
                g.Dispose();
                if (!NUSContent.SaveTGA(tmpBootDrcImg, BasePath + "\\meta\\bootDrcTex.tga"))
                    throw new Exception("Error creating \"bootDrcTex.tga\" file.");
            }
        }

        protected void InjectMeta(string shortName, string longName)
        {
            string titleId = TitleId;
            byte[] id = Useful.StrHexToByteArray(titleId, "");

            XmlWriterSettings xmlSettings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\n",
                NewLineHandling = NewLineHandling.Replace
            };

            XmlDocument xmlApp = new XmlDocument();
            XmlDocument xmlMeta = new XmlDocument();

            xmlApp.Load(BasePath + "\\code\\app.xml");
            xmlMeta.Load(BasePath + "\\meta\\meta.xml");

            XmlNode app_title_id = xmlApp.SelectSingleNode("app/title_id");
            XmlNode app_group_id = xmlApp.SelectSingleNode("app/group_id");

            XmlNode meta_product_code = xmlMeta.SelectSingleNode("menu/product_code");
            XmlNode meta_title_id = xmlMeta.SelectSingleNode("menu/title_id");
            XmlNode meta_group_id = xmlMeta.SelectSingleNode("menu/group_id");
            XmlNode meta_longname_ja = xmlMeta.SelectSingleNode("menu/longname_ja");
            XmlNode meta_longname_en = xmlMeta.SelectSingleNode("menu/longname_en");
            XmlNode meta_longname_fr = xmlMeta.SelectSingleNode("menu/longname_fr");
            XmlNode meta_longname_de = xmlMeta.SelectSingleNode("menu/longname_de");
            XmlNode meta_longname_it = xmlMeta.SelectSingleNode("menu/longname_it");
            XmlNode meta_longname_es = xmlMeta.SelectSingleNode("menu/longname_es");
            XmlNode meta_longname_zhs = xmlMeta.SelectSingleNode("menu/longname_zhs");
            XmlNode meta_longname_ko = xmlMeta.SelectSingleNode("menu/longname_ko");
            XmlNode meta_longname_nl = xmlMeta.SelectSingleNode("menu/longname_nl");
            XmlNode meta_longname_pt = xmlMeta.SelectSingleNode("menu/longname_pt");
            XmlNode meta_longname_ru = xmlMeta.SelectSingleNode("menu/longname_ru");
            XmlNode meta_longname_zht = xmlMeta.SelectSingleNode("menu/longname_zht");
            XmlNode meta_shortname_ja = xmlMeta.SelectSingleNode("menu/shortname_ja");
            XmlNode meta_shortname_en = xmlMeta.SelectSingleNode("menu/shortname_en");
            XmlNode meta_shortname_fr = xmlMeta.SelectSingleNode("menu/shortname_fr");
            XmlNode meta_shortname_de = xmlMeta.SelectSingleNode("menu/shortname_de");
            XmlNode meta_shortname_it = xmlMeta.SelectSingleNode("menu/shortname_it");
            XmlNode meta_shortname_es = xmlMeta.SelectSingleNode("menu/shortname_es");
            XmlNode meta_shortname_zhs = xmlMeta.SelectSingleNode("menu/shortname_zhs");
            XmlNode meta_shortname_ko = xmlMeta.SelectSingleNode("menu/shortname_ko");
            XmlNode meta_shortname_nl = xmlMeta.SelectSingleNode("menu/shortname_nl");
            XmlNode meta_shortname_pt = xmlMeta.SelectSingleNode("menu/shortname_pt");
            XmlNode meta_shortname_ru = xmlMeta.SelectSingleNode("menu/shortname_ru");
            XmlNode meta_shortname_zht = xmlMeta.SelectSingleNode("menu/shortname_zht");

            app_title_id.InnerText = titleId;
            app_group_id.InnerText = "0000" + id[5].ToString("X2") + id[6].ToString("X2");

            meta_product_code.InnerText = "WUP-N-WUMI";

            meta_title_id.InnerText = titleId;
            meta_group_id.InnerText = "0000" + id[5].ToString("X2") + id[6].ToString("X2");
            meta_longname_ja.InnerText = longName;
            meta_longname_en.InnerText = longName;
            meta_longname_fr.InnerText = longName;
            meta_longname_de.InnerText = longName;
            meta_longname_it.InnerText = longName;
            meta_longname_es.InnerText = longName;
            meta_longname_zhs.InnerText = longName;
            meta_longname_ko.InnerText = longName;
            meta_longname_nl.InnerText = longName;
            meta_longname_pt.InnerText = longName;
            meta_longname_ru.InnerText = longName;
            meta_longname_zht.InnerText = longName;
            meta_shortname_ja.InnerText = shortName;
            meta_shortname_en.InnerText = shortName;
            meta_shortname_fr.InnerText = shortName;
            meta_shortname_de.InnerText = shortName;
            meta_shortname_it.InnerText = shortName;
            meta_shortname_es.InnerText = shortName;
            meta_shortname_zhs.InnerText = shortName;
            meta_shortname_ko.InnerText = shortName;
            meta_shortname_nl.InnerText = shortName;
            meta_shortname_pt.InnerText = shortName;
            meta_shortname_ru.InnerText = shortName;
            meta_shortname_zht.InnerText = shortName;

            XmlWriter app = XmlWriter.Create(BasePath + "\\code\\app.xml", xmlSettings);
            XmlWriter meta = XmlWriter.Create(BasePath + "\\meta\\meta.xml", xmlSettings);

            xmlApp.Save(app);
            xmlMeta.Save(meta);

            app.Close();
            meta.Close();
        }

        protected void InjectMultimedia(string outputPath)
        {
            XmlWriterSettings xmlSettings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\n",
                NewLineHandling = NewLineHandling.Replace
            };

            XmlDocument xmlConfig = new XmlDocument();

            xmlConfig.Load(Path.Combine(outputPath, "config.xml"));

            XmlNodeList nodes = xmlConfig.LastChild.ChildNodes;
            XmlNodeList options = null;
            for (int i = 0; i < nodes.Count; i++)
                if (nodes.Item(i).Name == "nwf:options")
                {
                    options = nodes.Item(i).ChildNodes;
                    break;
                }

            XmlNode supportedWiiRemotes = null;
            XmlNode touchScrollingTV = null;
            XmlNode touchScrollingGP = null;
            if (options != null)
            {
                for (int i = 0; i < options.Count; i++)
                    if (options.Item(i).Name == "nwf:supportedWiiRemotes")
                    {
                        supportedWiiRemotes = options.Item(i);
                        break;
                    }

                if (supportedWiiRemotes != null)
                    supportedWiiRemotes.InnerText = "1";

                for (int i = 0; i < options.Count; i++)
                    if (options.Item(i).Name == "nwf:touchScrolling" &&
                        options.Item(i).Attributes.GetNamedItem("nwf:display").Value == "tv")
                    {
                        touchScrollingTV = options.Item(i);
                        break;
                    }

                if (touchScrollingTV != null)
                    touchScrollingTV.ChildNodes.Item(1).Attributes.GetNamedItem("value").Value = "true";

                for (int i = 0; i < options.Count; i++)
                    if (options.Item(i).Name == "nwf:touchScrolling" &&
                        options.Item(i).Attributes.GetNamedItem("nwf:display").Value == "gp")
                    {
                        touchScrollingGP = options.Item(i);
                        break;
                    }

                if (touchScrollingGP != null)
                    touchScrollingGP.ChildNodes.Item(1).Attributes.GetNamedItem("value").Value = "true";
            }

            XmlNode widget_content = xmlConfig.LastChild.LastChild;
            XmlAttributeCollection attributes = widget_content.Attributes;
            XmlNode src = attributes.GetNamedItem("src");
            src.InnerText = "/index.html";
            XmlWriter config = XmlWriter.Create(Path.Combine(outputPath, "config.xml"), xmlSettings);
            xmlConfig.Save(config);
            config.Close();

            Cll.JSON.Object json = InjectFolder(MultimediaPath, outputPath);

            Resources.folder.Save(Path.Combine(outputPath, "folder.png"), ImageFormat.Png);
            Resources.audio.Save(Path.Combine(outputPath, "audio.png"), ImageFormat.Png);
            Resources.image.Save(Path.Combine(outputPath, "image.png"), ImageFormat.Png);
            Resources.video.Save(Path.Combine(outputPath, "video.png"), ImageFormat.Png);
            Resources.random_audio.Save(Path.Combine(outputPath, "random_audio.png"), ImageFormat.Png);
            Resources.random_audio_blue.Save(Path.Combine(outputPath, "random_audio_blue.png"), ImageFormat.Png);
            Resources.back.Save(Path.Combine(outputPath, "back.png"), ImageFormat.Png);
            Resources.border.Save(Path.Combine(outputPath, "border.png"), ImageFormat.Png);
            Resources.next_audio.Save(Path.Combine(outputPath, "next_audio.png"), ImageFormat.Png);
            Resources.pause_audio.Save(Path.Combine(outputPath, "pause_audio.png"), ImageFormat.Png);
            Resources.play_audio.Save(Path.Combine(outputPath, "play_audio.png"), ImageFormat.Png);

            StreamWriter sw = File.CreateText(Path.Combine(outputPath, "index.html"));
            sw.Write(Resources.index);
            sw.Close();
            sw = File.CreateText(Path.Combine(outputPath, "wumplayer_style_1920.css"));
            sw.Write(Resources.wumplayer_style_1920);
            sw.Close();
            sw = File.CreateText(Path.Combine(outputPath, "wumplayer_style_1280.css"));
            sw.Write(Resources.wumplayer_style_1280);
            sw.Close();
            sw = File.CreateText(Path.Combine(outputPath, "wumplayer_style_854.css"));
            sw.Write(Resources.wumplayer_style_854);
            sw.Close();
            sw = File.CreateText(Path.Combine(outputPath, "wumplayer_style_640.css"));
            sw.Write(Resources.wumplayer_style_640);
            sw.Close();
            sw = File.CreateText(Path.Combine(outputPath, "wumplayer.js"));
            sw.Write(Resources.wumplayer);
            sw.Close();
            sw = File.CreateText(Path.Combine(outputPath, "wumplayer_fs.js"));
            sw.Write("var fs = '");
            sw.Write(json.ToString());
            sw.Write("';");
            sw.Close();
        }

        public void LoadBase(string path)
        {
            NUSContent.Format format = NUSContent.GetFormat(path);

            if (format == NUSContent.Format.Decrypted)
            {
                ValidateBase(path);

                if (Directory.Exists(BasePath))
                {
                    Directory.Delete(BasePath, true);
                    Base = null;
                }

                if (Useful.DirectoryCopy(path, BasePath, true))
                    Base = GetLoadedBase();
                else
                    throw new Exception("Could not load base \"" + path + "\".");
            }
            else if (format == NUSContent.Format.Encrypted)
            {
                ValidateEncryptedBase(path);

                if (Directory.Exists(BasePath))
                {
                    Directory.Delete(BasePath, true);
                    Base = null;
                }

                Directory.CreateDirectory(BasePath);
                NUSContent.Decrypt(path, BasePath);
                Base = GetLoadedBase();
            }
            else
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.AppendLine("The folder not contains a valid NUS content.");
                strBuilder.AppendLine("If it is an unpackaged (decrypted) NUS content, then:");
                strBuilder.AppendLine("The \"" + path + "\\code\" folder not exist.");
                strBuilder.AppendLine("Or \"" + path + "\\content\" folder not exist.");
                strBuilder.AppendLine("Or \"" + path + "\\meta\" folder not exist.");
                strBuilder.AppendLine("If it is an packaged (encrypted) NUS content, then:");
                strBuilder.AppendLine("The \"" + path + "\\title.tmd\" file not exist.");
                strBuilder.AppendLine("Or \"" + path + "\\title.tik\" file not exist.");
                strBuilder.AppendLine("Or \"" + path + "\\title.cert\" file not exist.");
                throw new Exception(strBuilder.ToString());
            }
        }

        public void ValidateBase(string path)
        {
            string[] folders = {
                Path.Combine(path, "content", "assets")
            };

            string[] files = {
                Path.Combine(path, "code", "app.xml"),
                Path.Combine(path, "code", "cos.xml"),
                Path.Combine(path, "code", "preload.txt"),
                Path.Combine(path, "code", "WiiULauncher.rpx"),
                Path.Combine(path, "code", "mvplayer.rpl"),
                Path.Combine(path, "code", "libz_rpl.rpl"),
                Path.Combine(path, "code", "libxslt_rpl.rpl"),
                Path.Combine(path, "code", "libxml2_rpl.rpl"),
                Path.Combine(path, "code", "libWTF_rpl.rpl"),
                Path.Combine(path, "code", "libwksup_rpl.rpl"),
                Path.Combine(path, "code", "libwkaud_rpl.rpl"),
                Path.Combine(path, "code", "libWebKit_rpl.rpl"),
                Path.Combine(path, "code", "libWebCore_rpl.rpl"),
                Path.Combine(path, "code", "libssl_rpl.rpl"),
                Path.Combine(path, "code", "libsqlite3_rpl.rpl"),
                Path.Combine(path, "code", "libshim_rpl.rpl"),
                Path.Combine(path, "code", "libpng15_rpl.rpl"),
                Path.Combine(path, "code", "libplatform_rpl.rpl"),
                Path.Combine(path, "code", "libpixman_rpl.rpl"),
                Path.Combine(path, "code", "libMIRegistry_rpl.rpl"),
                Path.Combine(path, "code", "libMIPluginTransport_rpl.rpl"),
                Path.Combine(path, "code", "libMIPluginPlayerAudio_rpl.rpl"),
                Path.Combine(path, "code", "libMIPluginDecoderWAV_rpl.rpl"),
                Path.Combine(path, "code", "libMIPluginDecoderOggVorbis_rpl.rpl"),
                Path.Combine(path, "code", "libMediaInterfaceRenderer_rpl.rpl"),
                Path.Combine(path, "code", "libMediaInterfaceCore_rpl.rpl"),
                Path.Combine(path, "code", "libjpeg_rpl.rpl"),
                Path.Combine(path, "code", "libJavaScriptCore_rpl.rpl"),
                Path.Combine(path, "code", "libicu_rpl.rpl"),
                Path.Combine(path, "code", "libffmpegfft_rpl.rpl"),
                Path.Combine(path, "code", "libcurl_rpl.rpl"),
                Path.Combine(path, "code", "libcrypto_rpl.rpl"),
                Path.Combine(path, "content", "app", "certificates", "ca-bundle.pem"),
                Path.Combine(path, "content", "app", "config.xml"),
                Path.Combine(path, "meta", "iconTex.tga"),
                Path.Combine(path, "meta", "bootTvTex.tga"),
                Path.Combine(path, "meta", "bootDrcTex.tga"),
                Path.Combine(path, "meta", "meta.xml")
            };

            ValidateBase(folders, files);
        }

        public void ValidateEncryptedBase(string path)
        {
            ValidateEncryptedBase(path, "WiiULauncher.rpx");
        }

        protected void ValidateBase(string[] folders, string[] files)
        {
            StringBuilder strBuilder = new StringBuilder();

            bool valid = true;
            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    strBuilder.AppendLine("This folder is missing: \"" + folder + "\"");
                    valid = false;
                }
            }

            foreach (string file in files)
            {
                if (!File.Exists(file))
                {
                    strBuilder.AppendLine("This file is missing: \"" + file + "\"");
                    valid = false;
                }
            }

            if (!valid)
                throw new Exception(strBuilder.ToString());
        }

        protected void ValidateEncryptedBase(string path, string cvFileName)
        {
            string appFileName = GetAppFileName(path);
            if (appFileName != cvFileName)
                throw new Exception("The \"" + appFileName + "\" not match the requested \"" + cvFileName + "\".");
        }

        protected string GetAppFileName(string path)
        {
            string unpackPath = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NUSConverter"), "unpack");

            if (File.Exists(Path.Combine(unpackPath, "cos.xml")))
                File.Delete(Path.Combine(unpackPath, "cos.xml"));

            NUSContent.Decrypt(path, "code\\cos.xml", Path.Combine(unpackPath, "cos.xml"));

            if (!File.Exists(Path.Combine(unpackPath, "cos.xml")))
                throw new Exception("The NUS content does not contains \"code\\cos.xml\" file.");

            XmlDocument xmlCos = new XmlDocument();
            xmlCos.Load(Path.Combine(unpackPath, "cos.xml"));
            XmlNode cos_argstr = xmlCos.SelectSingleNode("app/argstr");
            return cos_argstr.InnerText;
        }

        protected string GetValidOutputPath(string outputPath, string shortName)
        {
            if (!Directory.Exists(outputPath))
                throw new Exception("The output path \"" + outputPath + "\" not exist.");

            if (shortName.Length == 0)
                throw new Exception("The short name is empty.");

            char[] array = Useful.Windows1252ToASCII(shortName, '_').ToCharArray();
            char[] invalid = Path.GetInvalidFileNameChars();
            for (int i = 0; i < array.Length; i++)
            {
                foreach (char c in invalid)
                {
                    if (array[i] == c)
                        array[i] = '_';
                }
            }
            string folderName = new string(array);

            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(outputPath);
            strBuilder.Append("\\");
            strBuilder.Append(folderName);
            strBuilder.Append(" [");
            strBuilder.Append(TitleId);
            strBuilder.Append("]");
            return strBuilder.ToString();
        }


        public static Cll.JSON.Object InjectFolder(string inputPath, string outputPath)
        {
            string[] folders = Directory.GetDirectories(inputPath);
            string[] files = Directory.GetFiles(inputPath);

            string validFolderName = ValidFilename(Path.GetFileName(inputPath));
            string output = Path.Combine(outputPath, validFolderName);

            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);

            NumericComparer numericComparer = new NumericComparer();
            Array.Sort(folders, numericComparer.Compare);
            Array.Sort(files, numericComparer.Compare);

            Cll.JSON.Object folderJSON = new Cll.JSON.Object();
            folderJSON.AddMember("name", ValidName(Path.GetFileName(inputPath)));
            folderJSON.AddMember("foldername", ValidName(validFolderName));

            Cll.JSON.Array foldersArray = new Cll.JSON.Array();
            foreach (string folder in folders)
            {
                foldersArray.AddValue(InjectFolder(folder, output));
            }

            Cll.JSON.Array filesArray = new Cll.JSON.Array();
            foreach (string file in files)
            {
                try
                {
                    Cll.JSON.Object fileJSON = InjectFile(file, output);
                    if (fileJSON.Count != 0)
                        filesArray.AddValue(fileJSON);
                }
                catch (Exception e)
                {
                    Cll.Log.WriteLine(e.ToString());
                }
            }

            folderJSON.AddMember("folders", foldersArray);
            folderJSON.AddMember("files", filesArray);

            return folderJSON;
        }

        private static Cll.JSON.Object InjectFile(string inputFile, string outputPath)
        {
            string extension = Path.GetExtension(inputFile);
            Cll.JSON.Object fileJSON = new Cll.JSON.Object();

            if (extension == ".3gp" ||
                extension == ".avi" ||
                extension == ".flv" ||
                extension == ".m4v" ||
                extension == ".mkv" ||
                extension == ".mov" ||
                extension == ".mp4" ||
                extension == ".mpeg" ||
                extension == ".mpg" ||
                extension == ".ogv" ||
                extension == ".rm" ||
                extension == ".webm" ||
                extension == ".aac" ||
                extension == ".mp3" ||
                extension == ".m4a" ||
                extension == ".oga" ||
                extension == ".ogg" ||
                extension == ".wav" ||
                extension == ".wma")
            {
                Converter.EncoderResult result = Converter.Encoder(inputFile, outputPath);
                for (int i = 0; i < result.Name.Length; i++)
                {
                    fileJSON.AddMember("name", ValidName(result.Name[i]));
                    fileJSON.AddMember("filename", ValidName(ValidFilename(result.Name[i])));
                    fileJSON.AddMember("ext", result.Extension);
                    if (result.Extension == ".mp4")
                    {
                        fileJSON.AddMember("width", result.VideoInfo.Width);
                        fileJSON.AddMember("height", result.VideoInfo.Height);
                    }
                    else
                    {
                        fileJSON.AddMember("width", 0);
                        fileJSON.AddMember("height", 0);
                    }
                }
                Cll.Log.WriteLine("\"" + inputFile + "\" OK!");
            }
            else if (extension == ".jpg" ||
                extension == ".png" ||
                extension == ".bmp" ||
                extension == ".gif")
            {
                string validFileName = ValidFilename(Path.GetFileNameWithoutExtension(inputFile));
                File.Copy(inputFile, Path.Combine(outputPath, validFileName + extension));
                fileJSON.AddMember("name", ValidName(Path.GetFileNameWithoutExtension(inputFile)));
                fileJSON.AddMember("filename", ValidName(validFileName));
                fileJSON.AddMember("ext", extension);
                Bitmap img = new Bitmap(inputFile);
                fileJSON.AddMember("width", img.Width);
                fileJSON.AddMember("height", img.Height);
                img.Dispose();
                Cll.Log.WriteLine("\"" + inputFile + "\" OK!");
            }
            else
                Cll.Log.WriteLine("\"" + inputFile + "\" file is not supported.");

            return fileJSON;
        }

        private static string ValidName(string name)
        {
            return name.Replace("'", "\\'");
        }

        public static string ValidFilename(string filename)
        {
            char[] array = Useful.Windows1252ToASCII(filename, '_').ToCharArray();
            char[] invalid = Path.GetInvalidFileNameChars();
            for (int i = 0; i < array.Length; i++)
            {
                foreach (char c in invalid)
                {
                    if (array[i] == c)
                        array[i] = '_';
                }
            }
            return new string(array);
        }
    }
}

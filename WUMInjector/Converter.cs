using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;

namespace WUMInjector
{
    public class Converter
    {
        public struct EncoderResult
        {
            public string[] Name;
            public string Extension;
            public BasicVideoInfo VideoInfo;
            public BasicAudioInfo AudioInfo;
        }

        private static string ffmpegPath
        {
            get
            {
                return Path.Combine(MediaInjector.DataPath, "ffmpeg");
            }
        }
        private static string ffmpegBinPath
        {
            get
            {
                return Path.Combine(ffmpegPath, "bin", "ffmpeg.exe");
            }
        }
        private static string ffplayBinPath
        {
            get
            {
                return Path.Combine(ffmpegPath, "bin", "ffplay.exe");
            }
        }
        private static string ffprobeBinPath
        {
            get
            {
                return Path.Combine(ffmpegPath, "bin", "ffprobe.exe");
            }
        }

        public Converter()
        {
        }

        public static EncoderResult Encoder(string inputFile, string outputPath)
        {
            BasicMediaFormatInfo info = new BasicMediaFormatInfo(inputFile);
            int videoIndex = -1;
            int audioIndex = -1;

            for (int i = 0; i < info.MediaInfo.Length; i++)
            {
                if (info.MediaInfo[i].CodecType == "video")
                {
                    videoIndex = i;
                    break;
                }
            }

            for (int i = 0; i < info.MediaInfo.Length; i++)
            {
                if (info.MediaInfo[i].CodecType == "audio")
                {
                    audioIndex = i;
                    break;
                }
            }

            return Encoder(inputFile, outputPath, info, videoIndex, audioIndex);
        }

        private static EncoderResult Encoder(string inputFile, string outputPath,
            BasicMediaFormatInfo info, int videoIndex, int audioIndex)
        {
            EncoderResult encoderResult = new EncoderResult();

            encoderResult.VideoInfo = null;
            encoderResult.AudioInfo = null;
            if (videoIndex != -1 && info.MediaInfo[videoIndex].CodecType == "video" &&
                info.MediaInfo[videoIndex].CodecName != "mjpeg" &&
                info.MediaInfo[videoIndex].CodecName != "png")
                encoderResult.VideoInfo = new BasicVideoInfo((BasicVideoInfo)info.MediaInfo[videoIndex]);
            if (audioIndex != -1 && info.MediaInfo[audioIndex].CodecType == "audio")
                encoderResult.AudioInfo = new BasicAudioInfo((BasicAudioInfo)info.MediaInfo[audioIndex]);

            encoderResult.Extension = null;
            if (encoderResult.VideoInfo != null)
                encoderResult.Extension = ".mp4";
            else if (encoderResult.AudioInfo != null)
            {
                if (encoderResult.AudioInfo.CodecName == "vorbis" &&
                    encoderResult.AudioInfo.Channels <= 2)
                    encoderResult.Extension = ".ogg";
                else
                    encoderResult.Extension = ".m4a";
            }
            else
                throw new Exception("\"" + inputFile + "\" does not have multimedia content.");

            StringBuilder config = new StringBuilder();

            if (encoderResult.VideoInfo != null)
            {
                if (encoderResult.VideoInfo.Width * encoderResult.VideoInfo.Height > 1920 * 1080)
                {
                    double scale = 1920.0 / encoderResult.VideoInfo.Width;
                    double heightScale = 1080.0 / encoderResult.VideoInfo.Height;
                    if (scale > heightScale)
                        scale = heightScale;
                    encoderResult.VideoInfo.Width = (int)(encoderResult.VideoInfo.Width * scale);
                    encoderResult.VideoInfo.Height = (int)(encoderResult.VideoInfo.Height * scale);
                }

                if (encoderResult.VideoInfo.Width * encoderResult.VideoInfo.Height > 1280 * 720 && encoderResult.VideoInfo.FrameRate > 30.0)
                    encoderResult.VideoInfo.FrameRate = 30.0;

                if (encoderResult.VideoInfo.FrameRate > 60.0)
                    encoderResult.VideoInfo.FrameRate = 60.0;

                if (encoderResult.VideoInfo.CodecName == "h264" &&
                    encoderResult.VideoInfo.Width == ((BasicVideoInfo)info.MediaInfo[videoIndex]).Width &&
                    encoderResult.VideoInfo.Height == ((BasicVideoInfo)info.MediaInfo[videoIndex]).Height &&
                    encoderResult.VideoInfo.FrameRate == ((BasicVideoInfo)info.MediaInfo[videoIndex]).FrameRate &&
                    encoderResult.VideoInfo.Level <= 4.2)
                {
                    config.Append("-map 0:");
                    config.Append(videoIndex);
                    config.Append(" -c:v copy");
                }
                else
                {
                    config.Append("-map 0:");
                    config.Append(videoIndex);
                    config.Append(" -c:v libx264 -preset fast -crf 25 -s ");
                    config.Append(encoderResult.VideoInfo.Width);
                    config.Append("x");
                    config.Append(encoderResult.VideoInfo.Height);
                    config.Append(" -r ");
                    config.Append(encoderResult.VideoInfo.FrameRate);
                }
            }
            if (encoderResult.AudioInfo != null)
            {
                if (encoderResult.VideoInfo != null)
                    config.Append(" ");

                if ((encoderResult.AudioInfo.CodecName == "aac" ||
                    encoderResult.AudioInfo.CodecName == "vorbis") &&
                    encoderResult.AudioInfo.Channels <= 2)
                {
                    config.Append("-map 0:");
                    config.Append(audioIndex);
                    config.Append(" -c:a copy");
                }
                else
                {
                    config.Append("-map 0:");
                    config.Append(audioIndex);
                    config.Append(" -c:a aac -ac 2");
                }
            }

            if (info.Size > 4294967296)
            {
                int bitrate = 0;
                double totalDuration = 0;
                if (encoderResult.VideoInfo != null && encoderResult.AudioInfo == null)
                {
                    bitrate = encoderResult.VideoInfo.Bitrate;
                    totalDuration = encoderResult.VideoInfo.Duration;
                }
                else if (encoderResult.VideoInfo == null && encoderResult.AudioInfo != null)
                {
                    bitrate = encoderResult.AudioInfo.Bitrate;
                    totalDuration = encoderResult.AudioInfo.Duration;
                }
                else if (encoderResult.VideoInfo == null && encoderResult.AudioInfo == null)
                {
                    bitrate = encoderResult.VideoInfo.Bitrate + encoderResult.AudioInfo.Bitrate;
                    totalDuration = encoderResult.VideoInfo.Duration;
                }

                if (bitrate == 0)
                    bitrate = info.Bitrate;
                if (totalDuration == 0)
                    totalDuration = info.Duration;

                double duration = 33822867456.0 / bitrate;
                double start = 0.0;

                int parts = (int)(totalDuration / duration);
                if (totalDuration % duration != 0.0)
                    parts++;

                encoderResult.Name = new string[parts];

                for (int i = 0; i < parts; i++)
                {
                    encoderResult.Name[i] = Path.GetFileNameWithoutExtension(inputFile) + " part " + (i + 1).ToString();
                    string output = Path.Combine(outputPath, MediaInjector.ValidFilename(encoderResult.Name[i]) + encoderResult.Extension);
                    string cut = "-ss " + start.ToString() + " -noaccurate_seek -t " + duration.ToString();
                    string arguments = cut + " -i \"" + inputFile + "\" " + config.ToString() + " \"" + output + "\"";
                    FFmpeg(arguments);
                    start += duration;
                }
            }
            else
            {
                encoderResult.Name = new string[1];
                encoderResult.Name[0] = Path.GetFileNameWithoutExtension(inputFile);
                string output = Path.Combine(outputPath, MediaInjector.ValidFilename(encoderResult.Name[0]) + encoderResult.Extension);
                string arguments = "-i \"" + inputFile + "\" " + config.ToString() + " \"" + output + "\"";
                FFmpeg(arguments);
            }

            return encoderResult;
        }

        private static void FFmpeg(string arguments)
        {
            if (!CheckFFmpeg())
                throw new Exception("FFmpeg not found!");

            Process ffmpeg = Process.Start(ffmpegBinPath, arguments);
            ffmpeg.WaitForExit();

            if (ffmpeg.ExitCode == 0)
                ffmpeg.Dispose();
            else
            {
                ffmpeg.Dispose();
                throw new Exception("ffmpeg fail with arguments \"" + arguments + "\".");
            }
        }

        public static bool CheckFFmpeg()
        {
            if (Directory.Exists(ffmpegPath) &&
                File.Exists(ffmpegBinPath) &&
                File.Exists(ffplayBinPath) &&
                File.Exists(ffprobeBinPath))
                return true;
            return false;
        }

        public async static Task DownloadFFmpeg()
        {
            string ffmpegZip = Path.Combine(MediaInjector.DataPath, "ffmpeg.zip");

            FileStream fs = File.Create(ffmpegZip);
            fs.Write(global::WUMInjector.Properties.Resources.ffmpeg_4_3_1_win32_static, 0,
                global::WUMInjector.Properties.Resources.ffmpeg_4_3_1_win32_static.Length);
            fs.Close();

            await Task.Run(() => ZipFile.ExtractToDirectory(ffmpegZip, MediaInjector.DataPath));

            File.Delete(ffmpegZip);

            Directory.Move(Path.Combine(MediaInjector.DataPath, "ffmpeg-4.3.1-win32-static"), ffmpegPath);
        }

        public static string GetMediaInfo(string filename)
        {
            Process ffprobe = new Process();
            ffprobe.StartInfo.FileName = ffprobeBinPath;
            ffprobe.StartInfo.Arguments = "-print_format json -show_format -show_streams \"" + filename + "\"";
            ffprobe.StartInfo.UseShellExecute = false;
            ffprobe.StartInfo.RedirectStandardOutput = true;
            ffprobe.Start();

            StreamReader sr = ffprobe.StandardOutput;
            string output = sr.ReadToEnd();
            sr.Close();

            ffprobe.WaitForExit();

            if (ffprobe.ExitCode == 0)
            {
                ffprobe.Dispose();
                return output;
            }
            else
            {
                ffprobe.Dispose();
                throw new Exception("ffprobe fail with file \"" + filename + "\".");
            }
        }
    }
}

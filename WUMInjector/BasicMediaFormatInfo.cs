using System;
using System.Text;

namespace WUMInjector
{
    public class BasicMediaFormatInfo
    {
        public string FileName { private set; get; }
        public long Size { private set; get; }
        public string Format { private set; get; }
        public double Duration { private set; get; }
        public int Bitrate { private set; get; }
        public string Title { private set; get; }

        public BasicMediaInfo[] MediaInfo;

        public BasicMediaFormatInfo(string filename)
        {
            string mediaInfo = Converter.GetMediaInfo(filename);
            Cll.JSON.Element info = Cll.JSON.SyntacticAnalyzer.Read(mediaInfo);
            Cll.JSON.Object format = (Cll.JSON.Object)info.Value.GetValue("format");

            FileName = ((Cll.JSON.String)format.GetValue("filename")).Value;
            Size = Convert.ToInt64(((Cll.JSON.String)format.GetValue("size")).Value);
            Format = ((Cll.JSON.String)format.GetValue("format_name")).Value;
            Duration = Convert.ToDouble(((Cll.JSON.String)format.GetValue("duration")).Value);
            Bitrate = Convert.ToInt32(((Cll.JSON.String)format.GetValue("bit_rate")).Value);

            if (format.Contains("tags") && ((Cll.JSON.Object)format.GetValue("tags")).Contains("title"))
                Title = ((Cll.JSON.String)format.GetValue("tags").GetValue("title")).Value;
            else
                Title = "";

            int numStreams = Convert.ToInt32(((Cll.JSON.Number)info.Value.GetValue("format").GetValue("nb_streams")).Value);
            MediaInfo = new BasicMediaInfo[numStreams];

            for (int i = 0; i < MediaInfo.Length; i++)
            {
                Cll.JSON.Object streamInfo = (Cll.JSON.Object)info.Value.GetValue("streams").GetValue(i);
                string codecType = ((Cll.JSON.String)streamInfo.GetValue("codec_type")).Value;
                if (codecType == "video")
                    MediaInfo[i] = new BasicVideoInfo(streamInfo);
                else if (codecType == "audio")
                    MediaInfo[i] = new BasicAudioInfo(streamInfo);
                else if (codecType == "subtitle")
                    MediaInfo[i] = new BasicSubtitleInfo(streamInfo);
                else
                    MediaInfo[i] = new BasicMediaInfo(streamInfo);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("File name: \"" + FileName + "\"");
            sb.AppendLine("Size: " + Useful.GetFileSizeWithUnit(Size));
            sb.AppendLine("Format: " + Format);
            sb.AppendLine("Duration: " + GetTimeFormat((int)Duration));
            sb.AppendLine("Bit rate: " + GetBitrateFormat(Bitrate));
            sb.AppendLine("Title: \"" + Title + "\"");
            for (int i = 0; i < MediaInfo.Length; i++)
            {
                sb.AppendLine("Stream " + i + ": [");
                sb.AppendLine(MediaInfo[i].ToString());
                sb.AppendLine("]");
            }
            return sb.ToString();
        }

        public static string GetBitrateFormat(long bps)
        {
            if (bps < 0)
                throw new Exception("Bitrate cannot be negative.");

            double value = bps;
            int multiple = 0;
            string unit = "";

            while (value >= 1000 && multiple < 8)
            {
                value /= 1000;
                multiple++;
            }

            switch (multiple)
            {
                case 0: unit = "bps"; break; // bits/seconds
                case 1: unit = "kbps"; break; // Kilobits/seconds
                case 2: unit = "Mbps"; break; // Megabits/seconds
                case 3: unit = "Gbps"; break; // Gigabits/seconds
                case 4: unit = "Tbps"; break; // Terabits/seconds
                case 5: unit = "Pbps"; break; // Petabits/seconds
                case 6: unit = "Ebps"; break; // Exabits/seconds (Int64.MaxValue = 9.22 Ebps)
                case 7: unit = "Zbps"; break; // Zettabits/seconds
                case 8: unit = "Ybps"; break; // Yottabits/seconds
            }

            return Math.Round(value, 2).ToString(System.Globalization.CultureInfo.CurrentCulture) + " " + unit;
        }

        public static string GetTimeFormat(int seconds)
        {
            if (seconds < 0)
                throw new Exception("Time cannot be negative.");

            int minutes = (int)(seconds / 60.0);
            int hours = (int)(minutes / 60.0);
            int days = (int)(hours / 24.0);
            seconds -= minutes * 60;
            minutes -= hours * 60;
            hours -= days * 24;

            string time;

            if (days > 0)
            {
                time = days.ToString() + ":" + hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00") +
                    (days == 1 && hours == 0 && minutes == 0 && seconds == 0 ? " day" : " days");
            }
            else if (hours > 0)
            {
                time = hours.ToString() + ":" + minutes.ToString("00") + ":" + seconds.ToString("00") +
                    (hours == 1 && minutes == 0 && seconds == 0 ? " hour" : " hours");
            }
            else if (minutes > 0)
            {
                time = minutes.ToString() + ":" + seconds.ToString("00") +
                    (minutes == 1 && seconds == 0 ? " minute" : " minutes");
            }
            else
            {
                time = seconds.ToString() +
                    (seconds == 1 ? " second" : " seconds");
            }

            return time;
        }
    }

    public class BasicMediaInfo
    {
        public string CodecName { protected set; get; }
        public string CodecType { protected set; get; }
        public int Bitrate;
        public double FrameRate;
        public double Duration;
        public string Title { protected set; get; }
        public string Language { protected set; get; }

        public BasicMediaInfo(Cll.JSON.Object streamInfo)
        {
            CodecName = ((Cll.JSON.String)streamInfo.GetValue("codec_name")).Value;
            CodecType = ((Cll.JSON.String)streamInfo.GetValue("codec_type")).Value;
            if (streamInfo.Contains("bit_rate"))
                Bitrate = Convert.ToInt32(((Cll.JSON.String)streamInfo.GetValue("bit_rate")).Value);
            else
                Bitrate = 0;
            string frameRate = ((Cll.JSON.String)streamInfo.GetValue("r_frame_rate")).Value;
            string[] a = frameRate.Split(new char[] { '/' });
            if (a[1] != "0")
                FrameRate = Convert.ToDouble(a[0]) / Convert.ToDouble(a[1]);
            else
                FrameRate = Double.NaN;
            if (streamInfo.Contains("duration"))
                Duration = Convert.ToDouble(((Cll.JSON.String)streamInfo.GetValue("duration")).Value);
            else
                Duration = 0;
            if (streamInfo.Contains("tags") && ((Cll.JSON.Object)streamInfo.GetValue("tags")).Contains("title"))
                Title = ((Cll.JSON.String)streamInfo.GetValue("tags").GetValue("title")).Value;
            else
                Title = "";
            if (streamInfo.Contains("tags") && ((Cll.JSON.Object)streamInfo.GetValue("tags")).Contains("language"))
                Language = ((Cll.JSON.String)streamInfo.GetValue("tags").GetValue("language")).Value;
            else
                Language = "";
        }

        public BasicMediaInfo(BasicMediaInfo mediaInfo)
        {
            CodecName = mediaInfo.CodecName;
            CodecType = mediaInfo.CodecType;
            Bitrate = mediaInfo.Bitrate;
            FrameRate = mediaInfo.FrameRate;
            Duration = mediaInfo.Duration;
            Title = mediaInfo.Title;
            Language = mediaInfo.Language;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Codec name: " + CodecName);
            sb.Append("Codec type: " + CodecType);
            if (Bitrate != 0)
                sb.Append("\nBit rate: " + BasicMediaFormatInfo.GetBitrateFormat(Bitrate));
            if (!Double.IsNaN(FrameRate))
                sb.Append("\nFrame rate: " + FrameRate.ToString("0.00") + " FPS");
            if (Duration != 0)
                sb.Append("\nDuration: " + BasicMediaFormatInfo.GetTimeFormat((int)Duration));
            if (Title != "")
                sb.Append("\nTitle: \"" + Title + "\"");
            if (Language != "")
                sb.Append("\nLanguage: \"" + Language + "\"");
            return sb.ToString();
        }
    }

    public class BasicVideoInfo : BasicMediaInfo
    {
        public string Profile { private set; get; }
        public double Level { private set; get; }
        public int Width;
        public int Height;

        public BasicVideoInfo(Cll.JSON.Object streamInfo)
            : base(streamInfo)
        {
            if (streamInfo.Contains("profile"))
                Profile = ((Cll.JSON.String)streamInfo.GetValue("profile")).Value;
            else
                Profile = "none";
            Level = ((Cll.JSON.Number)streamInfo.GetValue("level")).Value;
            if (Level > 10.0) Level /= 10.0;
            Width = Convert.ToInt32(((Cll.JSON.Number)streamInfo.GetValue("width")).Value);
            Height = Convert.ToInt32(((Cll.JSON.Number)streamInfo.GetValue("height")).Value);
        }

        public BasicVideoInfo(BasicVideoInfo videoInfo)
            : base(videoInfo)
        {
            Profile = videoInfo.Profile;
            Level = videoInfo.Level;
            Width = videoInfo.Width;
            Height = videoInfo.Height;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine("Profile: " + Profile);
            sb.AppendLine("Level: " + Level.ToString());
            sb.AppendLine("Width: " + Width.ToString());
            sb.Append("Height: " + Height.ToString());
            return sb.ToString();
        }
    }

    public class BasicAudioInfo : BasicMediaInfo
    {
        public int Channels { private set; get; }
        public int SampleRate { private set; get; }

        public BasicAudioInfo(Cll.JSON.Object streamInfo)
            : base(streamInfo)
        {
            Channels = Convert.ToInt32(((Cll.JSON.Number)streamInfo.GetValue("channels")).Value);
            SampleRate = Convert.ToInt32(((Cll.JSON.String)streamInfo.GetValue("sample_rate")).Value);
        }

        public BasicAudioInfo(BasicAudioInfo audioInfo)
            : base(audioInfo)
        {
            Channels = audioInfo.Channels;
            SampleRate = audioInfo.SampleRate;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine("Channels: " + Channels);
            sb.Append("Sample rate: " + SampleRate.ToString());
            return sb.ToString();
        }
    }

    public class BasicSubtitleInfo : BasicMediaInfo
    {
        public BasicSubtitleInfo(Cll.JSON.Object streamInfo)
            : base(streamInfo)
        {
        }

        public BasicSubtitleInfo(BasicSubtitleInfo subtitleInfo)
            : base(subtitleInfo)
        {
        }
    }
}

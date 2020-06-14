using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WUMMInjector
{
    public class BootImage : IDisposable
    {
        private bool disposed = false;

        private Bitmap _background;
        private Bitmap _preview;

        public Bitmap Background
        {
            set
            {
                if (_background != null)
                    _background.Dispose();
                _background = value;
            }
            get { return _background; }
        }
        public Bitmap Preview
        {
            set
            {
                if (_preview != null)
                    _preview.Dispose();
                _preview = value;
            }
            get { return _preview; }
        }

        public BootImage()
        {
            _background = null;
            _preview = null;
        }

        ~BootImage()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (Background != null)
                    {
                        Background.Dispose();
                        Background = null;
                    }
                    if (Preview != null)
                    {
                        Preview.Dispose();
                        Preview = null;
                    }
                }
                disposed = true;
            }
        }

        public Bitmap Create()
        {
            Bitmap img = new Bitmap(1280, 720);
            Graphics g = Graphics.FromImage(img);
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Color.White);

            if (Background == null)
            {
                GraphicsPath sfi = new GraphicsPath();
                Font font = new Font("Trebuchet MS", 10.0F, FontStyle.Regular, GraphicsUnit.Point);
                StringFormat format = new StringFormat();
                Rectangle rectangleI = new Rectangle(970, 640, 320, 40);
                SolidBrush brushI = new SolidBrush(Color.FromArgb(64, 192, 192, 192));
                Pen outlineI = new Pen(Color.FromArgb(64, 255, 255, 255), 1.4F);

                g.Clear(Color.FromArgb(0, 0, 0));

                sfi.AddString("WUMM Injector", font.FontFamily,
                    (int)(FontStyle.Regular),
                    g.DpiY * 26.0F / 72.0F, rectangleI, format);
                g.DrawPath(outlineI, sfi);
                g.FillPath(brushI, sfi);
            }
            else
                g.DrawImage(Background, 0, 0, 1280, 720);

            if (Preview != null)
            {
                double scale = 1280.0 / Preview.Width;
                double heightScale = 720.0 / Preview.Height;
                if (scale > heightScale)
                    scale = heightScale;
                int previewWidth = (int)(Preview.Width * scale);
                int previewHeight = (int)(Preview.Height * scale);
                int previewX = (int)((1280.0 - previewWidth) / 2.0);
                int previewY = (int)((720.0 - previewHeight) / 2.0);

                g.DrawImage(Preview, previewX, previewY, previewWidth, previewHeight);
            }

            return img;
        }
    }
}

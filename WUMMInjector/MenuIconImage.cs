using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WUMMInjector
{
    public class MenuIconImage : IDisposable
    {
        private bool disposed = false;

        private Bitmap _frame;
        private Bitmap _background;
        private Bitmap _preview;

        public Bitmap Frame
        {
            set
            {
                if (_frame != null)
                    _frame.Dispose();
                _frame = value;
            }
            get { return _frame; }
        }
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

        public MenuIconImage()
        {
            _frame = null;
            _background = null;
            _preview = null;
        }

        ~MenuIconImage()
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
                    if (Frame != null)
                    {
                        Frame.Dispose();
                        Frame = null;
                    }
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
            Bitmap img = new Bitmap(128, 128);
            Graphics g = Graphics.FromImage(img);
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.Clear(Color.FromArgb(30, 30, 30));

            Rectangle rectangleH4V3 = new Rectangle(3, 9, 122, 92);

            if (Background == null)
                g.FillRectangle(new SolidBrush(Color.Black), rectangleH4V3);
            else
                g.DrawImage(Background, rectangleH4V3);

            if (Preview != null)
            {
                double scale = (double)rectangleH4V3.Width / Preview.Width;
                double heightScale = (double)rectangleH4V3.Height / Preview.Height;
                if (scale > heightScale)
                    scale = heightScale;
                int previewWidth = (int)(Preview.Width * scale);
                int previewHeight = (int)(Preview.Height * scale);
                int previewX = (int)((rectangleH4V3.Width - previewWidth) / 2.0) + rectangleH4V3.X;
                int previewY = (int)((rectangleH4V3.Height - previewHeight) / 2.0) + rectangleH4V3.Y;

                g.DrawImage(Preview, previewX, previewY, previewWidth, previewHeight);
            }

            if (Frame == null)
            {
                GraphicsPath vc = new GraphicsPath();
                Font font = new Font("Arial", 10.0F, FontStyle.Regular, GraphicsUnit.Point);
                StringFormat format = new StringFormat();
                format.Alignment = StringAlignment.Center;
                format.LineAlignment = StringAlignment.Center;

                vc.AddString("Multimedia", font.FontFamily,
                (int)(FontStyle.Bold | FontStyle.Italic),
                g.DpiY * 9.2F / 72.0F, new Rectangle(0, 101, 128, 27), format);
                g.DrawPath(new Pen(Color.Black, 2.0F), vc);
                g.FillPath(new SolidBrush(Color.FromArgb(147, 149, 152)), vc);
            }
            else
                g.DrawImage(Frame, new Rectangle(0, 0, 128, 128));

            return img;
        }
    }
}

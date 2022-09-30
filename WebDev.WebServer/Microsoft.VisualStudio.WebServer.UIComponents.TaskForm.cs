namespace Microsoft.VisualStudio.WebServer.UIComponents
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    public class TaskForm : MxForm
    {
        private BorderStyle _taskBorderStyle;
        private string _taskCaption;
        private string _taskDescription;
        private Image _taskGlyph;

        public TaskForm()
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        public TaskForm(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            base.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this._taskCaption != null)
            {
                Graphics dc = e.Graphics;
                Rectangle clientRectangle = base.ClientRectangle;
                Rectangle rect = new Rectangle(0, 0, clientRectangle.Width, 0x38);
                dc.FillRectangle(Brushes.White, rect);
                if (this._taskBorderStyle != BorderStyle.None)
                {
                    dc.DrawLine(SystemPens.ControlDark, 0, 0x39, rect.Width, 0x39);
                    dc.DrawLine(SystemPens.ControlLightLight, 0, 0x3a, rect.Width, 0x3a);
                }
                Font font = this.Font;
                Font font2 = new Font(font.FontFamily, font.SizeInPoints + 1f, FontStyle.Bold);
                if (this.RightToLeftLayout)
                {
                    TextRenderer.DrawText(dc, this._taskCaption, font2, new Point(8, 10), Color.Black);
                    if ((this._taskDescription != null) && (this._taskDescription.Length != 0))
                    {
                        TextRenderer.DrawText(dc, this._taskDescription, font, new Point(0x10, 14 + ((int) font2.GetHeight(dc))), Color.Black);
                    }
                }
                else
                {
                    dc.DrawString(this._taskCaption, font2, Brushes.Black, (float) 8f, (float) 10f);
                    if ((this._taskDescription != null) && (this._taskDescription.Length != 0))
                    {
                        dc.DrawString(this._taskDescription, font, Brushes.Black, (float) 16f, (float) (14f + font2.GetHeight(dc)));
                    }
                }
                if (this._taskGlyph != null)
                {
                    dc.DrawImage(this._taskGlyph, (rect.Width - 0x30) - 6, 4, 0x30, 0x30);
                }
                else
                {
                    dc.FillRectangle(Brushes.Beige, (rect.Width - 0x30) - 6, 4, 0x30, 0x30);
                    dc.DrawRectangle(Pens.Tan, (rect.Width - 0x30) - 6, 4, 0x30, 0x30);
                }
            }
        }

        [DefaultValue(0), Category("Appearance")]
        public BorderStyle TaskBorderStyle
        {
            get
            {
                return this._taskBorderStyle;
            }
            set
            {
                if ((value != BorderStyle.None) && (value != BorderStyle.FixedSingle))
                {
                    throw new ArgumentOutOfRangeException();
                }
                this._taskBorderStyle = value;
                if (base.IsHandleCreated)
                {
                    base.Invalidate();
                }
            }
        }

        [Category("Appearance"), DefaultValue("")]
        public string TaskCaption
        {
            get
            {
                if (this._taskCaption == null)
                {
                    return string.Empty;
                }
                return this._taskCaption;
            }
            set
            {
                this._taskCaption = value;
                if (base.IsHandleCreated)
                {
                    base.Invalidate();
                }
            }
        }

        [DefaultValue(""), Category("Appearance")]
        public string TaskDescription
        {
            get
            {
                if (this._taskDescription == null)
                {
                    return string.Empty;
                }
                return this._taskDescription;
            }
            set
            {
                this._taskDescription = value;
                if (base.IsHandleCreated)
                {
                    base.Invalidate();
                }
            }
        }

        [Category("Appearance"), DefaultValue((string) null)]
        public Image TaskGlyph
        {
            get
            {
                return this._taskGlyph;
            }
            set
            {
                this._taskGlyph = value;
                if (base.IsHandleCreated)
                {
                    base.Invalidate();
                }
            }
        }
    }
}

using log4net.Appender;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace usbip_tunnel
{
    public class RichTextBoxAppender : AppenderSkeleton
    {
        private RichTextBox _textBox;
        public RichTextBox AppenderTextBox { get { return _textBox; } set { _textBox = value; } }
        public string FormName { get; set; }
        public string TextBoxName { get; set; }

        private Control FindControlRecursive(Control root, string textBoxName)
        {
            if (root.Name == textBoxName) return root;
            foreach (Control c in root.Controls)
            {
                var t = FindControlRecursive(c, textBoxName);
                if (t != null) return t;
            }
            return null;
        }

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            if (_textBox == null)
            {
                if (string.IsNullOrEmpty(FormName) || string.IsNullOrEmpty(TextBoxName)) return;

                var form = Application.OpenForms[FormName];
                if (form == null) return;

                _textBox = (RichTextBox)FindControlRecursive(form, TextBoxName);
                if (_textBox == null) return;

                form.FormClosing += (s, e) => _textBox = null;
            }
            _textBox.BeginInvoke((MethodInvoker)delegate
            {
                if (loggingEvent.Level == Level.Debug)
                {
                    _textBox.SelectionStart = _textBox.TextLength;
                    _textBox.SelectionLength = 0;
                    _textBox.SelectionColor = Color.RoyalBlue;
                    _textBox.AppendText(RenderLoggingEvent(loggingEvent));
                    _textBox.SelectionColor = _textBox.ForeColor;
                }
                else if (loggingEvent.Level == Level.Info)
                {
                    _textBox.SelectionStart = _textBox.TextLength;
                    _textBox.SelectionLength = 0;
                    _textBox.SelectionColor = Color.ForestGreen;
                    _textBox.AppendText(RenderLoggingEvent(loggingEvent));
                    _textBox.SelectionColor = _textBox.ForeColor;
                }
                else if (loggingEvent.Level == Level.Warn)
                {
                    _textBox.SelectionStart = _textBox.TextLength;
                    _textBox.SelectionLength = 0;
                    _textBox.SelectionColor = Color.DarkOrange;
                    _textBox.AppendText(RenderLoggingEvent(loggingEvent));
                    _textBox.SelectionColor = _textBox.ForeColor;
                }
                else if (loggingEvent.Level == Level.Error)
                {
                    _textBox.SelectionStart = _textBox.TextLength;
                    _textBox.SelectionLength = 0;
                    _textBox.SelectionColor = Color.DarkRed;
                    _textBox.AppendText(RenderLoggingEvent(loggingEvent));
                    _textBox.SelectionColor = _textBox.ForeColor;
                }
                else if (loggingEvent.Level == Level.Fatal)
                {
                    _textBox.SelectionStart = _textBox.TextLength;
                    _textBox.SelectionLength = 0;
                    _textBox.SelectionColor = Color.Crimson;
                    _textBox.AppendText(RenderLoggingEvent(loggingEvent));
                    _textBox.SelectionColor = _textBox.ForeColor;
                }
                else
                {
                    _textBox.AppendText(RenderLoggingEvent(loggingEvent));
                }
            });
        }
    }
}

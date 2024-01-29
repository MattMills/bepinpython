using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Scripting.Hosting.Shell;

namespace bepinpython
{
    public class GuiHost : IConsole
    {
        private Form form;
        private TextBox inputBox;
        private TextBox outputBox;

        public GuiHost()
        {
            form = new Form();
            inputBox = new TextBox();
            outputBox = new TextBox();

            // Configure the form and the text boxes here...

            form.Controls.Add(inputBox);
            form.Controls.Add(outputBox);
            form.Show();
        }

        public string ReadLine(int autoIndentSize)
        {
            // Wait for the user to press enter, then return the text in the input box
            // This is a simplification, you would need to add event handlers to do this properly
            return inputBox.Text;
        }

        public void Write(string text, Style style)
        {
            // Add the text to the output box
            outputBox.Text += text;
        }

        public void WriteLine(string text, Style style)
        {
            // Add the text to the output box, followed by a newline
            outputBox.Text += text + Environment.NewLine;
        }

        public void WriteLine()
        {
            // Add a newline to the output box
            outputBox.Text += Environment.NewLine;
        }

        public TextWriter Output
        {
            get { return new StringWriter(); }
            set { throw new NotSupportedException("Setting the Output property is not supported."); }
        }

        public TextWriter ErrorOutput
        {
            get { return new StringWriter(); }
            set { throw new NotSupportedException("Setting the ErrorOutput property is not supported."); }
        }
    }
}
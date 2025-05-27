using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RevitAgent.LLM
{
    public partial class DialogueForm : Form
    {
        public string InputText;

        public DialogueForm()
        {
            InitializeComponent();
        }

        public void AddTextToDisplay(string text, string role)
        {
            DisplayTextBox.AppendText($"Role: <{role}>" + Environment.NewLine);
            DisplayTextBox.AppendText(text);
            string separator = Environment.NewLine + "----------------------------------------------------------------" + Environment.NewLine;
            DisplayTextBox.AppendText(separator);
        }

        private void ClearInputTextBox()
        {
            InputTextBox.Clear();
        }

        private void ClearDisplayTextBox()
        {
            DisplayTextBox.Clear();
        }

        private void EnterButton_Click(object sender, EventArgs e)
        {
            InputText = InputTextBox.Text;
            ClearInputTextBox();
        }

        private void ClearTextButton_Click(object sender, EventArgs e)
        {
            ClearInputTextBox();
        }

        private void ClearDisplayButton_Click(object sender, EventArgs e)
        {
            ClearDisplayTextBox();
        }        
    }
}

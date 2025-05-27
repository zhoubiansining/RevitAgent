using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace RevitAgent.LLM
{
    public class DialogueUtils
    {
        private string InputFilePath;
        private DialogueForm DialogueForm;
        bool UseForm;

        public DialogueUtils(string file_path)
        {
            InputFilePath = file_path;
            UseForm = false;
        }

        public DialogueUtils()
        {
            DialogueForm = new DialogueForm();
            UseForm = true;
        }

        public void AddTextToDisplay(string text, string role)
        {
            if (UseForm)
            {
                DialogueForm.AddTextToDisplay(text, role);
            }
        }

        public string GetUserInput()
        {
            if (!UseForm)
            {
                string input = string.Empty;
                int timeout = 180; // 180 seconds
                while (string.IsNullOrEmpty(input) && timeout > 0)
                {
                    if (File.Exists(InputFilePath))
                    {
                        input = File.ReadAllText(InputFilePath).Trim();
                    }
                    if (string.IsNullOrEmpty(input))
                    {
                        Thread.Sleep(1000); // wait for 1 second
                        timeout--;
                    }
                }
                if (string.IsNullOrEmpty(input))
                    return null;
                else
                    return input;
            }
            else
            {
                DialogResult result = DialogueForm.ShowDialog();
                if (result != DialogResult.OK)
                    return null;
                string input = DialogueForm.InputText;
                return input;
            }
        }

        public string GetUserInput(string prompt)
        {
            if (!UseForm)
            {
                string input = string.Empty;
                int timeout = 180; // 180 seconds
                while (string.IsNullOrEmpty(input) && timeout > 0)
                {
                    if (File.Exists(InputFilePath))
                    {
                        input = File.ReadAllText(InputFilePath).Trim();
                    }
                    if (string.IsNullOrEmpty(input))
                    {
                        Thread.Sleep(1000); // wait for 1 second
                        timeout--;
                    }
                }
                if (string.IsNullOrEmpty(input))
                    return null;
                else
                    return input;
            }
            else
            {
                AddTextToDisplay(prompt, "system");
                DialogResult result = DialogueForm.ShowDialog();
                if (result != DialogResult.OK)
                    return null;
                string input = DialogueForm.InputText;
                return input;
            }
        }

        public void DisplayForm()
        {
            if (UseForm)
            {
                DialogueForm.ShowDialog();
            }
        }

        public void Dispose()
        {            
            if (UseForm)
            {
                if (DialogueForm != null)
                {
                    DialogueForm.Dispose();
                    DialogueForm = null;
                }
            }
            else
            {
                InputFilePath = null;
            }
        }
    }
}

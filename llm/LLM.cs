using System;
using System.ClientModel;
using OpenAI;
using OpenAI.Chat;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RevitAgent.LLM
{
    internal class LLMConfig
    {
        internal string API_KEY = "your-api-key";
        internal string API_BASE = "your-api-base";
        internal string MODEL_NAME = "your-model-name";
    }
    public class LLM
    {
        private string API_BASE;
        private ApiKeyCredential API_KEY;
        private string MODEL_NAME;
        public OpenAIClient Client;
        public ChatClient ChatClient;
        public LLM(string api_key, string api_base, string model_name)
        {
            API_KEY = new ApiKeyCredential(api_key);
            API_BASE = api_base;
            MODEL_NAME = model_name;
            var clientOptions = new OpenAIClientOptions
            {
                Endpoint = new Uri(API_BASE),
            };
            Client = new OpenAIClient(credential: API_KEY, options: clientOptions);
            ChatClient = Client.GetChatClient(MODEL_NAME);
        }
        public LLM()
        {
            LLMConfig config = new LLMConfig();
            API_KEY = new ApiKeyCredential(config.API_KEY);
            API_BASE = config.API_BASE;
            MODEL_NAME = config.MODEL_NAME;
            var clientOptions = new OpenAIClientOptions
            {
                Endpoint = new Uri(API_BASE),
            };
            Client = new OpenAIClient(credential: API_KEY, options: clientOptions);
            ChatClient = Client.GetChatClient(MODEL_NAME);
        }
        public string Chat(string prompt, bool showLoading, string text)
        {
            if (showLoading)
            {
                using (var loadingForm = new LLMLoadingForm(text))
                {
                    loadingForm.Show();
                    loadingForm.Refresh();
                    Application.DoEvents();

                    string result = null;
                    var thread = new System.Threading.Thread(() =>
                    {
                        ChatCompletion completion = ChatClient.CompleteChat(prompt);
                        result = completion.Content[0].Text;
                    });
                    thread.Start();
                    while (thread.IsAlive)
                    {
                        Application.DoEvents();
                    }

                    loadingForm.Close();
                    return result;
                }
            }
            else
            {
                ChatCompletion completion = ChatClient.CompleteChat(prompt);
                return completion.Content[0].Text;
            }
        }
        public string Chat(List<ChatMessage> messages, bool showLoading, string text) 
        {
            if (showLoading)
            {
                using (var loadingForm = new LLMLoadingForm(text))
                {
                    loadingForm.Show();
                    loadingForm.Refresh();
                    Application.DoEvents();

                    string result = null;
                    var thread = new System.Threading.Thread(() =>
                    {
                        ChatCompletion completion = ChatClient.CompleteChat(messages);
                        result = completion.Content[0].Text;
                    });
                    thread.Start();
                    while (thread.IsAlive)
                    {
                        Application.DoEvents();
                    }

                    loadingForm.Close();
                    return result;
                }
            }
            else
            {
                ChatCompletion completion = ChatClient.CompleteChat(messages);
                return completion.Content[0].Text;
            }
        }
    }
    public class LLMLoadingForm : Form
    {
        private Label label;
        private Timer timer;
        private int dotCount = 0;
        private string baseText;

        public LLMLoadingForm(string text)
        {
            baseText = text;
            label = new Label()
            {
                Text = baseText,
                AutoSize = true,
                Font = new Font("Arial", 14, FontStyle.Bold)
            };
            Controls.Add(label);
            Text = "Notice";
            Size = new Size(1000, 400);
            StartPosition = FormStartPosition.CenterScreen;

            CenterLabel();

            timer = new Timer();
            timer.Interval = 500;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            dotCount = (dotCount + 1) % 5;
            label.Text = baseText + new string('.', dotCount);
        }

        private void CenterLabel()
        {
            label.Location = new Point((ClientSize.Width - label.Width) / 2, (ClientSize.Height - label.Height) / 2);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            base.OnFormClosed(e);
        }
    }
}

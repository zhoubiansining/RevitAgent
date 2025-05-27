using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using OpenAI.Chat;

namespace RevitAgent.LLM
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.UsingCommandData)]
    public class ChatWithLLM : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, Autodesk.Revit.DB.ElementSet elements)
        {
            try
            {
                LLMConfig config = new LLMConfig();
                LLM llm = new LLM(config.API_KEY, config.API_BASE, config.MODEL_NAME);
                TaskDialog.Show("Configuration", $"Using llm config: API_KEY={config.API_KEY},API_BASE={config.API_BASE},MODEL_NAME={config.MODEL_NAME}.");

                // Prompt user for input
                TaskDialog mainDialog = new TaskDialog("Chat");
                mainDialog.MainInstruction = "Please enter text to chat with the llm.";
                mainDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
                mainDialog.DefaultButton = TaskDialogResult.Ok;

                TaskDialogResult result = mainDialog.Show();
                if (result == TaskDialogResult.Ok)
                {
                    DialogueUtils dialogue = new DialogueUtils();
                    List<string> userMessages = new List<string>();
                    List<string> llmMessages = new List<string>();

                    try
                    {
                        while (true)
                        {
                            string userInput = dialogue.GetUserInput();
                            if (string.IsNullOrEmpty(userInput))
                            {
                                TaskDialog.Show("Notice", "Session ends since user input is empty or the operation has been cancelled.");
                                dialogue.Dispose();
                                return Result.Succeeded;
                            }
                            dialogue.AddTextToDisplay(userInput, "user");
                            userMessages.Add(userInput);
                            FormatMessages(userMessages, llmMessages, out List<ChatMessage> chatMessages);
                            string llmOutput = llm.Chat(chatMessages, true, "Please wait for the LLM to respond");
                            llmMessages.Add(llmOutput);
                            dialogue.AddTextToDisplay(llmOutput, "llm");
                        }                    
                    }
                    catch (Exception ex)
                    {
                        message = "Exception occurred when chatting with llm:\n" + ex.Message;
                        dialogue.Dispose();
                        return Result.Failed;
                    }
                }
                else
                {
                    TaskDialog.Show("Notice", "User cancelled the operation.");
                    return Result.Cancelled;
                }
            }
            catch (Exception ex)
            {
                message = "Exception occurred when preparing for chatting or cleaning up:\n" + ex.Message;
                return Result.Failed;
            }
        }

        private void FormatMessages(List<string> userMessages, List<string> llmMessages, out List<ChatMessage> chatMessages)
        {
            if (userMessages.Count != llmMessages.Count + 1)
            {
                throw new Exception("Invalid length of user messages or llm messages.");
            }
            chatMessages = new List<ChatMessage>();
            chatMessages.Add(new SystemChatMessage("You are a helpful assistant."));
            for (int i = 0; i < llmMessages.Count; i++)
            {
                chatMessages.Add(new UserChatMessage(userMessages[i]));
                chatMessages.Add(new AssistantChatMessage(llmMessages[i]));
            }
            chatMessages.Add(new UserChatMessage(userMessages[userMessages.Count - 1]));
        }
    }
}

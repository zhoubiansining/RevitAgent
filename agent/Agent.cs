using System;
using System.Collections.Generic;
using RevitAgent.Interpreter;
using RevitAgent.LLM;
using OpenAI.Chat;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Diagnostics;

namespace RevitAgent.Agent
{
    public class Agent
    {
        private int RetryTol;
        private bool UseChinese;
        private Interpreter.Interpreter Interpreter;
        private LLM.LLM LLM;
        private DialogueUtils DialogueUtils;
        private List<ChatMessage> ChatHistory;
        public Agent(bool useChinese, int retryTol)
        {
            RetryTol = retryTol;
            UseChinese = useChinese;
            Interpreter = new Interpreter.Interpreter(useChinese);
            LLM = new LLM.LLM();
            DialogueUtils = new DialogueUtils();
            ChatHistory = new List<ChatMessage>();
        }
        public void AddChatHistory(string role, string content)
        {
            switch (role)
            {
                case "system":
                    {
                        ChatHistory.Add(new SystemChatMessage(content));
                        break;
                    }
                case "user":
                    {
                        ChatHistory.Add(new UserChatMessage(content));
                        break;
                    }
                case "assistant":
                    {
                        ChatHistory.Add(new AssistantChatMessage(content));
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException("Unsupported chat role.");
                    }
            }
        }
        public void RemoveLastHistory()
        {
            if (ChatHistory.Count > 0)
            {
                ChatHistory.RemoveAt(ChatHistory.Count - 1);
            }
        }
        public Result StartSession(ref string message, bool debug)
        {
            try
            {               
                // Start the session
                StartPrompt startPrompt = new StartPrompt(UseChinese);               
                startPrompt.GetComponents(out Dictionary<string, string> promptComponents);
                string prompt = promptComponents["Instruction"];               
                AddChatHistory("system", prompt);
                if (debug)
                {
                    DialogueUtils.AddTextToDisplay(prompt, "system");
                }

                // Get the user's task input
                string inputTaskPrompt = UseChinese ? "请您描述需要完成的任务，并尽可能清晰地提供相关信息。" : "Please describe what needs to be accomplished and provide the information as clearly as possible.";
                string userPrompt = DialogueUtils.GetUserInput(inputTaskPrompt);
                if (userPrompt == null)
                {
                    message = "Failed to get user's task input.";
                    return Result.Failed;
                }
                string composedPrompt = "";
                if (UseChinese)
                {
                    composedPrompt = $"你的任务是: {userPrompt}\n\n" + promptComponents["RevitInfo"];
                }
                else
                {
                    composedPrompt = $"Your task is: {userPrompt}\n\n" + promptComponents["RevitInfo"];
                }
                AddChatHistory("user", composedPrompt);
                if (debug)
                {
                    DialogueUtils.AddTextToDisplay(composedPrompt, "user");
                }
                else
                {
                    DialogueUtils.AddTextToDisplay(userPrompt, "user");
                }

                string assistantResponse = UseChinese ? "好的，请问我可以调用哪些接口呢?" : "Alright, what interfaces can I call?";
                AddChatHistory("assistant", assistantResponse);
                if (debug)
                {
                    DialogueUtils.AddTextToDisplay(assistantResponse, "assistant");
                }

                string guideInstruction = UseChinese ? "下面，请你在我的引导下逐步地完成任务，给出合理的回复。" : "Now, please follow my guidance to complete the task step by step and give reasonable responses.";
                composedPrompt = promptComponents["CallInfo"] + '\n' + guideInstruction;
                AddChatHistory("user", composedPrompt);
                if (debug)
                {
                    DialogueUtils.AddTextToDisplay(composedPrompt, "user");
                }

                assistantResponse = UseChinese ? "好的，让我们开始吧。" : "Alright, let's get started.";
                AddChatHistory("assistant", assistantResponse);
                if (debug)
                {
                    DialogueUtils.AddTextToDisplay(assistantResponse, "assistant");
                }
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = "Exception occurred when starting session:\n" + e.Message;
                return Result.Failed;
            }
        }
        public Result StartTest(ref string message, string userPrompt)
        {
            try
            {
                // Start a test
                StartPrompt startPrompt = new StartPrompt(UseChinese);
                startPrompt.GetComponents(out Dictionary<string, string> promptComponents);
                string prompt = promptComponents["Instruction"];
                AddChatHistory("system", prompt);
                DialogueUtils.AddTextToDisplay(prompt, "system");

                // Compose the test prompt
                string composedPrompt = "";
                if (UseChinese)
                {
                    composedPrompt = $"你的任务是: {userPrompt}\n\n" + promptComponents["RevitInfo"];
                }
                else
                {
                    composedPrompt = $"Your task is: {userPrompt}\n\n" + promptComponents["RevitInfo"];
                }
                AddChatHistory("user", composedPrompt);
                DialogueUtils.AddTextToDisplay(composedPrompt, "user");

                string assistantResponse = UseChinese ? "好的，请问我可以调用哪些接口呢?" : "Alright, what interfaces can I call?";
                AddChatHistory("assistant", assistantResponse);
                DialogueUtils.AddTextToDisplay(assistantResponse, "assistant");

                string guideInstruction = UseChinese ? "下面，请你在我的引导下逐步地完成任务，给出合理的回复。" : "Now, please follow my guidance to complete the task step by step and give reasonable responses.";
                composedPrompt = promptComponents["CallInfo"] + '\n' + guideInstruction;
                AddChatHistory("user", composedPrompt);
                DialogueUtils.AddTextToDisplay(composedPrompt, "user");

                assistantResponse = UseChinese ? "好的，让我们开始吧。" : "Alright, let's get started.";
                AddChatHistory("assistant", assistantResponse);
                DialogueUtils.AddTextToDisplay(assistantResponse, "assistant");               
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = "Exception occurred when starting test:\n" + e.Message;
                return Result.Failed;
            }
        }
        public Result ExecuteRound(ExternalCommandData commandData, ref string message, ElementSet elements, bool debug, out bool shouldEnd)
        {            
            try
            {
                // Prompt the assistant to think
                ThinkPrompt thinkPrompt = new ThinkPrompt(UseChinese);
                thinkPrompt.GetComponents(out Dictionary<string, string> thinkPromptComponents);
                string prompt = thinkPromptComponents["All"];
                AddChatHistory("user", prompt);                
                if (debug)
                {
                    DialogueUtils.AddTextToDisplay(prompt, "user");
                    DialogueUtils.DisplayForm();
                }
                string waitThinkPrompt = UseChinese ? "请等待智能体思考" : "Please wait for the agent to think";
                string thinkResponse = LLM.Chat(ChatHistory, true, waitThinkPrompt);
                RemoveLastHistory();
                AddChatHistory("user", thinkPromptComponents["Instruction"]);
                AddChatHistory("assistant", thinkResponse);                
                if (debug)
                {
                    DialogueUtils.AddTextToDisplay(thinkResponse, "assistant");
                    DialogueUtils.DisplayForm();
                }
            }
            catch (Exception e)
            {
                message = "Exception occurred when executing think round:\n" + e.Message;
                shouldEnd = true;
                return Result.Failed;
            }

            try
            {
                // Prompt the assistant to act
                ActPrompt actPrompt = new ActPrompt(UseChinese);
                actPrompt.GetComponents(out Dictionary<string, string> actPromptComponents);
                string prompt = actPromptComponents["All"];
                AddChatHistory("user", prompt);               
                if (debug)
                {
                    DialogueUtils.AddTextToDisplay(prompt, "user");
                    DialogueUtils.DisplayForm();
                }
                Response response = null;
                string actResponse = null;
                int tol = RetryTol;
                while (response == null && tol > 0)
                {
                    string waitActPrompt = UseChinese ? "请等待智能体执行操作" : "Please wait for the agent to take action";
                    actResponse = LLM.Chat(ChatHistory, true, waitActPrompt);
                    response = Interpreter.Parse(actResponse);                    
                    if (debug)
                    {
                        DialogueUtils.AddTextToDisplay(actResponse, "assistant");
                        DialogueUtils.DisplayForm();
                    }
                    tol--;
                }
                if (response == null)
                {
                    message = "Failed to parse act response for too many times.";
                    shouldEnd = true;
                    return Result.Failed;
                }
                RemoveLastHistory();
                AddChatHistory("user", actPromptComponents["InstructionBrief"]);
                AddChatHistory("assistant", actResponse);
               
                if (response is ResponseToUser responseToUser)
                {
                    shouldEnd = responseToUser.End;
                    if (!shouldEnd)
                    {                       
                        string messageToUser = responseToUser.Message;
                        DialogueUtils.AddTextToDisplay(messageToUser, "assistant");
                        string inputResponsePrompt = UseChinese ? "请对智能体进行回复，以帮助其完成任务。" : "Please respond to the agent to help it accomplish the task.";
                        string userResponse = DialogueUtils.GetUserInput(inputResponsePrompt);
                        if (userResponse == null)
                        {
                            message = "Failed to get user's response.";
                            shouldEnd = true; 
                            return Result.Failed;
                        }
                        AddChatHistory("user", userResponse);
                        DialogueUtils.AddTextToDisplay(userResponse, "user");
                    }
                    else
                    {
                        string messageToUser = responseToUser.Message;
                        DialogueUtils.AddTextToDisplay(messageToUser, "assistant");
                        string endConversationPrompt = UseChinese ? "提示: 任务已完成。" : "Hint: The task has been completed.";
                        DialogueUtils.AddTextToDisplay(endConversationPrompt, "system");
                        DialogueUtils.DisplayForm();                 
                    }
                    return Result.Succeeded;
                }
                else if (response is ResponseToSystem responseToSystem)
                {                   
                    // Execute
                    Result result = Interpreter.ExecuteCall(commandData, ref message, elements, responseToSystem, false);
                    string systemResponse = Interpreter.FormSystemResponseMessage(result, message, UseChinese);
                    AddChatHistory("user", systemResponse);                   
                    if (debug)
                    {
                        DialogueUtils.AddTextToDisplay(systemResponse, "user");
                        DialogueUtils.DisplayForm();
                    }
                    shouldEnd = false;
                    return Result.Succeeded;
                }
                else
                {
                    message = "Invalid act response received.";
                    shouldEnd = true;
                    return Result.Failed;
                }
            }
            catch (Exception e)
            {
                message = "Exception occurred when executing act round:\n" + e.Message;
                shouldEnd = true;
                return Result.Failed;
            }
        }
        public Result ExecuteTestRound(ExternalCommandData commandData, ref string message, ElementSet elements, out bool shouldEnd, out string actResponse, out string systemResponse)
        {
            try
            {
                // Prompt the assistant to think
                ThinkPrompt thinkPrompt = new ThinkPrompt(UseChinese);
                thinkPrompt.GetComponents(out Dictionary<string, string> thinkPromptComponents);
                string prompt = thinkPromptComponents["All"];
                AddChatHistory("user", prompt);
                DialogueUtils.AddTextToDisplay(prompt, "user");
                string thinkResponse = LLM.Chat(ChatHistory, false, null);
                RemoveLastHistory();
                AddChatHistory("user", thinkPromptComponents["Instruction"]);
                AddChatHistory("assistant", thinkResponse);
                DialogueUtils.AddTextToDisplay(thinkResponse, "assistant");
            }
            catch (Exception e)
            {
                message = "Exception occurred when executing think round:\n" + e.Message;
                shouldEnd = true;
                actResponse = null;
                systemResponse = null;
                return Result.Failed;
            }

            try
            {
                // Prompt the assistant to act
                ActPrompt actPrompt = new ActPrompt(UseChinese);
                actPrompt.GetComponents(out Dictionary<string, string> actPromptComponents);
                string prompt = actPromptComponents["All"];
                AddChatHistory("user", prompt);
                DialogueUtils.AddTextToDisplay(prompt, "user");
                Response response = null;
                string rawActResponse = null;
                actResponse = null;
                int tol = RetryTol;
                while (response == null && tol > 0)
                {
                    rawActResponse = LLM.Chat(ChatHistory, false, null);
                    response = Interpreter.Parse(rawActResponse, out actResponse);
                    DialogueUtils.AddTextToDisplay(rawActResponse, "assistant");
                    tol--;
                }
                if (response == null)
                {
                    message = "Failed to parse act response for too many times.";
                    shouldEnd = true;
                    actResponse = null;
                    systemResponse = null;
                    return Result.Failed;
                }
                RemoveLastHistory();
                AddChatHistory("user", actPromptComponents["InstructionBrief"]);
                AddChatHistory("assistant", rawActResponse);

                if (response is ResponseToUser responseToUser)
                {
                    shouldEnd = true;
                    if (!responseToUser.End)
                    {
                        // Asking user is not allowed in test mode
                        message = "Asking user is not allowed in test mode.";
                        systemResponse = null;
                        return Result.Failed;
                    }
                    else
                    {
                        // Test ends
                        systemResponse = null;
                        return Result.Succeeded;
                    }
                }
                else if (response is ResponseToSystem responseToSystem)
                {
                    // Execute
                    Result result = Interpreter.ExecuteCall(commandData, ref message, elements, responseToSystem, true);
                    systemResponse = Interpreter.FormSystemResponseMessage(result, message, UseChinese);
                    AddChatHistory("user", systemResponse);
                    DialogueUtils.AddTextToDisplay(systemResponse, "user");
                    shouldEnd = false;
                    return Result.Succeeded;
                }
                else
                {
                    message = "Invalid act response received.";
                    shouldEnd = true;
                    actResponse = null;
                    systemResponse = null;
                    return Result.Failed;
                }
            }
            catch (Exception e)
            {
                message = "Exception occurred when executing act round:\n" + e.Message;
                shouldEnd = true;
                actResponse = null;
                systemResponse = null;
                return Result.Failed;
            }
        }
        public Result EndSession(ref string message, out bool acceptChanges)
        {
            try
            {
                DialogueUtils.Dispose();
                string endMessage = "The required task has been completed.\n--Please confirm the changes made to the project.\n--If you reject, all changes will be cancelled.";
                TaskDialog mainDialog = new TaskDialog("Confirm Changes");
                mainDialog.MainInstruction = endMessage;
                mainDialog.CommonButtons = TaskDialogCommonButtons.Ok | TaskDialogCommonButtons.Cancel;
                mainDialog.DefaultButton = TaskDialogResult.Cancel;

                TaskDialogResult result = mainDialog.Show();
                if (result == TaskDialogResult.Cancel)
                {
                    acceptChanges = false;
                }
                else
                {
                    acceptChanges = true;
                }
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = "Exception occurred when ending session:\n" + e.Message;
                acceptChanges = false;
                return Result.Failed;
            }
        }
        public Result EndSession(ref string message)
        {
            try
            {
                DialogueUtils.Dispose();
                TaskDialog.Show("Notice", "The committed changes will be cancelled by default.");
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = "Exception occurred when ending session:\n" + e.Message;
                return Result.Failed;
            }
        }
        public Result EndTest(ref string message)
        {
            try
            {
                DialogueUtils.Dispose();
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                message = "Exception occurred when ending test:\n" + e.Message;
                return Result.Failed;
            }
        }
    }
}

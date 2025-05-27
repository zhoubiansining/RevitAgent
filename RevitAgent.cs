using System;
using Autodesk.Revit.UI;
using RevitFunctions;

namespace RevitAgent
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.UsingCommandData)]
    public class AgentApplication : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel ribbonPanel = application.CreateRibbonPanel("Revit Agent");
            PushButton pushButton1 = ribbonPanel.AddItem(new PushButtonData("Hello",
                "Hello from Revit",
                @"C:\ProgramData\Autodesk\Revit\Addins\2024\RevitAgent.dll",
                "RevitAgent.Hello")) as PushButton;
            PushButton pushButton2 = ribbonPanel.AddItem(new PushButtonData("Chat",
                "Chat with a LLM",
                @"C:\ProgramData\Autodesk\Revit\Addins\2024\RevitAgent.dll",
                "RevitAgent.ChatWithLLM")) as PushButton;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}

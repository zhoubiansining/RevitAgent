using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAgent
{
    /// <summary>
    /// Start a new session with the agent.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RunAgent : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            // Select language
            TaskDialog langDialog = new TaskDialog("Select Language");
            langDialog.MainInstruction = "Please select a language for the new session:\n--Would you prefer to use Chinese over English (default)? ";
            langDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;            
            bool useChinese = langDialog.Show() == TaskDialogResult.Yes;

            // Debug
            TaskDialog debugDialog = new TaskDialog("Debug");
            debugDialog.MainInstruction = "Do you need to turn on debug mode? ";
            debugDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
            bool debug = debugDialog.Show() == TaskDialogResult.Yes;

            // Start a new session
            int retry = 3;
            Agent.Agent agent = new Agent.Agent(useChinese, retry);
            Result result = agent.StartSession(ref message, debug);
            if (result != Result.Succeeded)
            {
                string endMessage = "";
                agent.EndSession(ref endMessage);
                return Result.Failed;
            }

            // Start a transaction group
            TransactionGroup transactionGroup = new TransactionGroup(commandData.Application.ActiveUIDocument.Document);
            transactionGroup.Start("Agent Operations");

            // Execute conversation rounds
            int maxRounds = 30;
            while (maxRounds > 0)
            {
                if (debug)
                {
                    TaskDialog continueDialog = new TaskDialog("Round Continue Confimation");
                    continueDialog.MainInstruction = "Do you want to continue the next conversation round? ";
                    continueDialog.CommonButtons = TaskDialogCommonButtons.Yes | TaskDialogCommonButtons.No;
                    if (continueDialog.Show() == TaskDialogResult.No)
                    {
                        break;
                    }
                }

                string roundMessage = "";
                result = agent.ExecuteRound(commandData, ref roundMessage, elements, debug, out bool shouldEnd);
                if (result != Result.Succeeded)
                {
                    message = roundMessage;
                    transactionGroup.RollBack();
                    string endMessage = "";
                    agent.EndSession(ref endMessage);
                    return Result.Failed;
                }
                if (shouldEnd)
                {
                    break;
                }
                maxRounds--;
            }
            if (maxRounds == 0)
            {
                message = "The task conversation has reached the maximum rounds.";
                transactionGroup.RollBack();
                string endMessage = "";
                agent.EndSession(ref endMessage);
                return Result.Failed;
            }

            // End the session
            result = agent.EndSession(ref message, out bool acceptChanges);
            if (result != Result.Succeeded)
            {
                transactionGroup.RollBack();
                return Result.Failed;
            }
            if (!acceptChanges)
            {
                transactionGroup.RollBack();
                return Result.Cancelled;
            }
            else
            {
                transactionGroup.Assimilate();
                return Result.Succeeded;
            }            
        }
    }
}

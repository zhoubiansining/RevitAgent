using System.Collections;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFunctions
{
    /// <summary>
    /// Delete the designated elements
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class DeleteObject : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new System.NotImplementedException("Default object deletion is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementSet elementsToDelete, out Element elementFirstFailed, bool testModeOn)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            Transaction trans = new Transaction(document, "DeleteObject");
            elementFirstFailed = null;
            trans.Start();
            
            // check user selection
            if (elementsToDelete.Size < 1)
            {
                message = "At least one object should be designated before delete.";
                trans.RollBack();
                return Result.Cancelled;
            }

            try
            {
                // delete elements
                if (!testModeOn)
                {
                    TaskDialog.Show("Delete Warning", $"Please notice that some objects are about to be selected and deleted.\n");
                }
                IEnumerator e = elementsToDelete.GetEnumerator();
                bool MoreValue = e.MoveNext();
                while (MoreValue)
                {
                    Element component = e.Current as Element;
                    elementFirstFailed = component;
                    ElementId id = component.Id;
                    if (!testModeOn)
                    {
                        string showMessage = "";
                        Result result = ShowElements.Execute(commandData, ref showMessage, elements, id);
                        if (result != Result.Succeeded)
                        {
                            TaskDialog.Show("Warning", showMessage);
                        }
                    }
                    document.Delete(id);
                    if (!testModeOn)
                    {
                        TaskDialog.Show("Delete Warning", $"An object with ID=<{id}> has been deleted.\n");
                    }
                    MoreValue = e.MoveNext();
                }
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                foreach (Element c in elementsToDelete)
                {
                    elements.Insert(c);
                }
                message = "Exception occurred when deleting object(s):\n" + ex.Message;
                trans.RollBack();
                return Result.Failed;
            }

            trans.Commit();
            elementFirstFailed = null;
            return Result.Succeeded;
        }
    }
}

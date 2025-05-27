using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFunctions
{
    /// <summary>
    /// Show specific elements in the 3d view
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class ShowElements : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new System.NotImplementedException("Default element show is not supported.");          
        }
        public static Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ICollection<ElementId> elementIds)
        {
            UIApplication uiApp = commandData.Application;

            if (elementIds == null || elementIds.Count == 0)
            {
                message = "At least one element ID should be specified to display.";
                return Result.Failed;
            }

            try
            {
                // zoom view
                uiApp.ActiveUIDocument.Selection.SetElementIds(elementIds);
                uiApp.ActiveUIDocument.ShowElements(elementIds);
                uiApp.ActiveUIDocument.RefreshActiveView();
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when showing elements:\n" + ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        public static Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId elementId)
        {
            UIApplication uiApp = commandData.Application;

            if (elementId == null)
            {
                message = "Element ID is null.";
                return Result.Failed;
            }

            try
            {            
                // zoom view
                ICollection<ElementId> elementIds = new List<ElementId> { elementId };
                uiApp.ActiveUIDocument.Selection.SetElementIds(elementIds);
                uiApp.ActiveUIDocument.ShowElements(elementIds);
                uiApp.ActiveUIDocument.RefreshActiveView();
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when showing elements:\n" + ex.Message;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}

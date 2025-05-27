using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitInterface
{
    /// <summary>
    /// Show elements in 3D view with their id.
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
            throw new NotImplementedException("Default element show is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, List<string> elementIdStrings, string notice, bool testModeOn)
        {
            if (elementIdStrings == null || elementIdStrings.Count == 0)
            {
                message = "No element ID is specified.";
                return Result.Failed;
            }

            if (testModeOn)
            {
                // Skip showing elements during test
                return Result.Succeeded;
            }

            Document document = commandData.Application.ActiveUIDocument.Document;

            // Check element id validity
            ICollection<ElementId> elementIds = new List<ElementId>();
            foreach (string elementIdString in elementIdStrings)
            {
                ElementId elementId = null;
                try
                {
                    elementId = ElementId.Parse(elementIdString);
                }
                catch
                {
                    message = $"Invalid element ID: {elementIdString}.";
                    return Result.Failed;
                }
                
                Element element = document.GetElement(elementId);
                if (element == null)
                {
                    message = $"Element not found for ID: {elementIdString}.";
                    return Result.Failed;
                }
                elementIds.Add(elementId);
            }

            // Show elements            
            Result result = RevitFunctions.ShowElements.Execute(commandData, ref message, elements, elementIds);
            TaskDialog.Show("Notice", $"Notice for highlighted elements:\n{notice}");
            return result;
        }
    }
}

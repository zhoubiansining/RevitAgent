using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitInterface
{
    /// <summary>
    /// Delete elements with element id.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class DeleteElement : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new NotImplementedException("Default element delete is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, List<string> elementIdStrings, bool testModeOn)
        {
            if (elementIdStrings == null || elementIdStrings.Count == 0)
            {
                message = "No element ID is specified.";
                return Result.Failed;
            }

            Document document = commandData.Application.ActiveUIDocument.Document;

            // Check element id validity
            ElementSet elementIds = new ElementSet();
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
                elementIds.Insert(element);
            }

            // Delete elements
            try
            {
                RevitFunctions.DeleteObject deleteObject = new RevitFunctions.DeleteObject();
                Result result = deleteObject.Execute(commandData, ref message, elements, elementIds, out Element elementFirstFailed, testModeOn);
                if (elementFirstFailed != null)
                {
                    message = message + $"\nFailed to delete element with ID: {elementFirstFailed.Id}.";
                }
                return result;
            }
            catch (Exception ex)
            {
                message = message + "\nException occurred when deleting element(s):\n" + ex.Message;
                return Result.Failed;
            }
        }
    }
}

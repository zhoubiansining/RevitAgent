using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFunctions
{
    /// <summary>
    /// Retrieve elements(ids) that passes given filters
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RetrieveObject : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new System.NotImplementedException("Default object retrieve is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ICollection<ElementFilter> filters, bool isElementType, out ICollection<ElementId> elementIds)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            if (filters == null)
            {
                message = "Filters are null.";
                elementIds = null;
                return Result.Failed;
            }

            try
            {
                // filter elements
                FilteredElementCollector collector = new FilteredElementCollector(document);
                foreach (ElementFilter filter in filters)
                {
                    collector = collector.WherePasses(filter);
                }
                if (isElementType)
                {
                    collector = collector.WhereElementIsElementType();
                }
                else
                {
                    collector = collector.WhereElementIsNotElementType();
                }
                elementIds = collector.ToElementIds();
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when filtering and retrieving objects:\n" + ex.Message;
                elementIds = null;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}

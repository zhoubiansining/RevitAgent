using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFunctions
{
    /// <summary>
    /// Count the number of elements that passes given filters
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class CountElement : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new System.NotImplementedException("Default element count is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ICollection<ElementFilter> filters, bool isElementType, out int num)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            if (filters == null)
            {
                message = "Filters are null.";
                num = -1;
                return Result.Failed;
            }

            try
            {
                // filter elements and count
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
                num = collector.ToElementIds().Count;
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when filtering and counting elements:\n" + ex.Message;
                num = -1;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}

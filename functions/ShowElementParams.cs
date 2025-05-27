using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFunctions
{
    /// <summary>
    /// Show all parameters of the selected element
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class ShowElementParams : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new System.NotImplementedException("Target element must be designated");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, Element elementToShow)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            try
            {
                GetElementParameterInformation(document, elementToShow);
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Error occurred when showing parameters:\n" + ex.Message;

                return Result.Failed;
            }

            return Result.Succeeded;
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ParameterSet elementParameterSet)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            try
            {
                GetElementParameterInformation(document, elementParameterSet);
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Error occurred when showing parameters:\n" + ex.Message;

                return Result.Failed;
            }

            return Result.Succeeded;
        }
        void GetElementParameterInformation(Document document, Element element)
        {
            // Format the prompt information string
            string prompt = "Show parameters in selected Element: \n\r";

            StringBuilder st = new StringBuilder();
            // iterate element's parameters
            foreach (Parameter para in element.Parameters)
            {
                st.AppendLine(GetParameterInformation(para, document));
            }

            // Give the user some information
            TaskDialog.Show("Revit", prompt + st.ToString());
        }
        void GetElementParameterInformation(Document document, ParameterSet elementParameterSet)
        {
            // Format the prompt information string
            string prompt = "Show selected parameters in given element parameter set: \n\r";

            StringBuilder st = new StringBuilder();
            // iterate parameter set
            foreach (Parameter para in elementParameterSet)
            {
                st.AppendLine(GetParameterInformation(para, document));
            }

            // Give the user some information
            TaskDialog.Show("Revit", prompt + st.ToString());
        }
        string GetParameterInformation(Parameter para, Document document)
        {
            string defName = para.Definition.Name + "\t : ";
            string defValue = string.Empty;
            // Use different method to get parameter data according to the storage type
            switch (para.StorageType)
            {
                case StorageType.Double:
                    //covert the number into Metric
                    defValue = para.AsValueString();
                    break;
                case StorageType.ElementId:
                    //find out the name of the element
                    ElementId id = para.AsElementId();
                    if (id.Value >= 0)
                    {
                        defValue = document.GetElement(id).Name;
                    }
                    else
                    {
                        defValue = id.Value.ToString();
                    }
                    break;
                case StorageType.Integer:
                    defValue = para.AsInteger().ToString();
                    break;
                case StorageType.String:
                    defValue = para.AsString();
                    break;
                default:
                    defValue = "Unexposed parameter.";
                    break;
            }

            return defName + defValue;
        }
    }
}

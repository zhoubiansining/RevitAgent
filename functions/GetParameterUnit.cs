using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFunctions
{
    /// <summary>
    /// Get the unit of parameters
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class GetParameterUnit : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new System.NotImplementedException("Default parameter unit retrieve is not supported.");
        }
        public static string Execute(ref string message, Parameter parameter)
        {
            if (parameter == null)
            {
                message = "Parameter is null.";
                return null;
            }

            try
            {
                ForgeTypeId forgeTypeId = parameter.GetUnitTypeId();
                return UnitUtils.GetTypeCatalogStringForUnit(forgeTypeId);
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when retrieving parameter unit:\n" + ex.Message;
                return null;
            }
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId elementId, string parameterName, out string unit)
        {
            if (elementId == null)
            {
                message = "Element ID is null.";
                unit = null;
                return Result.Failed;
            }

            Document document = commandData.Application.ActiveUIDocument.Document;
            Element element = document.GetElement(elementId);
            if (element == null)
            {
                message = "Element not found.";
                unit = null;
                return Result.Failed;
            }         

            try
            {
                // retrieve the target parameter
                IList<Parameter> parameters = element.GetParameters(parameterName);
                if (parameters.Count == 0)
                {
                    message = "Parameter not found.";
                    elements.Insert(element);
                    unit = null;
                    return Result.Failed;
                }
                else if (parameters.Count > 1)
                {
                    message = "Cannot get parameter unit since multiple parameters with this name are found.";
                    elements.Insert(element);
                    unit = null;
                    return Result.Failed;
                }
                else
                {
                    Parameter parameter = parameters[0];

                    try
                    {
                        ForgeTypeId forgeTypeId = parameter.GetUnitTypeId();
                        unit = UnitUtils.GetTypeCatalogStringForUnit(forgeTypeId);
                    }
                    catch
                    {
                        message = "Cannot get parameter unit since this parameter is not of value type.";
                        elements.Insert(element);
                        unit = null;
                        return Result.Failed;
                    }
                }
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when retrieving parameter unit:\n" + ex.Message;
                elements.Insert(element);
                unit = null;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId elementId, BuiltInParameter builtInParameter, out string unit)
        {
            if (elementId == null)
            {
                message = "Element ID is null.";
                unit = null;
                return Result.Failed;
            }

            Document document = commandData.Application.ActiveUIDocument.Document;
            Element element = document.GetElement(elementId);
            if (element == null)
            {
                message = "Element not found.";
                unit = null;
                return Result.Failed;
            }

            try
            {
                // retrieve the target parameter              
                Parameter parameter = element.get_Parameter(builtInParameter);

                try
                {
                    ForgeTypeId forgeTypeId = parameter.GetUnitTypeId();
                    unit = UnitUtils.GetTypeCatalogStringForUnit(forgeTypeId);
                }
                catch
                {
                    message = "Cannot get parameter unit since this parameter is not of value type.";
                    elements.Insert(element);
                    unit = null;
                    return Result.Failed;
                }
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when retrieving parameter unit:\n" + ex.Message;
                elements.Insert(element);
                unit = null;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}

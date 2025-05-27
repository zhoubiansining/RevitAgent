using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFunctions
{
    /// <summary>
    /// Retrieve element parameters and attributes with id
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RetrieveObjectParams : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new System.NotImplementedException("Default parameter retrieve is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId elementId, out ParameterSet parameterSet)
        {
            if (elementId == null)
            {
                message = "Element ID is null.";
                parameterSet = null;
                return Result.Failed;
            }

            Document document = commandData.Application.ActiveUIDocument.Document;            
            Element element = document.GetElement(elementId);
            if (element == null)
            {
                message = "Element not found.";
                parameterSet = null;
                return Result.Failed;
            }

            try
            {
                // retrieve all parameters of that element
                parameterSet = element.Parameters;
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when retrieving object parameters:\n" + ex.Message;
                parameterSet = null;
                elements.Insert(element);
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId elementId, string parameterName, out ParameterSet parameterSet)
        {
            if (elementId == null)
            {
                message = "Element ID is null.";
                parameterSet = null;
                return Result.Failed;
            }

            Document document = commandData.Application.ActiveUIDocument.Document;
            Element element = document.GetElement(elementId);
            if (element == null)
            {
                message = "Element not found.";
                parameterSet = null;
                return Result.Failed;
            }

            try
            {
                // retrieve element parameters with specific name
                parameterSet = new ParameterSet();
                foreach (Parameter parameter in element.Parameters)
                {
                    if (parameter.Definition.Name == parameterName)
                    {
                        parameterSet.Insert(parameter);
                    }
                }
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when retrieving object parameters with parameter name:\n" + ex.Message;
                parameterSet = null;
                elements.Insert(element);
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId elementId, BuiltInParameter builtInParameter, out Parameter parameter)
        {
            if (elementId == null)
            {
                message = "Element ID is null.";
                parameter = null;
                return Result.Failed;
            }

            Document document = commandData.Application.ActiveUIDocument.Document;
            Element element = document.GetElement(elementId);
            if (element == null)
            {
                message = "Element not found.";
                parameter = null;
                return Result.Failed;
            }

            try
            {
                // retrieve the element parameter with unique built-in id                
                parameter = element.get_Parameter(builtInParameter);
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when retrieving object parameter with built-in id:\n" + ex.Message;
                parameter = null;
                elements.Insert(element);
                return Result.Failed;
            }           

            return Result.Succeeded;
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, List<ElementId> elementIds, out List<ParameterSet> parameterSets)
        {
            if (elementIds == null)
            {
                message = "Element Ids are null.";
                parameterSets = null;
                return Result.Failed;
            }
            Document document = commandData.Application.ActiveUIDocument.Document;
            parameterSets = new List<ParameterSet>(); // Initialize the out parameter

            foreach (ElementId elementId in elementIds)
            {
                try
                {
                    // retrieve parameters of all designated elements
                    Element element = document.GetElement(elementId);
                    parameterSets.Add(element.Parameters);
                }
                catch (System.Exception ex)
                {
                    // if revit threw an exception, try to catch it
                    message = "Exception occurred when retrieving object parameters:\n" + ex.Message;
                    parameterSets = null;
                    return Result.Failed;
                }
            }

            return Result.Succeeded;
        }
    }
}

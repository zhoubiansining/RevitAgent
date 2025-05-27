using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitFunctions;

namespace RevitInterface
{
    /// <summary>
    /// Retrieve all parameters of an element.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RetrieveAllParameters : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new NotImplementedException("Default parameter retrieve is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements,
            string elementIdString, out ParameterSet parameterSet)
        {
            if (elementIdString == null)
            {
                message = "Element ID is not specified.";
                parameterSet = null;
                return Result.Failed;
            }

            // Parse element id
            ElementId elementId = null;
            try
            {
                elementId = ElementId.Parse(elementIdString);
            }
            catch
            {
                message = $"Invalid element ID: {elementIdString}.";
                parameterSet = null;
                return Result.Failed;
            }

            // Get parameters
            try
            {
                RetrieveObjectParams retrieveObjectParams = new RetrieveObjectParams();
                Result result = retrieveObjectParams.Execute(commandData, ref message, elements, elementId, out parameterSet);
                return result;
            }
            catch (Exception ex)
            {
                message = message + "\nException occurred when retrieving parameters:\n" + ex.Message;
                parameterSet = null;
                return Result.Failed;
            }
        }
    }
    /// <summary>
    /// Retrieve specified parameter of an element with built-in id.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RetrieveParameterWithBuiltInId : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new NotImplementedException("Default parameter retrieve with built-in ID is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements,
            string elementIdString, string builtInParameterName, out ParameterSet parameterSet)
        {
            if (elementIdString == null)
            {
                message = "Element ID is not specified.";
                parameterSet = null;
                return Result.Failed;
            }
            if (builtInParameterName == null)
            {
                message = "Built-in parameter name is not specified.";
                parameterSet = null;
                return Result.Failed;
            }

            // Parse element id
            ElementId elementId = null;
            try
            {
                elementId = ElementId.Parse(elementIdString);
            }
            catch
            {
                message = $"Invalid element ID: {elementIdString}.";
                parameterSet = null;
                return Result.Failed;
            }

            // Parse built-in parameter
            if (Enum.TryParse(builtInParameterName, out BuiltInParameter builtInParameter))
            {
                // Get parameters
                try
                {
                    RetrieveObjectParams retrieveObjectParams = new RetrieveObjectParams();
                    Result result = retrieveObjectParams.Execute(commandData, ref message, elements, elementId, builtInParameter, out Parameter parameter);
                    parameterSet = new ParameterSet();
                    parameterSet.Insert(parameter);
                    return result;
                }
                catch (Exception ex)
                {
                    message = message + "\nException occurred when retrieving parameters with built-in ID:\n" + ex.Message;
                    parameterSet = null;
                    return Result.Failed;
                }
            }
            else
            {
                message = $"Invalid built-in parameter name: {builtInParameterName}.";
                parameterSet = null;
                return Result.Failed;
            }
        }
    }
    /// <summary>
    /// Retrieve specified parameter of an element with parameter name.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RetrieveParameterWithParameterName : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new NotImplementedException("Default parameter retrieve with parameter name is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements,
            string elementIdString, string parameterName, out ParameterSet parameterSet)
        {
            if (elementIdString == null)
            {
                message = "Element ID is not specified.";
                parameterSet = null;
                return Result.Failed;
            }
            if (parameterName == null)
            {
                message = "Parameter name is not specified.";
                parameterSet = null;
                return Result.Failed;
            }

            // Parse element id
            ElementId elementId = null;
            try
            {
                elementId = ElementId.Parse(elementIdString);
            }
            catch
            {
                message = $"Invalid element ID: {elementIdString}.";
                parameterSet = null;
                return Result.Failed;
            }

            // Get parameters
            try
            {
                RetrieveObjectParams retrieveObjectParams = new RetrieveObjectParams();
                Result result = retrieveObjectParams.Execute(commandData, ref message, elements, elementId, parameterName, out parameterSet);
                return result;
            }
            catch (Exception ex)
            {
                message = message + "\nException occurred when retrieving parameters with parameter name:\n" + ex.Message;
                parameterSet = null;
                return Result.Failed;
            }
        }
    }
}

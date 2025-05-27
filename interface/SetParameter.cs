using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitFunctions;

namespace RevitInterface
{
    /// <summary>
    /// Set parameters of an element with built-in id.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SetParameterWithBuiltInId : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new NotImplementedException("Default parameter set with built-in ID is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements,
            string elementIdString, string builtInParameterName, string valueTypeName, string valueString, bool testModeOn)
        {
            if (elementIdString == null)
            {
                message = "Element ID is not specified.";
                return Result.Failed;
            }
            if (builtInParameterName == null)
            {
                message = "Built-in parameter name is not specified.";
                return Result.Failed;
            }
            if (valueTypeName == null)
            {
                message = "The type of the parameter value is not specified.";
                return Result.Failed;
            }
            if (valueString == null)
            {
                message = "New parameter value is not specified.";
                return Result.Failed;
            }

            // Parse parameter value
            object parsedValue = null;
            try
            {
                switch (valueTypeName.ToLower())
                {
                    case "int":
                        parsedValue = int.Parse(valueString);
                        break;
                    case "double":
                        parsedValue = double.Parse(valueString);
                        break;
                    case "string":
                        parsedValue = valueString;
                        break;
                    case "elementid":
                        parsedValue = ElementId.Parse(valueString);
                        break;
                    default:
                        message = $"Unsupported parameter value type: {valueTypeName}.";
                        return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = "Exception occurred when parsing parameter value:\n" + ex.Message;
                return Result.Failed;
            }

            // Set parameter
            try
            {
                if (Enum.TryParse(builtInParameterName, out BuiltInParameter builtInParameter))
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
                    SetElementParameter setElementParameter = new SetElementParameter();
                    Result result = setElementParameter.Execute(commandData, ref message, elements, elementId, builtInParameter, parsedValue, testModeOn);
                    return result;
                }
                else
                {
                    message = $"Invalid built-in parameter name: {builtInParameterName}.";
                    return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = message + "\nException occurred when setting parameter with built-in ID:\n" + ex.Message;
                return Result.Failed;
            }
        }
    }
    /// <summary>
    /// Set parameters of an element with parameter name.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SetParameterWithParameterName : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new NotImplementedException("Default parameter set with parameter name is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements,
            string elementIdString, string parameterName, string valueTypeName, string valueString, bool testModeOn)
        {
            if (elementIdString == null)
            {
                message = "Element ID is not specified.";
                return Result.Failed;
            }
            if (parameterName == null)
            {
                message = "Parameter name is not specified.";
                return Result.Failed;
            }
            if (valueTypeName == null)
            {
                message = "The type of the parameter value is not specified.";
                return Result.Failed;
            }
            if (valueString == null)
            {
                message = "New parameter value is not specified.";
                return Result.Failed;
            }

            // Parse parameter value
            object parsedValue = null;
            try
            {
                switch (valueTypeName.ToLower())
                {
                    case "int":
                        parsedValue = int.Parse(valueString);
                        break;
                    case "double":
                        parsedValue = double.Parse(valueString);
                        break;
                    case "string":
                        parsedValue = valueString;
                        break;
                    case "elementid":
                        parsedValue = ElementId.Parse(valueString);
                        break;
                    default:
                        message = $"Unsupported parameter value type: {valueTypeName}.";
                        return Result.Failed;
                }
            }
            catch (Exception ex)
            {
                message = "Exception occurred when parsing parameter value:\n" + ex.Message;
                return Result.Failed;
            }

            // Set parameter
            try
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
                SetElementParameter setElementParameter = new SetElementParameter();
                Result result = setElementParameter.Execute(commandData, ref message, elements, elementId, parameterName, parsedValue, testModeOn);
                return result;
            }
            catch (Exception ex)
            {
                message = message + "\nException occurred when setting parameter with parameter name:\n" + ex.Message;
                return Result.Failed;
            }
        }
    }
}

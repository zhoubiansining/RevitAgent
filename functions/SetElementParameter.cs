using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFunctions
{
    /// <summary>
    /// Set element parameters
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class SetElementParameter : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new System.NotImplementedException("Default parameter set is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId elementId, string parameterName, object parameterValue, bool testModeOn)
        {
            if (!(parameterValue is int || parameterValue is double || parameterValue is string || parameterValue is ElementId))
            {
                message = "ParameterValue must be of type int, double, string or element id.";
                return Result.Failed;
            }

            if (elementId == null)
            {
                message = "Element ID is null.";
                return Result.Failed;
            }

            Document document = commandData.Application.ActiveUIDocument.Document;
            Element element = document.GetElement(elementId);
            if (element == null)
            {
                message = "Element not found.";
                return Result.Failed;
            }

            Transaction trans = new Transaction(document, "SetParameter");
            trans.Start();            

            try
            {
                // retrieve the target parameter
                IList<Parameter> parameters = element.GetParameters(parameterName);
                if (parameters.Count == 0)
                {
                    message = "Parameter not found.";
                    elements.Insert(element);
                    trans.RollBack();
                    return Result.Failed;
                }
                else if (parameters.Count > 1)
                {
                    message = "Cannot set parameter since multiple parameters with this name are found.";
                    elements.Insert(element);
                    trans.RollBack();
                    return Result.Failed;
                }
                else
                {
                    Parameter parameter = parameters[0];
                    string oldValue = parameter.AsValueString();

                    if (parameter.IsReadOnly)
                    {
                        message = "Parameter is read-only.";
                        elements.Insert(element);
                        trans.RollBack();
                        return Result.Failed;
                    }
                    else
                    {                                                  
                        if (parameterValue is int intValue)
                        {
                            if (parameter.StorageType != StorageType.Integer)
                            {
                                message = $"Parameter value type does not match. Parameter storage type is {parameter.StorageType}, not int.";
                                elements.Insert(element);
                                trans.RollBack();
                                return Result.Failed;
                            }
                            bool succeeded = parameter.Set(intValue);
                            if (!succeeded)
                            {
                                message = $"Parameter could not be set to the new int value: {intValue}.";
                                elements.Insert(element);
                                trans.RollBack();
                                return Result.Failed;
                            }
                        }
                        else if (parameterValue is double doubleValue)
                        {
                            if (parameter.StorageType != StorageType.Double)
                            {
                                message = $"Parameter value type does not match. Parameter storage type is {parameter.StorageType}, not double.";
                                elements.Insert(element);
                                trans.RollBack();
                                return Result.Failed;
                            }
                            // double value may need unit convertion
                            string unitMessage = "";
                            string unit = GetParameterUnit.Execute(ref unitMessage, parameter);
                            double internalValue = doubleValue;
                            if (!string.IsNullOrEmpty(unit))
                            {
                                Result result = ConvertUnit.ConvertToInternal(ref message, parameter.GetUnitTypeId(), doubleValue, out internalValue);
                                if (result != Result.Succeeded)
                                {
                                    message = "Internal unit convertion failed for the parameter value.";
                                    elements.Insert(element);
                                    trans.RollBack();
                                    return Result.Failed;
                                }
                            }
                            bool succeeded = parameter.Set(internalValue);
                            if (!succeeded)
                            {
                                message = $"Parameter could not be set to the new double value: {doubleValue}.";
                                elements.Insert(element);
                                trans.RollBack();
                                return Result.Failed;
                            }
                        }
                        else if (parameterValue is string stringValue)
                        {
                            if (parameter.StorageType != StorageType.String)
                            {
                                message = $"Parameter value type does not match. Parameter storage type is {parameter.StorageType}, not string.";
                                elements.Insert(element);
                                trans.RollBack();
                                return Result.Failed;
                            }
                            bool succeeded = parameter.Set(stringValue);
                            if (!succeeded)
                            {
                                message = $"Parameter could not be set to the new string value: {stringValue}.";
                                elements.Insert(element);
                                trans.RollBack();
                                return Result.Failed;
                            }
                        }
                        else if (parameterValue is ElementId elementIdValue)
                        {
                            if (parameter.StorageType != StorageType.ElementId)
                            {
                                message = $"Parameter value type does not match. Parameter storage type is {parameter.StorageType}, not elementid.";
                                elements.Insert(element);
                                trans.RollBack();
                                return Result.Failed;
                            }
                            bool succeeded = parameter.Set(elementIdValue);
                            if (!succeeded)
                            {
                                message = $"Parameter could not be set to the new elementid value: {elementIdValue}.";
                                elements.Insert(element);
                                trans.RollBack();
                                return Result.Failed;
                            }
                        }

                        if (!testModeOn)
                        {                            
                            string _ = "";
                            string unit = GetParameterUnit.Execute(ref _, parameter);
                            TaskDialog.Show("Parameter Set Warning", $"A parameter with ID=<{parameter.Id}>, Name=<{parameter.Definition.Name}>, Unit=<{unit}> has been set.\nThis parameter belongs to the element with ID=<{elementId}>.\nChanges in value: {oldValue} => {parameterValue}.\n");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {                
                // if revit threw an exception, try to catch it
                message = "Exception occurred when setting parameter:\n" + ex.Message;
                elements.Insert(element);
                trans.RollBack();
                return Result.Failed;
            }

            trans.Commit();

            if (!testModeOn)
            {
                string showMessage = "";
                Result result = ShowElements.Execute(commandData, ref showMessage, elements, elementId);
                if (result != Result.Succeeded)
                {
                    TaskDialog.Show("Warning", showMessage);
                }
            }

            return Result.Succeeded;
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId elementId, BuiltInParameter builtInParameter, object parameterValue, bool testModeOn)
        {
            if (!(parameterValue is int || parameterValue is double || parameterValue is string || parameterValue is ElementId))
            {
                message = "ParameterValue must be of type int, double, string or element id.";
                return Result.Failed;
            }

            if (elementId == null)
            {
                message = "Element ID is null.";
                return Result.Failed;
            }

            Document document = commandData.Application.ActiveUIDocument.Document;
            Element element = document.GetElement(elementId);
            if (element == null)
            {
                message = "Element not found.";
                return Result.Failed;
            }

            Transaction trans = new Transaction(document, "SetParameter");
            trans.Start();

            try
            {
                // retrieve the target parameter              
                Parameter parameter = element.get_Parameter(builtInParameter);
                string oldValue = parameter.AsValueString();

                if (parameter.IsReadOnly)
                {
                    message = "Parameter is read-only.";
                    elements.Insert(element);
                    trans.RollBack();
                    return Result.Failed;
                }
                else
                {
                    if (parameterValue is int intValue)
                    {
                        if (parameter.StorageType != StorageType.Integer)
                        {
                            message = $"Parameter value type does not match. Parameter storage type is {parameter.StorageType}, not int.";
                            elements.Insert(element);
                            trans.RollBack();
                            return Result.Failed;
                        }
                        bool succeeded = parameter.Set(intValue);
                        if (!succeeded)
                        {
                            message = $"Parameter could not be set to the new int value: {intValue}.";
                            elements.Insert(element);
                            trans.RollBack();
                            return Result.Failed;
                        }
                    }
                    else if (parameterValue is double doubleValue)
                    {
                        if (parameter.StorageType != StorageType.Double)
                        {
                            message = $"Parameter value type does not match. Parameter storage type is {parameter.StorageType}, not double.";
                            elements.Insert(element);
                            trans.RollBack();
                            return Result.Failed;
                        }
                        // double value may need unit convertion
                        string unitMessage = "";
                        string unit = GetParameterUnit.Execute(ref unitMessage, parameter);
                        double internalValue = doubleValue;
                        if (!string.IsNullOrEmpty(unit))
                        {
                            Result result = ConvertUnit.ConvertToInternal(ref message, parameter.GetUnitTypeId(), doubleValue, out internalValue);
                            if (result != Result.Succeeded)
                            {
                                message = "Internal unit convertion failed for the parameter value.";
                                elements.Insert(element);
                                trans.RollBack();
                                return Result.Failed;
                            }
                        }
                        bool succeeded = parameter.Set(internalValue);
                        if (!succeeded)
                        {
                            message = $"Parameter could not be set to the new double value: {doubleValue}.";
                            elements.Insert(element);
                            trans.RollBack();
                            return Result.Failed;
                        }
                    }
                    else if (parameterValue is string stringValue)
                    {
                        if (parameter.StorageType != StorageType.String)
                        {
                            message = $"Parameter value type does not match. Parameter storage type is {parameter.StorageType}, not string.";
                            elements.Insert(element);
                            trans.RollBack();
                            return Result.Failed;
                        }
                        bool succeeded = parameter.Set(stringValue);
                        if (!succeeded)
                        {
                            message = $"Parameter could not be set to the new string value: {stringValue}.";
                            elements.Insert(element);
                            trans.RollBack();
                            return Result.Failed;
                        }
                    }
                    else if (parameterValue is ElementId elementIdValue)
                    {
                        if (parameter.StorageType != StorageType.ElementId)
                        {
                            message = $"Parameter value type does not match. Parameter storage type is {parameter.StorageType}, not elementid.";
                            elements.Insert(element);
                            trans.RollBack();
                            return Result.Failed;
                        }
                        bool succeeded = parameter.Set(elementIdValue);
                        if (!succeeded)
                        {
                            message = $"Parameter could not be set to the new elementid value: {elementIdValue}.";
                            elements.Insert(element);
                            trans.RollBack();
                            return Result.Failed;
                        }
                    }

                    if (!testModeOn)
                    {                        
                        string _ = "";
                        string unit = GetParameterUnit.Execute(ref _, parameter);
                        TaskDialog.Show("Parameter Set Warning", $"A parameter with ID=<{parameter.Id}>, Name=<{parameter.Definition.Name}>, Unit=<{unit}> has been set.\nThis parameter belongs to the element with ID=<{elementId}>.\nChanges in value: {oldValue} => {parameterValue}.\n");
                    }
                }
            }
            catch (System.Exception ex)
            {                
                // if revit threw an exception, try to catch it
                message = "Exception occurred when setting parameter:\n" + ex.Message;
                elements.Insert(element);
                trans.RollBack();
                return Result.Failed;
            }

            trans.Commit();

            if (!testModeOn)
            {
                string showMessage = "";
                Result result = ShowElements.Execute(commandData, ref showMessage, elements, elementId);
                if (result != Result.Succeeded)
                {
                    TaskDialog.Show("Warning", showMessage);
                }
            }

            return Result.Succeeded;
        }
    }
}

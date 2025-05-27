using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitFunctions;

namespace RevitInterface
{
    /// <summary>
    /// Retrieve elements(IDs) that satisfy certain conditions.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class RetrieveElement : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new NotImplementedException("Default element retrieve is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, List<string> elementIdStrings, string builtInCategoryName, string className, 
            string familyIdString, string familySymbolIdString, string levelIdString, bool isElementType, 
            List<Dictionary<string, string>> parameterFilterArgs, out ICollection<ElementId> elementIds)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            Result result;

            // Create filters
            ICollection<ElementFilter> filters = new List<ElementFilter>();
            CreateElementFilter createElementFilter = new CreateElementFilter();

            // ID set filter
            if (elementIdStrings != null && elementIdStrings.Count > 0)
            {
                ICollection<ElementId> elementIdSet = new List<ElementId>();
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
                        elementIds = null;
                        return Result.Failed;
                    }

                    elementIdSet.Add(elementId);
                }

                result = createElementFilter.GetElementIdSetFilter(commandData, ref message, elements, elementIdSet, out ElementIdSetFilter elementIdSetFilter);
                if (result != Result.Succeeded)
                {
                    message = message + "\nFailed to create element ID set filter.";
                    elementIds = null;
                    return result;
                }
                filters.Add(elementIdSetFilter);
            }

            // Category filter
            if (!string.IsNullOrEmpty(builtInCategoryName))
            {
                if (Enum.TryParse(builtInCategoryName, out BuiltInCategory builtInCategory))
                {                   
                    result = createElementFilter.GetElementCategoryFilter(commandData, ref message, elements, builtInCategory, out ElementCategoryFilter elementCategoryFilter);
                    if (result != Result.Succeeded)
                    {
                        message = message + "\nFailed to create element category filter.";
                        elementIds = null;
                        return result;
                    }
                    filters.Add(elementCategoryFilter);
                }
                else
                {
                    message = $"Invalid built-in category name: {builtInCategoryName}.";
                    elementIds = null;
                    return Result.Failed;
                }
            }

            // Class filter
            if (!string.IsNullOrEmpty(className))
            {
                Type type = Type.GetType(className);
                if (type != null) {
                    result = createElementFilter.GetElementClassFilter(commandData, ref message, elements, type, out ElementClassFilter elementClassFilter);
                    if (result != Result.Succeeded)
                    {
                        message = message + "\nFailed to create element class filter.";
                        elementIds = null;
                        return result;
                    }
                    filters.Add(elementClassFilter);
                }
                else
                {
                    message = $"Invalid class name: {className}.";
                    elementIds = null;
                    return Result.Failed;
                }
            }

            // Family symbol filter
            if (!string.IsNullOrEmpty(familyIdString))
            {
                if (ElementId.TryParse(familyIdString, out ElementId familyId))
                {
                    result = createElementFilter.GetFamilySymbolFilter(commandData, ref message, elements, familyId, out FamilySymbolFilter familySymbolFilter);
                    if (result != Result.Succeeded)
                    {
                        message = message + "\nFailed to create family symbol filter.";
                        elementIds = null;
                        return result;
                    }
                    filters.Add(familySymbolFilter);
                }
                else
                {
                    message = $"Invalid family ID: {familyIdString}.";
                    elementIds = null;
                    return Result.Failed;
                }
            }

            // Family instance filter
            if (!string.IsNullOrEmpty(familySymbolIdString))
            {
                if (ElementId.TryParse(familySymbolIdString, out ElementId familySymbolId))
                {
                    result = createElementFilter.GetFamilyInstanceFilter(commandData, ref message, elements, familySymbolId, out FamilyInstanceFilter familyInstanceFilter);
                    if (result != Result.Succeeded)
                    {
                        message = message + "\nFailed to create family instance filter.";
                        elementIds = null;
                        return result;
                    }
                    filters.Add(familyInstanceFilter);
                }
                else
                {
                    message = $"Invalid family symbol ID: {familySymbolIdString}.";
                    elementIds = null;
                    return Result.Failed;
                }
            }

            // Level filter
            if (!string.IsNullOrEmpty(levelIdString))
            {
                if (ElementId.TryParse(levelIdString, out ElementId levelId))
                {
                    result = createElementFilter.GetElementLevelFilter(commandData, ref message, elements, levelId, out ElementLevelFilter elementLevelFilter);
                    if (result != Result.Succeeded)
                    {
                        message = message + "\nFailed to create element level filter.";
                        elementIds = null;
                        return result;
                    }
                    filters.Add(elementLevelFilter);
                }
                else
                {
                    message = $"Invalid level ID: {levelIdString}.";
                    elementIds = null;
                    return Result.Failed;
                }
            }

            // Parameter filter(s)
            if (parameterFilterArgs != null)
            {
                string parameterIdString;
                string valueTypeName;
                string valueString;
                string rule;

                foreach (Dictionary<string, string> parameterFilterArg in parameterFilterArgs)
                {
                    try
                    {
                        parameterIdString = parameterFilterArg["parameterIdString"];
                        valueTypeName = parameterFilterArg["valueTypeName"];
                        valueString = parameterFilterArg["valueString"];
                        rule = parameterFilterArg["rule"];
                    }
                    catch
                    {
                        message = "Invalid parameter filter argument. Argument must have keys: 'parameterIdString', 'valueTypeName', 'valueString' and 'rule'.";
                        elementIds = null;
                        return Result.Failed;
                    }
                    if (ElementId.TryParse(parameterIdString, out ElementId parameterId))
                    {
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
                                    elementIds = null;
                                    return Result.Failed;
                            }
                        }
                        catch (Exception ex)
                        {
                            message = $"Exception occurred when parsing parameter value '{valueString}':\n" + ex.Message;
                            elementIds = null;
                            return Result.Failed;
                        }

                        result = createElementFilter.GetElementParameterFilter(commandData, ref message, elements, parameterId, parsedValue, rule, out ElementParameterFilter elementParameterFilter);
                        if (result != Result.Succeeded)
                        {
                            message = message + $"\nFailed to create element parameter filter for parameter ID: {parameterIdString}.";
                            elementIds = null;
                            return result;
                        }
                        filters.Add(elementParameterFilter);
                    }
                    else
                    {
                        message = $"Invalid parameter ID: {parameterIdString}.";
                        elementIds = null;
                        return Result.Failed;
                    }
                }
            }

            // Retrieve elements
            RetrieveObject retrieveObject = new RetrieveObject();
            result = retrieveObject.Execute(commandData, ref message, elements, filters, isElementType, out elementIds);
            return result;
        }
    }
}

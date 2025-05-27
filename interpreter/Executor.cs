using System;
using System.Collections.Generic;
using System.Text.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitInterface;

namespace RevitAgent.Interpreter
{
    /// <summary>
    /// Execute a revit interface command.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class Executor : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new NotImplementedException("Default execution is not supported.");
        }
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements,
            string call, Dictionary<string, JsonElement> args, bool testModeOn)
        {
            if (call == null)
            {
                message = "Interface call is not specified.";
                return Result.Failed;
            }
            if (args == null)
            {
                message = "Arguments are not specified.";
                return Result.Failed;
            }

            Result result;

            try
            {
                switch (call)
                {
                    case "DeleteElement":
                        {
                            DeleteElement deleteElement = new DeleteElement();
                            List<string> elementIdStrings = !args.ContainsKey("elementIdStrings") || args["elementIdStrings"].ValueKind == JsonValueKind.Null ? null : JsonSerializer.Deserialize<List<string>>(args["elementIdStrings"].GetRawText());
                            result = deleteElement.Execute(commandData, ref message, elements, elementIdStrings, testModeOn);

                            if (result == Result.Succeeded)
                            {
                                message = "Elements deleted successfully.";
                            }
                            break;
                        }
                    case "RetrieveElement":
                        {
                            RetrieveElement retrieveElement = new RetrieveElement();
                            List<string> elementIdStrings = !args.ContainsKey("elementIdStrings") || args["elementIdStrings"].ValueKind == JsonValueKind.Null ? null : JsonSerializer.Deserialize<List<string>>(args["elementIdStrings"].GetRawText());
                            List<Dictionary<string, string>> parameterFilterArgs = !args.ContainsKey("parameterFilterArgs") || args["parameterFilterArgs"].ValueKind == JsonValueKind.Null ? null : JsonSerializer.Deserialize<List<Dictionary<string, string>>>(args["parameterFilterArgs"].GetRawText());
                            result = retrieveElement.Execute(commandData, ref message, elements, elementIdStrings,
                                !args.ContainsKey("builtInCategoryName") || args["builtInCategoryName"].ValueKind == JsonValueKind.Null ? null : args["builtInCategoryName"].GetString(),
                                !args.ContainsKey("className") || args["className"].ValueKind == JsonValueKind.Null ? null : args["className"].GetString(),
                                !args.ContainsKey("familyIdString") || args["familyIdString"].ValueKind == JsonValueKind.Null ? null : args["familyIdString"].GetString(),
                                !args.ContainsKey("familySymbolIdString") || args["familySymbolIdString"].ValueKind == JsonValueKind.Null ? null : args["familySymbolIdString"].GetString(),
                                !args.ContainsKey("levelIdString") || args["levelIdString"].ValueKind == JsonValueKind.Null ? null : args["levelIdString"].GetString(),
                                !args.ContainsKey("isElementType") || args["isElementType"].ValueKind == JsonValueKind.Null ? false : args["isElementType"].GetBoolean(),
                                parameterFilterArgs, out ICollection<ElementId> elementIds);

                            if (result == Result.Succeeded)
                            {
                                message = MessageComposer.Compose(commandData, elementIds);
                            }
                            break;
                        }
                    case "RetrieveAllParameters":
                        {
                            RetrieveAllParameters retrieveAllParameters = new RetrieveAllParameters();
                            result = retrieveAllParameters.Execute(commandData, ref message, elements, 
                                !args.ContainsKey("elementIdString") || args["elementIdString"].ValueKind == JsonValueKind.Null ? null : args["elementIdString"].GetString(),
                                out ParameterSet parameterSet);

                            if (result == Result.Succeeded)
                            {
                                message = MessageComposer.Compose(parameterSet);
                            }
                            break;
                        }
                    case "RetrieveParameterWithBuiltInId":
                        {
                            RetrieveParameterWithBuiltInId retrieveParameterWithBuiltInId = new RetrieveParameterWithBuiltInId();
                            result = retrieveParameterWithBuiltInId.Execute(commandData, ref message, elements,
                                !args.ContainsKey("elementIdString") || args["elementIdString"].ValueKind == JsonValueKind.Null ? null : args["elementIdString"].GetString(),
                                !args.ContainsKey("builtInParameterName") || args["builtInParameterName"].ValueKind == JsonValueKind.Null ? null : args["builtInParameterName"].GetString(), 
                                out ParameterSet parameterSet);

                            if (result == Result.Succeeded)
                            {
                                message = MessageComposer.Compose(parameterSet);
                            }
                            break;
                        }
                    case "RetrieveParameterWithParameterName":
                        {
                            RetrieveParameterWithParameterName retrieveParameterWithParameterName = new RetrieveParameterWithParameterName();
                            result = retrieveParameterWithParameterName.Execute(commandData, ref message, elements,
                                !args.ContainsKey("elementIdString") || args["elementIdString"].ValueKind == JsonValueKind.Null ? null : args["elementIdString"].GetString(),
                                !args.ContainsKey("parameterName") || args["parameterName"].ValueKind == JsonValueKind.Null ? null : args["parameterName"].GetString(), 
                                out ParameterSet parameterSet);

                            if (result == Result.Succeeded)
                            {
                                message = MessageComposer.Compose(parameterSet);
                            }
                            break;
                        }
                    case "SetParameterWithBuiltInId":
                        {
                            SetParameterWithBuiltInId setParameterWithBuiltInId = new SetParameterWithBuiltInId();
                            result = setParameterWithBuiltInId.Execute(commandData, ref message, elements, 
                                !args.ContainsKey("elementIdString") || args["elementIdString"].ValueKind == JsonValueKind.Null ? null : args["elementIdString"].GetString(),
                                !args.ContainsKey("builtInParameterName") || args["builtInParameterName"].ValueKind == JsonValueKind.Null ? null : args["builtInParameterName"].GetString(),
                                !args.ContainsKey("valueTypeName") || args["valueTypeName"].ValueKind == JsonValueKind.Null ? null : args["valueTypeName"].GetString(),
                                !args.ContainsKey("valueString") || args["valueString"].ValueKind == JsonValueKind.Null ? null : args["valueString"].GetString(),
                                testModeOn);

                            if (result == Result.Succeeded)
                            {
                                message = "Parameter set successfully.";
                            }
                            break;
                        }
                    case "SetParameterWithParameterName":
                        {
                            SetParameterWithParameterName setParameterWithParameterName = new SetParameterWithParameterName();
                            result = setParameterWithParameterName.Execute(commandData, ref message, elements,
                                !args.ContainsKey("elementIdString") || args["elementIdString"].ValueKind == JsonValueKind.Null ? null : args["elementIdString"].GetString(),
                                !args.ContainsKey("parameterName") || args["parameterName"].ValueKind == JsonValueKind.Null ? null : args["parameterName"].GetString(),
                                !args.ContainsKey("valueTypeName") || args["valueTypeName"].ValueKind == JsonValueKind.Null ? null : args["valueTypeName"].GetString(),
                                !args.ContainsKey("valueString") || args["valueString"].ValueKind == JsonValueKind.Null ? null : args["valueString"].GetString(),
                                testModeOn);

                            if (result == Result.Succeeded)
                            {
                                message = "Parameter set successfully.";
                            }
                            break;
                        }
                    case "ShowElements":
                        {                    
                            ShowElements showElements = new ShowElements();
                            List<string> elementIdStrings = !args.ContainsKey("elementIdStrings") || args["elementIdStrings"].ValueKind == JsonValueKind.Null ? null : JsonSerializer.Deserialize<List<string>>(args["elementIdStrings"].GetRawText());
                            string notice = !args.ContainsKey("notice") || args["notice"].ValueKind == JsonValueKind.Null ? null : args["notice"].GetString();
                            result = showElements.Execute(commandData, ref message, elements, elementIdStrings, notice, testModeOn);

                            if (result == Result.Succeeded)
                            {
                                message = "Elements shown successfully.";
                            }
                            break;
                        }
                    default:
                        {
                            message = $"Unsupported call: {call}.";
                            result = Result.Failed;
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                message = "Exception occurred when executing interface command:\n" + ex.Message;
                result = Result.Failed;
            }

            return result;
        }
    }
    public class MessageComposer
    {
        public static string Compose(ICollection<ElementId> elementIds)
        {
            if (elementIds == null || elementIds.Count == 0)
            {
                return "No corresponding elements found.";
            }

            int cnt = 1;
            string message = "Here are all corresponding elements found, represented by their element ID:\n";
            foreach (ElementId elementId in elementIds)
            {
                message += $"**{cnt}** element_id: {elementId};" + "\n";
                cnt++;
            }
            return message;
        }
        public static string Compose(ExternalCommandData commandData, ICollection<ElementId> elementIds)
        {
            if (elementIds == null || elementIds.Count == 0)
            {
                return "No corresponding elements found.";
            }

            int cnt = 1;
            string message = "Here are all corresponding elements found, represented by their element ID, name, corresponding category name, level ID and owner view ID:\n";
            foreach (ElementId elementId in elementIds)
            {
                Element element = commandData.Application.ActiveUIDocument.Document.GetElement(elementId);
                if (element == null) {
                    return $"Element ID {elementId} is invalid.\n";
                }
                message += $"**{cnt}** element_id: {elementId}, name: {element.Name}, category_name: {element.Category.Name}, level_id: {element.LevelId}, owner_view_id: {element.OwnerViewId};" + "\n";
                cnt++;
            }

            return message;
        }
        public static string Compose(ParameterSet parameterSet)
        {
            if (parameterSet == null || parameterSet.Size == 0)
            {
                return "No corresponding parameters found for the specified element.";
            }

            string message = "Here are all corresponding parameters found for the specified element:\n";
            int cnt = 1;
            foreach (Parameter parameter in parameterSet)
            {
                string _ = "";
                string unit = RevitFunctions.GetParameterUnit.Execute(ref _, parameter);
                message += $"**{cnt}** parameter_id: {parameter.Id}, parameter_name: {parameter.Definition.Name}, value: {parameter.AsValueString()}, unit: {unit};" + "\n";
                cnt++;
            }

            return message;
        }
    }
}

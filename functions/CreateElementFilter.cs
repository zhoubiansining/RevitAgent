using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFunctions
{
    /// <summary>
    /// Create an element filter with specific criteria.
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class CreateElementFilter : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new NotImplementedException("Default filter creation is not supported.\n");
        }
        public Result GetElementIdSetFilter(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ICollection<ElementId> elementIds, out ElementIdSetFilter elementIdSetFilter)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            try
            {
                // create filter wrapping a set of element ids
                elementIdSetFilter = new ElementIdSetFilter(elementIds);
            }
            catch (Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when creating filter:\n" + ex.Message;
                elementIdSetFilter = null;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        public Result GetElementCategoryFilter(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, BuiltInCategory category, out ElementCategoryFilter elementCategoryFilter)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            try
            {
                // create filter with a builtin category(like OST_Walls, etc.)
                elementCategoryFilter = new ElementCategoryFilter(category);
            }
            catch (Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when creating filter:\n" + ex.Message;
                elementCategoryFilter = null;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        public Result GetElementClassFilter(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, Type type, out ElementClassFilter elementClassFilter)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            try
            {
                // create filter with a class(family or family symbol like Wall, Floor, etc.)
                elementClassFilter = new ElementClassFilter(type);
            }
            catch (Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when creating filter:\n" + ex.Message;
                elementClassFilter = null;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        public Result GetFamilySymbolFilter(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId familyId, out FamilySymbolFilter familySymbolFilter)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            try
            {
                // create filter with a family id that finds all family symbols of that family
                familySymbolFilter = new FamilySymbolFilter(familyId);
            }
            catch (Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when creating filter:\n" + ex.Message;
                familySymbolFilter = null;
                return Result.Failed;
            }           

            return Result.Succeeded;
        }
        public Result GetFamilyInstanceFilter(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId familySymbolId, out FamilyInstanceFilter familyInstanceFilter)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            try
            {
                // create filter with a family symbol id that finds all family instances of that family symbol
                familyInstanceFilter = new FamilyInstanceFilter(document, familySymbolId);
            }
            catch (Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when creating filter:\n" + ex.Message;
                familyInstanceFilter = null;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        public Result GetElementLevelFilter(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId levelId, out ElementLevelFilter elementLevelFilter)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            try
            {
                // create filter with a level id that finds all instances on that level
                elementLevelFilter = new ElementLevelFilter(levelId);
            }
            catch (Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when creating filter:\n" + ex.Message;
                elementLevelFilter = null;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
        public Result GetElementParameterFilter(
            ExternalCommandData commandData,
            ref string message, ElementSet elements, ElementId parameterId, object value, string rule, out ElementParameterFilter elementParameterFilter)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            try
            {
                // provider
                ParameterValueProvider pvp = new ParameterValueProvider(parameterId);

                if (value is int intValue)
                {      
                    if (rule == "Equals")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericEquals();
                        FilterRule fRule = new FilterIntegerRule(pvp, fnrv, intValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "Greater")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericGreater();
                        FilterRule fRule = new FilterIntegerRule(pvp, fnrv, intValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "GreaterOrEqual")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericGreaterOrEqual();
                        FilterRule fRule = new FilterIntegerRule(pvp, fnrv, intValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "Less")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericLess();
                        FilterRule fRule = new FilterIntegerRule(pvp, fnrv, intValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "LessOrEqual")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericLessOrEqual();
                        FilterRule fRule = new FilterIntegerRule(pvp, fnrv, intValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else
                    {
                        message = $"Invalid rule '{rule}' for parameter value type 'int'.";
                        elementParameterFilter = null;
                        return Result.Failed;
                    }
                }
                else if (value is double doubleValue)
                {
                    double eps = 1.0e-6;

                    if (rule == "Equals")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericEquals();
                        FilterRule fRule = new FilterDoubleRule(pvp, fnrv, doubleValue, eps);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "Greater")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericGreater();
                        FilterRule fRule = new FilterDoubleRule(pvp, fnrv, doubleValue, eps);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "GreaterOrEqual")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericGreaterOrEqual();
                        FilterRule fRule = new FilterDoubleRule(pvp, fnrv, doubleValue, eps);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "Less")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericLess();
                        FilterRule fRule = new FilterDoubleRule(pvp, fnrv, doubleValue, eps);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "LessOrEqual")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericLessOrEqual();
                        FilterRule fRule = new FilterDoubleRule(pvp, fnrv, doubleValue, eps);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else
                    {
                        message = $"Invalid rule '{rule}' for parameter value type 'double'.";
                        elementParameterFilter = null;
                        return Result.Failed;
                    }
                }
                else if (value is string stringValue)
                {
                    if (rule == "Equals")
                    {
                        // evaluator
                        FilterStringRuleEvaluator fsrv = new FilterStringEquals();
                        FilterRule fRule = new FilterStringRule(pvp, fsrv, stringValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "Contains")
                    {
                        // evaluator
                        FilterStringRuleEvaluator fsrv = new FilterStringContains();
                        FilterRule fRule = new FilterStringRule(pvp, fsrv, stringValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "BeginsWith")
                    {
                        // evaluator
                        FilterStringRuleEvaluator fsrv = new FilterStringBeginsWith();
                        FilterRule fRule = new FilterStringRule(pvp, fsrv, stringValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "EndsWith")
                    {
                        // evaluator
                        FilterStringRuleEvaluator fsrv = new FilterStringEndsWith();
                        FilterRule fRule = new FilterStringRule(pvp, fsrv, stringValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else
                    {
                        message = $"Invalid rule '{rule}' for parameter value type 'string'.";
                        elementParameterFilter = null;
                        return Result.Failed;
                    }
                }
                else if (value is ElementId elementIdValue)
                {
                    if (rule == "Equals")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericEquals();
                        FilterRule fRule = new FilterElementIdRule(pvp, fnrv, elementIdValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "Greater")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericGreater();
                        FilterRule fRule = new FilterElementIdRule(pvp, fnrv, elementIdValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "GreaterOrEqual")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericGreaterOrEqual();
                        FilterRule fRule = new FilterElementIdRule(pvp, fnrv, elementIdValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "Less")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericLess();
                        FilterRule fRule = new FilterElementIdRule(pvp, fnrv, elementIdValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else if (rule == "LessOrEqual")
                    {
                        // evaluator
                        FilterNumericRuleEvaluator fnrv = new FilterNumericLessOrEqual();
                        FilterRule fRule = new FilterElementIdRule(pvp, fnrv, elementIdValue);
                        elementParameterFilter = new ElementParameterFilter(fRule);
                    }
                    else
                    {
                        message = $"Invalid rule '{rule}' for parameter value type 'elementid'.";
                        elementParameterFilter = null;
                        return Result.Failed;
                    }
                }
                else
                {
                    message = "Reference parameter value must be of type int, double, string or elementid.";
                    elementParameterFilter = null;
                    return Result.Failed;
                }
            }

            catch (Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when creating filter:\n" + ex.Message;
                elementParameterFilter = null;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}

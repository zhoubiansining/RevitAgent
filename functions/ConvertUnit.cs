using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitFunctions
{
    /// <summary>
    /// Convert values between different units
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    public class ConvertUnit : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            throw new System.NotImplementedException("Default unit convertion is not supported.");
        }
        public static Result ConvertToInternal(ref string message, ForgeTypeId forgeTypeId, double originalValue, out double internalValue)
        {           
            try
            {
                // convert the value to internal unit
                internalValue = UnitUtils.ConvertToInternalUnits(originalValue, forgeTypeId);
            }
            catch (System.Exception ex)
            {
                // if revit threw an exception, try to catch it
                message = "Exception occurred when converting value unit:\n" + ex.Message;
                internalValue = -1;
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}

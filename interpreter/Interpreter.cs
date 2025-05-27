using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Autodesk.Revit.UI;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;

namespace RevitAgent.Interpreter
{
    public class Response
    {        
        [JsonRequired]
        public string Receiver { get; set; }
    }
    public class ResponseToUser : Response
    {
        [JsonRequired]
        public bool End { get; set; }
        [JsonRequired]
        public string Message { get; set; }
    }
    public class ResponseToSystem : Response
    {        
        [JsonRequired]
        public string Call { get; set; }
        [JsonRequired]
        public Dictionary<string, JsonElement> Args { get; set; }
    }
    public class ResponseParser
    {
        static public string ExtractJsonString(string message)
        {
            // Regular expression to match JSON object
            string pattern = @"\{(?:[^{}]|(?<open>\{)|(?<-open>\}))+(?(open)(?!))\}";
            Match match = Regex.Match(message, pattern, RegexOptions.IgnorePatternWhitespace);

            if (match.Success)
            {
                // Return the JSON object found
                return match.Value;
            }
            return string.Empty;
        }
        static public Response ParseResponse(string jsonString)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never
            };

            try
            {
                ResponseToUser responseToUser = JsonSerializer.Deserialize<ResponseToUser>(jsonString, options);
                if (responseToUser.Receiver.ToLower() == "user")
                    return responseToUser;
                else
                    throw new Exception();
            }
            catch
            {
                try
                {
                    ResponseToSystem responseToSystem = JsonSerializer.Deserialize<ResponseToSystem>(jsonString, options);
                    if (responseToSystem.Receiver.ToLower() == "system")
                        return responseToSystem;
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
    /// <summary>
    /// Interprets act messages sent by the agent and forms response messages sent by system. Executes interface commands and process execution results.
    /// </summary>
    public class Interpreter
    {
        private bool UseChinese;
        private Executor Executor;
        public Interpreter(bool useChinese)
        {
            UseChinese = useChinese;
            Executor = new Executor();
        }
        public Response Parse(string message)
        {
            // Extract the largest JSON substring from the message
            string jsonString = ResponseParser.ExtractJsonString(message);
            Response response = ResponseParser.ParseResponse(jsonString);
            return response;
        }
        public Response Parse(string message, out string jsonString)
        {
            // Extract the largest JSON substring from the message
            jsonString = ResponseParser.ExtractJsonString(message);
            Response response = ResponseParser.ParseResponse(jsonString);
            return response;
        }
        public Result ExecuteCall(ExternalCommandData commandData, ref string message, ElementSet elements, ResponseToSystem responseToSystem, bool testModeOn)
        {
            string maybeCall = responseToSystem.Call;
            string call;
            switch (maybeCall.ToLower())
            {
                case "deleteelement":
                    {
                        call = "DeleteElement";
                        break;
                    }
                case "retrieveelement":
                    {
                        call = "RetrieveElement";
                        break;
                    }
                case "retrieveallparameters":
                    {
                        call = "RetrieveAllParameters";
                        break;
                    }
                case "retrieveparameterwithbuiltinid":
                    {
                        call = "RetrieveParameterWithBuiltInId";
                        break;
                    }
                case "retrieveparameterwithparametername":
                    {
                        call = "RetrieveParameterWithParameterName";
                        break;
                    }
                case "setparameterwithbuiltinid":
                    {
                        call = "SetParameterWithBuiltInId";
                        break;
                    }
                case "setparameterwithparametername":
                    {
                        call = "SetParameterWithParameterName";
                        break;
                    }
                case "showelements":
                    {
                        call = "ShowElements";
                        break;
                    }
                default:
                    {
                        call = maybeCall;
                        if (!testModeOn)
                        {
                            TaskDialog.Show("Warning", "Invalid call name received.");
                        }
                        break;
                    }
            }
            Dictionary<string, JsonElement> args = responseToSystem.Args;
            Result result = Executor.Execute(commandData, ref message, elements, call, args, testModeOn);
            return result;
        }
        public string FormSystemResponseMessage(Result result, string message, bool useChinese)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            if (useChinese)
            {
                string status = result == Result.Succeeded ? "成功" : "失败";
                var response = new
                {
                    Title = "调用Revit接口的执行结果",
                    Status = status,
                    Result = message
                };
                return JsonSerializer.Serialize(response, options);
            }
            else
            {
                string status = result == Result.Succeeded ? "Succeeded" : "Failed";
                var response = new
                {
                    Title = "Execution results of called Revit interface",
                    Status = status,
                    Result = message
                };
                return JsonSerializer.Serialize(response, options);
            }
        }
    }
}


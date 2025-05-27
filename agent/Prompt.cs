using System.Collections.Generic;
using System.Text.Json;
using RevitAgent.Interpreter;

namespace RevitAgent.Agent
{
    public class Prompt
    {
        public bool UseChinese;
        public string Instruction;
    }
    public class StartPrompt : Prompt
    {
        private List<string> Interfaces;
        public Dictionary<string, Dictionary<string, string>> CallsInfo;
        public StartPrompt(bool useChinese)
        {
            UseChinese = useChinese;
            Interfaces = InterfaceInfo.Interfaces;
            CallsInfo = new Dictionary<string, Dictionary<string, string>>();

            foreach (string interfaceName in Interfaces)
            {
                InterfaceInfo.RetrieveInterfaceInfo(interfaceName, useChinese, out Dictionary<string, string> interfaceInfo);               
                CallsInfo.Add(interfaceName, interfaceInfo);
            }

            if (useChinese)
            {
                Instruction = "你是一个建筑信息模型（BIM）软件 Autodesk Revit 中的智能助手，你需要根据用户的需求完成特定的 Revit 项目任务。\n";
            }
            else
            {
                Instruction = "You are an intelligent assistant in the Building Information Modeling (BIM) software Autodesk Revit, and you need to complete specific Revit project tasks according to user requirements.\n";
            }
        }
        public string ComposeCallInfo()
        {
            if (UseChinese)
            {
                string message = "下面是所有可供调用的 Revit 接口名称及其详细的功能介绍和输入输出信息，你后续进行的所有接口调用都应参考这部分内容的规定:\n";
                int cnt = 1;
                foreach (var item in CallsInfo)
                {
                    message += $"\n接口{cnt}: {item.Key}\n";
                    message += $"功能: {item.Value["functionality_complete"]}";
                    message += $"输出: {item.Value["output"]}";
                    cnt++;
                }
                return message;
            }
            else
            {
                string message = "The following are the names of all the Revit interfaces that can be called, along with detailed descriptions of their functionality and input/output information. All interface calls you make later should follow the rules in this section:\n";
                int cnt = 1;
                foreach (var item in CallsInfo)
                {
                    message += $"\nInterface {cnt}: {item.Key}\n";
                    message += $"Functionality: {item.Value["functionality_complete"]}";
                    message += $"Output: {item.Value["output"]}";
                    cnt++;
                }
                return message;
            }
        }
        public string ComposeRevitInfo()
        {
            string message = "";
            if (UseChinese)
            {
                message += "以下是一些关于 Revit 的提示信息，以便你更好地完成任务。\n";
                message += "1. Revit 是一款由 Autodesk 公司开发的建筑信息模型（BIM）软件，用于建筑、结构和机电工程等领域的设计和施工。\n" + 
                    "2. Revit 项目中，所有的建筑部件及其附属物均称为元素，元素可能代表具体的一面墙、一根柱，也可能代表一个类别或一个族（即预定义的一类部件）。元素具有唯一标识，即元素 ID，通过 ID 可以获取元素的相关参数信息。\n" +
                    "3. Revit 中的每个元素具有相应的参数，不同类别的元素具有的参数通常不同。这些参数分为两类，即内置参数和非内置参数。两类参数均有唯一的元素 ID，内置参数还有特殊的枚举类标识符（我们通常称之为 built-in ID/built-in ID name），形如 'HOST_AREA_COMPUTED'，而非内置参数则没有。\n" +
                    "4. 获取或设置参数时，如果参数为内置参数，且已知其内置 ID，则可以通过内置 ID 定位到相应的参数。否则，可以使用参数名来定位。但请注意，参数名是一个字符串，与参数的 ID 无关，也不一定是唯一的。\n" +
                    "5. 数值类参数一般存储为 double 类型。设置参数时，如果参数具有单位，则需要考虑设置的数值对应的单位是否正确。具体设置时的单位要求，请参考接口信息。\n" + 
                    "6. 用户观察项目的方式通常是 Revit 3D 视图，但视图并不会主动更新展示的内容。因此，如果需要向用户展示特定的元素或组件，请主动调用相关接口以更新视图。\n" +
                    "7. Revit 中的元素分类具有层次结构，从大到小依次为: 类别，族，族符号，族实例。请注意区分类别和类，元素的筛选更多地依靠类别进行，而不是类。这是因为，类并不属于这个层次结构，是对 Revit 所有物体更高层的抽象。\n";
            }
            else
            {
                message += "Here are some tips about Revit to help you better complete the task.\n";
                message += "1. Revit is a Building Information Modeling (BIM) software developed by Autodesk for design and construction in fields such as architecture, structure, and MEP engineering.\n" +
                    "2. In a Revit project, all building components and their attachments are called elements, which may represent a specific wall, a column, a category, or a family (i.e., a predefined type of component). Elements have unique identifiers, i.e., element IDs, which can be used to obtain related parameter information.\n" +
                    "3. Each element in Revit has corresponding parameters, and elements of different categories usually have different parameters. These parameters are divided into two categories: built-in parameters and non-built-in parameters. Both types of parameters have unique element IDs, and built-in parameters also have special enumeration class identifiers (we usually call this built-in ID/built-in ID name), such as 'HOST_AREA_COMPUTED', while non-built-in parameters do not.\n" +
                    "4. When getting or setting parameters, if the parameter is a built-in parameter and its built-in ID is known, the corresponding parameter can be located by the built-in ID. Otherwise, the parameter can be located using the parameter name. However, please note that the parameter name is a string, unrelated to the parameter ID, and not necessarily unique.\n" +
                    "5. Numeric parameters are generally stored as double types. When setting parameters, if the parameter has a unit, you need to consider whether the unit corresponding to the set value is correct. For specific unit requirements during setting, please refer to the interface information.\n" +
                    "6. Users usually observe the project in Revit 3D views, but the view does not actively update the displayed content. Therefore, if you need to show specific elements or components to the user, actively call the relevant interfaces to update the view.\n" +
                    "7. Element classification in Revit has a hierarchical structure, from large to small: category, family, family symbol, family instance. Please note the difference between category and class, element filtering relies more on category than class. This is because class does not belong to this hierarchy and is an abstraction of all objects in Revit.\n"; 
            }
            return message;
        }
        public string Compose()
        {
            string message = Instruction;
            message += ComposeRevitInfo() + '\n';
            message += ComposeCallInfo();
            return message;
        }
        public void GetComponents(out Dictionary<string, string> components)
        {
            components = new Dictionary<string, string>
            {
                { "Instruction", Instruction },
                { "RevitInfo", ComposeRevitInfo() },
                { "CallInfo", ComposeCallInfo() },
                { "All", Compose() }
            };
        }
    }
    public class ThinkPrompt : Prompt
    {
        public string Example;
        public ThinkPrompt(bool useChinese)
        {
            UseChinese = useChinese;
            if (useChinese) 
            {
                Instruction = "根据我提出的任务需求，并结合历史对话信息，请你有条理地思考接下来应该完成什么工作，应该调用哪些 Revit 接口，或者需要我补充什么必要信息。如果任务已经完成，请你思考应该如何向我报告和总结结果。请注意，你此阶段思考的内容将不会被执行，你只需以自然语言的形式回复即可。\n";
                Example = "用户想要获取所有墙实例的 ID，并且用户给出了墙的内置类别 ID 为 \"OST_Walls\"。在提供的接口中，\"RetrieveElement\" 接口可以通过内置类别过滤符合条件的元素实例。所以下一步，我们可以调用这个接口，将输入参数 \"builtInCategoryName\" 设置为 \"OST_Walls\"，\"isElementType\" 设置为 false，其余参数设置为 null。\n";
            }
            else
            {
                Instruction = "Based on the task requirements I've presented, and taking into account the historical conversation information, please think in an organized way about what should be accomplished next, what Revit interfaces should be called, or what necessary information you need me to add. If the task has been completed, please think about how you should report and summarize the results to me. Please note that what you think about at this stage will not be executed, you can simply reply in natural language.\n";
                Example = "The user wants to get the IDs of all wall instances and the user has given the built-in category ID of the wall as \"OST_Walls\". Among the interfaces provided, the \"RetrieveElement\" interface can filter the eligible element instances by the built-in category. So next, we can call this interface and set the input parameter \"builtInCategoryName\" to \"OST_Walls\", set \"isElementType\" to false and the remaining parameters to null.\n";
            }
        }
        public string Compose()
        {
            string message = Instruction;
            if (UseChinese) 
            {
                message += "\n下面是一个回复的样例:\n";
            }
            else
            {
                message += "\nHere is an example of a reply:\n";
            }
            message += Example;
            return message;
        }
        public void GetComponents(out Dictionary<string, string> components)
        {
            components = new Dictionary<string, string>
            {
                { "Instruction", Instruction },
                { "Example", Example },
                { "All", Compose() }
            };
        }
    }
    public class ActPrompt : Prompt
    {
        private List<string> Interfaces;
        public Dictionary<string, Dictionary<string, string>> CallsInfo;
        public string Example;
        public ActPrompt(bool useChinese)
        {
            UseChinese = useChinese;
            Interfaces = InterfaceInfo.Interfaces;
            CallsInfo = new Dictionary<string, Dictionary<string, string>>();

            foreach (string interfaceName in Interfaces)
            {
                InterfaceInfo.RetrieveInterfaceInfo(interfaceName, useChinese, out Dictionary<string, string> interfaceInfo);
                CallsInfo.Add(interfaceName, interfaceInfo);
            }

            if (useChinese)
            {
                Instruction = "结合任务需求及你上述思考的内容，请你用 JSON 格式完成回复。你可以选择回复给 Revit 系统以调用特定的 Revit 接口继续完成任务，也可以回复给我来询问更多的必要信息或者在任务完成后告知我结果，两种回复具有不同的格式要求。\n" +
                    "如果你选择回复给系统以调用接口，你的 JSON 格式回复应包含以下项:\n" +
                    "1. Receiver: 一个字符串，表示接收回复的目标，此时应为 \"system\"。\n" +
                    "2. Call: 一个字符串，代表你想要调用的接口名。\n" +
                    "3. Args: 一个字典，包含你要调用的接口的所有输入参数名和相应的参数值。字典的每一项中，键应为参数名，值为参数值，且参数值类型应当与接口规定匹配。请注意，所有接口均不允许缺省参数。你需要提供规定的所有输入参数，即使其值为 null。\n" +
                    "如果你希望回复给我以获取更多必要信息或者告知我结果，你的 JSON 格式回复应包含以下项:\n" +
                    "1. Receiver: 一个字符串，表示接收回复的目标，此时应为 \"user\"。\n" +
                    "2. End: 一个布尔值，表示任务在当前是否完成。如果任务已经全部完成，那么值应为 true，否则请设置为 false。当你将该值设为 true 时，你的回复 Message 应为任务完成后的结果的报告，反之则应为完成任务时所需必要信息的询问。\n" +
                    "3. Message: 一个字符串，代表你希望回复给我的文字信息，应该是结果的报告或者必要信息的询问。\n" +
                    "请你严格按照上述规定的 JSON 格式进行回复，不需要输出额外的内容来解释你的回复。如果你选择回复给系统来调用接口，我会在之后告知你执行的结果。\n";
                var exampleJsonObject = new
                {
                    Receiver = "system",                 
                    Call = "RetrieveElement",
                    Args = new Dictionary<string, object>
                    {
                        { "elementIdStrings", null },
                        { "builtInCategoryName", "OST_Walls" },
                        { "className", null },
                        { "familyIdString", null },
                        { "familySymbolIdString", null },
                        { "levelIdString", null },
                        { "isElementType", false },
                        { "parameterFilterArgs", null }
                    }
                };
                Example = JsonSerializer.Serialize(exampleJsonObject) + '\n';
            }
            else
            {
                Instruction = "Based on the task requirements and the contents of your thinking above, please reply in JSON format. You can choose to reply to the Revit system to call a specific Revit interface to continue completing the task, or reply to me to ask for more necessary information or report the results after the task is completed. The two types of replies have different format requirements.\n" +
                    "If you choose to reply to the system to call an interface, your JSON format reply should include the following items:\n" +
                    "1. Receiver: A string indicating the target of the reply, which should be \"system\" at this time.\n" +
                    "2. Call: A string representing the name of the interface you want to call.\n" +
                    "3. Args: A dictionary containing all the input parameter names and corresponding parameter values of the interface you want to call. For each item in the dictionary, the key should be the parameter name, the value should be the parameter value, and the parameter value type should match the interface requirements. Note that all interfaces do not allow default parameters. You need to supply all input parameters as specified, even if their values are null.\n" +
                    "If you want to reply to me to get more necessary information or report the results, your JSON format reply should include the following items:\n" +
                    "1. Receiver: A string indicating the target of the reply, which should be \"user\" at this time.\n" +
                    "2. End: A boolean value indicating whether the task is completed at the moment. If the task is completed, the value should be true, otherwise set it to false. When you set this value to true, your reply Message should be the report of the results after the task is completed, otherwise it should be the inquiry of the necessary information when completing the task.\n" +
                    "3. Message: A string representing the text message you want to reply to me, which should be the report of the results or the inquiry of the necessary information.\n" +
                    "Please strictly follow the JSON format specified above for your reply, and do not output additional content to explain your reply. If you choose to reply to the system to call the interface, I'll let you know the execution result later.\n";
                var exampleJsonObject = new
                {                   
                    Receiver = "system",
                    Call = "RetrieveElement",
                    Args = new Dictionary<string, object>
                    {
                        { "elementIdStrings", null },
                        { "builtInCategoryName", "OST_Walls" },
                        { "className", null },
                        { "familyIdString", null },
                        { "familySymbolIdString", null },
                        { "levelIdString", null },
                        { "isElementType", false },
                        { "parameterFilterArgs", null }
                    }
                };
                Example = JsonSerializer.Serialize(exampleJsonObject) + '\n';
            }
        }
        public string GetInstructionBrief()
        {
            if (UseChinese)
            {
                return "结合任务需求及你上述思考的内容，请你用 JSON 格式完成回复。你可以选择回复给 Revit 系统以调用特定的 Revit 接口继续完成任务，也可以回复给我来询问更多的必要信息或者在任务完成后告知我结果，两种回复具有不同的格式要求。";
            }
            else
            {
                return "Based on the task requirements and the contents of your thinking above, please reply in JSON format. You can choose to reply to the Revit system to call a specific Revit interface to continue completing the task, or reply to me to ask for more necessary information or report the results after the task is completed. The two types of replies have different format requirements.";
            }
        }
        public string ComposeCallInfo()
        {
            if (UseChinese)
            {
                string message = "为了防止你忘记，下面列出了所有可供调用的 Revit 接口名称及其简单的功能介绍。其详细的输入输出信息请参考最开始的提示。\n";
                foreach (var item in CallsInfo)
                {
                    message += $"{item.Key}: {item.Value["functionality_brief"]}";
                }
                return message;
            }
            else
            {
                string message = "Just in case you forget, the names of all available Revit interfaces and a brief description of their functions are listed below. Please refer to the prompt at the very beginning for their detailed input and output information.\n";
                foreach (var item in CallsInfo)
                {
                    message += $"{item.Key}: {item.Value["functionality_brief"]}";
                }
                return message;
            }
        }
        public string Compose()
        {
            string message = Instruction;   

            if (UseChinese)
            {
                message += "\n下面是一个格式正确的回复的样例:\n";
            }
            else
            {
                message += "\nHere is an example of a properly formatted reply:\n";
            }
            message += Example;
            message += '\n' + ComposeCallInfo();
            return message;
        }
        public void GetComponents(out Dictionary<string, string> components)
        {
            components = new Dictionary<string, string>
            {
                { "Instruction", Instruction },
                { "InstructionBrief", GetInstructionBrief() },
                { "Example", Example },
                { "CallInfo", ComposeCallInfo() },
                { "All", Compose() }
            };
        }
    }
}

using System;
using System.Collections.Generic;

namespace RevitAgent.Agent
{
    public class InterfaceInfo
    {
        public static List<string> Interfaces = new List<string> {
                    "DeleteElement", "RetrieveElement", "RetrieveAllParameters", "RetrieveParameterWithBuiltInId",
                    "RetrieveParameterWithParameterName", "SetParameterWithBuiltInId", "SetParameterWithParameterName", "ShowElements"
                };
        public static void RetrieveInterfaceInfo(string interfaceName, out Dictionary<string, string> interfaceInfo)
        {
            switch (interfaceName)
            {
                case "DeleteElement":
                    interfaceInfo = new Dictionary<string, string> {
                                { "functionality_brief_english", "This interface deletes element(s) from the Revit project given their element ID(s).\n" },
                                { "functionality_brief_chinese", "此接口会根据元素 ID 从 Revit 项目中删除元素。\n"},
                                { "functionality_complete_english",
                                    "This interface is used to delete the designated elements in the project. According to the list of string representations of element IDs provided by the input, the interface tries to find the corresponding elements and delete them one by one.\n" +
                                    "The requirements and meanings of the input parameters of this interface are as follows:\n" +
                                    "1. elementIdStrings: A list of string representations of element IDs of all elements to be deleted.\n" },
                                { "functionality_complete_chinese",
                                    "该接口用于删除项目中指定的元素。根据输入提供的元素 ID 的字符串表示的列表，接口试图找到相应的元素并逐个删除之。\n" +
                                    "该接口所需输入参数的要求及含义如下:\n" +
                                    "1. elementIdStrings: 需要删除的所有元素的 ID 的字符串表示构成的列表。\n" },
                                { "output_english", "This interface outputs a string message indicating whether the deletion was successful. If failed, a message showing the failure information is returned.\n"},
                                { "output_chinese", "此接口输出一个字符串消息，指示删除是否成功。如果失败，将返回一条显示失败信息的消息。\n"}
                            };
                    break;
                case "RetrieveElement":
                    interfaceInfo = new Dictionary<string, string> {
                                { "functionality_brief_english", "This interface filters elements in the Revit project according to several conditions and returns the matching elements' information.\n" },
                                { "functionality_brief_chinese", "该接口根据若干条件过滤 Revit 项目中的元素，并返回匹配的元素的信息。\n"},
                                { "functionality_complete_english",
                                    "This interface is used to filter elements in the project according to the input conditions, and return the collection of elements that meet the conditions. For input filter parameters that are not needed, please set them to null.\n" +
                                    "The requirements and meanings of the input parameters of this interface are as follows:\n" +
                                    "1. elementIdStrings: A list of string representations of the IDs of all elements to be considered. If this parameter is neither null nor empty, then subsequent filtering will be based on the list of elements represented by this parameter. Otherwise, subsequent filtering will be based on all project elements.\n" +
                                    "2. builtInCategoryName: The built-in category name to which the element belongs.\n" +
                                    "3. className: The class name to which the element belongs.\n" +
                                    "4. familyIdString: The string representation of the family ID to which the element belongs.\n" +
                                    "5. familySymbolIdString: The string representation of the family symbol ID to which the element belongs.\n" +
                                    "6. levelIdString: The string representation of the level ID of the element.\n" +
                                    "7. isElementType: A boolean value representing whether the element is an element type or an instance.\n" +
                                    "8. parameterFilterArgs: The list of element parameter filtering conditions. Each element of the list is a dictionary, and each dictionary must contain the following four items:\n" +
                                    "    parameterIdString: The string representation of the parameter ID. Note that this is not the name of the parameter or the built-in ID, but the element ID of the parameter. If you are unsure of the ID of the parameter, call other interfaces first to get its ID.\n" +
                                    "    valueTypeName: The type name of the parameter value (supports 'int', 'double', 'string', 'elementid').\n" +
                                    "    valueString: The string representation of the standard value for filtering. In particular, please note that if the parameter is of type 'double' and has units, the units of the standard value provided here should match the units in which the parameter is stored within Revit, usually imperial units. Please make sure that the units corresponding to the values you provide are correct, otherwise do not use parameter filtering.\n" +
                                    "    rule: The standard for filtering (relative to the standard value, numeric parameters support 'Equals', 'Greater', 'GreaterOrEqual', 'Less', 'LessOrEqual'; string parameters support 'Equals', 'Contains', 'BeginsWith', 'EndsWith';).\n" },
                                { "functionality_complete_chinese",
                                    "该接口用于根据输入的条件筛选项目中的元素，并返回符合条件的元素的集合。对于不需要的输入过滤参数，请将其设置为 null。\n" +
                                    "该接口所需输入参数的要求及含义如下:\n" +
                                    "1. elementIdStrings: 纳入考虑的所有元素的 ID 的字符串表示构成的列表。如果该参数不为 null 也不为空，那么后续的筛选将基于该参数代表的元素列表进行。否则，后续的筛选将基于全部项目元素进行。\n" +
                                    "2. builtInCategoryName: 元素所属的内置类别名。\n" +
                                    "3. className: 元素所属的类名。\n" +
                                    "4. familyIdString: 元素所属的族 ID 的字符串表示。\n" +
                                    "5. familySymbolIdString: 元素所属的族符号 ID 的字符串表示。\n" +
                                    "6. levelIdString: 元素所在的标高 ID 的字符串表示。\n" +
                                    "7. isElementType: 一个布尔值表示元素是元素类型还是实例。\n" +
                                    "8. parameterFilterArgs: 元素参数筛选条件的列表。列表中每个元素为一个字典，每个字典必须含有以下四项:\n" +
                                    "    parameterIdString: 参数 ID 的字符串表示。请注意这不是参数名或内置 ID，而是参数具有的元素 ID。如果你不确定参数的 ID，请先调用其它接口获取其 ID。\n" +
                                    "    valueTypeName: 参数值的类型名（支持'int', 'double', 'string', 'elementid'）。" +
                                    "    valueString: 筛选标准值的字符串表示。请特别注意，如果参数为 'double' 类型，且具有单位，那么此处提供的标准值的单位应与该参数在 Revit 内部的存储单位一致，通常为英制单位。请确保你提供的数值对应单位是正确的，否则请不要使用参数筛选。\n" +
                                    "    rule: 筛选的标准（相对于标准值，数值型参数支持'Equals', 'Greater', 'GreaterOrEqual', 'Less', 'LessOrEqual'; 字符串型参数支持'Equals', 'Contains', 'BeginsWith', 'EndsWith';）。\n"},
                                { "output_english", "This interface outputs the element ID, name, category name, level ID, and owner view ID of each retrieved matching element. If failed, a message showing the failure information is returned.\n"},
                                { "output_chinese", "此接口输出检索到的所有匹配元素的元素 ID、名称、所属类别的名称、所在标高的 ID 和所属视图的 ID。如果失败，将返回一条显示失败信息的消息。\n"}
                            };
                    break;
                case "RetrieveAllParameters":
                    interfaceInfo = new Dictionary<string, string> {
                                { "functionality_brief_english", "This interface retrieves all parameters of an element from the Revit project given its element ID.\n" },
                                { "functionality_brief_chinese", "此接口可从 Revit 项目中检索给定元素 ID 的所有参数并返回。\n"},
                                { "functionality_complete_english",
                                    "This interface is used to get all parameters of a specified element in the project. According to the element ID provided by the input, the interface tries to find the corresponding element and get all its parameters.\n" +
                                    "The requirements and meanings of the input parameters of this interface are as follows:\n" +
                                    "1. elementIdString: The string representation of the element ID.\n" },
                                { "functionality_complete_chinese",
                                    "该接口用于获取项目中指定元素的所有参数。根据输入提供的元素 ID，接口试图找到相应的元素并获取其所有参数。\n" +
                                    "该接口所需输入参数的要求及含义如下:\n" +
                                    "1. elementIdString: 元素 ID 的字符串表示。\n" },
                                { "output_english", "This interface outputs a string showing the parameter ID, parameter name, parameter value and the corresponding unit for each parameter of the specified element. If failed, a message showing the failure information is returned.\n"},
                                { "output_chinese", "此接口输出一个字符串，显示指定元素的各个参数的参数 ID、参数名、参数值和相应的单位。如果失败，将返回一条显示失败信息的消息。\n"}
                            };
                    break;
                case "RetrieveParameterWithBuiltInId":
                    interfaceInfo = new Dictionary<string, string> {
                                { "functionality_brief_english", "This interface retrieves a parameter of an element from the Revit project given the element ID and the built-in parameter ID.\n" },
                                { "functionality_brief_chinese", "此接口可依据元素 ID 和内置参数 ID 从 Revit 项目中检索并返回给定元素的一个参数。\n"},
                                { "functionality_complete_english",
                                    "This interface is used to get a parameter of a specified element in the project. According to the element ID and built-in parameter ID name provided by the input, the interface tries to find the corresponding element and get the specified parameter. Note that the parameter matched by the built-in parameter ID is unique. If the interface finds the matched parameter, it will fetch the parameter.\n" +
                                    "The requirements and meanings of the input parameters of this interface are as follows:\n" +
                                    "1. elementIdString: The string representation of the element ID.\n" +
                                    "2. builtInParameterName: The built-in parameter ID name of the parameter to be retrieved.\n" },
                                { "functionality_complete_chinese",
                                    "该接口用于获取项目中指定元素的一个参数。根据输入提供的元素 ID 及内置参数 ID 名，接口试图寻找到相应的元素并获取其指定参数。注意，内置参数 ID 匹配的参数是唯一的，如果接口找到了匹配的参数，接口将获取该参数。\n" +
                                    "该接口所需输入参数的要求及含义如下:\n" +
                                    "1. elementIdString: 元素 ID 的字符串表示。\n" +
                                    "2. builtInParameterName: 要获取参数的内置参数 ID 名。\n" },
                                { "output_english", "This interface outputs a string showing the parameter ID, parameter name, parameter value and the corresponding unit of the matching parameter of the specified element. If failed, a message showing the failure information is returned.\n"},
                                { "output_chinese", "此接口输出一个字符串，显示指定元素的匹配参数的参数 ID、参数名、参数值和相应的单位。如果失败，将返回一条显示失败信息的消息。\n"}
                            };
                    break;
                case "RetrieveParameterWithParameterName":
                    interfaceInfo = new Dictionary<string, string> {
                                { "functionality_brief_english", "This interface retrieves one or more parameters of an element from the Revit project given the element ID and parameter name.\n" },
                                { "functionality_brief_chinese", "此接口可依据元素 ID 和参数名从 Revit 项目中检索并返回给定元素的一个或多个参数。\n"},
                                { "functionality_complete_english",
                                    "This interface is used to get one or more parameters for a specified element in a project. Based on the element ID and parameter name provided by the input, the interface attempts to find the corresponding element and get the parameter(s) matching the name. Note that the parameter name match may not be unique. If the interface finds more than one match, the interface will fetch all matching parameters.\n" +
                                    "The requirements and meanings of the input parameters of this interface are as follows:\n" +
                                    "1. elementIdString: The string representation of the element ID.\n" +
                                    "2. parameterName: The name of the parameter to be retrieved.\n" },
                                { "functionality_complete_chinese",
                                    "该接口用于获取项目中指定元素的一个或多个参数。根据输入提供的元素 ID 及参数名，接口试图寻找到相应的元素并获取与名称相匹配的参数。注意，参数名匹配的参数可能不是唯一的，如果接口找到了多个匹配的参数，接口将获取所有匹配的参数。\n" +
                                    "该接口所需输入参数的要求及含义如下:\n" +
                                    "1. elementIdString: 元素 ID 的字符串表示。\n" +
                                    "2. parameterName: 要获取参数的参数名。\n" },
                                { "output_english", "This interface outputs a string showing the parameter ID, parameter name, parameter value and the corresponding unit for each matching parameter of the specified element. If failed, a message showing the failure information is returned.\n"},
                                { "output_chinese", "此接口输出一个字符串，显示指定元素的所有匹配参数的参数 ID、参数名、参数值和相应的单位。如果失败，将返回一条显示失败信息的消息。\n"}
                            };
                    break;
                case "SetParameterWithBuiltInId":
                    interfaceInfo = new Dictionary<string, string> {
                                { "functionality_brief_english", "This interface sets a parameter of an element in the Revit project to a new value given the element ID and the built-in parameter ID.\n" },
                                { "functionality_brief_chinese", "此接口可依据元素 ID 和内置参数 ID 将 Revit 项目中给定元素的一个内置参数设定为新的值。\n"},
                                { "functionality_complete_english",
                                    "This interface is used to set the value of a parameter of a specified element in the project. According to the element ID and built-in parameter ID name provided by the input, the interface tries to find the corresponding element and parameter. If the corresponding parameter is found, the interface will try to set it to the given value.\n" +
                                    "The requirements and meanings of the input parameters of this interface are as follows:\n" +
                                    "1. elementIdString: The string representation of the element ID.\n" +
                                    "2. builtInParameterName: The built-in parameter ID name of the parameter to be set.\n" +
                                    "3. valueTypeName: The type name of the parameter value (supports 'int', 'double', 'string', 'elementid').\n" +
                                    "4. valueString: The string representation of the parameter value to be set. Please note in particular that if the parameter is of type 'double' and has units, then the units corresponding to the new parameter value supplied here should be the same as the units output by the RetrieveParameter related interfaces, which is usually not the unit in which the parameter is stored inside Revit. Please make sure that you are supplying the correct units for the value, otherwise use the relevant interface to get the correct units for the parameter and convert them first.\n" },
                                { "functionality_complete_chinese",
                                    "该接口用于设定项目指定元素的一个参数的值。根据输入提供的元素 ID 及内置参数 ID 名，接口试图寻找到相应的元素及参数，如果找到了对应的参数，接口将尝试将其设定为所给的值。\n" +
                                    "该接口所需输入参数的要求及含义如下:\n" +
                                    "1. elementIdString: 元素 ID 的字符串表示。\n" +
                                    "2. builtInParameterName: 要设置参数的内置参数 ID 名。\n" +
                                    "3. valueTypeName: 参数值的类型名（支持'int', 'double', 'string', 'elementid'）。\n" +
                                    "4. valueString: 要设置的参数值的字符串表示。请特别注意，如果参数为 'double' 类型，且具有单位，那么此处提供的新参数值对应的单位应与 RetrieveParameter 相关接口输出的单位一致，这通常不是该参数在 Revit 内部的存储单位。请确保你提供的数值对应单位是正确的，否则请先使用相关接口获取正确的参数单位并进行单位转换。\n" },
                                { "output_english", "This interface outputs a string message indicating whether the parameter setting was successful. If failed, a message showing the failure information is returned.\n"},
                                { "output_chinese", "此接口输出一个字符串消息，指示参数设置是否成功。如果失败，将返回一条显示失败信息的消息。\n"}
                            };
                    break;
                case "SetParameterWithParameterName":
                    interfaceInfo = new Dictionary<string, string> {
                                { "functionality_brief_english", "This interface sets a parameter of an element in the Revit project to a new value given the element ID and the parameter name.\n" },
                                { "functionality_brief_chinese", "此接口可依据元素 ID 和参数名将 Revit 项目中给定元素的一个参数设定为新的值。\n"},
                                { "functionality_complete_english",
                                    "This interface is used to set the value of a parameter of a specified element in the project. According to the element ID and parameter name provided by the input, the interface tries to find the corresponding element and parameter. If a unique parameter is found, the interface will try to set it to the given value. Note that if multiple parameters with the same name are found, the interface will not modify them.\n" +
                                    "The requirements and meanings of the input parameters of this interface are as follows:\n" +
                                    "1. elementIdString: The string representation of the element ID.\n" +
                                    "2. parameterName: The name of the parameter to be set.\n" +
                                    "3. valueTypeName: The type name of the parameter value (supports 'int', 'double', 'string', 'elementid').\n" +
                                    "4. valueString: The string representation of the parameter value to be set. Please note in particular that if the parameter is of type 'double' and has units, then the units corresponding to the new parameter value supplied here should be the same as the units output by the RetrieveParameter related interfaces, which is usually not the unit in which the parameter is stored inside Revit. Please make sure that you are supplying the correct units for the value, otherwise use the relevant interface to get the correct units for the parameter and convert them first.\n" },
                                { "functionality_complete_chinese",
                                    "该接口用于设定项目指定元素的一个参数的值。根据输入提供的元素 ID 及参数名，接口试图寻找到相应的元素及参数，如果找到了唯一的参数，接口将尝试将其设定为所给的值。注意，如果找到了多个同名参数，接口将不会对其进行修改。\n" +
                                    "该接口所需输入参数的要求及含义如下:\n" +
                                    "1. elementIdString: 元素 ID 的字符串表示。\n" +
                                    "2. parameterName: 要设置的参数名称。\n" +
                                    "3. valueTypeName: 参数值的类型名（支持'int', 'double', 'string', 'elementid'）。\n" +
                                    "4. valueString: 要设置的参数值的字符串表示。请特别注意，如果参数为 'double' 类型，且具有单位，那么此处提供的新参数值对应的单位应与 RetrieveParameter 相关接口输出的单位一致，这通常不是该参数在 Revit 内部的存储单位。请确保你提供的数值对应单位是正确的，否则请先使用相关接口获取正确的参数单位并进行单位转换。\n" },
                                { "output_english", "This interface outputs a string message indicating whether the parameter setting was successful. If failed, a message showing the failure information is returned.\n"},
                                { "output_chinese", "此接口输出一个字符串消息，指示参数设置是否成功。如果失败，将返回一条显示失败信息的消息。\n"}
                            };
                    break;
                case "ShowElements":
                    interfaceInfo = new Dictionary<string, string> {
                                { "functionality_brief_english", "This interface shows specified elements in Revit 3D views with their ID.\n" },
                                { "functionality_brief_chinese", "此接口根据元素 ID 在 Revit 3D 视图中显示相应的元素。\n"},
                                { "functionality_complete_english",
                                    "This interface is used to highlight designated elements to the user in the Revit 3D views. According to the list of string representations of element IDs provided by the input, the interface tries to find the corresponding elements and show them in the 3D view.\n" +
                                    "The requirements and meanings of the input parameters of this interface are as follows:\n" +
                                    "1. elementIdStrings: A list of string representations of element IDs of all elements to be shown.\n" +
                                    "2. notice: A string message you want to convey to user after the elements are shown. You may explain the characteristics of the highlighted elements or the reason for the display.\n" },
                                { "functionality_complete_chinese",
                                    "该接口用于在 Revit 3D 视图中向用户突出显示指定元素。根据输入提供的元素 ID 的字符串表示的列表，接口试图找到相应的元素并在 3D 视图中显示之。\n" +
                                    "该接口所需输入参数的要求及含义如下:\n" +
                                    "1. elementIdStrings: 需要显示的所有元素的 ID 的字符串表示构成的列表。\n" +
                                    "2. notice: 元素显示后，你希望向用户传达的字符串信息。你可以解释突出显示的元素的特征或者显示的原因。\n" },
                                { "output_english", "This interface outputs a string message indicating whether the showing was successful. If failed, a message showing the failure information is returned.\n"},
                                { "output_chinese", "此接口输出一个字符串消息，指示显示是否成功。如果失败，将返回一条显示失败信息的消息。\n"}
                            };
                    break;
                default:
                    throw new NotImplementedException("Unrecognized interface name, please check if the interface name is correct.");
            }
        }
        public static void RetrieveInterfaceInfo(string interfaceName, bool useChinese, out Dictionary<string, string> interfaceInfoLanguageSpecified)
        {
            RetrieveInterfaceInfo(interfaceName, out Dictionary<string, string> interfaceInfo);
            interfaceInfoLanguageSpecified = new Dictionary<string, string>();
            if (useChinese)
            {
                interfaceInfoLanguageSpecified.Add("functionality_brief", interfaceInfo["functionality_brief_chinese"]);
                interfaceInfoLanguageSpecified.Add("functionality_complete", interfaceInfo["functionality_complete_chinese"]);
                interfaceInfoLanguageSpecified.Add("output", interfaceInfo["output_chinese"]);
            }
            else
            {
                interfaceInfoLanguageSpecified.Add("functionality_brief", interfaceInfo["functionality_brief_english"]);
                interfaceInfoLanguageSpecified.Add("functionality_complete", interfaceInfo["functionality_complete_english"]);
                interfaceInfoLanguageSpecified.Add("output", interfaceInfo["output_english"]);
            }
        }
    }
}

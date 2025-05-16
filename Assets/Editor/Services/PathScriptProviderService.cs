using System.Collections.Generic;
using System.Linq;

public static class PathScriptProviderService
{
    private static readonly Dictionary<ScriptType, string> Scripts = new()
    {
        { ScriptType.MonoBehaviour , "Assets/Editor/Templates/Scripts/MonoBehaviourTemplate.txt"},
        { ScriptType.ScriptableObject , "Assets/Editor/Templates/Scripts/ScriptableObjectTemplate.txt"},
        { ScriptType.Interface , "Assets/Editor/Templates/Scripts/InterfaceTemplate.txt"},
        { ScriptType.EmptyClass , "Assets/Editor/Templates/Scripts/EmptyClassTemplate.txt"},
    };

    public static string GetScriptPath(ScriptType type)
    {
        return Scripts.FirstOrDefault(script => script.Key == type).Value;
    }

    public static string GetImportPath()
    {
        return "Assets/Editor/Templates/Imports/BasicImports.txt";
    }
}

using System.Collections.Generic;
using System.Linq;
using Scriptvana.Editor.Models;

namespace Scriptvana.Editor.Services
{
    public static class PathScriptProviderService
    {
        private static readonly Dictionary<ScriptType, string> Scripts = new()
        {
            { ScriptType.MonoBehaviour, "Assets/Scriptvana/Editor/Templates/Scripts/MonoBehaviourTemplate.txt" },
            { ScriptType.ScriptableObject, "Assets/Scriptvana/Editor/Templates/Scripts/ScriptableObjectTemplate.txt" },
            { ScriptType.Interface, "Assets/Scriptvana/Editor/Templates/Scripts/InterfaceTemplate.txt" },
            { ScriptType.EmptyClass, "Assets/Scriptvana/Editor/Templates/Scripts/EmptyClassTemplate.txt" },
        };

        public static string GetScriptPath(ScriptType type)
        {
            return Scripts.FirstOrDefault(script => script.Key == type).Value;
        }

        public static string GetImportPath()
        {
            return "Assets/Scriptvana//Editor/Templates/Imports/BasicImports.txt";
        }
    }
}

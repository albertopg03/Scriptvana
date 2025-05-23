using System.Collections.Generic;
using System.Linq;
using Scriptvana.Editor.Models;

namespace Scriptvana.Editor.Services
{
    /// <summary>
    /// Servicio que provee todas las rutas importantes que requiere la tool. Principalmente dónde se sitúan
    /// las plantillas de los tipos de scripts. Todas las rutas son relativas al proyecto, desde Assets.
    /// </summary>
    public static class PathScriptProviderService
    {
        // Diccionario que almacena todas las rutas de cada tipo de plantilla de script posible de generar.
        private static readonly Dictionary<ScriptType, string> Scripts = new()
        {
            { ScriptType.MonoBehaviour, "Assets/Scriptvana/Editor/Templates/Scripts/MonoBehaviourTemplate.txt" },
            { ScriptType.ScriptableObject, "Assets/Scriptvana/Editor/Templates/Scripts/ScriptableObjectTemplate.txt" },
            { ScriptType.Interface, "Assets/Scriptvana/Editor/Templates/Scripts/InterfaceTemplate.txt" },
            { ScriptType.EmptyClass, "Assets/Scriptvana/Editor/Templates/Scripts/EmptyClassTemplate.txt" },
        };

        /// <summary>
        /// Función enargada de devolver, dado un tipo de script, dónde se encuentra su plantilla, retornando
        /// la ruta desde Assets. Esta función permite no acceder directamente al diccionario.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetScriptPath(ScriptType type)
        {
            return Scripts.FirstOrDefault(script => script.Key == type).Value;
        }

        /// <summary>
        /// Simplemente retorna la ruta donde se encuentra la plantilla con los imports básicos.
        /// </summary>
        /// <returns></returns>
        public static string GetImportPath()
        {
            return "Assets/Scriptvana//Editor/Templates/Imports/BasicImports.txt";
        }
    }
}

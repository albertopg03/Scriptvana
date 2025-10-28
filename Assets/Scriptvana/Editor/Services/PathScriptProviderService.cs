using System.Collections.Generic;
using System.Linq;
using Scriptvana.Editor.Models;
using UnityEngine;

namespace Scriptvana.Editor.Services
{
    /// <summary>
    /// Servicio que provee todas las rutas importantes que requiere la tool. Principalmente dónde se sitúan
    /// las plantillas de los tipos de scripts. Todas las rutas son relativas a Resources.
    /// </summary>
    public static class PathScriptProviderService
    {
        // Diccionario que almacena todas las rutas de cada tipo de plantilla de script dentro de Resources
        private static readonly Dictionary<ScriptType, string> Scripts = new()
        {
            { ScriptType.MonoBehaviour, "Templates/MonoBehaviourTemplate" },
            { ScriptType.ScriptableObject, "Templates/ScriptableObjectTemplate" },
            { ScriptType.Interface, "Templates/InterfaceTemplate" },
            { ScriptType.EmptyClass, "Templates/EmptyClassTemplate" },
        };

        /// <summary>
        /// Función encargada de devolver el contenido de la plantilla dado un tipo de script.
        /// </summary>
        /// <param name="type">Tipo de script</param>
        /// <returns>Contenido de la plantilla como string</returns>
        public static string GetScriptTemplate(ScriptType type)
        {
            string resourcePath = Scripts.FirstOrDefault(script => script.Key == type).Value;

            if (string.IsNullOrEmpty(resourcePath))
            {
                Debug.LogError($"No se encontró la ruta para el tipo de script: {type}");
                return null;
            }

            TextAsset templateAsset = Resources.Load<TextAsset>(resourcePath);

            if (templateAsset == null)
            {
                Debug.LogError($"No se pudo cargar la plantilla desde Resources: {resourcePath}");
                return null;
            }

            return templateAsset.text;
        }

        /// <summary>
        /// Retorna el contenido de la plantilla con los imports básicos.
        /// </summary>
        /// <returns>Contenido de los imports como string</returns>
        public static string GetImportTemplate()
        {
            TextAsset importAsset = Resources.Load<TextAsset>("Templates/Imports/BasicImports");

            if (importAsset == null)
            {
                Debug.LogWarning("No se encontró la plantilla de imports en Resources/Templates/Imports/BasicImports");
                return string.Empty;
            }

            return importAsset.text;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using Scriptvana.Editor.Models;
using UnityEditor;
using UnityEngine;
using static System.IO.File;
using Object = UnityEngine.Object;

namespace Scriptvana.Editor.Services
{
    /// <summary>
    /// Servicio que se encarga de gestionar la creación del script tras rellenar los datos del formulario básico.
    /// </summary>
    public class ScriptGeneratorService
    {
        /// <summary>
        /// Función encargada de crear un fichero dada toda su configuración. Esa configuración viene del formulario
        /// básico que indica el usuario a la hora de crear un fichero.
        /// </summary>
        /// <param name="scriptType">Tipo de script. Según el tipo, se usará un template u otro.</param>
        /// <param name="scriptName">Nombre de la clase.</param>
        /// <param name="pathRelativeToAssets">Ruta relativa al proyecto donde se creará el script.</param>
        /// <param name="nSpace">(Opcional) Nombre del namespace donde se creará el script.</param>
        /// <param name="autoReload">Este parámetro solo se usa si solo se crea un script, permitiendo recargar Unity tras crear un solo fichero.
        /// En caso de que haya varios scripts, este parámetro debe estar a false, ya que la encarga de crear scripts de forma masiva, será
        /// la función de CreateFiles.</param>
        /// <returns></returns>
        public bool CreateFile(ScriptType scriptType, string scriptName, string pathRelativeToAssets,
            string nSpace = "", bool autoReload = false)
        {
            string fullPathInAssets = Path.Combine(pathRelativeToAssets, scriptName + ".cs");
            string directoryPath = Path.GetDirectoryName(fullPathInAssets);

            // si se ha indicado una ruta y no existe ya previamente...
            if (directoryPath != null && !Directory.Exists(directoryPath))
            {
                Debug.Log($"<color=orange>Scriptvana:</color> Creando directorio: {directoryPath}");
                Directory.CreateDirectory(directoryPath);
            }

            // obtemeos las rutas del tipo de script elegido y de los imports a incluir
            string templatePath = PathScriptProviderService.GetScriptPath(scriptType);
            string templateImportsPath = PathScriptProviderService.GetImportPath();

            try
            {
                // comprobación de que no exista un fichero igual previamente
                if (!Exists(templatePath))
                {
                    Debug.LogError($"No se encontró la plantilla en: {templatePath}");
                    return false;
                }

                // se obtiene el contenido de las plantillas del tipo de script y de los imports, para luego volcar sobre el nuevo script
                string scriptContent = ReadAllText(templatePath);
                string importContent = Exists(templateImportsPath) ? ReadAllText(templateImportsPath) : "";

                // Reemplazos básicos
                scriptContent = scriptContent.Replace("{scriptName}", scriptName)
                    .Replace("{imports}", importContent);

                // Lógica del namespace opcional
                if (!string.IsNullOrWhiteSpace(nSpace))
                {
                    scriptContent = scriptContent.Replace("{namespace_open}", $"namespace {nSpace}\n{{")
                        .Replace("{namespace_close}", "}");
                }
                else
                {
                    scriptContent = scriptContent.Replace("{namespace_open}", string.Empty)
                        .Replace("{namespace_close}", string.Empty);
                }

                if (Exists(fullPathInAssets))
                {
                    Debug.LogWarning(
                        $"<color=yellow>Scriptvana:</color> El script '{scriptName}.cs' ya existe en '{fullPathInAssets}'.");
                    EditorUtility.DisplayDialog("Advertencia",
                        $"El script '{scriptName}.cs' ya existe en:\n{fullPathInAssets}", "Aceptar");
                    return false;
                }

                // se vuelva el contenido de las plantillas y de lo rellenado al nuevo script.
                WriteAllText(fullPathInAssets, scriptContent);

                // validación que solo entra si se va a crear un script
                if (autoReload)
                {
                    Debug.Log(
                        $"<color=green>Scriptvana:</color> Script generado en <color=cyan>{fullPathInAssets}</color>");
                    AssetDatabase.Refresh();

                    Object asset = AssetDatabase.LoadAssetAtPath<Object>(fullPathInAssets);
                    if (asset != null)
                    {
                        Selection.activeObject = asset;
                        EditorGUIUtility.PingObject(asset);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"<color=red>Scriptvana Error:</color> Error creando '{scriptName}.cs': {e.Message}");
                EditorUtility.DisplayDialog("Error de Generación",
                    $"No se pudo crear el script {scriptName}.cs.\nRuta: {fullPathInAssets}\nError: {e.Message}",
                    "Aceptar");
                return false;
            }
        }

        /// <summary>
        /// Permite recorrer cada uno de los scripts almacenados temporalmente para finalmente crearlos. Hace uso
        /// de la función CreateFile, que crea cada script de forma individual, y finalmente, tras terminar de
        /// recorrer cada uno de los scripts, se recarga Unity para que el motor detecte los nuevos scripts.
        /// </summary>
        /// <param name="scripts">Lista de scripts almacenados temporalmente, que se van a generar.</param>
        public void CreateFiles(Dictionary<int, ScriptDefinition> scripts)
        {
            foreach (KeyValuePair<int, ScriptDefinition> script in scripts)
            {
                CreateFile(script.Value.Type, script.Value.Name, script.Value.Path, script.Value.NSpace);
            }

            // recarga Unity para detectar el/los nuevos scripts
            AssetDatabase.Refresh();
        }
    }
}
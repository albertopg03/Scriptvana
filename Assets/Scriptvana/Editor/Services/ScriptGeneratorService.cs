using System;
using System.Collections.Generic;
using System.IO;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Persistence;
using UnityEditor;
using UnityEngine;
using static System.IO.File;
using Object = UnityEngine.Object;

namespace Scriptvana.Editor.Services
{
    /// <summary>
    /// Servicio que se encarga de gestionar la creacion del script tras rellenar los datos del formulario basico.
    /// </summary>
    public class ScriptGeneratorService
    {
        /// <summary>
        /// Crea un fichero con la configuracion indicada.
        /// </summary>
        public bool CreateFile(
            ScriptType scriptType,
            string scriptName,
            string pathRelativeToAssets,
            out string generatedFilePath,
            string nSpace = "",
            bool autoReload = false)
        {
            generatedFilePath = string.Empty;
            pathRelativeToAssets = ScriptConfigurationService.NormalizeFolderPath(pathRelativeToAssets);

            string fullPathInAssets = Path.Combine(pathRelativeToAssets, scriptName + ".cs");
            string directoryPath = Path.GetDirectoryName(fullPathInAssets);

            if (directoryPath != null && !Directory.Exists(directoryPath))
            {
                if (!RoutePersistence.AutoCreateDirectories)
                {
                    EditorUtility.DisplayDialog(
                        "Ruta no valida",
                        $"La carpeta '{directoryPath}' no existe y la creacion automatica de directorios esta desactivada.",
                        "Aceptar");
                    return false;
                }

                Debug.Log($"<color=orange>[SCRIPTVANA]:</color> Creando directorio: {directoryPath}");
                Directory.CreateDirectory(directoryPath);
            }

            try
            {
                string scriptContent = PathScriptProviderService.GetScriptTemplate(scriptType);
                string importContent = PathScriptProviderService.GetImportTemplate();

                if (string.IsNullOrEmpty(scriptContent))
                {
                    Debug.LogError($"[SCRIPTVANA]:No se pudo cargar la plantilla para el tipo: {scriptType}");
                    return false;
                }

                scriptContent = scriptContent.Replace("{scriptName}", scriptName)
                    .Replace("{imports}", importContent);

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

                string headerComment = ScriptConfigurationService.BuildHeaderComment();
                if (!string.IsNullOrWhiteSpace(headerComment))
                {
                    scriptContent = $"{headerComment}\n\n{scriptContent}";
                }

                if (Exists(fullPathInAssets))
                {
                    Debug.LogWarning(
                        $"<color=yellow>[SCRIPTVANA]:</color> El script '{scriptName}.cs' ya existe en '{fullPathInAssets}'.");
                    EditorUtility.DisplayDialog(
                        "Advertencia",
                        $"El script '{scriptName}.cs' ya existe en:\n{fullPathInAssets}",
                        "Aceptar");
                    return false;
                }

                WriteAllText(fullPathInAssets, scriptContent);
                generatedFilePath = fullPathInAssets.Replace("\\", "/");

                if (autoReload)
                {
                    Debug.Log(
                        $"<color=green>[SCRIPTVANA]:</color> Script generado en <color=cyan>{fullPathInAssets}</color>");
                    AssetDatabase.Refresh();

                    Object asset = AssetDatabase.LoadAssetAtPath<Object>(generatedFilePath);
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
                Debug.LogError($"<color=red>[SCRIPTVANA]:</color> Error creando '{scriptName}.cs': {e.Message}");
                EditorUtility.DisplayDialog(
                    "Error de Generacion",
                    $"No se pudo crear el script {scriptName}.cs.\nRuta: {fullPathInAssets}\nError: {e.Message}",
                    "Aceptar");
                return false;
            }
        }

        /// <summary>
        /// Recorre los scripts almacenados temporalmente para finalmente crearlos.
        /// </summary>
        public List<string> CreateFiles(Dictionary<int, ScriptDefinition> scripts)
        {
            List<string> generatedFiles = new List<string>();

            foreach (KeyValuePair<int, ScriptDefinition> script in scripts)
            {
                if (CreateFile(script.Value.Type, script.Value.Name, script.Value.Path,
                        out string generatedFile, script.Value.NSpace, false))
                {
                    generatedFiles.Add(generatedFile);
                }
            }

            AssetDatabase.Refresh();
            HandlePostGenerationOpen(generatedFiles);
            return generatedFiles;
        }

        private void HandlePostGenerationOpen(List<string> generatedFiles)
        {
            if (generatedFiles.Count == 0)
            {
                return;
            }

            string firstFile = generatedFiles[0];
            Object fileAsset = AssetDatabase.LoadAssetAtPath<Object>(firstFile);

            switch (GenerationPersistence.OpenMode)
            {
                case PostGenerationOpenMode.SelectScript:
                    if (fileAsset != null)
                    {
                        Selection.activeObject = fileAsset;
                        EditorGUIUtility.PingObject(fileAsset);
                    }
                    break;

                case PostGenerationOpenMode.OpenScript:
                    if (fileAsset != null)
                    {
                        AssetDatabase.OpenAsset(fileAsset);
                    }
                    break;

                case PostGenerationOpenMode.PingFolder:
                    string folderPath = Path.GetDirectoryName(firstFile)?.Replace("\\", "/");
                    if (!string.IsNullOrWhiteSpace(folderPath))
                    {
                        Object folderAsset = AssetDatabase.LoadAssetAtPath<Object>(folderPath);
                        if (folderAsset != null)
                        {
                            Selection.activeObject = folderAsset;
                            EditorGUIUtility.PingObject(folderAsset);
                        }
                    }
                    break;
            }
        }
    }
}

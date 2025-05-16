using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static System.IO.File;

public class ScriptGeneratorService
{
    public bool CreateFile(string scriptName, string pathRelativeToAssets, string nSpace = "", bool autoReload = false)
    {
        string fullPathInAssets = Path.Combine(pathRelativeToAssets, scriptName + ".cs");
        string directoryPath = Path.GetDirectoryName(fullPathInAssets);

        if (!Directory.Exists(directoryPath))
        {
            Debug.Log($"<color=orange>Scriptvana:</color> Creando directorio: {directoryPath}");
            Directory.CreateDirectory(directoryPath);
        }

        string templatePath = "Assets/Editor/Templates/Scripts/MonoBehaviourTemplate.txt";
        string templateImportsPath = "Assets/Editor/Templates/Imports/BasicImports.txt";
        
        try
        {
            if (!Exists(templatePath))
            {
                Debug.LogError($"No se encontró la plantilla en: {templatePath}");
                return false;
            }

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
                Debug.LogWarning($"<color=yellow>Scriptvana:</color> El script '{scriptName}.cs' ya existe en '{fullPathInAssets}'.");
                EditorUtility.DisplayDialog("Advertencia", $"El script '{scriptName}.cs' ya existe en:\n{fullPathInAssets}", "Aceptar");
                return false;
            }

            WriteAllText(fullPathInAssets, scriptContent);

            if (autoReload)
            {
                Debug.Log($"<color=green>Scriptvana:</color> Script generado en <color=cyan>{fullPathInAssets}</color>");
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
        catch (System.Exception e)
        {
            Debug.LogError($"<color=red>Scriptvana Error:</color> Error creando '{scriptName}.cs': {e.Message}");
            EditorUtility.DisplayDialog("Error de Generación", $"No se pudo crear el script {scriptName}.cs.\nRuta: {fullPathInAssets}\nError: {e.Message}", "Aceptar");
            return false;
        }
    }



    public void CreateFiles(Dictionary<int, ScriptDefinition> scripts)
    {
        foreach(KeyValuePair<int, ScriptDefinition> script in scripts)
        {
            CreateFile(script.Value.Name, script.Value.Path, script.Value.NSpace);
        }
        
        // recarga Unity para detectar el/los nuevos scripts
        AssetDatabase.Refresh();
    }
}

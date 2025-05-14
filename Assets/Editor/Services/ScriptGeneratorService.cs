using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static System.IO.File;

public class ScriptGeneratorService
{
    public bool CreateFile(string scriptName, string pathRelativeToAssets, bool autoReload = false)
    {
        // Usamos Path.Combine para manejar correctamente las barras (/) en diferentes sistemas operativos
        string fullPathInAssets = System.IO.Path.Combine(pathRelativeToAssets, scriptName + ".cs");

        // Obtener la ruta de la carpeta para asegurar que existe
        string directoryPath = System.IO.Path.GetDirectoryName(fullPathInAssets);

        if (!System.IO.Directory.Exists(directoryPath))
        {
            Debug.Log($"<color=orange>Scriptvana (Simple Test):</color> Creando directorio: {directoryPath}");
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        // contenido
        string scriptContent = $@"using UnityEngine;

public class {scriptName} : MonoBehaviour
{{
    // Script generado por Scriptvana (prueba simple con StreamWriter)

    void Start()
    {{
    }}

    void Update()
    {{
    }}
}}"; 

        try
        {
             // Comprobar si el archivo ya existe para no sobrescribir sin querer
             if (Exists(fullPathInAssets))
             {
                  Debug.LogWarning($"<color=yellow>Scriptvana (Simple Test):</color> El script '{scriptName}.cs' ya existe en '{fullPathInAssets}'. No se sobrescribirá.");
                  EditorUtility.DisplayDialog("Advertencia (Test)", $"El script '{scriptName}.cs' ya existe en la ruta:\n{fullPathInAssets}\nNo se generará de nuevo.", "Aceptar");
                  return false; // Salir del método si el archivo existe
             }

             WriteAllText(fullPathInAssets, scriptContent);


             if (autoReload)
             {
                 Debug.Log($"<color=green>Scriptvana (Simple Test):</color> Script '{scriptName}.cs' generado correctamente en <color=cyan>{fullPathInAssets}</color>");

                 // recarga Unity para detectar el/los nuevos scripts
                 AssetDatabase.Refresh();

                 // resaltar el script (seguramente lo quitaré luego ya que si son varios archivos, no se podrán remarcar todos)
                 Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullPathInAssets);
                 if (asset != null)
                 {
                     Selection.activeObject = asset;
                     EditorGUIUtility.PingObject(asset);

                     return true;
                 }
             }
             
        }
        catch (System.Exception e)
        {
            Debug.LogError($"<color=red>Scriptvana Error (Simple Test):</color> Falló la creación del script '{scriptName}.cs' en '{fullPathInAssets}'. Error: {e.Message}");
            EditorUtility.DisplayDialog("Error de Generación (Test)", $"No se pudo crear el script {scriptName}.cs.\nRuta: {fullPathInAssets}\nError: {e.Message}", "Aceptar");

            return false;
        }

        return true;
    }

    public void CreateFiles(List<ScriptDefinition> scripts)
    {
        foreach (ScriptDefinition script in scripts)
        {
            CreateFile(script.Name, script.Path);
        }
        
        // recarga Unity para detectar el/los nuevos scripts
        AssetDatabase.Refresh();
    }
}

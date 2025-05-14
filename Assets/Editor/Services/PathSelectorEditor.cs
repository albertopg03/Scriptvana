using UnityEditor;
using UnityEngine;

public class PathSelectorEditor 
{
    public string SelectPath(string pathSelected)
    {
        string currentPath = pathSelected;
        if (string.IsNullOrEmpty(currentPath))
        {
            currentPath = "Assets/";
        }

        string selectedPath = EditorUtility.OpenFolderPanel("Selecciona la Carpeta de Guardado", currentPath, "");

        if (!string.IsNullOrEmpty(selectedPath))
        {
            if (selectedPath.StartsWith(Application.dataPath))
            {
                pathSelected = "Assets" + selectedPath.Substring(Application.dataPath.Length);
            }
            else
            {
                EditorUtility.DisplayDialog("Error de Ruta", "La carpeta seleccionada debe estar dentro de la carpeta 'Assets' del proyecto.", "Aceptar");
                pathSelected = string.Empty;
            }
        }
        
        return pathSelected; 
    }
}

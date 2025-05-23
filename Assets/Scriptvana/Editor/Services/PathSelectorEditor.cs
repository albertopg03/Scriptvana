using UnityEditor;
using UnityEngine;

namespace Scriptvana.Editor.Services
{
    /// <summary>
    /// Provee al usuario de una ventana para elegir qué ruta desea crear un script.
    /// </summary>
    public class PathSelectorEditor 
    {
        /// <summary>
        /// Muestra una ventana para que el usuario elija una ruta, y, si la ruta elegida es válida para poder
        /// almacenar un script, entonces con esta función obtenemos esa ruta relativa a la carpeta Assets.
        /// En caso de que no sea válida, gestionamos el error aportándole feedback de lo ocurrido al usuario.
        /// </summary>
        /// <param name="pathSelected"></param>
        /// <returns></returns>
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
}

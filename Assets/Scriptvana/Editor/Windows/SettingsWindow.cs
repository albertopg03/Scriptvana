using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scriptvana.Editor.Windows
{
    /// <summary>
    /// Clase encargada de contener la lógica principal para la ventana
    /// de ajustes de usuario de la herramienta. Ajustes que podrá tocar
    /// el usuario para una mayor experiencia sobre la herramienta.
    /// </summary>
    public class SettingsWindow : EditorWindow
    {
        // contenedor principal de la ventana
        [SerializeField]
        private VisualTreeAsset visualTreeAsset;

        [MenuItem("Tools/Scriptvana Settings")]
        public static void ShowWindow()
        {
            SettingsWindow window = GetWindow<SettingsWindow>();
            window.titleContent = new GUIContent("Scriptvana Settings");
            window.minSize = new Vector2(800, 350);
            window.maxSize = new Vector2(1200, 350);
        }

        private void CreateGUI()
        {
            var root = rootVisualElement;
            var layout = visualTreeAsset.Instantiate();
            root.Add(layout);
        }
    }
}

using Scriptvana.Editor.Windows.Base;
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
    public class SettingsWindow : BaseEditorWindow<SettingsWindow>
    {
        [MenuItem("Tools/Scriptvana/Settings")]
        public static void Open() =>
            ShowWindow(typeof(SettingsWindow), "Scriptvana Settings", new Vector2(800, 350), new Vector2(1200, 350));

        protected override void OnAfterCreateGUI(VisualElement layout)
        {
            //TODO
        }
    }
}

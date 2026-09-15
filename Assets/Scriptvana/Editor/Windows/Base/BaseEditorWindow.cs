using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace Scriptvana.Editor.Windows.Base
{
    public abstract class BaseEditorWindow<T> : EditorWindow where T : EditorWindow
    {
        /// <summary>
        /// Ruta relativa a Resources del Visual Tree que se cargara para la ventana.
        /// </summary>
        protected abstract string VisualTreeResourcePath { get; }
        protected virtual IReadOnlyList<string> StyleSheetResourcePaths => Array.Empty<string>();

        /// <summary>
        /// Crea y muestra la ventana.
        /// </summary>
        protected static void ShowWindow(Type windowType, string title, Vector2 minSize, Vector2 maxSize)
        {
            EditorWindow window = GetWindow(windowType);
            window.titleContent = new GUIContent(title);
            window.minSize = minSize;
            window.maxSize = maxSize;
        }

        /// <summary>
        /// Logica comun de inicializacion del UI Toolkit.
        /// </summary>
        protected virtual void CreateGUI()
        {
            var root = rootVisualElement;
            root.Clear();

            VisualTreeAsset visualTree = Resources.Load<VisualTreeAsset>(VisualTreeResourcePath);
            if (visualTree == null)
            {
                string message = $"[SCRIPTVANA]: Visual tree not found at Resources path '{VisualTreeResourcePath}' for {GetType().Name}.";
                Debug.LogError(message);
                root.Add(new HelpBox(message, HelpBoxMessageType.Error));
                return;
            }

            var layout = visualTree.Instantiate();
            AttachStyleSheets(layout);
            root.Add(layout);
            OnAfterCreateGUI(layout);
        }

        private void AttachStyleSheets(VisualElement layout)
        {
            foreach (string resourcePath in StyleSheetResourcePaths)
            {
                if (string.IsNullOrWhiteSpace(resourcePath))
                {
                    continue;
                }

                StyleSheet styleSheet = Resources.Load<StyleSheet>(resourcePath);
                if (styleSheet == null)
                {
                    Debug.LogWarning($"[SCRIPTVANA]: Stylesheet not found at Resources path '{resourcePath}'.");
                    continue;
                }

                if (!layout.styleSheets.Contains(styleSheet))
                {
                    layout.styleSheets.Add(styleSheet);
                }
            }
        }

        /// <summary>
        /// Metodo para que cada ventana implemente su logica tras la carga del layout.
        /// </summary>
        protected abstract void OnAfterCreateGUI(VisualElement layout);
    }
}

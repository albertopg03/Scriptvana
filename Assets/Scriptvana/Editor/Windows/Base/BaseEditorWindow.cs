using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scriptvana.Editor.Windows.Base
{
    public abstract class BaseEditorWindow<T> : EditorWindow where T: EditorWindow
    {
        [SerializeField]
        protected VisualTreeAsset visualTreeAsset;

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
        /// Lógica común de inicialización del UI Toolkit.
        /// </summary>
        protected virtual void CreateGUI()
        {
            var root = rootVisualElement;
            var layout = visualTreeAsset.Instantiate();
            root.Add(layout);

            OnAfterCreateGUI(layout);
        }

        /// <summary>
        /// Método para que cada ventana implemente su lógica tras la carga del layout.
        /// </summary>
        protected abstract void OnAfterCreateGUI(VisualElement layout);
    }
}

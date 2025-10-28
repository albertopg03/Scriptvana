using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scriptvana.Editor.Windows.Base
{
    public abstract class BaseEditorWindow<T> : EditorWindow where T : EditorWindow
    {
        /// <summary>
        /// Ruta relativa del UXML dentro de Resources/UI/
        /// Ejemplo: "SettingsWindow" cargar� "Resources/UI/SettingsWindow.uxml"
        /// </summary>
        protected abstract string UxmlResourcePath { get; }

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
        /// L�gica com�n de inicializaci�n del UI Toolkit.
        /// </summary>
        protected virtual void CreateGUI()
        {
            var root = rootVisualElement;

            // Cargar UXML desde Resources
            var visualTreeAsset = Resources.Load<VisualTreeAsset>($"UI/{UxmlResourcePath}");

            if (visualTreeAsset == null)
            {
                Debug.LogError($"No se pudo cargar el UXML: Resources/UI/{UxmlResourcePath}.uxml");
                return;
            }

            var layout = visualTreeAsset.Instantiate();
            root.Add(layout);
            OnAfterCreateGUI(layout);
        }

        /// <summary>
        /// M�todo para que cada ventana implemente su l�gica tras la carga del layout.
        /// </summary>
        protected abstract void OnAfterCreateGUI(VisualElement layout);
    }
}
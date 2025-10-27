using Scriptvana.Editor.Windows.Base;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Services;
using Scriptvana.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Scriptvana.Editor.Persistence;

namespace Scriptvana.Editor.Windows
{
    /// <summary>
    /// Clase encargada de contener la lógica principal para la ventana
    /// de ajustes de usuario de la herramienta. Ajustes que podrá tocar
    /// el usuario para una mayor experiencia sobre la herramienta.
    /// </summary>
    public class SettingsWindow : BaseEditorWindow<SettingsWindow>
    {
        // campos del formulario
        private IntegerField _minCharactersField;

        // botón de guardado
        private Button _saveSettingsButton;


        [MenuItem("Tools/Scriptvana/Settings")]
        public static void Open() =>
            ShowWindow(typeof(SettingsWindow), "Scriptvana Settings", new Vector2(350, 600), new Vector2(350, 1200));

        protected override void OnAfterCreateGUI(VisualElement layout)
        {
            // iniciación de los campos
            _minCharactersField = layout.Q<IntegerField>("minCharactersField");
            _saveSettingsButton = layout.Q<Button>("saveSettingsButton");

            // eventos
            _saveSettingsButton.clicked += OnSaveSettings;

            // función de relleno de datos guardados
            PopulateSavedSettingsData();
        }

        /// <summary>
        /// Almacena todos los valores del formulario en los prefs del editor
        /// </summary>
        private void OnSaveSettings()
        {
            NormalizeNamePersistence.MinScriptNameLength = _minCharactersField.value;
        }

        /// <summary>
        /// Settea a todos los inputs del panel con los valores almacenados guardados por el usuario
        /// </summary>
        private void PopulateSavedSettingsData()
        {
            _minCharactersField.value = NormalizeNamePersistence.MinScriptNameLength;
        }
    }
}

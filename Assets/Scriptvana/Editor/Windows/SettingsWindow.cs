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
        // ScriptableObject con todos los datos de los que dispone la tool.
        [Header("Iconos")][SerializeField] private IconData iconData;

        // campos del formulario
        private IntegerField _minCharactersField;
        private Toggle _namingConvertionField;
        private Toggle _manuallyEdiablePathField;
        private TextField _defaultPathField;

        // botón de guardado
        private Button _saveSettingsButton;
        private Button _browseButton;

        [MenuItem("Tools/Scriptvana/Settings")]
        public static void Open() =>
            ShowWindow(typeof(SettingsWindow), "Scriptvana Settings", new Vector2(350, 300), new Vector2(350, 300));

        protected override void OnAfterCreateGUI(VisualElement layout)
        {
            // iniciación de los campos
            _minCharactersField = layout.Q<IntegerField>("minCharactersField");
            _saveSettingsButton = layout.Q<Button>("saveSettingsButton");
            _namingConvertionField = layout.Q<Toggle>("namingConvertionField");
            _manuallyEdiablePathField = layout.Q<Toggle>("manuallyEdiablePathField");
            _defaultPathField = layout.Q<TextField>("defaultPathField");

            _browseButton = layout.Q<Button>("browseButton");

            // eventos
            _saveSettingsButton.clicked += OnSaveSettings;
            _browseButton.clicked += OnBrowse;

            // función de relleno de datos guardados
            PopulateSavedSettingsData();

            // cambiado por iconImage de Unity 6..., permite cargar los iconos
            AddCenteredIconToButton(_browseButton, iconData.iconFolder, new Vector2(20, 20));
        }

        private void OnBrowse()
        {
            PathSelectorEditor pathSelector = new PathSelectorEditor();
            _defaultPathField.value = pathSelector.SelectPath(_defaultPathField.value);
        }

        /// <summary>
        /// Almacena todos los valores del formulario en los prefs del editor
        /// </summary>
        private void OnSaveSettings()
        {
            NormalizeNamePersistence.MinScriptNameLength = _minCharactersField.value;
            NormalizeNamePersistence.UseNormalizeName = _namingConvertionField.value;

            RoutePersistence.ManualEditablePath = _manuallyEdiablePathField.value;
            RoutePersistence.DefaultPath = _defaultPathField.value;

            Debug.Log("Ajustes guardados correctamente");
        }

        /// <summary>
        /// Settea a todos los inputs del panel con los valores almacenados guardados por el usuario
        /// </summary>
        private void PopulateSavedSettingsData()
        {
            _minCharactersField.value = NormalizeNamePersistence.MinScriptNameLength;
            _namingConvertionField.value = NormalizeNamePersistence.UseNormalizeName;

            _manuallyEdiablePathField.value = RoutePersistence.ManualEditablePath;
            _defaultPathField.value = RoutePersistence.DefaultPath;
        }

        private void AddCenteredIconToButton(Button button, Texture2D texture, Vector2 size)
        {
            button.style.flexDirection = FlexDirection.Row;
            button.style.justifyContent = Justify.Center;
            button.style.alignItems = Align.Center;

            var icon = new VisualElement();
            icon.style.backgroundImage = new StyleBackground(texture);
            icon.style.width = size.x;
            icon.style.height = size.y;

            button.Add(icon);
        }
    }
}

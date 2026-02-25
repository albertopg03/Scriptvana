using Scriptvana.Editor.Windows.Base;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Services;
using Scriptvana.Editor.Persistence;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Scriptvana.Editor.Windows.Helpers;

namespace Scriptvana.Editor.Windows
{
    public class SettingsWindow : BaseEditorWindow<SettingsWindow>
    {
        [SerializeField] private VisualTreeAsset _visualTree;
        protected override VisualTreeAsset VisualTree => _visualTree;

        private IntegerField _minCharactersField;
        private Toggle _namingConvertionField;
        private Toggle _manuallyEdiablePathField;
        private TextField _defaultPathField;
        private Button _saveSettingsButton;
        private Button _browseButton;

        [MenuItem("Tools/Scriptvana/Settings")]
        public static void Open() =>
            ShowWindow(typeof(SettingsWindow), "Scriptvana Settings", new Vector2(350, 300), new Vector2(350, 300));

        protected override void OnAfterCreateGUI(VisualElement layout)
        {
            _minCharactersField = layout.Q<IntegerField>("minCharactersField");
            _saveSettingsButton = layout.Q<Button>("saveSettingsButton");
            _namingConvertionField = layout.Q<Toggle>("namingConvertionField");
            _manuallyEdiablePathField = layout.Q<Toggle>("manuallyEdiablePathField");
            _defaultPathField = layout.Q<TextField>("defaultPathField");
            _browseButton = layout.Q<Button>("browseButton");

            _saveSettingsButton.clicked += OnSaveSettings;
            _browseButton.clicked += OnBrowse;

            PopulateSavedSettingsData();

            EditorIconHelper.AddCenteredIconToButton(_browseButton, IconData.Instance.iconFolder, new Vector2(20, 20));
        }

        private void OnBrowse()
        {
            PathSelectorEditor pathSelector = new PathSelectorEditor();
            _defaultPathField.value = pathSelector.SelectPath(_defaultPathField.value);
        }

        private void OnSaveSettings()
        {
            NormalizeNamePersistence.MinScriptNameLength = _minCharactersField.value;
            NormalizeNamePersistence.UseNormalizeName = _namingConvertionField.value;
            RoutePersistence.ManualEditablePath = _manuallyEdiablePathField.value;
            RoutePersistence.DefaultPath = _defaultPathField.value;

            Debug.Log("[SCRIPTVANA]: Ajustes guardados correctamente");
        }

        private void PopulateSavedSettingsData()
        {
            _minCharactersField.value = NormalizeNamePersistence.MinScriptNameLength;
            _namingConvertionField.value = NormalizeNamePersistence.UseNormalizeName;
            _manuallyEdiablePathField.value = RoutePersistence.ManualEditablePath;
            _defaultPathField.value = RoutePersistence.DefaultPath;
        }
    }
}
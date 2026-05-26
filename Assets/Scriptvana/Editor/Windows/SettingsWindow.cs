using System;
using System.Collections.Generic;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Persistence;
using Scriptvana.Editor.Services;
using Scriptvana.Editor.Windows.Base;
using Scriptvana.Editor.Windows.Helpers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scriptvana.Editor.Windows
{
    public class SettingsWindow : BaseEditorWindow<SettingsWindow>
    {
        [SerializeField]
        private VisualTreeAsset _visualTree;
        protected override VisualTreeAsset VisualTree => _visualTree;
        protected override IReadOnlyList<string> StyleSheetResourcePaths => new[] { "UI/UnityThemes/MainWindowStyle" };

        private IntegerField _minCharactersField;
        private DropdownField _nameNormalizationModeField;
        private DropdownField _defaultNamespaceModeField;
        private TextField _fixedNamespaceField;
        private Toggle _manuallyEdiablePathField;
        private TextField _defaultPathField;
        private Toggle _restrictToBasePathField;
        private TextField _basePathField;
        private Toggle _autoCreateDirectoriesField;
        private DropdownField _openGeneratedAssetModeField;
        private DropdownField _postGenerationBehaviorField;
        private TextField _additionalImportsField;
        private Toggle _includeHeaderCommentField;
        private TextField _headerCommentField;
        private Label _saveStatusLabel;
        private Button _saveSettingsButton;
        private Button _browseButton;
        private Button _basePathBrowseButton;

        [MenuItem("Tools/Scriptvana/Settings")]
        public static void Open() =>
            ShowWindow(typeof(SettingsWindow), "Scriptvana Settings", new Vector2(620, 720), new Vector2(620, 720));

        protected override void OnAfterCreateGUI(VisualElement layout)
        {
            _minCharactersField = layout.Q<IntegerField>("minCharactersField");
            _nameNormalizationModeField = layout.Q<DropdownField>("nameNormalizationModeField");
            _defaultNamespaceModeField = layout.Q<DropdownField>("defaultNamespaceModeField");
            _fixedNamespaceField = layout.Q<TextField>("fixedNamespaceField");
            _manuallyEdiablePathField = layout.Q<Toggle>("manuallyEdiablePathField");
            _defaultPathField = layout.Q<TextField>("defaultPathField");
            _restrictToBasePathField = layout.Q<Toggle>("restrictToBasePathField");
            _basePathField = layout.Q<TextField>("basePathField");
            _autoCreateDirectoriesField = layout.Q<Toggle>("autoCreateDirectoriesField");
            _openGeneratedAssetModeField = layout.Q<DropdownField>("openGeneratedAssetModeField");
            _postGenerationBehaviorField = layout.Q<DropdownField>("postGenerationBehaviorField");
            _additionalImportsField = layout.Q<TextField>("additionalImportsField");
            _includeHeaderCommentField = layout.Q<Toggle>("includeHeaderCommentField");
            _headerCommentField = layout.Q<TextField>("headerCommentField");
            _saveStatusLabel = layout.Q<Label>("saveStatusLabel");
            _saveSettingsButton = layout.Q<Button>("saveSettingsButton");
            _browseButton = layout.Q<Button>("browseButton");
            _basePathBrowseButton = layout.Q<Button>("basePathBrowseButton");

            ConfigureFields();
            BindEvents();
            PopulateSavedSettingsData();
            RefreshConditionalFields();
            SetSaveStatus("Review the configuration and press Save to apply it.");

            EditorIconHelper.AddCenteredIconToButton(_browseButton, IconData.Instance.iconFolder, new Vector2(20, 20));
            EditorIconHelper.AddCenteredIconToButton(_basePathBrowseButton, IconData.Instance.iconFolder, new Vector2(20, 20));
        }

        private void ConfigureFields()
        {
            _nameNormalizationModeField.choices = new List<string>(Enum.GetNames(typeof(NameNormalizationMode)));
            _defaultNamespaceModeField.choices = new List<string>(Enum.GetNames(typeof(DefaultNamespaceMode)));
            _openGeneratedAssetModeField.choices = new List<string>(Enum.GetNames(typeof(PostGenerationOpenMode)));
            _postGenerationBehaviorField.choices = new List<string>(Enum.GetNames(typeof(PostGenerationBehavior)));

            _additionalImportsField.multiline = true;
            _headerCommentField.multiline = true;
        }

        private void BindEvents()
        {
            _saveSettingsButton.clicked += OnSaveSettings;
            _browseButton.clicked += OnBrowseDefaultPath;
            _basePathBrowseButton.clicked += OnBrowseBasePath;

            _minCharactersField.RegisterValueChangedCallback(_ => MarkSettingsAsDirty());
            _nameNormalizationModeField.RegisterValueChangedCallback(_ => MarkSettingsAsDirty());
            _defaultNamespaceModeField.RegisterValueChangedCallback(_ =>
            {
                RefreshConditionalFields();
                MarkSettingsAsDirty();
            });
            _fixedNamespaceField.RegisterValueChangedCallback(_ => MarkSettingsAsDirty());
            _manuallyEdiablePathField.RegisterValueChangedCallback(_ => MarkSettingsAsDirty());
            _defaultPathField.RegisterValueChangedCallback(_ => MarkSettingsAsDirty());
            _restrictToBasePathField.RegisterValueChangedCallback(_ =>
            {
                RefreshConditionalFields();
                MarkSettingsAsDirty();
            });
            _basePathField.RegisterValueChangedCallback(_ => MarkSettingsAsDirty());
            _autoCreateDirectoriesField.RegisterValueChangedCallback(_ => MarkSettingsAsDirty());
            _openGeneratedAssetModeField.RegisterValueChangedCallback(_ => MarkSettingsAsDirty());
            _postGenerationBehaviorField.RegisterValueChangedCallback(_ => MarkSettingsAsDirty());
            _additionalImportsField.RegisterValueChangedCallback(_ => MarkSettingsAsDirty());
            _includeHeaderCommentField.RegisterValueChangedCallback(_ =>
            {
                RefreshConditionalFields();
                MarkSettingsAsDirty();
            });
            _headerCommentField.RegisterValueChangedCallback(_ => MarkSettingsAsDirty());
        }

        private void OnBrowseDefaultPath()
        {
            PathSelectorEditor pathSelector = new PathSelectorEditor();
            _defaultPathField.value = ScriptConfigurationService.NormalizeFolderPath(
                pathSelector.SelectPath(_defaultPathField.value));
        }

        private void OnBrowseBasePath()
        {
            PathSelectorEditor pathSelector = new PathSelectorEditor();
            _basePathField.value = ScriptConfigurationService.NormalizeFolderPath(
                pathSelector.SelectPath(_basePathField.value));
        }

        private void OnSaveSettings()
        {
            string basePath = ScriptConfigurationService.NormalizeFolderPath(_basePathField.value);
            string defaultPath = ScriptConfigurationService.NormalizeFolderPath(_defaultPathField.value);

            if (_restrictToBasePathField.value && !ScriptConfigurationService.IsPathInsideBasePath(defaultPath, basePath))
            {
                defaultPath = basePath;
            }

            NormalizeNamePersistence.MinScriptNameLength = Mathf.Max(1, _minCharactersField.value);
            NormalizeNamePersistence.NormalizationMode =
                Enum.Parse<NameNormalizationMode>(_nameNormalizationModeField.value);

            NamespacePersistence.DefaultMode =
                Enum.Parse<DefaultNamespaceMode>(_defaultNamespaceModeField.value);
            NamespacePersistence.FixedNamespace = _fixedNamespaceField.value?.Trim() ?? string.Empty;

            RoutePersistence.ManualEditablePath = _manuallyEdiablePathField.value;
            RoutePersistence.DefaultPath = defaultPath;
            RoutePersistence.RestrictToBasePath = _restrictToBasePathField.value;
            RoutePersistence.BasePath = basePath;
            RoutePersistence.AutoCreateDirectories = _autoCreateDirectoriesField.value;

            GenerationPersistence.OpenMode =
                Enum.Parse<PostGenerationOpenMode>(_openGeneratedAssetModeField.value);
            GenerationPersistence.PostBehavior =
                Enum.Parse<PostGenerationBehavior>(_postGenerationBehaviorField.value);

            TemplateDefaultsPersistence.AdditionalImports = _additionalImportsField.value ?? string.Empty;
            TemplateDefaultsPersistence.IncludeHeaderComment = _includeHeaderCommentField.value;
            TemplateDefaultsPersistence.HeaderCommentText = _headerCommentField.value ?? string.Empty;

            _defaultPathField.value = defaultPath;
            _basePathField.value = basePath;

            RefreshConditionalFields();
            SetSaveStatus("Settings saved correctly.");
            Debug.Log("[SCRIPTVANA]: Ajustes guardados correctamente");
        }

        private void PopulateSavedSettingsData()
        {
            _minCharactersField.value = NormalizeNamePersistence.MinScriptNameLength;
            _nameNormalizationModeField.value = NormalizeNamePersistence.NormalizationMode.ToString();

            _defaultNamespaceModeField.value = NamespacePersistence.DefaultMode.ToString();
            _fixedNamespaceField.value = NamespacePersistence.FixedNamespace;

            _manuallyEdiablePathField.value = RoutePersistence.ManualEditablePath;
            _defaultPathField.value = ScriptConfigurationService.GetDefaultScriptPath();
            _restrictToBasePathField.value = RoutePersistence.RestrictToBasePath;
            _basePathField.value = ScriptConfigurationService.GetRestrictedBasePath();
            _autoCreateDirectoriesField.value = RoutePersistence.AutoCreateDirectories;

            _openGeneratedAssetModeField.value = GenerationPersistence.OpenMode.ToString();
            _postGenerationBehaviorField.value = GenerationPersistence.PostBehavior.ToString();

            _additionalImportsField.value = TemplateDefaultsPersistence.AdditionalImports;
            _includeHeaderCommentField.value = TemplateDefaultsPersistence.IncludeHeaderComment;
            _headerCommentField.value = TemplateDefaultsPersistence.HeaderCommentText;
        }

        private void RefreshConditionalFields()
        {
            bool useFixedNamespace = _defaultNamespaceModeField.value == DefaultNamespaceMode.Fixed.ToString();
            _fixedNamespaceField.SetEnabled(useFixedNamespace);

            bool restrictToBasePath = _restrictToBasePathField.value;
            _basePathField.SetEnabled(restrictToBasePath);
            _basePathBrowseButton.SetEnabled(restrictToBasePath);

            bool includeHeaderComment = _includeHeaderCommentField.value;
            _headerCommentField.SetEnabled(includeHeaderComment);
        }

        private void MarkSettingsAsDirty()
        {
            SetSaveStatus("You have unsaved changes.");
        }

        private void SetSaveStatus(string message)
        {
            _saveStatusLabel.text = message;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainWindow : EditorWindow
{
    // contenedor principal de la ventana
    [SerializeField]
    private VisualTreeAsset visualTreeAsset;

    // campos
    private TextField _scriptNameField;
    private DropdownField _scriptTypeField;
    private TextField _nameSpaceField;
    private TextField _pathTextField;
    private Button _browseButton;
    private Button _saveButton;
    private Button _createButton;
    private Button _exitEditorModeButton;

    private ListView _scriptListView;

    // lista de scripts
    private int _indexScript = 0;
    private ScriptDefinition _selectedScript = null;
    private Dictionary<int, ScriptDefinition> scriptList = new();

    [MenuItem("Tools/Scriptvana")]
    public static void ShowWindow()
    {
        MainWindow window = GetWindow<MainWindow>();
        window.titleContent = new GUIContent("Scriptvana");
        window.minSize = new Vector2(600, 200);
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;
        var layout = visualTreeAsset.Instantiate();
        root.Add(layout);

        _scriptNameField = layout.Q<TextField>("scriptNameField");
        _scriptTypeField = layout.Q<DropdownField>("typeScriptField");
        _nameSpaceField = layout.Q<TextField>("nameSpaceField");
        _pathTextField = layout.Q<TextField>("pathTextField");
        _browseButton = layout.Q<Button>("browseButton");
        _saveButton = layout.Q<Button>("saveFormButton");
        _createButton = layout.Q<Button>("createScriptButton");
        _exitEditorModeButton = layout.Q<Button>("exitEditorModeButton");

        _scriptListView = layout.Q<ListView>("ScriptListView");
        InitScriptListView();

        _exitEditorModeButton.SetEnabled(false);
        
        _browseButton.clicked += OnBrowse;
        _saveButton.clicked += OnAdd;
        _createButton.clicked += OnGenerate;
        _exitEditorModeButton.clicked += OnExitEditorMode;
        _pathTextField.value = "Assets/";

        // valores del dropdown
        _scriptTypeField.choices = new List<string>(Enum.GetNames(typeof(ScriptType)));
    }

    private void InitScriptListView()
    {
        var items = scriptList.Values.ToList();
        _scriptListView.itemsSource = items;

        _scriptListView.makeItem = () =>
        {
            VisualElement rowOption = new VisualElement();
            rowOption.style.flexDirection = FlexDirection.Row;
            rowOption.style.justifyContent = Justify.SpaceBetween;

            Label label = new Label();
            label.name = "scriptLabel";
            label.style.flexGrow = 1;

            Button deleteOptionBtn = new Button();
            deleteOptionBtn.text = "X";
            deleteOptionBtn.name = "deleteButton";
            deleteOptionBtn.style.width = 20;
            deleteOptionBtn.style.marginLeft = 10;

            rowOption.Add(label);
            rowOption.Add(deleteOptionBtn);

            return rowOption;
        };

        _scriptListView.bindItem = (element, i) =>
        {
            List<ScriptDefinition> itemsLocal = scriptList.Values.ToList(); 
            ScriptDefinition dataScript = itemsLocal[i];

            Label label = element.Q<Label>("scriptLabel");
            label.text = dataScript?.Name ?? string.Empty;

            Button button = element.Q<Button>("deleteButton");
            button.clickable.clicked -= null; // limpiar handlers anteriores
            button.clickable.clicked += () =>
            {
                var keyToRemove = scriptList.FirstOrDefault(x => x.Value == dataScript).Key;
                if (scriptList.Remove(keyToRemove))
                {
                    InitScriptListView();
                    _scriptListView.Rebuild();
                }
            };
        };

        _scriptListView.selectionType = SelectionType.Single;
        _scriptListView.fixedItemHeight = 24;
        
        _scriptListView.selectionChanged += selected =>
        {
            _selectedScript = selected.FirstOrDefault() as ScriptDefinition;
            if (_selectedScript != null)
            {
                _exitEditorModeButton.SetEnabled(true);
                RefreshForm(_selectedScript);
            }
        };

        /*
        _scriptListView.RegisterCallback<FocusOutEvent>(evt =>
        {
            indexSelectedScript = -1;
            _selectedScript = null;
        });
        */
    }

    private void RefreshForm(ScriptDefinition scriptSelected)
    {
        Debug.Log("ID SELECCIONADO -> "  + scriptSelected.Id);
        
        _scriptNameField.value = scriptSelected.Name;
        _pathTextField.value = scriptSelected.Path;
        _scriptTypeField.value = scriptSelected.Type.ToString();
    }


    private void OnBrowse()
    {
        PathSelectorEditor pathSelector = new PathSelectorEditor();
        _pathTextField.value = pathSelector.SelectPath(_pathTextField.value);
    }

    private void OnAdd()
    {
        ScriptType scriptTypeSelected = (ScriptType)Enum.Parse(typeof(ScriptType), _scriptTypeField.value);

        if (_selectedScript != null)
        {
            // Comparar si hubo cambios
            bool hasChanges =
                _selectedScript.Name != _scriptNameField.value ||
                _selectedScript.Type != scriptTypeSelected ||
                _selectedScript.Path != _pathTextField.value ||
                _selectedScript.NSpace != _nameSpaceField.value;

            if (hasChanges)
            {
                // Actualizar script existente
                _selectedScript.Name = _scriptNameField.value;
                _selectedScript.Type = scriptTypeSelected;
                _selectedScript.Path = _pathTextField.value;
                _selectedScript.NSpace = _nameSpaceField.value;

                Debug.Log($"Script actualizado: {_selectedScript.Name}");
            }
            else
            {
                Debug.Log("Sin cambios detectados. Nada que hacer.");
            }
        }
        else
        {
            // Crear nuevo script
            var newScript = new ScriptDefinition(
                _indexScript,
                _scriptNameField.value,
                scriptTypeSelected,
                _nameSpaceField.value,
                _pathTextField.value
            );

            scriptList.Add(_indexScript, newScript);
            _indexScript++;

            Debug.Log($"Script nuevo añadido: {newScript.Name}");
        }

        // Refrescar UI
        _scriptListView.itemsSource = scriptList.Values.ToList();
        _scriptListView.Rebuild();

        // Limpiar selección para evitar confusiones
        _scriptListView.selectedIndex = -1;
        _selectedScript = null;
        
        // limpiar formulario
        ClearForm();
    }


    private void OnGenerate()
    {
        ScriptGeneratorService generator = new ScriptGeneratorService();
        generator.CreateFiles(scriptList);
    }

    private void ClearForm()
    {
        _scriptNameField.value = "";
        _nameSpaceField.value = "";
        _pathTextField.value = "Assets/";
        _scriptTypeField.value = _scriptTypeField.choices.FirstOrDefault();
    }

    private void OnExitEditorMode()
    {
        _scriptListView.ClearSelection();
        _selectedScript = null;
        _exitEditorModeButton.SetEnabled(false);
        
        ClearForm();
    }
}

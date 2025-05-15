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

    private ListView _scriptListView;

    // lista de scripts
    private int indexScript = 0;
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

        _scriptListView = layout.Q<ListView>("ScriptListView");
        InitScriptListView();

        _browseButton.clicked += OnBrowse;
        _saveButton.clicked += OnAdd;
        _createButton.clicked += OnGenerate;
        _pathTextField.value = "Assets/";

        // valores del dropdown
        _scriptTypeField.choices = new List<string>(Enum.GetNames(typeof(ScriptType)));
    }

    private void InitScriptListView()
    {
        _scriptListView.itemsSource = scriptList.Values.ToList();
        _scriptListView.makeItem = () => new Label();
        _scriptListView.bindItem = (element, i) =>
        {
            var scriptData = scriptList.Values.ToList()[i];
            (element as Label).text = scriptData.Name;
        };
        _scriptListView.selectionType = SelectionType.Single;
        _scriptListView.fixedItemHeight = 20;
    }

    private void OnBrowse()
    {
        PathSelectorEditor pathSelector = new PathSelectorEditor();
        _pathTextField.value = pathSelector.SelectPath(_pathTextField.value);
    }

    private void OnAdd()
    {
        ScriptType scriptTypeSelected = (ScriptType)Enum.Parse(typeof(ScriptType), _scriptTypeField.value);

        ScriptDefinition script = new ScriptDefinition(
            _scriptNameField.value,
            scriptTypeSelected,
            _nameSpaceField.value,
            _pathTextField.value
        );

        Debug.Log(script.ToString());

        scriptList.Add(indexScript, script);
        indexScript++;

        // Actualizar ListView tras añadir un nuevo script
        _scriptListView.itemsSource = scriptList.Values.ToList();
        _scriptListView.Rebuild();

        Debug.Log($"NUMERO SCRIPTS ALMACENADOS -> {scriptList.Count}");
    }

    private void OnGenerate()
    {
        ScriptGeneratorService generator = new ScriptGeneratorService();
        generator.CreateFiles(scriptList);
    }
}

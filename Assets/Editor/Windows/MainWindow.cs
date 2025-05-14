using System;
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
        _saveButton =  layout.Q<Button>("saveFormButton");

        _browseButton.clicked += OnBrawse;
        _saveButton.clicked += OnSave;
        _pathTextField.value = "Assets/";
        
        // valores del dropdown
        _scriptTypeField.choices = Enum.GetNames(typeof(ScriptType)).ToList();
    }

    private void OnBrawse()
    {
        PathSelectorEditor pathSelector = new PathSelectorEditor();
        
        _pathTextField.value = pathSelector.SelectPath(_pathTextField.value);
    }

    private void OnSave()
    {
        // quitar mas adelante de aqui esta lógica
        ScriptType scriptTypeSelected = (ScriptType)Enum.Parse(typeof(ScriptType), _scriptTypeField.value);
        
        ScriptDefinition script = new ScriptDefinition(_scriptNameField.value, scriptTypeSelected,
            _nameSpaceField.value, _pathTextField.value);

        Debug.Log(script.ToString());

        // Generacion del script
        ScriptGeneratorService generator = new ScriptGeneratorService();
        generator.CreateFile(script.Name, script.Path);
    }
}

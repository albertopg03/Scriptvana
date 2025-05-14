using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using static System.IO.File;

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

        _browseButton.clicked += OnBrowseButtonClicked;
        _saveButton.clicked += OnSave;
        _pathTextField.value = "Assets/";
        
        // valores del dropdown
        _scriptTypeField.choices = Enum.GetNames(typeof(ScriptType)).ToList();
    }
    
    private void OnBrowseButtonClicked()
    {
        string currentPath = _pathTextField.value;
        if (string.IsNullOrEmpty(currentPath))
        {
            currentPath = "Assets/";
        }

        string selectedPath = EditorUtility.OpenFolderPanel("Selecciona la Carpeta de Guardado", currentPath, "");

        if (!string.IsNullOrEmpty(selectedPath))
        {
            if (selectedPath.StartsWith(Application.dataPath))
            {
                _pathTextField.value = "Assets" + selectedPath.Substring(Application.dataPath.Length);
            }
            else
            {
                EditorUtility.DisplayDialog("Error de Ruta", "La carpeta seleccionada debe estar dentro de la carpeta 'Assets' del proyecto.", "Aceptar");
                _pathTextField.value = ""; 
            }
        }
    }

    private void OnSave()
    {
        // quitar mas adelante de aqui esta lógica
        ScriptType scriptTypeSelected = (ScriptType)Enum.Parse(typeof(ScriptType), _scriptTypeField.value);
        
        ScriptDefinition script = new ScriptDefinition(_scriptNameField.value, scriptTypeSelected,
            _nameSpaceField.value, _pathTextField.value);

        Debug.Log(script.ToString());

        CreateFile(script.Name, script.Path);
    }

    private void CreateFile(string scriptName, string pathRelativeToAssets)
    {
        // Usamos Path.Combine para manejar correctamente las barras (/) en diferentes sistemas operativos
        string fullPathInAssets = System.IO.Path.Combine(pathRelativeToAssets, scriptName + ".cs");

        // Obtener la ruta de la carpeta para asegurar que existe
        string directoryPath = System.IO.Path.GetDirectoryName(fullPathInAssets);

        if (!System.IO.Directory.Exists(directoryPath))
        {
            Debug.Log($"<color=orange>Scriptvana (Simple Test):</color> Creando directorio: {directoryPath}");
            System.IO.Directory.CreateDirectory(directoryPath);
        }

        // contenido
        string scriptContent = $@"using UnityEngine;

public class {scriptName} : MonoBehaviour
{{
    // Script generado por Scriptvana (prueba simple con StreamWriter)

    void Start()
    {{
    }}

    void Update()
    {{
    }}
}}"; 

        try
        {
             // Comprobar si el archivo ya existe para no sobrescribir sin querer
             if (Exists(fullPathInAssets))
             {
                  Debug.LogWarning($"<color=yellow>Scriptvana (Simple Test):</color> El script '{scriptName}.cs' ya existe en '{fullPathInAssets}'. No se sobrescribirá.");
                  EditorUtility.DisplayDialog("Advertencia (Test)", $"El script '{scriptName}.cs' ya existe en la ruta:\n{fullPathInAssets}\nNo se generará de nuevo.", "Aceptar");
                  return; // Salir del método si el archivo existe
             }

             WriteAllText(fullPathInAssets, scriptContent);

             Debug.Log($"<color=green>Scriptvana (Simple Test):</color> Script '{scriptName}.cs' generado correctamente en <color=cyan>{fullPathInAssets}</color>");

             // recarga Unity para detectar el/los nuevos scripts
             UnityEditor.AssetDatabase.Refresh();

             // resaltar el script (seguramente lo quitaré luego ya que si son varios archivos, no se podrán remarcar todos)
             UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(fullPathInAssets);
             if (asset != null)
             {
                 Selection.activeObject = asset;
                 EditorGUIUtility.PingObject(asset); 
             }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"<color=red>Scriptvana Error (Simple Test):</color> Falló la creación del script '{scriptName}.cs' en '{fullPathInAssets}'. Error: {e.Message}");
            EditorUtility.DisplayDialog("Error de Generación (Test)", $"No se pudo crear el script {scriptName}.cs.\nRuta: {fullPathInAssets}\nError: {e.Message}", "Aceptar");
        }
    }
}

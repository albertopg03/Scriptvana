using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;

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
    
    [MenuItem("Tools/Scriptvana")]
    public static void ShowWindow()
    {
        MainWindow window = GetWindow<MainWindow>();
        window.titleContent = new GUIContent("Scriptvana");
        window.minSize = new Vector2(800, 600);
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

        _browseButton.clicked += OnBrowseButtonClicked;
        _pathTextField.value = "Assets/";
    }
    
    private void OnBrowseButtonClicked()
    {
        string currentPath = _pathTextField.value;
        if (string.IsNullOrEmpty(currentPath))
        {
            currentPath = "Assets/"; // Valor por defecto si no hay nada escrito
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
                _pathTextField.value = ""; // Limpiar el campo si la ruta no es válida
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Services;
using Scriptvana.Icons;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scriptvana.Editor.Windows
{
    public class MainWindow : EditorWindow
    {
        // contenedor principal de la ventana
        [SerializeField]
        private VisualTreeAsset visualTreeAsset;

        [Header("Iconos")] [SerializeField] private IconData iconData;

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
            window.minSize = new Vector2(800, 320);
            window.maxSize = new Vector2(1200, 320);
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

            _browseButton.iconImage = iconData.iconFolder;

            // valores del dropdown
            _scriptTypeField.choices = new List<string>(Enum.GetNames(typeof(ScriptType)));
            _scriptTypeField.index = (int)ScriptType.MonoBehaviour;
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
                rowOption.style.alignItems = Align.Center;
                rowOption.style.paddingLeft = 4;
                rowOption.style.paddingRight = 4;

                Label label = new Label();
                label.name = "scriptLabel";
                label.AddToClassList("truncate-label"); 

                Button deleteOptionBtn = new Button();
                deleteOptionBtn.iconImage = iconData.iconClose;
                deleteOptionBtn.name = "deleteButton";
                deleteOptionBtn.style.width = 30;
                deleteOptionBtn.style.height = 20;
                deleteOptionBtn.style.marginLeft = 4;

                rowOption.Add(label);
                rowOption.Add(deleteOptionBtn);

                return rowOption;
            };


            _scriptListView.bindItem = (element, i) =>
            {
                List<ScriptDefinition> itemsLocal = scriptList.Values.ToList(); 
                ScriptDefinition dataScript = itemsLocal[i];

                Label label = element.Q<Label>("scriptLabel");
                label.text = string.Concat(dataScript?.Name ?? string.Empty, ".cs");

                Button button = element.Q<Button>("deleteButton");
                button.clickable.clicked -= null; // limpiar handlers anteriores
                button.clickable.clicked += () =>
                {
                    var keyToRemove = scriptList.FirstOrDefault(x => x.Value == dataScript).Key;
                    if (scriptList.Remove(keyToRemove))
                    {
                        InitScriptListView();
                        OnExitEditorMode();
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
        }

        private void RefreshForm(ScriptDefinition scriptSelected)
        {
            _scriptNameField.value = scriptSelected.Name;
            _pathTextField.value = scriptSelected.Path;
            _scriptTypeField.value = scriptSelected.Type.ToString();
            _nameSpaceField.value = scriptSelected.NSpace;
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
                    if (!Validation(_selectedScript, true)) return;
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
                
                if (!Validation(newScript)) return;
                
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

        private bool Validation(ScriptDefinition script, bool isEditing = false)
        {
            // objetos validadores
            var validator = new BasicScriptValidationService(script);
            var scriptListValidator = new ListScriptsValidationService(script, scriptList.Values);
        
            // listas de errores
            List<string> formErrors = validator.GetErrorMessages();
            List<string> scriptListErrors = !isEditing ? scriptListValidator.GetErrorMessages() : new List<string>();
        
            List<string> allErrors = formErrors.Concat(scriptListErrors).ToList();
            
            if (allErrors.Count > 0)
            {
                string fullErrorMessage = string.Join("\n\n", allErrors);
            
                // Mostrar diálogo con todos los errores
                EditorUtility.DisplayDialog(
                    "Error de Validación", 
                    $"Se encontraron los siguientes errores:\n\n{fullErrorMessage}", 
                    "OK"
                );

                return false;
            }

            return true;
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
}
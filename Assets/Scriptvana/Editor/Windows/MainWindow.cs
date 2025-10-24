using Scriptvana.Editor.Models;
using Scriptvana.Editor.Services;
using Scriptvana.Editor.Windows.Base;
using Scriptvana.Icons;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scriptvana.Editor.Windows
{
    /// <summary>
    /// Vista de la herramienta que se encarga de mostrar el formulario, la lista de scripts que se van añadiendo,
    /// el formulario y los botones.
    /// También recoge los datos del formulario, y llama a los servicios de rutas y validaciones para asegurarse
    /// y recoger para así mostrar al usuario si hay algún problema en algún momento del flujo de uso de la herramienta.
    /// </summary>
    public class MainWindow : BaseEditorWindow<MainWindow>
    {
        // ScriptableObject con todos los datos de los que dispone la tool.
        [Header("Iconos")] [SerializeField] private IconData iconData;

        // campos del formulario
        private TextField _scriptNameField;
        private DropdownField _scriptTypeField;
        private TextField _nameSpaceField;
        private TextField _pathTextField;
        private Button _browseButton;
        private Button _saveButton;
        private Button _createButton;
        private Button _exitEditorModeButton;

        // vista del listado de scripts agregados temporalmente, previo a su generación.
        private ListView _scriptListView;

        // lista de scripts
        private int _indexScript;
        private ScriptDefinition _selectedScript;
        private Dictionary<int, ScriptDefinition> _scriptList = new();

        [MenuItem("Tools/Scriptvana/Manager")]
        public static void Open() =>
            ShowWindow(typeof(MainWindow), "Scriptvana", new Vector2(800, 350), new Vector2(1200, 350));


        /// <summary>
        /// Genera y bindea todos los elementos de la vista con lo creado desde el UI Toolkit.
        /// </summary>
        protected override void OnAfterCreateGUI(VisualElement layout)
        {
            // bindea los campos creados desde el UI Toolkit para poder trabajarlos desde el código
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
            
            // suscribe a los botones con ciertas funciones
            _browseButton.clicked += OnBrowse;
            _saveButton.clicked += OnAdd;
            _createButton.clicked += OnGenerate;
            _exitEditorModeButton.clicked += OnExitEditorMode;
            _pathTextField.value = "Assets/";

            // cambiado por iconImage de Unity 6..., permite cargar los iconos
            AddCenteredIconToButton(_browseButton, iconData.iconFolder, new Vector2(20, 20));

            // valores del dropdown
            _scriptTypeField.choices = new List<string>(Enum.GetNames(typeof(ScriptType)));
            _scriptTypeField.index = (int)ScriptType.MonoBehaviour;
        }

        /// <summary>
        /// Inicializa de forma dinámica desde código la lista de scripts, ya que al haber un número dinámico
        /// de posibles scripts en la lista, no se pueden crear directamente desde el UI Toolkit.
        /// </summary>
        private void InitScriptListView()
        {
            var items = _scriptList.Values.ToList();
            _scriptListView.itemsSource = items;

            // crea cada elemento (item) de la lista. 
            // 1. makeItem crea el elemento, no le asigna lógica
            // 2. bindItem asigna lógica al elemento tras crearlo
            _scriptListView.makeItem = () =>
            {
                // primero crea un contenedor donde irá cada información del script añadido y le da un estilo
                VisualElement rowOption = new VisualElement();
                rowOption.style.flexDirection = FlexDirection.Row;
                rowOption.style.justifyContent = Justify.SpaceBetween;
                rowOption.style.alignItems = Align.Center;
                rowOption.style.paddingLeft = 4;
                rowOption.style.paddingRight = 4;

                // crea un label (texto) para mostrar el nombre del script
                Label label = new Label();
                label.name = "scriptLabel";
                label.AddToClassList("truncate-label"); // clase personalizada para truncar el texto si es muy largo.

                // crea el botón para eliminar dicho elemento de la lista
                Button deleteOptionBtn = new Button();
                
                // cambiado por la propiedad iconImage de Unity 6... le asigna un icono al botón
                AddCenteredIconToButton(deleteOptionBtn, iconData.iconClose, new Vector2(16, 16));
                
                // estiliza el  botón de borrado
                deleteOptionBtn.name = "deleteButton";
                deleteOptionBtn.style.width = 30;
                deleteOptionBtn.style.height = 20;
                deleteOptionBtn.style.marginLeft = 4;

                // agrega los elementos al contenedor VisualElement
                rowOption.Add(label);
                rowOption.Add(deleteOptionBtn);

                // tras crear el contenedor, lo devuelve para confirmarlo como elemento de la lista
                return rowOption;
            };

            // tras crear el elemento, lo bindea con la lógica de los scripts
            _scriptListView.bindItem = (element, i) =>
            {
                List<ScriptDefinition> itemsLocal = _scriptList.Values.ToList(); 
                ScriptDefinition dataScript = itemsLocal[i];

                Label label = element.Q<Label>("scriptLabel");
                label.text = string.Concat(dataScript?.Name ?? string.Empty, ".cs");

                Button button = element.Q<Button>("deleteButton");
                button.clickable.clicked -= null; // limpiar handlers anteriores
                button.clickable.clicked += () =>
                {
                    var keyToRemove = _scriptList.FirstOrDefault(x => x.Value == dataScript).Key;
                    if (_scriptList.Remove(keyToRemove))
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
        
        /// <summary>
        /// Función auxiliar que permite añadir un icono a un botón. Esto se ha creado para dar soporte
        /// a los iconos para versiones inferiores al Unity 6, ya que solo en esta versión en adelante,
        /// contamos con el atributo itemImage. En anteriores versiones, da error.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="texture"></param>
        /// <param name="size"></param>
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

        /// <summary>
        /// Actualiza el formulario con los datos dado un modelo de datos
        /// </summary>
        /// <param name="scriptSelected"></param>
        private void RefreshForm(ScriptDefinition scriptSelected)
        {
            _scriptNameField.value = scriptSelected.Name;
            _pathTextField.value = scriptSelected.Path;
            _scriptTypeField.value = scriptSelected.Type.ToString();
            _nameSpaceField.value = scriptSelected.NSpace;
        }

        /// <summary>
        /// Evento que ejecuta una lógica tras pulsar el botón para navegar entre rutas.
        /// </summary>
        private void OnBrowse()
        {
            PathSelectorEditor pathSelector = new PathSelectorEditor();
            _pathTextField.value = pathSelector.SelectPath(_pathTextField.value);
        }

        /// <summary>
        /// Evento para poder añadir un script nuevo a la lista temporal
        /// </summary>
        private void OnAdd()
        {
            ScriptType scriptTypeSelected = (ScriptType)Enum.Parse(typeof(ScriptType), _scriptTypeField.value);

            // si no hay un script seleccionado previamente...
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
                    var editionTmpScript = new ScriptDefinition(
                        _selectedScript.Id,
                        _scriptNameField.value,
                        scriptTypeSelected,
                        _nameSpaceField.value,
                        _pathTextField.value
                    );

                    if (!Validation(editionTmpScript, true)) return;

                    // Actualizar script existente
                    _selectedScript.Name = _scriptNameField.value;
                    _selectedScript.Type = scriptTypeSelected;
                    _selectedScript.Path = _pathTextField.value;
                    _selectedScript.NSpace = _nameSpaceField.value;

                    //Debug.Log($"Script actualizado: {_selectedScript.Name}");
                }
                else
                {
                    //Debug.Log("Sin cambios detectados. Nada que hacer.");
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
                
                _scriptList.Add(_indexScript, newScript);
                _indexScript++;
                
                //Debug.Log($"Script nuevo añadido: {newScript.Name}");
            }

            // Refrescar UI
            _scriptListView.itemsSource = _scriptList.Values.ToList();
            _scriptListView.Rebuild();

            // Limpiar selección para evitar confusiones
            _scriptListView.selectedIndex = -1;
            _selectedScript = null;
            
            // limpiar formulario
            ClearForm();
        }

        /// <summary>
        /// Función que hace de puente con los servicios de validación necesarios para mantener una coherencia
        /// y evitar posibles errores a la hora de crear los scripts.
        /// </summary>
        /// <param name="script"></param>
        /// <param name="isEditing"></param>
        /// <returns></returns>
        private bool Validation(ScriptDefinition script, bool isEditing = false)
        {
            // objetos validadores
            var validator = new BasicScriptValidationService(script);
            var scriptListValidator = new ListScriptsValidationService(script, _scriptList.Values);
        
            // listas de errores
            List<string> formErrors = validator.GetErrorMessages();
            List<string> scriptListErrors = !isEditing ? scriptListValidator.GetErrorMessages() : new List<string>();
        
            // lista con TODOS los errores posibles
            List<string> allErrors = formErrors.Concat(scriptListErrors).ToList();
            
            // si hay errores...
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

        /// <summary>
        /// Evento que se ejecuta al pulsar el botón de generar los scripts.
        /// </summary>
        private void OnGenerate()
        {
            ScriptGeneratorService generator = new ScriptGeneratorService();
            generator.CreateFiles(_scriptList);
        }


        /// <summary>
        /// Limpia/resetea el formulario
        /// </summary>
        private void ClearForm()
        {
            _scriptNameField.value = "";
            _nameSpaceField.value = "";
            _pathTextField.value = "Assets/";
            _scriptTypeField.value = _scriptTypeField.choices.FirstOrDefault();
        }

        /// <summary>
        /// Permite salir del modo edición. Este modo facilita el mantener el mismo flujo de trabajo con el formulario,
        /// pero que al darle al botón de añadir script, en lugar de agregar un script diferente a la lista, modifica
        /// el script seleccionado.
        /// Este evento permite salir de ese modo para que el usuario pueda crear sin conflicto alguno un nuevo script.
        /// </summary>
        private void OnExitEditorMode()
        {
            _scriptListView.ClearSelection();
            _selectedScript = null;
            _exitEditorModeButton.SetEnabled(false);
            
            ClearForm();
        }
    }
}
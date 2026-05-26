using Scriptvana.Editor.Models;
using Scriptvana.Editor.Persistence;
using Scriptvana.Editor.Services;
using Scriptvana.Editor.Windows.Base;
using Scriptvana.Editor.Windows.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scriptvana.Editor.Windows
{
    /// <summary>
    /// Vista de la herramienta que se encarga de mostrar el formulario, la lista de scripts que se van aniadiendo,
    /// el formulario y los botones.
    /// Tambien recoge los datos del formulario, y llama a los servicios de rutas y validaciones para asegurarse
    /// y recoger para asi­ mostrar al usuario si hay algun problema en algun momento del flujo de uso de la herramienta.
    /// </summary>
    public class MainWindow : BaseEditorWindow<MainWindow>
    {
        [SerializeField]
        private VisualTreeAsset _visualTree;
        protected override VisualTreeAsset VisualTree => _visualTree;
        protected override IReadOnlyList<string> StyleSheetResourcePaths => new[] { "UI/UnityThemes/MainWindowStyle" };

        // campos del formulario
        private TextField _scriptNameField;
        private DropdownField _scriptTypeField;
        private TextField _nameSpaceField;
        private TextField _pathTextField;
        private Label _editStatusLabel;
        private Button _browseButton;
        private Button _saveButton;
        private Button _newScriptButton;
        private Button _createButton;

        // vista del listado de scripts agregados temporalmente, previo a su generacion.
        private ListView _scriptListView;

        // lista de scripts
        private int _indexScript;
        private ScriptDefinition _selectedScript;
        private string _lastAutoNamespace = string.Empty;
        private Dictionary<int, ScriptDefinition> _scriptList = new();

        [MenuItem("Tools/Scriptvana/Manager")]
        public static void Open() =>
            ShowWindow(typeof(MainWindow), "Scriptvana", new Vector2(800, 350), new Vector2(1200, 350));


        // =========== EVENTOS

        /// <summary>
        /// Genera y bindea todos los elementos de la vista con lo creado desde el UI Toolkit.
        /// </summary>
        protected override void OnAfterCreateGUI(VisualElement layout)
        {
            // bindea los campos creados desde el UI Toolkit para poder trabajarlos desde el cÃ³digo
            _scriptNameField = layout.Q<TextField>("scriptNameField");
            _scriptTypeField = layout.Q<DropdownField>("typeScriptField");
            _nameSpaceField = layout.Q<TextField>("nameSpaceField");
            _pathTextField = layout.Q<TextField>("pathTextField");
            _editStatusLabel = layout.Q<Label>("editStatusLabel");
            _browseButton = layout.Q<Button>("browseButton");
            _saveButton = layout.Q<Button>("saveFormButton");
            _newScriptButton = layout.Q<Button>("newScriptButton");
            _createButton = layout.Q<Button>("createScriptButton");

            _scriptListView = layout.Q<ListView>("ScriptListView");
            InitScriptListView();

            // modificaciones individuales para ciertos campos
            _pathTextField.isReadOnly = !RoutePersistence.ManualEditablePath;

            // suscribe a los botones con ciertas funciones
            _browseButton.clicked += OnBrowse;
            _saveButton.clicked += OnAdd;
            _newScriptButton.clicked += OnStartNewScript;
            _createButton.clicked += OnGenerate;
            _scriptNameField.RegisterValueChangedCallback(OnScriptNameChanged);

            _createButton.SetEnabled(false);
            EditorIconHelper.AddCenteredIconToButton(_browseButton, IconData.Instance.iconFolder, new Vector2(20, 20));

            // valores del dropdown
            _scriptTypeField.choices = new List<string>(Enum.GetNames(typeof(ScriptType)));
            _scriptTypeField.index = (int)ScriptType.MonoBehaviour;
            ApplyDefaultFormValues();
            RefreshEditionUI();
        }

        /// <summary>
        /// Evento que ejecuta una logica tras pulsar el boton para navegar entre rutas.
        /// </summary>
        private void OnBrowse()
        {
            PathSelectorEditor pathSelector = new PathSelectorEditor();
            string previousPath = _pathTextField.value;
            _pathTextField.value = ScriptConfigurationService.NormalizeFolderPath(
                pathSelector.SelectPath(_pathTextField.value));

            if (_selectedScript == null)
            {
                SyncNamespaceWithCurrentPath(previousPath);
            }
        }

        /// <summary>
        /// Evento para poder aniadir un script nuevo a la lista temporal
        /// </summary>
        private void OnAdd()
        {
            string normalizedName = ScriptConfigurationService.NormalizeName(_scriptNameField.value);
            string normalizedPath = ScriptConfigurationService.NormalizeFolderPath(_pathTextField.value);
            string effectiveNamespace = ScriptConfigurationService.ResolveNamespace(
                _nameSpaceField.value,
                normalizedPath,
                normalizedName);

            _scriptNameField.value = normalizedName;
            _pathTextField.value = normalizedPath;
            _nameSpaceField.value = effectiveNamespace;

            ScriptType scriptTypeSelected = (ScriptType)Enum.Parse(typeof(ScriptType), _scriptTypeField.value);

            // si no hay un script seleccionado previamente...
            if (_selectedScript != null)
            {
                // Comparar si hubo cambios
                bool hasChanges =
                    _selectedScript.Name != normalizedName ||
                    _selectedScript.Type != scriptTypeSelected ||
                    _selectedScript.Path != normalizedPath ||
                    _selectedScript.NSpace != effectiveNamespace;

                if (hasChanges)
                {
                    var editionTmpScript = new ScriptDefinition(
                        _selectedScript.Id,
                        normalizedName,
                        scriptTypeSelected,
                        effectiveNamespace,
                        normalizedPath
                    );

                    if (!Validation(editionTmpScript, true)) return;

                    // Actualizar script existente
                    _selectedScript.Name = normalizedName;
                    _selectedScript.Type = scriptTypeSelected;
                    _selectedScript.Path = normalizedPath;
                    _selectedScript.NSpace = effectiveNamespace;
                }
            }
            else
            {
                // Crear nuevo script
                var newScript = new ScriptDefinition(
                    _indexScript,
                    normalizedName,
                    scriptTypeSelected,
                    effectiveNamespace,
                    normalizedPath
                );

                if (!Validation(newScript)) return;

                _scriptList.Add(_indexScript, newScript);
                _indexScript++;
            }

            // Refrescar UI
            _scriptListView.itemsSource = _scriptList.Values.ToList();
            _scriptListView.Rebuild();

            // habilitar dinamicamente el botÃ³n de crear 
            _createButton.SetEnabled(_scriptList.Count > 0);

            // Limpiar seleccion para evitar confusiones
            ExitEditMode();
        }

        /// <summary>
        /// Evento que se ejecuta al pulsar el botonn de generar los scripts.
        /// </summary>
        private void OnGenerate()
        {
            if (!_createButton.enabledSelf)
            {
                return;
            }

            _createButton.SetEnabled(false);

            try
            {
                ScriptGeneratorService generator = new ScriptGeneratorService();
                List<string> generatedFiles = generator.CreateFiles(_scriptList);
                if (generatedFiles.Count == 0)
                {
                    return;
                }

                switch (GenerationPersistence.PostBehavior)
                {
                    case PostGenerationBehavior.ClearFormOnly:
                        ExitEditMode();
                        break;

                    case PostGenerationBehavior.ClearListAndForm:
                        _scriptList.Clear();
                        _scriptListView.itemsSource = _scriptList.Values.ToList();
                        _scriptListView.Rebuild();
                        ExitEditMode();
                        break;
                }
            }
            finally
            {
                _createButton.SetEnabled(_scriptList.Count > 0);
            }
        }

        /// <summary>
        /// Permite salir del modo edicion. Este modo facilita el mantener el mismo flujo de trabajo con el formulario,
        /// pero que al darle al boton de aÃ±adir script, en lugar de agregar un script diferente a la lista, modifica
        /// el script seleccionado.
        /// Este evento permite salir de ese modo para que el usuario pueda crear sin conflicto alguno un nuevo script.
        /// </summary>
        private void OnStartNewScript()
        {
            ExitEditMode();
        }

        /// <summary>
        /// Permite salir del modo edicion y volver al flujo de creacion de un script nuevo.
        /// </summary>
        private void ExitEditMode()
        {
            _scriptListView.ClearSelection();
            _selectedScript = null;
            RefreshEditionUI();
            ClearForm();
        }

        // =========== Funcionalidades

        /// <summary>
        /// Inicializa de forma dinÃ¡mica desde codigo la lista de scripts, ya que al haber un numero dinamico
        /// de posibles scripts en la lista, no se pueden crear directamente desde el UI Toolkit.
        /// </summary>
        private void InitScriptListView()
        {
            var items = _scriptList.Values.ToList();
            _scriptListView.itemsSource = items;

            // crea cada elemento (item) de la lista. 
            // 1. makeItem crea el elemento, no le asigna logica
            // 2. bindItem asigna logica al elemento tras crearlo
            _scriptListView.makeItem = () =>
            {
                // primero crea un contenedor donde iran cada informacion del script aniadido y le da un estilo
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

                // crea el boton para eliminar dicho elemento de la lista
                Button deleteOptionBtn = new Button();

                EditorIconHelper.AddCenteredIconToButton(deleteOptionBtn, IconData.Instance.iconClose, new Vector2(16, 16));

                // estiliza el  boton de borrado
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

            // tras crear el elemento, lo bindea con la logica de los scripts
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
                        _scriptListView.itemsSource = _scriptList.Values.ToList();
                        ExitEditMode();
                        _scriptListView.Rebuild();

                        _createButton.SetEnabled(_scriptList.Count > 0);
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
                    RefreshForm(_selectedScript);
                }
                else
                {
                    ClearForm();
                }

                RefreshEditionUI();
            };
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

        private void OnScriptNameChanged(ChangeEvent<string> _)
        {
            if (!NamespacePersistence.UseScriptNameAsDefaultNamespace)
            {
                return;
            }

            UpdateNamespaceFromCurrentScriptName();
        }

        /// <summary>
        /// Sincroniza el estado visual del formulario en funcion de si el usuario esta creando o editando.
        /// </summary>
        private void RefreshEditionUI()
        {
            bool isEditing = _selectedScript != null;

            _saveButton.text = isEditing ? "Apply Changes" : "Add Script";
            _newScriptButton.style.display = isEditing ? DisplayStyle.Flex : DisplayStyle.None;
            _newScriptButton.SetEnabled(isEditing);
            _editStatusLabel.text = isEditing
                ? $"Editing {_selectedScript.Name}.cs"
                : "Creating a new script";
        }

        /// <summary>
        /// Funcion que hace de puente con los servicios de validacion necesarios para mantener una coherencia
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

                // Mostrar diÃ¡logo con todos los errores
                EditorUtility.DisplayDialog(
                    "Error de ValidaciÃ³n",
                    $"Se encontraron los siguientes errores:\n\n{fullErrorMessage}",
                    "OK"
                );

                return false;
            }

            return true;
        }

        /// <summary>
        /// Limpia/resetea el formulario
        /// </summary>
        private void ClearForm()
        {
            _scriptNameField.value = "";
            _pathTextField.value = ScriptConfigurationService.GetDefaultScriptPath();
            _scriptTypeField.value = _scriptTypeField.choices.FirstOrDefault();
            _nameSpaceField.value = ScriptConfigurationService.GetDefaultNamespace(_pathTextField.value, _scriptNameField.value);
            _lastAutoNamespace = _nameSpaceField.value;
        }

        private void ApplyDefaultFormValues()
        {
            ClearForm();
        }

        private void SyncNamespaceWithCurrentPath(string previousPath)
        {
            if (NamespacePersistence.UseScriptNameAsDefaultNamespace)
            {
                return;
            }

            string previousAutoNamespace = ScriptConfigurationService.GetDefaultNamespace(previousPath);
            bool shouldUpdateNamespace =
                string.IsNullOrWhiteSpace(_nameSpaceField.value) ||
                _nameSpaceField.value == previousAutoNamespace ||
                _nameSpaceField.value == _lastAutoNamespace;

            if (!shouldUpdateNamespace)
            {
                return;
            }

            _nameSpaceField.value = ScriptConfigurationService.GetDefaultNamespace(_pathTextField.value);
            _lastAutoNamespace = _nameSpaceField.value;
        }

        private void UpdateNamespaceFromCurrentScriptName()
        {
            _nameSpaceField.value = ScriptConfigurationService.BuildNamespaceFromScriptName(_scriptNameField.value);
            _lastAutoNamespace = _nameSpaceField.value;
        }
    }
}


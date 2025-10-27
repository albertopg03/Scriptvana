using System.Collections.Generic;
using System.Linq;
using Scriptvana.Editor.Models;

namespace Scriptvana.Editor.Validations.Rules
{
    /// <summary>
    /// Regla que permite validar que la lista de scripts está correcta y no haya ninguna incoherencia o conflicto
    /// entre los scripts ya almacenados y los que se van a almacenar a continuación.
    /// </summary>
    public class ScriptListViewRule : IValidationRule
    {
        private readonly IEnumerable<ScriptDefinition> _existingScripts;
        private readonly ScriptDefinition _selectedScript;

        public ScriptListViewRule(IEnumerable<ScriptDefinition> existingScripts, ScriptDefinition selectedScript)
        {
            _existingScripts = existingScripts;
            _selectedScript = selectedScript;
        }

        public ValidationResult Validate(ScriptDefinition newScript)
        {
            // Verificar duplicados por nombre y ruta
            bool existsDuplicate = _existingScripts.Any(s => 
                !_selectedScript?.Equals(s) == true && 
                s.Name.Equals(newScript.Name, System.StringComparison.OrdinalIgnoreCase) &&
                s.Path.Equals(newScript.Path, System.StringComparison.OrdinalIgnoreCase));

            if (existsDuplicate)
            {
                return ValidationResult.Invalid(
                    $"Ya existe un script con el nombre '{newScript.Name}' en la ruta '{newScript.Path}'"
                );
            }

            return ValidationResult.Valid;
        }
    }
}
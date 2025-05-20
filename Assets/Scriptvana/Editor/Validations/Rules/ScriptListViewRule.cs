using System.Collections.Generic;
using System.Linq;
using Scriptvana.Editor.Models;
using UnityEngine;

namespace Scriptvana.Editor.Validations.Rules
{
    public class ScriptListViewRule : IValidationRule
    {
        private readonly IEnumerable<ScriptDefinition> _existingScripts;

        public ScriptListViewRule(IEnumerable<ScriptDefinition> existingScripts)
        {
            _existingScripts = existingScripts;
        }

        public ValidationResult Validate(ScriptDefinition newScript)
        {
            // Verificar duplicados por nombre y ruta
            bool existsDuplicate = _existingScripts.Any(s => 
                s.Name.Equals(newScript.Name, System.StringComparison.OrdinalIgnoreCase) &&
                s.Path.Equals(newScript.Path, System.StringComparison.OrdinalIgnoreCase));

            if (existsDuplicate)
            {
                return ValidationResult.Invalid(
                    $"Ya existe un script con el nombre '{newScript.Name}' en la ruta '{newScript.Path}'",
                    ValidationSeverity.Error
                );
            }

            return ValidationResult.Valid;
        }
    }
}
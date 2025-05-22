using Scriptvana.Editor.Models;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Scriptvana.Editor.Validations.Rules
{
    public class ScriptNameRule : IValidationRule
    {
        public ValidationResult Validate(ScriptDefinition script)
        {
            if (string.IsNullOrWhiteSpace(script.Name))
                return ValidationResult.Invalid("El nombre del script no puede estar vacío");

            var regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
            if (!regex.IsMatch(script.Name))
                return ValidationResult.Invalid("El nombre solo puede contener letras, números y _");

            if (script.Name.EndsWith(" "))
                return ValidationResult.Invalid("El nombre no puede terminar con espacio");

            if (char.IsLower(script.Name[0]))
            {
                Debug.LogWarning("Recomendación: Usa PascalCase (primera letra mayúscula)");
                return ValidationResult.Invalid("Recomendación: Usa PascalCase (primera letra mayúscula)", 
                    ValidationSeverity.Warning,
                    true
                );
            }
            
            return ValidationResult.Valid;
        }
    }
}
using Scriptvana.Editor.Models;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Scriptvana.Editor.Validations.Rules
{
    /// <summary>
    /// Regla para verificar que el nombre del script indicado por el usuario, es válido y no puede probocar
    /// ningún tipo de error durante su generación.
    /// </summary>
    public class ScriptNameRule : IValidationRule
    {
        public ValidationResult Validate(ScriptDefinition script)
        {
            // verificar si está vacío
            if (string.IsNullOrWhiteSpace(script.Name))
                return ValidationResult.Invalid("El nombre del script no puede estar vacío");

            // verificar que el nombre especificado no tengas ni letras, ni números ni "_"
            var regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");
            if (!regex.IsMatch(script.Name))
                return ValidationResult.Invalid("El nombre solo puede contener letras, números y _");

            // verificar que no tenga un espacio vacío al final
            if (script.Name.EndsWith(" "))
                return ValidationResult.Invalid("El nombre no puede terminar con espacio");

            // indicarle al usuario como recomendación que utilice el formato PascalCase que es más común y 
            // recomendado para este tipo de casos que empezarlo por minúscula.
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
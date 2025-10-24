using Scriptvana.Editor.Models;
using UnityEditor;

namespace Scriptvana.Editor.Validations.Rules
{
    public class LimitCharactersNameRule : IValidationRule
    {
        public ValidationResult Validate(ScriptDefinition script)
        {
            // Comprobar que el nombre del script corresponda con el l�mite establecido
            if (script.Name.Length < EditorPrefs.GetInt("minCharactersField"))
            {
                return ValidationResult.Invalid(
                        "El n�mero de caracteres del nombre del script es superior al establecido. Consulta los ajustes de Scriptvana.",
                        ValidationSeverity.Error,
                        false
                    );
            }


            return ValidationResult.Valid;
        }
    }
}


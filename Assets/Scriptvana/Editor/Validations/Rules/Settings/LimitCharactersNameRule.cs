using Scriptvana.Editor.Models;
using Scriptvana.Editor.Persistence;
using UnityEditor;

namespace Scriptvana.Editor.Validations.Rules.Settings
{
    public class LimitCharactersNameRule : IValidationRule
    {
        public ValidationResult Validate(ScriptDefinition script)
        {
            // Comprobar que el nombre del script corresponda con el l�mite establecido
            if (script.Name.Length < NormalizeNamePersistence.MinScriptNameLength)
            {
                return ValidationResult.Invalid(
                        "El n�mero de caracteres del nombre del script es inferior al establecido. Consulta los ajustes de Scriptvana.",
                        ValidationSeverity.Error,
                        false
                    );
            }

            return ValidationResult.Valid;
        }
    }
}


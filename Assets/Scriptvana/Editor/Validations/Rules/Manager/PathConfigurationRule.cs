using System.IO;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Persistence;
using Scriptvana.Editor.Services;
using Scriptvana.Editor.Validations;

namespace Scriptvana.Editor.Validations.Rules
{
    public class PathConfigurationRule : IValidationRule
    {
        public ValidationResult Validate(ScriptDefinition script)
        {
            if (string.IsNullOrWhiteSpace(script.Path))
            {
                return ValidationResult.Invalid("La ruta del script no puede estar vacía.");
            }

            string normalizedPath = ScriptConfigurationService.NormalizeFolderPath(script.Path);
            if (!normalizedPath.StartsWith("Assets"))
            {
                return ValidationResult.Invalid("La ruta del script debe estar dentro de Assets.");
            }

            if (RoutePersistence.RestrictToBasePath)
            {
                string basePath = ScriptConfigurationService.GetRestrictedBasePath();
                if (!ScriptConfigurationService.IsPathInsideBasePath(normalizedPath, basePath))
                {
                    return ValidationResult.Invalid(
                        $"La ruta del script debe estar dentro de la ruta base configurada: {basePath}.");
                }
            }

            if (!RoutePersistence.AutoCreateDirectories && !Directory.Exists(normalizedPath))
            {
                return ValidationResult.Invalid(
                    "La carpeta indicada no existe y la creación automática de directorios está desactivada.");
            }

            return ValidationResult.Valid;
        }
    }
}

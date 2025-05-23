using Scriptvana.Editor.Models;

namespace Scriptvana.Editor.Validations
{
    /// <summary>
    /// Interfaz con el objetivo de implementar una función de validación dado un script. El script será el script
    /// que el usuario debe generar, y la función deberá comprobar que lo especificado sea correcto.
    /// </summary>
    public interface IValidationRule
    {
        ValidationResult Validate(ScriptDefinition script);
    }
    
    /// <summary>
    /// Tipo de validación.
    /// 1. Error -> fuerza un bloqueo y mensaje en una ventana emergente si se deteca un error. Hasta que no se
    /// solucione, no se puede continuar con la creación del script.
    /// 2. Warning -> recomendaciones para el usuario a la hora de especificar ciertos nombres. No es bloqueante
    /// pero que le permita al usuario entender qué es lo recomendado, por si desea o no cambiarlo posteriormente.
    /// </summary>
    public enum ValidationSeverity
    {
        Error,
        Warning
    }

    /// <summary>
    /// Clase que contiene toda la información relacionada con la respuesta de una validación:
    /// 1. IsValid -> se indica si el "error" es bloqueante o no. Si es true, entonces no bloquea la generación.
    /// 2. Message -> cualquier mensaje que alguna regla de validación haya arrojado, independientemente de su tipo.
    /// 3. Severity -> etiqueta para identificar el tipo de mensaje reportado.
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public ValidationSeverity Severity { get; set; }

        // Helper para resultados válidos
        public static ValidationResult Valid => new ValidationResult { IsValid = true };

        // Helper para crear resultados inválidos
        public static ValidationResult Invalid(string message, ValidationSeverity severity = ValidationSeverity.Error, bool isValid = false)
        {
            return new ValidationResult
            {
                IsValid = isValid,
                Message = message,
                Severity = severity
            };
        }
    }
}
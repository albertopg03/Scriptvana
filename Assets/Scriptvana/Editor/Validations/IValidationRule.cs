using Scriptvana.Editor.Models;

namespace Scriptvana.Editor.Validations
{
    public interface IValidationRule
    {
        ValidationResult Validate(ScriptDefinition script);
    }
    
    public enum ValidationSeverity
    {
        Error,
        Warning,
        Info
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public ValidationSeverity Severity { get; set; }

        // Helper para resultados válidos
        public static ValidationResult Valid => new ValidationResult { IsValid = true };

        // Helper para crear resultados inválidos
        public static ValidationResult Invalid(string message, ValidationSeverity severity = ValidationSeverity.Error)
        {
            return new ValidationResult
            {
                IsValid = false,
                Message = message,
                Severity = severity
            };
        }
    }
}
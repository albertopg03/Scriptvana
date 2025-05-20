using Scriptvana.Editor.Models;
using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Scriptvana.Editor.Validations.Rules
{
    public class NameSpaceRule : IValidationRule
    {
        public ValidationResult Validate(ScriptDefinition script)
        {
            // Namespace vacío es válido (opcional)
            if (string.IsNullOrWhiteSpace(script.NSpace))
            {
                return ValidationResult.Valid;
            }

            // Validar caracteres permitidos
            var regex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*(\.[a-zA-Z_][a-zA-Z0-9_]*)*$");
            if (!regex.IsMatch(script.NSpace))
            {
                return ValidationResult.Invalid(
                    "Formato de namespace inválido. Solo puede contener letras, números, puntos y _. Ejemplo: 'Gameplay.Characters'"
                );
            }

            // Validar no empiece/termine con punto
            if (script.NSpace.StartsWith(".") || script.NSpace.EndsWith("."))
            {
                return ValidationResult.Invalid("El namespace no puede empezar ni terminar con punto");
            }

            // Validar puntos consecutivos
            if (script.NSpace.Contains(".."))
            {
                return ValidationResult.Invalid("El namespace no puede tener puntos consecutivos");
            }

            // Validar palabras reservadas
            string[] reservedKeywords = { "Unity", "System", "UnityEngine" };
            foreach (var keyword in reservedKeywords)
            {
                if (script.NSpace.Equals(keyword, StringComparison.OrdinalIgnoreCase) || 
                    script.NSpace.StartsWith(keyword + "."))
                {
                    return ValidationResult.Invalid(
                        $"Evita usar '{keyword}' como namespace raíz (puede causar conflictos)",
                        ValidationSeverity.Warning // Cambiado a Warning ya que no es un error técnico
                    );
                }
            }

            // Recomendación de estilo (como advertencia)
            if (char.IsLower(script.NSpace[0]))
            {
                Debug.LogWarning("Recomendación: Usa PascalCase para namespaces (primera letra mayúscula)");
                
                return ValidationResult.Invalid(
                    "Recomendación: Usa PascalCase para namespaces (primera letra mayúscula)",
                    ValidationSeverity.Warning,
                    true
                );
            }

            return ValidationResult.Valid;
        }
    }
}
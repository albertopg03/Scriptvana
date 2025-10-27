using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Validations;

namespace Scriptvana.Editor.Validations.Rules.Settings
{
    public class NamingConventionRule : IValidationRule
    {
        private readonly bool _isEnabled;

        public NamingConventionRule(bool isEnabled)
        {
            _isEnabled = isEnabled;
        }

        private static readonly HashSet<string> CSharpKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "abstract","as","base","bool","break","byte","case","catch","char","checked","class","const",
            "continue","decimal","default","delegate","do","double","else","enum","event","explicit","extern",
            "false","finally","fixed","float","for","foreach","goto","if","implicit","in","int","interface",
            "internal","is","lock","long","namespace","new","null","object","operator","out","override","params",
            "private","protected","public","readonly","ref","return","sbyte","sealed","short","sizeof","stackalloc",
            "static","string","struct","switch","this","throw","true","try","typeof","uint","ulong","unchecked",
            "unsafe","ushort","using","virtual","void","volatile","while"
        };

        public ValidationResult Validate(ScriptDefinition script)
        {
            if (!_isEnabled)
                return ValidationResult.Valid;

            if (script == null)
            {
                return ValidationResult.Invalid(
                    "El ScriptDefinition es nulo.",
                    ValidationSeverity.Error,
                    false
                );
            }

            var name = script.Name?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                return ValidationResult.Invalid(
                    "El nombre del script est� vac�o.",
                    ValidationSeverity.Error,
                    false
                );
            }

            // Comprobar que no empieza por n�mero
            if (char.IsDigit(name[0]))
            {
                return ValidationResult.Invalid(
                    "El nombre del script no puede comenzar con un n�mero.",
                    ValidationSeverity.Error,
                    false
                );
            }

            // Comprobar que solo contiene caracteres v�lidos (letras, n�meros o guion bajo)
            if (!Regex.IsMatch(name, @"^[A-Za-z_][A-Za-z0-9_]*$"))
            {
                return ValidationResult.Invalid(
                    "El nombre del script contiene caracteres no v�lidos. Solo se permiten letras, n�meros y guion bajo.",
                    ValidationSeverity.Error,
                    false
                );
            }

            // Comprobar si es palabra reservada
            if (CSharpKeywords.Contains(name))
            {
                return ValidationResult.Invalid(
                    $"'{name}' es una palabra reservada de C#. Por favor, elige otro nombre.",
                    ValidationSeverity.Error,
                    false
                );
            }

            // Comprobar convenci�n de nombres
            if (!IsPascalCase(name))
            {
                return ValidationResult.Invalid(
                    $"El nombre '{name}' no cumple la convenci�n PascalCase (ejemplo: MyAwesomeScript).",
                    ValidationSeverity.Warning,
                    false
                );
            }

            return ValidationResult.Valid;
        }

        private bool IsPascalCase(string input)
        {
            // Empieza con may�scula y no contiene guiones ni underscores
            return Regex.IsMatch(input, @"^[A-Z][A-Za-z0-9]*$");
        }
    }
}

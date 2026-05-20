using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Validations;

namespace Scriptvana.Editor.Validations.Rules.Settings
{
    public class NamingConventionRule : IValidationRule
    {
        private readonly NameNormalizationMode _mode;

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

        public NamingConventionRule(NameNormalizationMode mode)
        {
            _mode = mode;
        }

        public ValidationResult Validate(ScriptDefinition script)
        {
            if (_mode == NameNormalizationMode.Disabled)
            {
                return ValidationResult.Valid;
            }

            if (script == null)
            {
                return ValidationResult.Invalid(
                    "El ScriptDefinition es nulo.",
                    ValidationSeverity.Error,
                    false
                );
            }

            string name = script.Name?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(name))
            {
                return ValidationResult.Invalid(
                    "El nombre del script esta vacio.",
                    ValidationSeverity.Error,
                    false
                );
            }

            if (char.IsDigit(name[0]))
            {
                return ValidationResult.Invalid(
                    "El nombre del script no puede comenzar con un numero.",
                    ValidationSeverity.Error,
                    false
                );
            }

            if (!Regex.IsMatch(name, @"^[A-Za-z_][A-Za-z0-9_]*$"))
            {
                return ValidationResult.Invalid(
                    "El nombre del script contiene caracteres no validos. Solo se permiten letras, numeros y guion bajo.",
                    ValidationSeverity.Error,
                    false
                );
            }

            if (CSharpKeywords.Contains(name))
            {
                return ValidationResult.Invalid(
                    $"'{name}' es una palabra reservada de C#. Por favor, elige otro nombre.",
                    ValidationSeverity.Error,
                    false
                );
            }

            if (!MatchesSelectedConvention(name))
            {
                return ValidationResult.Invalid(
                    GetConventionMessage(name),
                    ValidationSeverity.Warning,
                    false
                );
            }

            return ValidationResult.Valid;
        }

        private bool MatchesSelectedConvention(string input)
        {
            return _mode switch
            {
                NameNormalizationMode.PascalCase => Regex.IsMatch(input, @"^[A-Z][A-Za-z0-9]*$"),
                NameNormalizationMode.PascalCaseWithUnderscores => Regex.IsMatch(input, @"^[A-Z][A-Za-z0-9_]*$"),
                NameNormalizationMode.UppercaseFirstLetter => Regex.IsMatch(input, @"^[A-Z].*$"),
                _ => true
            };
        }

        private string GetConventionMessage(string name)
        {
            return _mode switch
            {
                NameNormalizationMode.PascalCaseWithUnderscores =>
                    $"El nombre '{name}' no cumple la convencion PascalCase con underscores opcionales.",
                NameNormalizationMode.UppercaseFirstLetter =>
                    $"El nombre '{name}' debe empezar con mayuscula.",
                _ =>
                    $"El nombre '{name}' no cumple la convencion PascalCase (ejemplo: MyAwesomeScript)."
            };
        }
    }
}

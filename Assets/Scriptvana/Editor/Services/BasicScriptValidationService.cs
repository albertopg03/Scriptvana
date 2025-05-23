using System.Collections.Generic;
using System.Linq;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Validations;
using Scriptvana.Editor.Validations.Rules;

namespace Scriptvana.Editor.Services
{
    /// <summary>
    /// Servicio de validación del formulario básico para crear un script.
    /// En esencia, lo que hace es engrapar "reglas" de validación en un solo script,
    /// permitiendo escalabilidad por si en el futuro se quieren añadir más validaciones.
    /// </summary>
    public class BasicScriptValidationService
    {
        private readonly ScriptDefinition _script;
        private readonly List<ValidationResult> _validationResults = new();

        public BasicScriptValidationService(ScriptDefinition script)
        {
            _script = script;
            Validate();
        }

        /// <summary>
        /// Función para obtener todos los errores que ha detectado el servicio de validación.
        /// Consideramos error si...
        /// 1. Se obtiene un mensaje no apto de validación
        /// 2. Que tenga la etiqueta error
        /// </summary>
        /// <returns></returns>
        public List<string> GetErrorMessages()
        {
            return _validationResults
                .Where(result => !result.IsValid || result.Severity == ValidationSeverity.Error)
                .Select(result => result.Message)
                .ToList();
        }

        /// <summary>
        /// Engrapador que contiene todas las validaciones que debe contemplar el formulario de creación de un script.
        /// Crea una lista de reglas de validación para ser unicamente iteradas y analizar si hay algun error.
        /// Pasamos por cada regla y almacenados los errores que puedan ocurrir.
        /// </summary>
        private void Validate()
        {
            var validators = new List<IValidationRule>
            {
                new ScriptNameRule(),
                new NameSpaceRule()
            };

            foreach (var validator in validators)
            {
                var result = validator.Validate(_script);
                if (!result.IsValid)
                {
                    _validationResults.Add(result);
                }
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Validations;
using Scriptvana.Editor.Validations.Rules;

namespace Scriptvana.Editor.Services
{
    /// <summary>
    /// Servicio de validación centrada en la lista de scripts. Cuando el usuario ha añadido un script (por lo tanto,
    /// ha pasado por la validación básica -> BasicScriptValidationService.cs), se tiene que comprobar otra serie
    /// de reglas para verificar que no hay ningún problema, ya que puede haber, por ejemplo, conflicto con scripts
    /// ya añadidos, ya que estén repetidos.
    /// </summary>
    public class ListScriptsValidationService
    {
        private readonly ScriptDefinition _script;
        private readonly IEnumerable<ScriptDefinition> _allScripts;
        private readonly List<ValidationResult> _validationResults = new();

        public ListScriptsValidationService(ScriptDefinition script, IEnumerable<ScriptDefinition> allScripts)
        {
            _script = script;
            _allScripts = allScripts;
            Validate();
        }
        
        /// <summary>
        /// Clase que devuelve todos los errores que detecte el servicio de validación de la lista de scripts
        /// almacenados temporalmente.
        /// </summary>
        /// <returns></returns>
        public List<string> GetErrorMessages()
        {
            return _validationResults
                .Where(result => !result.IsValid)
                .Select(result => result.Message)
                .ToList();
        }

        /// <summary>
        /// Función validadora que engrapa todas las reglas para verificar que la lista está correcta
        /// </summary>
        private void Validate()
        {
            var validators = new List<IValidationRule>
            {
                new ScriptListViewRule(_allScripts, _script) 
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
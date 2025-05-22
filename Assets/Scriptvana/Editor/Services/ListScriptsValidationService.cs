using System.Collections.Generic;
using System.Linq;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Validations;
using Scriptvana.Editor.Validations.Rules;

namespace Scriptvana.Editor.Services
{
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
        
        public List<string> GetErrorMessages()
        {
            return _validationResults
                .Where(result => !result.IsValid)
                .Select(result => result.Message)
                .ToList();
        }

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
using System.Collections.Generic;
using System.Linq;
using Scriptvana.Editor.Models;
using Scriptvana.Editor.Validations;
using Scriptvana.Editor.Validations.Rules;

namespace Scriptvana.Editor.Services
{
    public class BasicScriptValidationService
    {
        private readonly ScriptDefinition _script;
        private readonly List<ValidationResult> _validationResults = new();

        public BasicScriptValidationService(ScriptDefinition script)
        {
            _script = script;
            Validate();
        }

        public List<ValidationResult> GetValidationResults()
        {
            return _validationResults;
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
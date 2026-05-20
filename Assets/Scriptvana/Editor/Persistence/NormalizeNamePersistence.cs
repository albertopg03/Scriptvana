using Scriptvana.Editor.Models;
using UnityEditor;

namespace Scriptvana.Editor.Persistence
{
    public static class NormalizeNamePersistence
    {
        public static int MinScriptNameLength
        {
            get => EditorPrefs.GetInt("minCharactersField", 4);
            set => EditorPrefs.SetInt("minCharactersField", value);
        }

        public static bool UseNormalizeName
        {
            get => NormalizationMode != NameNormalizationMode.Disabled;
            set
            {
                if (!value)
                {
                    NormalizationMode = NameNormalizationMode.Disabled;
                    return;
                }

                if (NormalizationMode == NameNormalizationMode.Disabled)
                {
                    NormalizationMode = NameNormalizationMode.PascalCase;
                }
            }
        }

        public static NameNormalizationMode NormalizationMode
        {
            get
            {
                string storedValue = EditorPrefs.GetString("nameNormalizationMode", NameNormalizationMode.Disabled.ToString());
                return System.Enum.TryParse(storedValue, out NameNormalizationMode mode)
                    ? mode
                    : NameNormalizationMode.Disabled;
            }
            set
            {
                EditorPrefs.SetString("nameNormalizationMode", value.ToString());
                EditorPrefs.SetBool("useNormalizeNameField", value != NameNormalizationMode.Disabled);
            }
        }
    }
}

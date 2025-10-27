using UnityEditor;

namespace Scriptvana.Editor.Persistence
{
    public static class NormalizeNamePersistence
    {
        public static int MinScriptNameLength
        {
            get => EditorPrefs.GetInt("minCharactersField");
            set => EditorPrefs.SetInt("minCharactersField", value);
        }

        public static bool UseNormalizeName
        {
            get => EditorPrefs.GetBool("useNormalizeNameField");
            set => EditorPrefs.SetBool("useNormalizeNameField", value);
        }
    }
}

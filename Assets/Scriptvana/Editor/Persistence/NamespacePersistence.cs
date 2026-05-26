using Scriptvana.Editor.Models;
using UnityEditor;

namespace Scriptvana.Editor.Persistence
{
    public static class NamespacePersistence
    {
        public static DefaultNamespaceMode DefaultMode
        {
            get
            {
                string storedValue = EditorPrefs.GetString("defaultNamespaceMode", DefaultNamespaceMode.Empty.ToString());
                return System.Enum.TryParse(storedValue, out DefaultNamespaceMode mode)
                    ? mode
                    : DefaultNamespaceMode.Empty;
            }
            set => EditorPrefs.SetString("defaultNamespaceMode", value.ToString());
        }

        public static string FixedNamespace
        {
            get => EditorPrefs.GetString("fixedNamespace", string.Empty);
            set => EditorPrefs.SetString("fixedNamespace", value ?? string.Empty);
        }

        public static bool UseScriptNameAsDefaultNamespace
        {
            get => EditorPrefs.GetBool("useScriptNameAsDefaultNamespace", false);
            set => EditorPrefs.SetBool("useScriptNameAsDefaultNamespace", value);
        }
    }
}

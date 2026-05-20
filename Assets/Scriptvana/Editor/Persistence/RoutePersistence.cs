using UnityEditor;

namespace Scriptvana.Editor.Persistence
{
    public static class RoutePersistence
    {
        public static bool ManualEditablePath
        {
            get => EditorPrefs.GetBool("manuallyEdiablePathField");
            set => EditorPrefs.SetBool("manuallyEdiablePathField", value);
        }

        public static string DefaultPath
        {
            get => EditorPrefs.GetString("defaultPath", "Assets/");
            set => EditorPrefs.SetString("defaultPath", value);
        }

        public static bool RestrictToBasePath
        {
            get => EditorPrefs.GetBool("restrictToBasePath", false);
            set => EditorPrefs.SetBool("restrictToBasePath", value);
        }

        public static string BasePath
        {
            get => EditorPrefs.GetString("basePath", "Assets/");
            set => EditorPrefs.SetString("basePath", value);
        }

        public static bool AutoCreateDirectories
        {
            get => EditorPrefs.GetBool("autoCreateDirectories", true);
            set => EditorPrefs.SetBool("autoCreateDirectories", value);
        }
    }
}

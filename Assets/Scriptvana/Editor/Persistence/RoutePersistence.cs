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
    }
}

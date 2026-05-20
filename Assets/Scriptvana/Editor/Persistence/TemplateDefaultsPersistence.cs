using UnityEditor;

namespace Scriptvana.Editor.Persistence
{
    public static class TemplateDefaultsPersistence
    {
        public static string AdditionalImports
        {
            get => EditorPrefs.GetString("additionalImports", string.Empty);
            set => EditorPrefs.SetString("additionalImports", value ?? string.Empty);
        }

        public static bool IncludeHeaderComment
        {
            get => EditorPrefs.GetBool("includeHeaderComment", false);
            set => EditorPrefs.SetBool("includeHeaderComment", value);
        }

        public static string HeaderCommentText
        {
            get => EditorPrefs.GetString("headerCommentText", "Generated with Scriptvana");
            set => EditorPrefs.SetString("headerCommentText", value ?? string.Empty);
        }
    }
}

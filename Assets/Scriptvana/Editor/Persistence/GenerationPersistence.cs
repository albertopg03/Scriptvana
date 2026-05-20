using Scriptvana.Editor.Models;
using UnityEditor;

namespace Scriptvana.Editor.Persistence
{
    public static class GenerationPersistence
    {
        public static PostGenerationOpenMode OpenMode
        {
            get
            {
                string storedValue = EditorPrefs.GetString("postGenerationOpenMode", PostGenerationOpenMode.None.ToString());
                return System.Enum.TryParse(storedValue, out PostGenerationOpenMode mode)
                    ? mode
                    : PostGenerationOpenMode.None;
            }
            set => EditorPrefs.SetString("postGenerationOpenMode", value.ToString());
        }

        public static PostGenerationBehavior PostBehavior
        {
            get
            {
                string storedValue = EditorPrefs.GetString("postGenerationBehavior", PostGenerationBehavior.KeepEverything.ToString());
                return System.Enum.TryParse(storedValue, out PostGenerationBehavior behavior)
                    ? behavior
                    : PostGenerationBehavior.KeepEverything;
            }
            set => EditorPrefs.SetString("postGenerationBehavior", value.ToString());
        }
    }
}

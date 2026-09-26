using System.IO;
using UnityEditor;
using UnityEngine;

namespace Scriptvana.Editor.Models
{
    /// <summary>
    /// Icon assets used by the Scriptvana editor windows.
    /// </summary>
    [CreateAssetMenu(fileName = "IconData", menuName = "Scriptvana/IconData")]
    public class IconData : ScriptableObject
    {
        public Texture2D iconFolder;
        public Texture2D iconClose;

        private static IconData _instance;
        private static string _iconsPath;
        private static bool _missingIconWarningShown;

        /// <summary>
        /// Loads icons relative to this script, under either Assets or Packages.
        /// </summary>
        public static IconData Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Resolve the installed location without relying on serialized GUIDs.
                    _instance = CreateInstance<IconData>();
                    _instance.hideFlags = HideFlags.HideAndDontSave;
                    string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(_instance));
                    string editorPath = Path.GetDirectoryName(Path.GetDirectoryName(scriptPath));
                    _iconsPath = string.IsNullOrEmpty(editorPath)
                        ? null
                        : editorPath.Replace('\\', '/') + "/Resources/Icons/Imgs/";
                }

                if (_instance.iconFolder == null || _instance.iconClose == null)
                {
                    if (_instance.iconFolder == null)
                        _instance.iconFolder = LoadIcon("folder.png");
                    if (_instance.iconClose == null)
                        _instance.iconClose = LoadIcon("close.png");

                    if ((_instance.iconFolder == null || _instance.iconClose == null) && !_missingIconWarningShown)
                    {
                        _missingIconWarningShown = true;
                        Debug.LogWarning("[SCRIPTVANA]: Could not load folder.png or close.png from Editor/Resources/Icons/Imgs. Check that the textures and their .meta files are installed.");
                    }
                }

                return _instance;
            }
        }

        private static Texture2D LoadIcon(string fileName)
        {
            return string.IsNullOrEmpty(_iconsPath)
                ? null
                : AssetDatabase.LoadAssetAtPath<Texture2D>(_iconsPath + fileName);
        }
    }
}

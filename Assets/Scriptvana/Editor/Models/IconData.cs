using UnityEngine;

namespace Scriptvana.Editor.Models
{
    /// <summary>
    /// ScriptableObject con todos los datos relacionados con todos los iconos que contiene la tool.
    /// </summary>
    [CreateAssetMenu(fileName = "IconData", menuName = "Scriptvana/IconData")]
    public class IconData : ScriptableObject
    {
        public Texture2D iconFolder;
        public Texture2D iconClose;

        private static IconData _instance;

        /// <summary>
        /// Obtiene la instancia única de IconData desde Resources
        /// </summary>
        public static IconData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<IconData>("Icons/IconData");
                    if (_instance == null)
                    {
                        Debug.LogError("No se encontró IconData en Resources/Icons/");
                    }
                }
                return _instance;
            }
        }
    }
}
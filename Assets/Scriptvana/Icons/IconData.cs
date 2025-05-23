using UnityEngine;

namespace Scriptvana.Icons
{
    /// <summary>
    /// ScriptableObject con todos los datos relacionados con todos los iconos que contiene la tool.
    /// </summary>
    [CreateAssetMenu(fileName = "IconData", menuName = "Scriptvana/IconData")]
    public class IconData : ScriptableObject
    {
        public Texture2D iconFolder;
        public Texture2D iconClose;
    }
}

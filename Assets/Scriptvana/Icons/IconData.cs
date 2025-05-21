using UnityEngine;

namespace Scriptvana.Icons
{
    [CreateAssetMenu(fileName = "IconData", menuName = "Scriptvana/IconData")]

    public class IconData : ScriptableObject
    {
        public Texture2D iconFolder;
        public Texture2D iconClose;
    }
}

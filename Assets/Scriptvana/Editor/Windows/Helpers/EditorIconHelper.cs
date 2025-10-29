using UnityEngine;
using UnityEngine.UIElements;

namespace Scriptvana.Editor.Windows.Helpers
{
    public static class EditorIconHelper
    {
        /// <summary>
        /// Función auxiliar que permite añadir un icono a un botón. Esto se ha creado para dar soporte
        /// a los iconos para versiones inferiores al Unity 6, ya que solo en esta versión en adelante,
        /// contamos con el atributo itemImage. En anteriores versiones, da error.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="texture"></param>
        /// <param name="size"></param>
        public static void AddCenteredIconToButton(Button button, Texture2D texture, Vector2 size)
        {
            button.style.flexDirection = FlexDirection.Row;
            button.style.justifyContent = Justify.Center;
            button.style.alignItems = Align.Center;

            var icon = new VisualElement();
            icon.style.backgroundImage = new StyleBackground(texture);
            icon.style.width = size.x;
            icon.style.height = size.y;

            button.Add(icon);
        }
    }
}

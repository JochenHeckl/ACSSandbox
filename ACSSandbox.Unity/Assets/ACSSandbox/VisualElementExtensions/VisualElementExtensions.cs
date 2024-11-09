using UnityEngine.UIElements;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace JH.VisualElementExtensions
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public static class VisualElementExtensions
    {
        public static void UniformPadding(this VisualElement visualElement, StyleLength padding)
        {
            visualElement.style.paddingTop = padding;
            visualElement.style.paddingLeft = padding;
            visualElement.style.paddingRight = padding;
            visualElement.style.paddingBottom = padding;
        }

        public static void SymmetricPadding(
            this VisualElement visualElement,
            StyleLength paddingVertical,
            StyleLength paddingHorizontal
        )
        {
            visualElement.style.paddingTop = paddingVertical;
            visualElement.style.paddingLeft = paddingHorizontal;
            visualElement.style.paddingRight = paddingHorizontal;
            visualElement.style.paddingBottom = paddingVertical;
        }
    }
}

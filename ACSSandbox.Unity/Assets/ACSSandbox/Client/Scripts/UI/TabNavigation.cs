using System;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ACSSandbox.Client
{
    public class TabNavigation : MonoBehaviour
    {
        ClientInputActions inputActions;
        Selectable initiallySelected;

        void OnEnable()
        {
            inputActions = new ClientInputActions();
            inputActions.UI.Enable();
            inputActions.UI.TabNavigation.performed += Navigate;
        }

        void OnDisable()
        {
            inputActions.UI.TabNavigation.performed -= Navigate;
            inputActions.UI.Disable();
            inputActions = null;
        }

        private void Navigate(InputAction.CallbackContext context)
        {
            Selectable selected =
                EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();

            if (selected == null)
            {
                initiallySelected.Select();
                return;
            }

            var shiftIsDown = Keyboard.current.shiftKey.isPressed;

            var next = shiftIsDown
                ? selected.FindSelectableOnUp()
                : selected.FindSelectableOnDown();

            if (next == null)
            {
                next = shiftIsDown
                    ? selected.FindSelectableOnLeft()
                    : selected.FindSelectableOnRight();
            }

            if (next != null)
            {
                next.Select();
            }
        }
    }
}

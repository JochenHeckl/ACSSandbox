using JH.DataBinding;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ACSSandbox.Client
{
    public class LoginPanelView : View
    {
        public GameObject initiallySelected;

        private void Awake()
        {
            EventSystem.current.SetSelectedGameObject(initiallySelected);
        }
    }
}

using System;
using ACSSandbox.Server;
using JH.AppConfig;
using JH.VisualElementExtensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ACSSandbox.Editor
{
    [CustomEditor(typeof(BootstrapServer))]
    public class BootstrapServerEditor : UnityEditor.Editor
    {
        private BootstrapServer bootstrapServer;

        public void OnEnable()
        {
            bootstrapServer = target as BootstrapServer;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var defaultEditor = new VisualElement();
            InspectorElement.FillDefaultInspector(defaultEditor, serializedObject, this);

            var buttonSection = new VisualElement();
            MakeButtonSectionStyle(buttonSection);

            var openAppConfigClientButton = new Button(EditConfigData)
            {
                text = "Open AppConfig Server"
            };

            MakeButtonStyle(openAppConfigClientButton);
            buttonSection.Add(openAppConfigClientButton);

            defaultEditor.Add(buttonSection);

            return defaultEditor;
        }

        private void EditConfigData()
        {
            bootstrapServer.ServerAppConfig.Save();

            EditorUtility.OpenWithDefaultApp(bootstrapServer.ServerAppConfig.ConfigFilePath);
        }

        private void MakeButtonSectionStyle(VisualElement visualElement)
        {
            visualElement.SymmetricPadding(8, 32);
            visualElement.style.flexDirection = FlexDirection.Row;
            visualElement.style.alignItems = Align.Center;
        }

        private void MakeButtonStyle(Button visualElement)
        {
            visualElement.style.fontSize = 18;
            visualElement.style.height = 32;
            visualElement.style.flexGrow = 1f;
        }
    }
}

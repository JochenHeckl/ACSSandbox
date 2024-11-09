using System;
using ACSSandbox.Client;
using JH.AppConfig;
using JH.VisualElementExtensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ACSSandbox.Editor
{
    [CustomEditor(typeof(BootstrapClient))]
    public class BootstrapClientEditor : UnityEditor.Editor
    {
        private BootstrapClient bootstrapClient;

        public void OnEnable()
        {
            bootstrapClient = target as BootstrapClient;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var defaultEditor = new VisualElement();
            InspectorElement.FillDefaultInspector(defaultEditor, serializedObject, this);

            var buttonSection = new VisualElement();
            MakeButtonSectionStyle(buttonSection);

            var randomizeClientIdButton = new Button(RandomizeClientId)
            {
                text = "Randomize ClientId",
            };
            MakeButtonStyle(randomizeClientIdButton);
            buttonSection.Add(randomizeClientIdButton);

            var openAppConfigClientButton = new Button(EditConfigData)
            {
                text = "Open AppConfig Client",
            };
            MakeButtonStyle(openAppConfigClientButton);
            buttonSection.Add(openAppConfigClientButton);

            defaultEditor.Add(buttonSection);

            return defaultEditor;
        }

        private void EditConfigData()
        {
            bootstrapClient.ClientAppConfig.Save();
            EditorUtility.OpenWithDefaultApp(bootstrapClient.ClientAppConfig.ConfigFilePath);
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

        private void RandomizeClientId()
        {
            var newId = Guid.NewGuid();

            var appConfig = bootstrapClient.ClientAppConfig;
            appConfig.Data.clientId = newId.ToString("N");

            appConfig.Save();
        }
    }
}

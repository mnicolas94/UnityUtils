using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Utils.UI;

namespace Utils.Editor.CustomEditors
{
    [CustomEditor(typeof(NotificationPopupSpawner))]
    public class NotificationPopupSpawnerEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            // draw default inspector
            var serializedProperties = PropertiesUtils.GetSerializedProperties(serializedObject);
            foreach (var serializedProperty in serializedProperties)
            {
                var propertyField = new PropertyField(serializedProperty);
                root.Add(propertyField);
            }

            var createButton = new Button(CreatePrefab)
            {
                text = "Create prefab"
            };
            root.Add(createButton);

            return root;
        }

        private void CreatePrefab()
        {
            // get template path
            var prefabTemplate = Resources.Load<NotificationPopup>("NotificationText.facticus");
            var templatePath = AssetDatabase.GetAssetPath(prefabTemplate);

            // get new path
            var spawnerPath = AssetDatabase.GetAssetPath(target);
            var dir = Path.GetDirectoryName(spawnerPath);
            var newPath = Path.Combine(dir, "NotificationText.prefab");
            newPath = AssetDatabase.GenerateUniqueAssetPath(newPath);

            // copy prefab
            AssetDatabase.CopyAsset(templatePath, newPath);
            var newPrefab = AssetDatabase.LoadAssetAtPath<NotificationPopup>(newPath);
            
            var spawner = target as NotificationPopupSpawner;
            spawner.NotificationPrefab = newPrefab;
            EditorUtility.SetDirty(spawner);
        }
    }
}
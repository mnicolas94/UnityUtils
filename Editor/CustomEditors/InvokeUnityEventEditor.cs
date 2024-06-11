using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Utils.Behaviours;

namespace Utils.Editor.CustomEditors
{
    [CustomEditor(typeof(InvokeUnityEvent))]
    public class InvokeUnityEventEditor : UnityEditor.Editor
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

            var createButton = new Button(InvokeEvent)
            {
                text = "Invoke"
            };
            root.Add(createButton);

            return root;
        }

        private void InvokeEvent()
        {
            ((InvokeUnityEvent) target).Invoke();
        }
    }
}
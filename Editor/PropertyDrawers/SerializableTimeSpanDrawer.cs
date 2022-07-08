using UnityEditor;
using UnityEngine;
using Utils.Serializables;

namespace Utils.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SerializableTimeSpan))]
    public class SerializableTimeSpanDrawer : PropertyDrawer
    {
        private const string DaysPath = "days";
        private const string HoursPath = "hours";
        private const string MinutesPath = "minutes";
        private const string SecondsPath = "seconds";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var daysProperty = property.FindPropertyRelative(DaysPath);
            var hoursProperty = property.FindPropertyRelative(HoursPath);
            var minutesProperty = property.FindPropertyRelative(MinutesPath);
            var secondsProperty = property.FindPropertyRelative(SecondsPath);
            
            float pwidth = position.width / 4;
            float pheight = position.height / 2;
            
            var daysLabelRect = new Rect(position.x, position.y, pwidth, pheight);            
            var hoursLabelRect = new Rect(position.x + pwidth * 1, position.y, pwidth, pheight);            
            var minutesLabelRect = new Rect(position.x + pwidth * 2, position.y, pwidth, pheight);            
            var secondsLabelRect = new Rect(position.x + pwidth * 3, position.y, pwidth, pheight);
            EditorGUI.LabelField(daysLabelRect, daysProperty.name);
            EditorGUI.LabelField(hoursLabelRect, hoursProperty.name);
            EditorGUI.LabelField(minutesLabelRect, minutesProperty.name);
            EditorGUI.LabelField(secondsLabelRect, secondsProperty.name);
            
            var daysRect = new Rect(position.x, position.y + pheight, pwidth, pheight);            
            var hoursRect = new Rect(position.x + pwidth * 1, position.y + pheight, pwidth, pheight);            
            var minutesRect = new Rect(position.x + pwidth * 2, position.y + pheight, pwidth, pheight);            
            var secondsRect = new Rect(position.x + pwidth * 3, position.y + pheight, pwidth, pheight);
            EditorGUI.PropertyField(daysRect, daysProperty, GUIContent.none);
            EditorGUI.PropertyField(hoursRect, hoursProperty, GUIContent.none);
            EditorGUI.PropertyField(minutesRect, minutesProperty, GUIContent.none);
            EditorGUI.PropertyField(secondsRect, secondsProperty, GUIContent.none);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property) * 2;
        }
    }
}
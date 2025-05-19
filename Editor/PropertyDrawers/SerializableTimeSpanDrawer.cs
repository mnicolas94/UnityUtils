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
        private static readonly GUIContent DaysLabel = new (DaysPath);
        private static readonly GUIContent HoursLabel = new (HoursPath);
        private static readonly GUIContent MinutesLabel = new (MinutesPath);
        private static readonly GUIContent SecondsLabel = new (SecondsPath);
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var daysProperty = property.FindPropertyRelative(DaysPath);
            var hoursProperty = property.FindPropertyRelative(HoursPath);
            var minutesProperty = property.FindPropertyRelative(MinutesPath);
            var secondsProperty = property.FindPropertyRelative(SecondsPath);
            
            var valueRect = EditorGUI.PrefixLabel(position, label);
            float pwidth = valueRect.width / 4;
            float pheight = valueRect.height / 2;
            
            var daysLabelRect = new Rect(valueRect.x, valueRect.y, pwidth, pheight);            
            var hoursLabelRect = new Rect(valueRect.x + pwidth * 1, valueRect.y, pwidth, pheight);            
            var minutesLabelRect = new Rect(valueRect.x + pwidth * 2, valueRect.y, pwidth, pheight);            
            var secondsLabelRect = new Rect(valueRect.x + pwidth * 3, valueRect.y, pwidth, pheight);
            EditorGUI.LabelField(daysLabelRect, DaysLabel);
            EditorGUI.LabelField(hoursLabelRect, HoursLabel);
            EditorGUI.LabelField(minutesLabelRect, MinutesLabel);
            EditorGUI.LabelField(secondsLabelRect, SecondsLabel);
            
            var daysRect = new Rect(valueRect.x, valueRect.y + pheight, pwidth, pheight);            
            var hoursRect = new Rect(valueRect.x + pwidth * 1, valueRect.y + pheight, pwidth, pheight);            
            var minutesRect = new Rect(valueRect.x + pwidth * 2, valueRect.y + pheight, pwidth, pheight);            
            var secondsRect = new Rect(valueRect.x + pwidth * 3, valueRect.y + pheight, pwidth, pheight);
            EditorGUI.PropertyField(daysRect, daysProperty, GUIContent.none);
            EditorGUI.PropertyField(hoursRect, hoursProperty, GUIContent.none);
            EditorGUI.PropertyField(minutesRect, minutesProperty, GUIContent.none);
            EditorGUI.PropertyField(secondsRect, secondsProperty, GUIContent.none);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}
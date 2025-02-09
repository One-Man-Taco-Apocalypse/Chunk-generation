#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
public class MinMaxSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        MinMaxSliderAttribute minMax = (MinMaxSliderAttribute)attribute;

        if (property.propertyType == SerializedPropertyType.Vector2)
        {
            Vector2 range = property.vector2Value;

            float minValue = range.x;
            float maxValue = range.y;

            EditorGUI.PrefixLabel(position, label);

            Rect sliderRect = new Rect(position.x + 120, position.y, position.width - 120, position.height);
            EditorGUI.MinMaxSlider(sliderRect, ref minValue, ref maxValue, minMax.min, minMax.max);

            range.x = minValue;
            range.y = maxValue;

            property.vector2Value = range;
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use with Vector2 only!");
        }
    }
}
#endif

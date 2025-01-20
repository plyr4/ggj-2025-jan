#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

[CustomPropertyDrawer(typeof(GStateRef))]
public class GStateRefDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty classTypeProperty = property.FindPropertyRelative("_type");

        Type baseType = typeof(GStateBase);
        var subtypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract)
            .ToArray();

        string[] options = subtypes.Select(t => t.Name).ToArray();

        int selectedIndex = Array.IndexOf(options, classTypeProperty.stringValue);
        if (selectedIndex == -1) selectedIndex = 0;

        int newIndex = EditorGUI.Popup(position, label.text, selectedIndex, options);
        classTypeProperty.stringValue = subtypes[newIndex].FullName;
    }
}
#endif
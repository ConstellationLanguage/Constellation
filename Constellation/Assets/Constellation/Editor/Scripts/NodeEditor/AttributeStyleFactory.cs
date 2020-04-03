using Constellation;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor
{
    public static class AttributeStyleFactory
    {
        public static Ray Draw(Parameter.AttributeType type, Rect size, Rect attributeArea, Ray Value, ConstellationEditorStyles editorStyles, out bool canBeFocused)
        {
            canBeFocused = false;
            switch (type)
            {
                case Parameter.AttributeType.Value:
                    canBeFocused = true;
                    var valueToReturn = Value.Set(EditorGUI.FloatField(size, " ", Value.GetFloat(), editorStyles.NodeValueAttributeStyle));
                    EditorGUI.LabelField(new Rect(size.x, size.y - 8, 30, 30), "<>", editorStyles.NodeValueAttributeLabelStyle);
                    return valueToReturn;
                case Parameter.AttributeType.Word:
                    canBeFocused = true;
                    return Value.Set(EditorGUI.TextField(size, "", Value.GetString(), editorStyles.NodeWordAttributeStyle));
                case Parameter.AttributeType.Conditionals:
                    canBeFocused = true;
                    return IfCharacterFilter(size, Value);
                case Parameter.AttributeType.Then:
                    canBeFocused = true;
                    return ThenCharacterFilter(size, Value);
                case Parameter.AttributeType.Else:
                    canBeFocused = true;
                    return ElseCharacterFilter(size, Value);
                case Parameter.AttributeType.NoteField:
                    canBeFocused = true;
                    GUI.color = new Color(0.9f, 0.85f, 0.25f);
                    var textAreaValue = Value.Set(EditorGUI.TextArea(attributeArea, Value.GetString()));
                    GUI.color = Color.white;
                    //noteSkin.alignment = TextAnchor.UpperLeft;
                    return textAreaValue;
                case Parameter.AttributeType.ReadOnlyValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeReadOnlyAttributeStyle);
                    return Value;
                case Parameter.AttributeType.ReadOnlyXValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeXAtrributeStyle);
                    return Value;
                case Parameter.AttributeType.ReadOnlyYValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeYAtrributeStyle);
                    return Value;
                case Parameter.AttributeType.ReadOnlyZValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeZAtrributeStyle);
                    return Value;
                case Parameter.AttributeType.ReadOnlyValueR:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeRAtrributeStyle);
                    return Value;
                case Parameter.AttributeType.ReadOnlyValueG:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeGAtrributeStyle);
                    return Value;
                case Parameter.AttributeType.ReadOnlyValueB:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeBAtrributeStyle);
                    return Value;
                case Parameter.AttributeType.ReadOnlyValueA:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeAAtrributeStyle);
                    return Value;
                case Parameter.AttributeType.RenameNodeTitle:
                    EditorGUI.LabelField(size, Value.GetString());
                    return Value;
                default:
                    return Value;
            }
        }

        public static Ray Reset(Parameter.AttributeType type, Ray Value)
        {

            switch (type)
            {
                case Parameter.AttributeType.Value:
                    return Value;
                case Parameter.AttributeType.Word:
                    return Value.Set(Value.GetString());
                case Parameter.AttributeType.Conditionals:
                    return Value.Set(Value.GetString());
                case Parameter.AttributeType.Then:
                    return Value.Set(Value.GetString());
                case Parameter.AttributeType.Else:
                    return Value.Set(Value.GetString());
                case Parameter.AttributeType.NoteField:
                    return Value.Set(Value.GetString());
                case Parameter.AttributeType.ReadOnlyValue:
                    Value.Set(0);
                    return Value;
                case Parameter.AttributeType.ReadOnlyXValue:
                    Value.Set("X");
                    return Value;
                case Parameter.AttributeType.ReadOnlyYValue:
                    Value.Set("Y");
                    return Value;
                case Parameter.AttributeType.ReadOnlyZValue:
                    Value.Set("Z");
                    return Value;
                case Parameter.AttributeType.ReadOnlyValueR:
                    Value.Set("R");
                    return Value;
                case Parameter.AttributeType.ReadOnlyValueG:
                    Value.Set("G");
                    return Value;
                case Parameter.AttributeType.ReadOnlyValueB:
                    Value.Set("B");
                    return Value;
                case Parameter.AttributeType.ReadOnlyValueA:
                    Value.Set("A");
                    return Value;
                default:
                    return Value;
            }
        }

        private static Ray IfCharacterFilter(Rect size, Ray Value)
        {
            return Value.Set(Regex.Replace(EditorGUI.TextField(size, "if", Value.GetString()), "[a-zA-Z ]", ""));
        }

        private static Ray ThenCharacterFilter(Rect size, Ray Value)
        {
            return Value.Set(Regex.Replace(EditorGUI.TextField(size, "then", Value.GetString()), "[a-zA-Z ]", ""));
        }

        private static Ray ElseCharacterFilter(Rect size, Ray Value)
        {
            return Value.Set(Regex.Replace(EditorGUI.TextField(size, "else", Value.GetString()), "[a-zA-Z ]", ""));
        }
    }
}

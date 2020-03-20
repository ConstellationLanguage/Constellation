using Constellation;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor
{
    public static class AttributeStyleFactory
    {
        public static Variable Draw(Attribute.AttributeType type, Rect size, Rect attributeArea, Variable Value, ConstellationEditorStyles editorStyles, out bool canBeFocused)
        {
            canBeFocused = false;
            switch (type)
            {
                case Attribute.AttributeType.Value:
                    canBeFocused = true;
                    var valueToReturn = Value.Set(EditorGUI.FloatField(size, " ", Value.GetFloat(), editorStyles.NodeValueAttributeStyle));
                    EditorGUI.LabelField(new Rect(size.x, size.y - 8, 30, 30), "<>", editorStyles.NodeValueAttributeLabelStyle);
                    return valueToReturn;
                case Attribute.AttributeType.Word:
                    canBeFocused = true;
                    return Value.Set(EditorGUI.TextField(size, "", Value.GetString(), editorStyles.NodeWordAttributeStyle));
                case Attribute.AttributeType.Conditionals:
                    canBeFocused = true;
                    return IfCharacterFilter(size, Value);
                case Attribute.AttributeType.Then:
                    canBeFocused = true;
                    return ThenCharacterFilter(size, Value);
                case Attribute.AttributeType.Else:
                    canBeFocused = true;
                    return ElseCharacterFilter(size, Value);
                case Attribute.AttributeType.NoteField:
                    canBeFocused = true;
                    GUI.color = new Color(0.9f, 0.85f, 0.25f);
                    var textAreaValue = Value.Set(EditorGUI.TextArea(attributeArea, Value.GetString()));
                    GUI.color = Color.white;
                    //noteSkin.alignment = TextAnchor.UpperLeft;
                    return textAreaValue;
                case Attribute.AttributeType.ReadOnlyValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeReadOnlyAttributeStyle);
                    return Value;
                case Attribute.AttributeType.ReadOnlyXValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeXAtrributeStyle);
                    return Value;
                case Attribute.AttributeType.ReadOnlyYValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeYAtrributeStyle);
                    return Value;
                case Attribute.AttributeType.ReadOnlyZValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeZAtrributeStyle);
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueR:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeRAtrributeStyle);
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueG:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeGAtrributeStyle);
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueB:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeBAtrributeStyle);
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueA:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeAAtrributeStyle);
                    return Value;
                case Attribute.AttributeType.RenameNodeTitle:
                    EditorGUI.LabelField(size, Value.GetString());
                    return Value;
                default:
                    return Value;
            }
        }

        public static Variable Reset(Attribute.AttributeType type, Variable Value)
        {

            switch (type)
            {
                case Attribute.AttributeType.Value:
                    return Value;
                case Attribute.AttributeType.Word:
                    return Value.Set(Value.GetString());
                case Attribute.AttributeType.Conditionals:
                    return Value.Set(Value.GetString());
                case Attribute.AttributeType.Then:
                    return Value.Set(Value.GetString());
                case Attribute.AttributeType.Else:
                    return Value.Set(Value.GetString());
                case Attribute.AttributeType.NoteField:
                    return Value.Set(Value.GetString());
                case Attribute.AttributeType.ReadOnlyValue:
                    Value.Set(0);
                    return Value;
                case Attribute.AttributeType.ReadOnlyXValue:
                    Value.Set("X");
                    return Value;
                case Attribute.AttributeType.ReadOnlyYValue:
                    Value.Set("Y");
                    return Value;
                case Attribute.AttributeType.ReadOnlyZValue:
                    Value.Set("Z");
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueR:
                    Value.Set("R");
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueG:
                    Value.Set("G");
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueB:
                    Value.Set("B");
                    return Value;
                case Attribute.AttributeType.ReadOnlyValueA:
                    Value.Set("A");
                    return Value;
                default:
                    return Value;
            }
        }

        private static Variable IfCharacterFilter(Rect size, Variable Value)
        {
            return Value.Set(Regex.Replace(EditorGUI.TextField(size, "if", Value.GetString()), "[a-zA-Z ]", ""));
        }

        private static Variable ThenCharacterFilter(Rect size, Variable Value)
        {
            return Value.Set(Regex.Replace(EditorGUI.TextField(size, "then", Value.GetString()), "[a-zA-Z ]", ""));
        }

        private static Variable ElseCharacterFilter(Rect size, Variable Value)
        {
            return Value.Set(Regex.Replace(EditorGUI.TextField(size, "else", Value.GetString()), "[a-zA-Z ]", ""));
        }
    }
}

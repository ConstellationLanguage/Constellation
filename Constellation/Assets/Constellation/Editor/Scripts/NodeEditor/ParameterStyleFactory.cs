using Constellation;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace ConstellationUnityEditor
{
    public static class ParametersStyleFactory
    {
        public static Ray Draw(Parameter.ParameterType type, Rect size, Rect attributeArea, Ray Value, ConstellationEditorStyles editorStyles, out bool canBeFocused)
        {
            canBeFocused = false;
            switch (type)
            {
                case Parameter.ParameterType.Value:
                    canBeFocused = true;
                    var valueToReturn = Value.Set(EditorGUI.FloatField(size, " ", Value.GetFloat(), editorStyles.NodeValueParameterStyle));
                    EditorGUI.LabelField(new Rect(size.x, size.y - 8, 30, 30), "<>", editorStyles.NodeValueParameterLabelStyle);
                    return valueToReturn;
                case Parameter.ParameterType.Word:
                    canBeFocused = true;
                    return Value.Set(EditorGUI.TextField(size, "", Value.GetString(), editorStyles.NodeWordParameterStyle));
                case Parameter.ParameterType.Conditionals:
                    canBeFocused = true;
                    return IfCharacterFilter(size, Value, editorStyles);
                case Parameter.ParameterType.Then:
                    canBeFocused = true;
                    return ThenCharacterFilter(size, Value, editorStyles);
                case Parameter.ParameterType.Else:
                    canBeFocused = true;
                    return ElseCharacterFilter(size, Value, editorStyles);
                case Parameter.ParameterType.NoteField:
                    canBeFocused = true;
                    GUI.color = new Color(0.9f, 0.85f, 0.25f);
                    var textAreaValue = Value.Set(EditorGUI.TextArea(attributeArea, Value.GetString()));
                    GUI.color = Color.white;
                    //noteSkin.alignment = TextAnchor.UpperLeft;
                    return textAreaValue;
                case Parameter.ParameterType.ReadOnlyValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeReadOnlyParameterStyle);
                    return Value;
                case Parameter.ParameterType.ReadOnlyXValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeXParameterStyle);
                    return Value;
                case Parameter.ParameterType.ReadOnlyYValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeYParameterStyle);
                    return Value;
                case Parameter.ParameterType.ReadOnlyZValue:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeZParameterStyle);
                    return Value;
                case Parameter.ParameterType.ReadOnlyValueR:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeRParameterStyle);
                    return Value;
                case Parameter.ParameterType.ReadOnlyValueG:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeGParameterStyle);
                    return Value;
                case Parameter.ParameterType.ReadOnlyValueB:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeBParameterStyle);
                    return Value;
                case Parameter.ParameterType.ReadOnlyValueA:
                    EditorGUI.LabelField(size, Value.GetString(), editorStyles.NodeAParameterStyle);
                    return Value;
                case Parameter.ParameterType.RenameNodeTitle:
                    EditorGUI.LabelField(size, Value.GetString());
                    return Value;
                default:
                    return Value;
            }
        }

        public static Ray Reset(Parameter.ParameterType type, Ray Value)
        {

            switch (type)
            {
                case Parameter.ParameterType.Value:
                    return Value;
                case Parameter.ParameterType.Word:
                    return Value.Set(Value.GetString());
                case Parameter.ParameterType.Conditionals:
                    return Value.Set(Value.GetString());
                case Parameter.ParameterType.Then:
                    return Value.Set(Value.GetString());
                case Parameter.ParameterType.Else:
                    return Value.Set(Value.GetString());
                case Parameter.ParameterType.NoteField:
                    return Value.Set(Value.GetString());
                case Parameter.ParameterType.ReadOnlyValue:
                    Value.Set(0);
                    return Value;
                case Parameter.ParameterType.ReadOnlyXValue:
                    Value.Set("X");
                    return Value;
                case Parameter.ParameterType.ReadOnlyYValue:
                    Value.Set("Y");
                    return Value;
                case Parameter.ParameterType.ReadOnlyZValue:
                    Value.Set("Z");
                    return Value;
                case Parameter.ParameterType.ReadOnlyValueR:
                    Value.Set("R");
                    return Value;
                case Parameter.ParameterType.ReadOnlyValueG:
                    Value.Set("G");
                    return Value;
                case Parameter.ParameterType.ReadOnlyValueB:
                    Value.Set("B");
                    return Value;
                case Parameter.ParameterType.ReadOnlyValueA:
                    Value.Set("A");
                    return Value;
                default:
                    return Value;
            }
        }

        private static Ray IfCharacterFilter(Rect size, Ray Value, ConstellationEditorStyles editorStyles)
        {
            EditorGUI.LabelField(new Rect(size.x, size.y - 8, 30, 30), "if", editorStyles.NodeValueParameterLabelStyle);
            return Value.Set(Regex.Replace(EditorGUI.TextField(size, "    ", Value.GetString(), editorStyles.NodeWordParameterStyle), "[a-zA-Z ]", ""));
        }

        private static Ray ThenCharacterFilter(Rect size, Ray Value, ConstellationEditorStyles editorStyles)
        {
            EditorGUI.LabelField(new Rect(size.x, size.y - 8, 30, 30), "then", editorStyles.NodeValueParameterLabelStyle);
            return Value.Set(Regex.Replace(EditorGUI.TextField(size, "    ", Value.GetString(), editorStyles.NodeWordParameterStyle), "[a-zA-Z ]", ""));
        }

        private static Ray ElseCharacterFilter(Rect size, Ray Value, ConstellationEditorStyles editorStyles)
        {
            EditorGUI.LabelField(new Rect(size.x, size.y - 8, 30, 30), "else", editorStyles.NodeValueParameterLabelStyle);
            return Value.Set(Regex.Replace(EditorGUI.TextField(size, "    ", Value.GetString(), editorStyles.NodeWordParameterStyle), "[a-zA-Z ]", ""));
        }
    }
}

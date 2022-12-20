using System;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.Localization.UI;
using UnityEngine;

namespace GamesLab.UnityLocalizationExtension.Editor
{
    [CustomPropertyDrawer(typeof(Gameslab.UnityLocalizationExtension.SimpleLocalizedString), true)]
    class SimpleLocalizedStringPropertyDrawer : LocalizedStringPropertyDrawer
    {
        public override void OnGUI(Data data, Rect position, SerializedProperty property, GUIContent label)
        {
            var rowPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            var foldoutRect = new Rect(rowPosition.x, rowPosition.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            var dropDownPosition = new Rect(foldoutRect.x, rowPosition.y, rowPosition.width, EditorGUIUtility.singleLineHeight);
            rowPosition.MoveToNextLine();

            EditorGUI.BeginProperty(foldoutRect, label, property);

            if (data.deferredSetReference.HasValue)
            {
                data.SelectedTableCollection = data.deferredSetReference.Value.collection;
                data.SelectedTableEntry = data.deferredSetReference.Value.entry;
                data.deferredSetReference = null;
                GUI.changed = true;
            }
            
            property.isExpanded = false;
            if (EditorGUI.DropdownButton(dropDownPosition, data.FieldLabel, FocusType.Passive))
            {
                ShowPicker(data, dropDownPosition);
            }
            
            if (data.SelectedTableEntry != null)
            {
                var stringPropertyData = (StringPropertyData)data;

                for (int i = 0; i < stringPropertyData.LocaleFields.Count; ++i)
                {
                    var field = stringPropertyData.LocaleFields[i];
                    
                    var labelPos = new Rect(rowPosition.x, rowPosition.y, rowPosition.width, rowPosition.height);

                    var localeName = field.Locale.LocaleName;
                    var bracketIndex = localeName.IndexOf("(", StringComparison.Ordinal);
                    var localeTag = localeName
                        .Substring(
                            bracketIndex + 1, 
                            localeName.Length - bracketIndex - 2)
                        .ToUpper();

                    var textPreview = $"{localeTag}: {field.SmartEditor.Label}";
                    EditorGUI.LabelField(labelPos, textPreview);
                    rowPosition.MoveToNextLine();
                }
            }
            
            EditorGUI.EndProperty();
            
            // Missing table collection warning
            if (data.warningMessage != null)
            {
                rowPosition.height = data.warningMessageHeight;
                EditorGUI.HelpBox(rowPosition, data.warningMessage.text, MessageType.Warning);
                rowPosition.MoveToNextLine();
                rowPosition.height = EditorGUIUtility.singleLineHeight;
            }
        }
        
        public override float GetPropertyHeight(Data data, SerializedProperty property, GUIContent label)
        {
            var stringPropertyData = (StringPropertyData)data;
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing; // Foldout field height

            if (data.warningMessage != null)
            {
                data.warningMessageHeight = EditorStyles.helpBox.CalcHeight(data.warningMessage, EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth);
                height += data.warningMessageHeight;
            }

            if (data.SelectedTableEntry != null)
            {
                foreach (var field in stringPropertyData.LocaleFields)
                {
                    height += EditorStyles.foldoutHeader.fixedHeight + EditorGUIUtility.standardVerticalSpacing; // Locale label/foldout
                    if (field.Expanded && field.SmartEditor != null)
                    {
                        height += field.SmartEditor.Height + EditorGUIUtility.standardVerticalSpacing;                        height += field.SmartEditor.Height + EditorGUIUtility.standardVerticalSpacing;

                    }
                }
            }
            return height;
        }
    }
}
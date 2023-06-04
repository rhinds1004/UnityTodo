using System;
using UnityEditor;
using UnityEngine;

namespace UnityTodo {
    [Serializable] internal class Task {
        public string title;
        public string description;
        public float progress;
        [HideInInspector] public bool isEditing = true;

        [CustomPropertyDrawer(typeof(Task))]
        class customDrawer : PropertyDrawer {

            [NonSerialized] static readonly Color progressOutlineCol = new( 0.7f, 0.7f, 0.7f );
            [NonSerialized] static readonly Color progressBackCol = new( 0.2f, 0.2f, 0.2f );
            [NonSerialized] static readonly Color finishedProgressCol = new( 0, 0.3f, 0 );
            [NonSerialized] static readonly Color unfinishedProgressCol = new( 0, 0.5f, 0 );
            [NonSerialized] static readonly Color finishedTitleCol = new( 0.75f, 0.75f, 0.75f );
            [NonSerialized] static readonly Color unfinishedTitleCol = new( 0.95f, 0.95f, 0.95f );
            [NonSerialized] static readonly Color finishedDescCol = new( 0.7f, 0.7f, 0.7f );
            [NonSerialized] static readonly Color unfinishedDescCol = new( 0.9f, 0.9f, 0.9f );

            float lastDescriptionWidth;
            
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
                var titleProp = property.FindPropertyRelative( nameof(title) );
                var descriptionProp = property.FindPropertyRelative( nameof(description) );
                var isEditingProp = property.FindPropertyRelative( nameof(isEditing) );
                var progressProp = property.FindPropertyRelative( nameof(progress) );

                    
                using (new EditorGUI.PropertyScope( position, label, property )) {
                    position.height = EditorGUIUtility.singleLineHeight;
                    position.y += 4;
                    
                    // edit button
                    var editRect = new Rect( position.x, position.y, 25, 25 );
                    var editTexture = isEditingProp.boolValue
                        ? EditorGUIUtility.FindTexture( "SaveAs@2x" )
                        : EditorGUIUtility.FindTexture( "d_editicon.sml" );
                    if (GUI.Button( editRect, editTexture )) 
                    {
                        isEditingProp.boolValue = !isEditingProp.boolValue;
                        if (!isEditingProp.boolValue) {
                            property.serializedObject.ApplyModifiedProperties();
                        }
                    }

                    position.x += 30;
                    position.width -= 50;
                    
                    // title prop
                    position.height += 10;
                    using (new GUIUtilities.GUIColor(progressProp.floatValue == 1 ? finishedTitleCol : unfinishedTitleCol)) {
                        if (isEditingProp.boolValue) {
                            titleProp.stringValue = EditorGUI.TextField( position, titleProp.stringValue, GUIStyles.GetNormalTextField() );
                        }
                        else {
                            var title = progressProp.floatValue == 1
                                ? GUIUtilities.StrikeThrough(titleProp.stringValue)
                                : titleProp.stringValue;
                            EditorGUI.SelectableLabel( position, title, GUIStyles.GetNormalLabel() );
                        }
                    }
                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    position.x -= 30;
                    position.width += 50 - 15;
                    
                    // description prop
                    if (isEditingProp.boolValue) {
                        position.height = Mathf.Max( GUIStyles.GetSmallTextField().CalcHeight(
                            new GUIContent( descriptionProp.stringValue ), position.width ), 50 );
                        descriptionProp.stringValue = EditorGUI.TextArea( position, descriptionProp.stringValue,
                            GUIStyles.GetSmallTextField() );
                        position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    }
                    else if (!string.IsNullOrEmpty( descriptionProp.stringValue )) {
                        using (new GUIUtilities.GUIColor( progressProp.floatValue == 1 ? finishedDescCol : unfinishedDescCol )) {
                            position.height = GUIStyles.GetSmallLabel().CalcHeight(
                                new GUIContent( descriptionProp.stringValue ), position.width );
                            var title = progressProp.floatValue == 1
                                ? GUIUtilities.StrikeThrough( descriptionProp.stringValue )
                                : descriptionProp.stringValue;
                            EditorGUI.SelectableLabel( position, title, GUIStyles.GetSmallLabel() );
                            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                        }
                    }

                    if (Event.current.type == EventType.Repaint)
                        lastDescriptionWidth = position.width;
                    position.height = EditorGUIUtility.singleLineHeight;

                    // progress prop
                    if (isEditingProp.boolValue) {
                        progressProp.floatValue = EditorGUI.Slider( position, progressProp.floatValue, 0, 1 );
                    }
                    else {
                        var rect = new Rect(
                            position.x + position.width * 0,
                            position.y + position.height * 0.25f,
                            position.width * (1 - 0 * 2),
                            position.height * (1 - 0.25f * 2) );
                        EditorGUI.DrawRect( rect, progressOutlineCol );
                        rect.x += 1;
                        rect.y += 1;
                        rect.width -= 2;
                        rect.height -= 2;
                        EditorGUI.DrawRect( rect, progressBackCol );
                        rect.width *= progressProp.floatValue;
                        EditorGUI.DrawRect( rect, progressProp.floatValue == 1 ? finishedProgressCol : unfinishedProgressCol );
                    }
                }
            }

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
                var descriptionProp = property.FindPropertyRelative( nameof(description) );
                var isEditingProp = property.FindPropertyRelative( nameof(isEditing) );
                var h = 4 + (EditorGUIUtility.singleLineHeight + 10) + EditorGUIUtility.standardVerticalSpacing;

                if (isEditingProp.boolValue) {
                    h += Mathf.Max( GUIStyles.GetSmallTextField().CalcHeight(
                        new GUIContent( descriptionProp.stringValue ), lastDescriptionWidth ), 50 ) 
                         + EditorGUIUtility.standardVerticalSpacing;
                }
                else if (!string.IsNullOrEmpty( descriptionProp.stringValue )) {
                    h += GUIStyles.GetSmallLabel().CalcHeight(
                        new GUIContent( descriptionProp.stringValue ), lastDescriptionWidth ) 
                         + EditorGUIUtility.standardVerticalSpacing;
                }

                h += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                return h;
            }

        }
    }
}
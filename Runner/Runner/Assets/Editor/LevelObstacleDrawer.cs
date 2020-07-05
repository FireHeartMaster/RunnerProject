using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(LevelObstacle))]
public class LevelObstacleDrawer : PropertyDrawer
{

    float GetTextureGuiHeight(SerializedProperty property)
    {
        LevelObstacle levelObstacle = (LevelObstacle)fieldInfo.GetValue(property.serializedObject.targetObject);
        float multiplicationFactor = propertyWidth / (levelObstacle.lineThickness * (levelObstacle.Width + 1) + levelObstacle.blockThickness * (levelObstacle.Width));
        return (levelObstacle.lineThickness * (levelObstacle.Length + 1) + levelObstacle.blockThickness * (levelObstacle.Length)) * multiplicationFactor;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = base.GetPropertyHeight(property, label);
        height += GetTextureGuiHeight(property) + 3 * (verticalSpacing + labelHeight) + verticalSpacing;
        return height;
    }

    float propertyWidth = EditorGUIUtility.currentViewWidth - 35f;
    float verticalSpacing = 5f;

    float labelWidth = 10f;
    float labelHeight = 10f;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        Event guiEvent = Event.current;
        LevelObstacle levelObstacle = (LevelObstacle)fieldInfo.GetValue(property.serializedObject.targetObject);

        labelWidth = GUI.skin.label.CalcSize(label).x;
        labelHeight = GUI.skin.label.CalcSize(label).y;
        
        propertyWidth = EditorGUIUtility.currentViewWidth - 35f;

        Rect labelRect = new Rect(position.x, position.y, propertyWidth, labelHeight);
        GUI.Label(labelRect, label);
        //labelRect.y += labelHeight + verticalSpacing;

        GUIContent guiContent = new GUIContent("Width");
        /*GUI.Label(labelRect, guiContent);
        labelRect.x += GUI.skin.label.CalcSize(guiContent).x;
        labelRect.width = propertyWidth - GUI.skin.label.CalcSize(guiContent).x;
        EditorGUI.PropertyField(labelRect, property.FindPropertyRelative("lineThickness"), GUIContent.none);

        labelRect.y += labelHeight + verticalSpacing;
        labelRect.x = position.x;
        guiContent.text = "Block Thickness";
        GUI.Label(labelRect, guiContent);
        labelRect.x += GUI.skin.label.CalcSize(guiContent).x;
        labelRect.width = propertyWidth - GUI.skin.label.CalcSize(guiContent).x;
        EditorGUI.PropertyField(labelRect, property.FindPropertyRelative("blockThickness"), GUIContent.none);*/

        labelRect.y += labelHeight + verticalSpacing;
        labelRect.x = position.x;
        guiContent.text = "Number Of Block Types: ";
        GUI.Label(labelRect, guiContent);
        labelRect.x += GUI.skin.label.CalcSize(guiContent).x;
        labelRect.width = propertyWidth - GUI.skin.label.CalcSize(guiContent).x;
        int newNumberOfBlockTypes = EditorGUI.IntField(labelRect, levelObstacle.NumberOfBlockTypes);
        if (newNumberOfBlockTypes != levelObstacle.NumberOfBlockTypes) levelObstacle.NumberOfBlockTypes = newNumberOfBlockTypes;

        labelRect.y += labelHeight + verticalSpacing;
        labelRect.x = position.x;
        guiContent.text = "Width: ";
        GUI.Label(labelRect, guiContent);
        labelRect.x += GUI.skin.label.CalcSize(guiContent).x;
        labelRect.width = propertyWidth - GUI.skin.label.CalcSize(guiContent).x;
        int newWidth = EditorGUI.IntField(labelRect, levelObstacle.Width);
        if (newWidth != levelObstacle.Width) levelObstacle.Width = newWidth;

        labelRect.y += labelHeight + verticalSpacing;
        labelRect.x = position.x;
        guiContent.text = "Length: ";
        GUI.Label(labelRect, guiContent);
        labelRect.x += GUI.skin.label.CalcSize(guiContent).x;
        labelRect.width = propertyWidth - GUI.skin.label.CalcSize(guiContent).x;
        int newLength = EditorGUI.IntField(labelRect, levelObstacle.Length);
        if (newLength != levelObstacle.Length) levelObstacle.Length = newLength;

        labelRect.y += labelHeight + verticalSpacing;

        GUIStyle style = new GUIStyle();
        style.normal.background = levelObstacle.GenerateTexture();
        Rect textureRect = new Rect(position.x, labelRect.y, propertyWidth, GetTextureGuiHeight(property));
        GUI.Label(textureRect, GUIContent.none, style);

        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
        {
            if (textureRect.Contains(guiEvent.mousePosition))
            {
                //Debug.Log("mouse click");
                int posI = (int)(((guiEvent.mousePosition.y - textureRect.y) / textureRect.height) * levelObstacle.Length);
                int posJ = (int)(((guiEvent.mousePosition.x - textureRect.x) / textureRect.width) * levelObstacle.Width);
                //Debug.Log("posI, posJ: " + posI + ", " + posJ);
                levelObstacle.GenerateTexture(posI, posJ);
                foreach (var item in ActiveEditorTracker.sharedTracker.activeEditors)
                    if (item.serializedObject == property.serializedObject)
                    { item.Repaint(); return; }
            }
        }

        //EditorGUI.PropertyField(labelRect, property.FindPropertyRelative("blockThickness"), new GUIContent("blockThickness"));


    }


}

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
        height += GetTextureGuiHeight(property) + 6 * (verticalSpacing + labelHeight) + verticalSpacing;
        return height;
    }

    float propertyWidth = EditorGUIUtility.currentViewWidth - 35f;
    float verticalSpacing = 5f;

    float labelWidth = 10f;
    float labelHeight = 10f;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Event guiEvent = Event.current;
        LevelObstacle levelObstacle = (LevelObstacle)fieldInfo.GetValue(property.serializedObject.targetObject);

        labelWidth = GUI.skin.label.CalcSize(label).x;
        labelHeight = GUI.skin.label.CalcSize(label).y;
        
        propertyWidth = EditorGUIUtility.currentViewWidth - 35f;

        Rect labelRect = new Rect(position.x, position.y, propertyWidth, labelHeight);
        GUI.Label(labelRect, label);

        GUIContent guiContent = new GUIContent("Width");

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
        guiContent.text = "Number Of Collectable Types: ";
        GUI.Label(labelRect, guiContent);
        labelRect.x += GUI.skin.label.CalcSize(guiContent).x;
        labelRect.width = propertyWidth - GUI.skin.label.CalcSize(guiContent).x;
        int newNumberOfCollectableTypes = EditorGUI.IntField(labelRect, levelObstacle.NumberOfGrabbableTypes);
        if (newNumberOfCollectableTypes != levelObstacle.NumberOfGrabbableTypes) levelObstacle.NumberOfGrabbableTypes = newNumberOfCollectableTypes;

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
        labelRect.x = position.x;
        guiContent.text = "Grid Data: ";
        GUI.Label(labelRect, guiContent);
        labelRect.x += GUI.skin.label.CalcSize(guiContent).x;
        labelRect.width = propertyWidth - GUI.skin.label.CalcSize(guiContent).x;
        
        GridScriptableObject gridData = EditorGUI.ObjectField(labelRect, levelObstacle.gridData, typeof(GridScriptableObject), false) as GridScriptableObject;
        if (true/*(gridData != levelObstacle.gridData || levelObstacle.gridData == null) && gridData != null*/)
        {
            levelObstacle.gridData = gridData;
            //levelObstacle.LoadGrid(gridData.obstacleGrid, gridData.numberOfBlockTypes);
        }

        int numberOfButtons = 4;
        float percentOfWholeSpace = 1f / numberOfButtons;

        labelRect.y += labelHeight + verticalSpacing;
        labelRect.x = position.x;
        labelRect.width = propertyWidth * percentOfWholeSpace;
        if (GUI.Button(labelRect, new GUIContent("Load", "Load data from selected scriptable object"))){
            if (gridData != null)
            {
                //levelObstacle.LoadGrid(gridData.obstacleGrid, gridData.numberOfBlockTypes, gridData.collectableGrid, gridData.numberOfGrabbableTypes);
                levelObstacle.LoadGrid(gridData.auxObstacleGrid, gridData.auxCollectableGrid, gridData.length, gridData.width, gridData.numberOfBlockTypes, gridData.numberOfGrabbableTypes);
            }
        }

        labelRect.x += propertyWidth * percentOfWholeSpace;
        if (GUI.Button(labelRect, new GUIContent("Save", "Save data to selected scriptable object")))
        {
            if (gridData != null)
            {
                levelObstacle.SaveGrid();
                EditorUtility.SetDirty(levelObstacle.gridData);
                //AssetDatabase.SaveAssets();
                //EditorApplication.SaveAssets();
            }
        }

        labelRect.x += propertyWidth * percentOfWholeSpace;
        if (GUI.Button(labelRect, new GUIContent("Basic Floor", "Reset grid to basic floor in all positions")))
        {
            levelObstacle.ResetGridToBasicFloor();
        }

        labelRect.x += propertyWidth * percentOfWholeSpace;
        if (GUI.Button(labelRect, new GUIContent("Reset", "Reset grid to empty in all positions")))
        {
            levelObstacle.ResetGrid();
        }

        labelRect.x = position.x;
        labelRect.width = propertyWidth;

        labelRect.y += labelHeight + verticalSpacing;

        GUIStyle style = new GUIStyle();
        style.normal.background = levelObstacle.GenerateTexture();
        Rect textureRect = new Rect(position.x, labelRect.y, propertyWidth, GetTextureGuiHeight(property));
        GUI.Label(textureRect, GUIContent.none, style);

        if (guiEvent.type == EventType.MouseDown && (guiEvent.button == 0 || guiEvent.button == 1))
        {
            if (textureRect.Contains(guiEvent.mousePosition))
            {
                int posI = (int)(((guiEvent.mousePosition.y - textureRect.y) / textureRect.height) * levelObstacle.Length);
                int posJ = (int)(((guiEvent.mousePosition.x - textureRect.x) / textureRect.width) * levelObstacle.Width);
                
                levelObstacle.GenerateTexture(posI, posJ, guiEvent.button);
                foreach (var item in ActiveEditorTracker.sharedTracker.activeEditors)
                    if (item.serializedObject == property.serializedObject)
                    { item.Repaint(); break; } 
            }
        }

    }


}

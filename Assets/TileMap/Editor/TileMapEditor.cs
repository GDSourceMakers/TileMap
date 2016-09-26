﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BasicUtility.TileMap
{
    [CustomEditor(typeof(TileMap))]
    public class TileMapEditor : Editor
    {

        SerializedProperty tileSize;

        SerializedProperty chunkSize_x;
        SerializedProperty chunkSize_y;

        SerializedProperty chunkCount_x;
        SerializedProperty chunkCount_y;

        SerializedProperty map_x;
        SerializedProperty map_y;

        SerializedProperty oriantion;

        SerializedProperty layers;
        private ReorderableList list;
        bool layersFold;

        public static float ExpandedHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 5;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(tileSize);

            Vector2 c_size = new Vector2(chunkSize_x.intValue, chunkSize_y.intValue);
            c_size = EditorGUILayout.Vector2Field("Chnuk Size", c_size);
            chunkSize_x.intValue = (int)c_size.x;
            chunkSize_y.intValue = (int)c_size.y;

            Vector2 c_count = new Vector2(chunkCount_x.intValue, chunkCount_y.intValue);
            c_count = EditorGUILayout.Vector2Field("Chnuk Count", c_count);
            chunkCount_x.intValue = (int)c_count.x;
            chunkCount_y.intValue = (int)c_count.y;

            map_x.intValue = chunkSize_x.intValue * chunkCount_x.intValue;
            map_y.intValue = chunkSize_y.intValue * chunkCount_y.intValue;

            EditorGUILayout.LabelField("Map size", "X " + map_x.intValue + "\t Y" + map_y.intValue);

            EditorGUILayout.PropertyField(oriantion);

            layersFold = serializedObject.FindProperty("layers").isExpanded;
            layersFold = EditorGUILayout.Foldout(layersFold, new GUIContent("Layers", "Tile layers"));
            serializedObject.FindProperty("layers").isExpanded = layersFold;
            if (layersFold)
            {
                list.DoLayoutList();
                //layers = serializedObject.FindProperty("layers");
                //EditorGUILayout.PropertyField(layers,true);
            }


            serializedObject.ApplyModifiedProperties();
        }

        void OnEnable()
        {
            tileSize = serializedObject.FindProperty("tileSize");

            chunkSize_x = serializedObject.FindProperty("chunk_size_x");
            chunkSize_y = serializedObject.FindProperty("chunk_size_y");

            chunkCount_x = serializedObject.FindProperty("chunk_count_x");
            chunkCount_y = serializedObject.FindProperty("chunk_count_y");

            map_x = serializedObject.FindProperty("map_x");
            map_y = serializedObject.FindProperty("map_y");

            oriantion = serializedObject.FindProperty("oriantion");

            layers = serializedObject.FindProperty("layers");



            list = new ReorderableList(serializedObject, serializedObject.FindProperty("layers"), true, true, true, true);

            list.drawElementBackgroundCallback = DrawBack;

            list.elementHeightCallback = ElementHeight;

            list.elementHeight = EditorGUIUtility.singleLineHeight;

            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Layers");
            };

            list.drawElementCallback = DrawLayerList;

            list.onAddCallback = AddItem;
        }



        public float ElementHeight(int index)
        {
            Repaint();
            SerializedProperty property = list.serializedProperty.GetArrayElementAtIndex(index);

            if (property.FindPropertyRelative("isExpanded").boolValue)
            {
                if (!property.FindPropertyRelative("showGrid").boolValue)
                {
                    return ExpandedHeight;
                }
                else
                {
                    return ExpandedHeight + (EditorGUIUtility.singleLineHeight);
                }
            }
            else
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        public void DrawLayerList(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty property = list.serializedProperty.GetArrayElementAtIndex(index);

            bool folded;

            folded = property.FindPropertyRelative("isExpanded").boolValue;
            folded = EditorGUI.Foldout(new Rect(rect.x + 10, rect.y + 2, rect.width - 10,
                EditorGUIUtility.singleLineHeight), folded, property.FindPropertyRelative("name").stringValue);

            if (folded)
            {
                //EditorGUILayout.PropertyField(property.FindPropertyRelative("name"));


                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 1, rect.width, EditorGUIUtility.singleLineHeight),
                    property.FindPropertyRelative("name"), new GUIContent("Name"));

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2, rect.width, EditorGUIUtility.singleLineHeight),
                    property.FindPropertyRelative("position"), new GUIContent("Position"));

                property.FindPropertyRelative("rotation").quaternionValue = Quaternion.Euler(
                    EditorGUI.Vector3Field(
                    new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Rotation"),
                    property.FindPropertyRelative("rotation").quaternionValue.eulerAngles));

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 4, rect.width, EditorGUIUtility.singleLineHeight),
                    property.FindPropertyRelative("showGrid"), new GUIContent("Show grid"));

                if (property.FindPropertyRelative("showGrid").boolValue)
                {
                    EditorGUI.PropertyField(
                    new Rect(rect.x + EditorGUI.indentLevel, rect.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 5, rect.width, EditorGUIUtility.singleLineHeight),
                    property.FindPropertyRelative("color"), new GUIContent("Grid Color"));
                }
            }
            property.FindPropertyRelative("isExpanded").boolValue = folded;

        }

        public void DrawBack(Rect rect, int index, bool active, bool focused)
        {
            SerializedProperty property = list.serializedProperty.GetArrayElementAtIndex(index);

            if (property.FindPropertyRelative("isExpanded").boolValue)
            {
                if (!property.FindPropertyRelative("showGrid").boolValue)
                {
                    rect.height = ExpandedHeight;
                }
                else
                {
                    rect.height = ExpandedHeight + (EditorGUIUtility.singleLineHeight);
                }
            }
            else
            {
                rect.height = EditorGUIUtility.singleLineHeight;
            }
            rect.x += 3;
            rect.width -= 5;
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, GUI.skin.settings.selectionColor);
            tex.Apply();
            if (active)
                GUI.DrawTexture(rect, tex as Texture);
        }

        public void AddItem(ReorderableList list)
        {
            TileMap TMap = (TileMap)target;
            TMap.layers.Add(new TileLayer(TMap));

            EditorUtility.SetDirty(target);
        }



        void OnSceneGUI()
        {

            serializedObject.Update();

            for (int i = 0; i < layers.arraySize; i++)
            {
                SerializedProperty layer = layers.GetArrayElementAtIndex(i);

                SerializedProperty rot = layer.FindPropertyRelative("rotation");
                SerializedProperty pos = layer.FindPropertyRelative("position");

                if (Tools.current == Tool.Move)
                {
                    if (Tools.pivotRotation == PivotRotation.Global)
                    {
                        pos.vector3Value = Handles.PositionHandle(pos.vector3Value, Quaternion.identity);
                    }
                    else
                    {
                        pos.vector3Value = Handles.PositionHandle(pos.vector3Value, rot.quaternionValue);
                    }
                }
                else if (Tools.current == Tool.Rotate)
                {
                    rot.quaternionValue = Handles.RotationHandle(rot.quaternionValue, pos.vector3Value);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
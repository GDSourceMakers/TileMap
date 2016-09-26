using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace BasicUtility.TileMap
{

    [CustomEditor(typeof(AtlasArray))]
    public class AtlasArrayEditor : Editor
    {

        SerializedProperty x_tex_size;
        SerializedProperty y_tex_size;

        SerializedProperty x_size;
        SerializedProperty y_size;

        SerializedProperty x_count;
        SerializedProperty y_count;

        SerializedProperty textures;
        private ReorderableList list;
        bool layersFold;

        public static float ExpandedHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

        void OnEnable()
        {
            x_tex_size = serializedObject.FindProperty("x_tex_size");
            y_tex_size = serializedObject.FindProperty("y_tex_size");

            x_size = serializedObject.FindProperty("x_size");
            y_size = serializedObject.FindProperty("y_size");

            x_count = serializedObject.FindProperty("x_count");
            y_count = serializedObject.FindProperty("y_count");

            textures = serializedObject.FindProperty("textures");


            list = new ReorderableList(serializedObject, serializedObject.FindProperty("textures"), true, true, true, true);

            //list.drawElementBackgroundCallback = DrawBack;

            //list.elementHeight = EditorGUIUtility.singleLineHeight;

            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Layers");
            };

            list.drawElementCallback = DrawLayerList;

            list.onAddCallback = AddItem;



            list.onRemoveCallback = re =>
            {
                AtlasArray TMap = (AtlasArray)target;
                TMap.textures.RemoveAt(list.index);

                EditorUtility.SetDirty(target);
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(x_tex_size);
            EditorGUILayout.PropertyField(y_tex_size);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(x_count);
            EditorGUILayout.PropertyField(y_count);


            EditorGUILayout.LabelField("Tile Size", "X " + x_size.intValue + "\t Y " + y_size.intValue);


            list.DoLayoutList();

            if (GUILayout.Button("Rebuild"))
            {
                ((AtlasArray)target).GenerateArray();
            }

            AtlasArray atlasArraytarget = ((AtlasArray)target);

            bool trigger = false;

            for (int i = 0; i < atlasArraytarget.textures.Count; i++)
            {
                if (atlasArraytarget.textures[i].texelSize.x != x_tex_size.intValue || atlasArraytarget.textures[i].texelSize.y != y_tex_size.intValue)
                {
                    trigger = true;
                }
            }

            if (trigger)
            {
                EditorGUILayout.HelpBox("Warning \n Please use texures with the same size",MessageType.Error,true);
            }
            serializedObject.ApplyModifiedProperties();
        }




        public float ElementHeight(int index)
        {
            Repaint();
            SerializedProperty property = list.serializedProperty.GetArrayElementAtIndex(index);


            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        }

        public void DrawLayerList(Rect rect, int index, bool isActive, bool isFocused)
        {


            SerializedProperty property = list.serializedProperty.GetArrayElementAtIndex(index);


            //EditorGUILayout.PropertyField(property.FindPropertyRelative("name"));

            EditorGUI.LabelField(
                new Rect(rect.x, rect.y + 2, 20, EditorGUIUtility.singleLineHeight),
                index.ToString()
                );

            //property.

            property.objectReferenceValue = (Texture2D)EditorGUI.ObjectField(
                new Rect(rect.x + 20, rect.y + 2, rect.width - 25, EditorGUIUtility.singleLineHeight),
                (Texture2D)property.objectReferenceValue, typeof(Texture2D), false);

        }

        public void DrawBack(Rect rect, int index, bool active, bool focused)
        {
            SerializedProperty property = list.serializedProperty.GetArrayElementAtIndex(index);


            rect.height = EditorGUIUtility.singleLineHeight;

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
            AtlasArray TMap = (AtlasArray)target;
            TMap.textures.Add(null);

            EditorUtility.SetDirty(target);
        }



        [MenuItem("Assets/Create/Sprites/Atlas Array")]
        public static void CreateAsset()
        {
            CustomAssetUtility.CreateAsset<AtlasArray>();
        }
    }
}
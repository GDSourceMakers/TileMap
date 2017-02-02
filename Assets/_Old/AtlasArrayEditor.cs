using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
/*
namespace BasicUtility.TileMap
{

	[CustomEditor(typeof(AtlasArray))]
	public class AtlasArrayEditor : Editor
	{

		SerializedProperty textureSizeX;
		SerializedProperty textureSizeY;

		SerializedProperty spriteSizeX;
		SerializedProperty spriteSizeY;

		SerializedProperty spriteCountX;
		SerializedProperty spriteCountY;

		SerializedProperty missingTexture;
		//SerializedProperty textures;
		private ReorderableList list;
		bool layersFold;

		bool dirtySub;

		public static float ExpandedHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

		void OnEnable()
		{
			if (serializedObject == null)
			{
				return;
			}

			textureSizeX = serializedObject.FindProperty("textureSizeX");
			textureSizeY = serializedObject.FindProperty("textureSizeY");

			spriteSizeX = serializedObject.FindProperty("spriteSizeX");
			spriteSizeY = serializedObject.FindProperty("spriteSizeY");

			spriteCountX = serializedObject.FindProperty("spriteCountX");
			spriteCountY = serializedObject.FindProperty("spriteCountY");

			//textures = serializedObject.FindProperty("textures");

			missingTexture = serializedObject.FindProperty("missingTexture");

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

			EditorGUILayout.PropertyField(textureSizeX, new GUIContent("With", "With of the atlas"));
			EditorGUILayout.PropertyField(textureSizeY, new GUIContent("Height", "Height of the atlas"));

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(spriteCountX, new GUIContent("Horisontal count"));
			EditorGUILayout.PropertyField(spriteCountY, new GUIContent("Vertical count"));

			if (spriteCountX.intValue != 0 && spriteCountY.intValue != 0)
			{
				dirtySub = (!(textureSizeX.intValue / spriteCountX.intValue == spriteSizeX.intValue) || !(textureSizeY.intValue / spriteCountY.intValue == spriteSizeY.intValue) || dirtySub);


				spriteSizeX.intValue = textureSizeX.intValue / spriteCountX.intValue;
				spriteSizeY.intValue = textureSizeY.intValue / spriteCountY.intValue;
			}
			else
			{
				spriteSizeX.intValue = 0;
				spriteSizeY.intValue = 0;
			}

			EditorGUILayout.LabelField("Tile Size", "X " + spriteSizeX.intValue + "\t Y " + spriteSizeY.intValue);

			missingTexture.objectReferenceValue = (Texture2D)EditorGUILayout.ObjectField("Missing Texture", (Texture2D)missingTexture.objectReferenceValue, typeof(Texture2D), false);

			list.DoLayoutList();

			if (GUILayout.Button("Rebuild"))
			{
				((AtlasArray)target).GenerateAssets();
				dirtySub = false;
			}

			if (GUILayout.Button("Clear"))
			{
				((AtlasArray)target).EraseSubAsset();
			}

			AtlasArray atlasArraytarget = ((AtlasArray)target);

			#region Error Checking

			for (int i = 0; i < atlasArraytarget.textures.Count; i++)
			{
				if (atlasArraytarget.textures[i] != null)
				{
					if (atlasArraytarget.textures[i].width != textureSizeX.intValue || atlasArraytarget.textures[i].height != textureSizeY.intValue)
					{

						EditorGUILayout.HelpBox("Warning \n Please use texures with the same size", MessageType.Error, true);
						break;
					}
				}
				else
				{
					EditorGUILayout.HelpBox("Warning \n Please assigne texture for every entry", MessageType.Error, true);
					break;
				}
			}

			if (spriteCountX.intValue == 0 || spriteCountY.intValue == 0)
			{
				EditorGUILayout.HelpBox("Warning \nPlease set the horisontal sprite count \nIt can not be 0", MessageType.Error);
			}

			if (spriteCountX.intValue == 0 || spriteCountY.intValue == 0)
			{
				EditorGUILayout.HelpBox("Warning \nPlease set the vrtical sprite count \nIt can not be 0", MessageType.Error);
			}

			if (dirtySub)
			{
				EditorGUILayout.HelpBox("Warning \nNedds Rebuild", MessageType.Warning);
			}

				#endregion

				if (atlasArraytarget.textures.Count == 1 && atlasArraytarget.textures[0] != null)
				{
					textureSizeX.intValue = atlasArraytarget.textures[0].width;
					textureSizeY.intValue = atlasArraytarget.textures[0].height;
				}


			serializedObject.ApplyModifiedProperties();
		}

		public float ElementHeight(int index)
		{
			Repaint();
			//SerializedProperty property = list.serializedProperty.GetArrayElementAtIndex(index);


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
			//SerializedProperty property = list.serializedProperty.GetArrayElementAtIndex(index);


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


		public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
		{
			//Debug.Log("asd");
			AtlasArray array = target as AtlasArray;

			if (array == null || array.textures == null || array.textures.Count == 0 || array.textures[0] == null)
				return null;

			Texture2D cache = new Texture2D(width, height);
			EditorUtility.CopySerialized(AssetPreview.GetAssetPreview(array.textures[0]), cache);
			return cache;
		}

		/*
		public override void DrawPreview(Rect previewArea)
		{
			AtlasArray array = target as AtlasArray;
			GUI.DrawTexture(previewArea, array.textures[0]);
		}
		
		[MenuItem("Assets/Create/Sprites/Atlas Array")]
		public static void CreateAsset()
		{
			CustomAssetUtility.CreateAsset<AtlasArray>();
		}

	}
}
*/
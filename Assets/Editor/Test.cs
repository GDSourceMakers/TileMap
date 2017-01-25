using UnityEngine;
using System.Collections;
using UnityEditor;


// DrawGizmoGrid.cs
// draws a useful reference grid in the editor in Unity. 
// 09/01/15 - Hayden Scott-Baron
// twitter.com/docky 
// no attribution needed, but please tell me if you like it ^_^

/*
public class DisplayProgressBarExample : EditorWindow
{
	static int secs = 0;
	static double startVal = 0;
	static double progress = 0;


	[MenuItem("Example/Simple Progress Bar")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		DisplayProgressBarExample window = (DisplayProgressBarExample)EditorWindow.GetWindow(typeof(DisplayProgressBarExample));
		window.Show();
	}

	void OnGUI()
	{
		if (GUILayout.Button("Rebuild"))
		{
			AtlasArray a = CustomAssetUtility.CreateAsset<AtlasArray>();


			for (int i = 0; i < 50; i++)
			{
				CreateSubAsset(ScriptableObject.CreateInstance<TileSprite>(),a);
				//EditorUtility.DisplayProgressBar("Simple Progress Bar", "Shows a progress bar for the given seconds",(float)i/50);
			}

		}
	}

	void OnInspectorUpdate()
	{
		Repaint();

	}

    
	public void CreateSubAsset(TileSprite asset, AtlasArray parent)
	{

		string path = AssetDatabase.GetAssetPath(parent);

		//string assetPathAndName = path + "/ " + asset.name + ".asset";

		AssetDatabase.AddObjectToAsset(asset, parent);
		AssetDatabase.ImportAsset(path);
		AssetDatabase.SaveAssets();
	}
    

	public class Test2 : MonoBehaviour
	{
		// universal grid scale
		public float gridScale = 1f;

		// extents of the grid
		public int minX = -15;
		public int minY = -15;
		public int maxX = 15;
		public int maxY = 15;

		// nudges the whole grid rel
		public Vector3 gridOffset = Vector3.zero;

		// is this an XY or an XZ grid?
		public bool topDownGrid = true;

		// choose a colour for the gizmos
		public int gizmoMajorLines = 5;
		public Color gizmoLineColor = new Color(0.4f, 0.4f, 0.3f, 1f);

		// rename + centre the gameobject upon first time dragging the script into the editor. 
		void Reset()
		{
			if (name == "GameObject")
				name = "~~ GIZMO GRID ~~";

			transform.position = Vector3.zero;
		}

		// draw the grid :) 
		void OnDrawGizmos()
		{
			// orient to the gameobject, so you can rotate the grid independently if desired
			Gizmos.matrix = transform.localToWorldMatrix;

			// set colours
			Color dimColor = new Color(gizmoLineColor.r, gizmoLineColor.g, gizmoLineColor.b, 0.25f * gizmoLineColor.a);
			Color brightColor = Color.Lerp(Color.white, gizmoLineColor, 0.75f);

			// draw the horizontal lines
			for (int x = minX; x < maxX + 1; x++)
			{
				// find major lines
				Gizmos.color = (x % gizmoMajorLines == 0 ? gizmoLineColor : dimColor);
				if (x == 0)
					Gizmos.color = brightColor;

				Vector3 pos1 = new Vector3(x, minY, 0) * gridScale;
				Vector3 pos2 = new Vector3(x, maxY, 0) * gridScale;

				// convert to topdown/overhead units if necessary
				if (topDownGrid)
				{
					pos1 = new Vector3(pos1.x, 0, pos1.y);
					pos2 = new Vector3(pos2.x, 0, pos2.y);
				}

				Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
			}

			// draw the vertical lines
			for (int y = minY; y < maxY + 1; y++)
			{
				// find major lines
				Gizmos.color = (y % gizmoMajorLines == 0 ? gizmoLineColor : dimColor);
				if (y == 0)
					Gizmos.color = brightColor;

				Vector3 pos1 = new Vector3(minX, y, 0) * gridScale;
				Vector3 pos2 = new Vector3(maxX, y, 0) * gridScale;

				// convert to topdown/overhead units if necessary
				if (topDownGrid)
				{
					pos1 = new Vector3(pos1.x, 0, pos1.y);
					pos2 = new Vector3(pos2.x, 0, pos2.y);
				}

				Gizmos.DrawLine((gridOffset + pos1), (gridOffset + pos2));
			}
		}
	}
}
*/
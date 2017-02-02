using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
[CustomEditor(typeof(TileSprite))]
public class TileSpriteInspector : Editor
{
	/*
	public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
	{
		Debug.Log("asd");
		TileSprite sprite = target as TileSprite;

		if (sprite == null)
			return null;

		Texture2D cache = new Texture2D(width, height);
		EditorUtility.CopySerialized(AssetPreview.GetAssetPreview(s), cache);
		return cache;
	}
	
}
*/
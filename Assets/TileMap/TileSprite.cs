using UnityEngine;
using System.Collections;

public class TileSprite : ScriptableObject {

    AtlasArray array;
    int layer;
    int x_pos;
    int y_pos;
    new Vector2[] uv = new Vector2[4];


    public TileSprite(AtlasArray arr,int i ,int x, int y, Vector2[] uvNew)
    {
        array = arr;
        layer = i;
        x_pos = x;
        x_pos = y;
        uv = uvNew;
    }
	
}


/*
[HideInInspector]
	public Color[] _sprite_color;

	public Color[] sprite_color
	{
		get
		{
			if (_sprite_color.Length > 0)
			{
				return _sprite_color;
			}
			else
			{
				_sprite_color = GetTexture();
				return _sprite_color;
			}
		}
		set { _sprite_color = value; }
	}

	Color[] GetTexture()
	{

		Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
												(int)sprite.textureRect.y,
												(int)sprite.textureRect.width,
												(int)sprite.textureRect.height);
		return pixels;
	}

	public void GenerateColorArray()
	{

	}
 */

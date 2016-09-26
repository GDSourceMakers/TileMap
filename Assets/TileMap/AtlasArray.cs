using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AtlasArray : ScriptableObject
{
    public TileSprite[,,] spriteArray;

    public List<Texture2D> textures;

    public int x_tex_size;
    public int y_tex_size;

    public int x_size;
    public int y_size;

    public int x_count;
    public int y_count;

    Texture2DArray array;

    public void GenerateArray()
    {
        if (textures[0] != null)
        {
            x_size = x_tex_size / x_count;
            y_size = y_tex_size / y_count;

            for (int i = 0; i < textures.Count; i++)
            {
                for (int x = 0; x < x_count; x++)
                {
                    for (int y = 0; y < y_count; y++)
                    {
                        Vector2[] uv = new Vector2[4];
                        uv[0] = new Vector2(x, y);
                        uv[1] = new Vector2(x + x_size, y);
                        uv[2] = new Vector2(x, y + y_size);
                        uv[3] = new Vector2(x + x_size, y + y_size);
                        spriteArray[i, x, y] = new TileSprite(this, i, x, y, uv);
                    }
                }
            }
        }
    }


}

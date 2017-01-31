using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BasicUtility.TileMap;
using System;

[RequireComponent(typeof(TileMap))]
public class TestGen : MonoBehaviour
{

	//public GameObject testSprite;
	//public TileMap tMap;
	//public List<TileLayer> test;

	public TileMap map;
	public Sprite tileSprite;
	public Sprite tileSpriteSecound;

	public int x_max;
	public int y_max;

    public Color testColor;

	// Use this for initialization
	void Start()
	{
		for (int i = 0; i < x_max; i++)
		{
			for (int j = 0; j < y_max; j++)
			{

				map.grid[i, j, 0] = new TestTileClass(tileSprite, testColor);

			}
		}
        //TestAdvancedTile large = new TestAdvancedTile(tileSprite,tileSpriteSecound);
        //map.SetTile(large, new TilePosition(1, 0, 0, map));
        //map.SetTile(large, new TilePosition(1, 1, 0, map));

        map.GenerateAllMesh();

		StartCoroutine(Draw());
	}

	// Update is called once per frame
	void Update()
	{
		//Debug.LogError("asd");
	}


	public IEnumerator Draw()
	{
		for (int i = 0; i < x_max; i++)
		{
			for (int j = 0; j < y_max; j++)
			{
				map.grid[i, j, 0].colider = !map.grid[i, j, 0].colider;
				if (map.grid[i, j, 0].sprite == tileSpriteSecound)
				{
					map.grid[i, j, 0].sprite = tileSprite;
					
				}
				else
				{
					map.grid[i, j, 0].sprite = tileSpriteSecound;
				}
				//map.GenerateCollision();
				yield return j;
			}
		}

		StartCoroutine(Draw());
	}

}

public class TestTileClass : TileBehaviour
{
	public TestTileClass(Sprite setTexture, Color tColor)
	{
		TileSetup(true, true,this);

		sprite = setTexture;

        color = tColor;

        colider = true;
	}

	public void Start()
	{
		Debug.Log("test");
	}
}

public class TestTileClass2 : TileBehaviour
{
	public TestTileClass2(Sprite setTexture)
	{
		TileSetup(true, true, this);

		sprite = setTexture;

		colider = true;
	}

	public void Start()
	{
		Debug.Log("test");
	}
}

public class TestAdvancedTile : TileBehaviour, IAdvancedSprite
{
    Sprite a;
    Sprite b;

    public TestAdvancedTile(Sprite set_a,Sprite set_b)
    {
        TileSetup(true, true, this);
        a = set_a;
        b = set_b;
    }

    public Sprite GetSprite(int x, int y, Layer l)
    {
        if (x == 0)
        {
            return a;
        }
        else
        {
            return b;
        }
    }
}
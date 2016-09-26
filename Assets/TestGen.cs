using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BasicUtility.TileMap;

[RequireComponent(typeof(TileMap))]
public class TestGen : MonoBehaviour
{

	//public GameObject testSprite;
	//public TileMap tMap;
	//public List<TileLayer> test;

	public TileMap map;
	public Sprite tileSprite;
	public Sprite tileSpriteSecound;

	// Use this for initialization
	void Start()
	{
		for (int i = 0; i < 64; i++)
		{
			for (int j = 0; j < 64; j++)
			{

				map.grid[i, j, 0] = new TestTileClass(tileSprite);

			}
		}
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
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < 16; j++)
			{
				map.grid[i, j, 0].colider = !map.grid[i, j, 0].colider;
				map.grid[i, j, 0].sprite = tileSpriteSecound;
				//map.GenerateCollision();
				yield return j;
			}
		}

		StartCoroutine(Draw());
	}

}

public class TestTileClass : TileClass
{
	public TestTileClass(Sprite setTexture)
	{
		sprite = setTexture;

		colider = true;
	}

}

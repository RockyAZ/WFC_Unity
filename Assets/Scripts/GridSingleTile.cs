using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Enums;


[RequireComponent(typeof(SpriteRenderer))]
public class GridSingleTile : MonoBehaviour
{
	public Action<Vector2Int> ON_CHANGED;

	private SpriteRenderer _spriteRenderer;

	public List<TileDirs> possibleTiles = new List<TileDirs>();
	public Vector2Int coordinates;

	//foreach int[] 0-top|1-right|2-bot|3-left
	//if one of int[] become 0 -> change neighbours

	public bool Solved { get; private set; }

	public Dictionary<TileDirType, int[]> tilesTypeAmount = new Dictionary<TileDirType, int[]>();

	void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Initialize(List<TileDirs> startTiles, Vector2Int coordinate)
	{
		possibleTiles.AddRange(startTiles);
		coordinates = coordinate;

		tilesTypeAmount[TileDirType.Grass] = new int[4];
		tilesTypeAmount[TileDirType.Road] = new int[4];

		foreach (var tile in startTiles)
		{
			for (int i = 0; i < tile.DirTypes.Length; i++)
			{
				tilesTypeAmount[tile.DirTypes[i]][i]++;
			}
		}
	}

	public void SetTile(TileDirs tile)
	{
		//possibleTiles
		Array.Fill(tilesTypeAmount[TileDirType.Grass], 0);
		Array.Fill(tilesTypeAmount[TileDirType.Road], 0);

		for (int i = 0; i < tile.DirTypes.Length; i++)
		{
			tilesTypeAmount[tile.DirTypes[i]][i]++;
		}

		SetSprite(tile.Sprite);
		transform.rotation = tile.Rotation;
		possibleTiles.Clear();
		possibleTiles.Add(tile);
		Solved = true;
		Debug.Log("SOLVED:"+coordinates);
		//Debug.Log("tile.Sprite:" + tile.Sprite.name);
	}

	public void SetRandomPossibleTile()
	{
		SetTile(possibleTiles.GetRandomElement());
	}

	private TileDir RepeatTileDir(TileDir currentDir, int amountToAdd)
	{
		return (TileDir)UnityEngine.Mathf.Repeat((int)currentDir + amountToAdd, Enums.EnumsValues.TileDirAmount);
	}

	public void NeighborChanged(GridSingleTile neighbor, TileDir neighborDir)
	{
		//print((TileDir)UnityEngine.Mathf.Repeat((int)TileDir.Left + 2, Enums.Enums.TileDirAmount));

		var removeList = new List<TileDirs>();

		foreach (var possibleTile in possibleTiles)
		{
			if (neighbor.tilesTypeAmount[possibleTile.DirTypes[(int)neighborDir]][(int)RepeatTileDir(neighborDir, 2)] <= 0)
			{
				removeList.Add(possibleTile);
			}
		}

		int deletedAmount = 0;

		foreach (var tileToRemove in removeList)
		{
			deletedAmount++;
			possibleTiles.Remove(tileToRemove);
		}

		if(deletedAmount > 0)
			ON_CHANGED.Invoke(coordinates);
	}

	private void SetSprite(Sprite sprite)
	{
		_spriteRenderer.sprite = sprite;
	}
}

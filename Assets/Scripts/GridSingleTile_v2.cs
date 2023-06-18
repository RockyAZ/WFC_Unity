using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Enums;

public struct TileChangeAnswer
{
	public bool[] ChangeInfoArray;
	public bool AnyDirEnded;

	public TileChangeAnswer(bool[] changeInfoArray, bool anyDirEnded)
	{
		ChangeInfoArray = changeInfoArray;
		AnyDirEnded = anyDirEnded;
	}
}


[RequireComponent(typeof(SpriteRenderer))]
public class GridSingleTile_v2 : MonoBehaviour
{
	private SpriteRenderer _spriteRenderer;

	public List<TileDirs> possibleTiles = new List<TileDirs>();
	public Vector2Int coordinates;

	//foreach int[] 0-top|1-right|2-bot|3-left

	public bool Solved { get; private set; }

	//x = tile direction | y = tile type
	private int[] tilesTypeAmount;

	void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Initialize(List<TileDirs> startTiles, Vector2Int coordinate)
	{
		possibleTiles.AddRange(startTiles);
		coordinates = coordinate;

		tilesTypeAmount = new int[EnumsValues.TileDirAmount * EnumsValues.TileDirTypeAmount];

		foreach (var tile in startTiles)
		{
			for (int i = 0; i < tile.DirTypes.Length; i++)
			{
				//tilesTypeAmount[((int)tile.DirTypes[i] * EnumsValues.TileDirAmount) + i]++;
				AddTileAmount((int)tile.DirTypes[i], i, 1);
			}
		}
	}

	public TileChangeAnswer SetTile(TileDirs tile)
	{
		int[] tmpCopy = new int[tilesTypeAmount.Length];
		int size = sizeof(int);
		int length = tilesTypeAmount.Length * size;
		System.Buffer.BlockCopy(tilesTypeAmount, 0, tmpCopy, 0, length);

		Array.Fill(tilesTypeAmount, 0);

		for (int i = 0; i < tile.DirTypes.Length; i++)
		{
			//tilesTypeAmount[((int)tile.DirTypes[i] * EnumsValues.TileDirAmount) + i]++;
			AddTileAmount((int)tile.DirTypes[i], i, 1);
		}

		SetSprite(tile.Sprite);
		transform.rotation = tile.Rotation;
		possibleTiles.Clear();
		possibleTiles.Add(tile);
		Solved = true;

		bool[] result = new bool[tilesTypeAmount.Length];
		bool wasSingleEnd = false;

		for (int i = 0; i < tilesTypeAmount.Length; i++)
		{
			if (tilesTypeAmount[i] <= 0 && tmpCopy[i] > 0)
			{
				result[i] = true;
				wasSingleEnd = true;
			}
			else
				result[i] = false;
		}

		return new TileChangeAnswer(result, wasSingleEnd);
	}

	public TileChangeAnswer SetRandomPossibleTile()
	{
		return SetTile(possibleTiles.GetRandomElement());
	}

	private TileDir RepeatTileDir(TileDir currentDir, int amountToAdd)
	{
		return (TileDir)UnityEngine.Mathf.Repeat((int)currentDir + amountToAdd, Enums.EnumsValues.TileDirAmount);
	}

	public TileChangeAnswer NeighborChanged(GridSingleTile_v2 neighbor, TileDir neighborDir)
	{
		var removeList = new List<TileDirs>();

		int[] tmpCopy = new int[tilesTypeAmount.Length];
		int size = sizeof(int);
		int length = tilesTypeAmount.Length * size;
		System.Buffer.BlockCopy(tilesTypeAmount, 0, tmpCopy, 0, length);


		foreach (var possibleTile in possibleTiles)
		{
			if(neighbor.GetTileAmount((int)possibleTile.DirTypes[(int)neighborDir], (int)RepeatTileDir(neighborDir, 2)) <= 0)
			{
				removeList.Add(possibleTile);
			}
		}

		foreach (var tileToRemove in removeList)
		{
			for (int i = 0; i < tileToRemove.DirTypes.Length; i++)
			{
				//tilesTypeAmount[((int)tile.DirTypes[i] * EnumsValues.TileDirAmount) + i]++;
				AddTileAmount((int)tileToRemove.DirTypes[i], i, -1);
			}
			possibleTiles.Remove(tileToRemove);
		}

		bool[] result = new bool[tilesTypeAmount.Length];
		bool wasSingleEnd = false;

		for (int i = 0; i < tilesTypeAmount.Length; i++)
		{
			if (tilesTypeAmount[i] <= 0 && tmpCopy[i] > 0)
			{
				result[i] = true;
				wasSingleEnd = true;

			}
			else
				result[i] = false;
		}

		return new TileChangeAnswer(result, wasSingleEnd);
	}

	private void SetSprite(Sprite sprite)
	{
		_spriteRenderer.sprite = sprite;
	}

	private void AddTileAmount(int dirType, int tileDir, int amount)
	{
		tilesTypeAmount[(dirType * EnumsValues.TileDirAmount) + tileDir] += amount;
	}

	public int GetTileAmount(int dirType, int tileDir)
	{
		return tilesTypeAmount[(dirType * EnumsValues.TileDirAmount) + tileDir];
	}
}
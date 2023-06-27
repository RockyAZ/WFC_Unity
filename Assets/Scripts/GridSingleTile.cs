using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using Enums;

[RequireComponent(typeof(SpriteRenderer))]
public class GridSingleTile : MonoBehaviour
{
	private static HashSet<GridSingleTile> _pendingCells = new HashSet<GridSingleTile>();

	private SpriteRenderer _spriteRenderer;
	private Grid _grid;

	//[HideInInspector]
	//domain
	
	//TODO
	//use bool array instead of list
	
	public List<TileDirs> possibleTiles;
	[HideInInspector]
	public Vector2Int coordinates;

	public bool Solved;
	public int PossibleTilesAmount => possibleTiles.Count;

	//x = tile direction | y = tile type
	private int[] tilesTypeAmount;

	void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Initialize(List<TileDirs> startTiles, Vector2Int coordinate, Grid grid)
	{
		possibleTiles = new List<TileDirs>();
		foreach (var VARIABLE in startTiles)
		{
			possibleTiles.Add(VARIABLE);
		}
		//possibleTiles.AddRange(startTiles);
		coordinates = coordinate;
		_grid = grid;

		tilesTypeAmount = new int[EnumsValues.TileDirAmount * EnumsValues.TileDirTypeAmount];

		foreach (var tile in startTiles)
		{
			for (int i = 0; i < tile.DirTypes.Length; i++)
			{
				AddTileAmount((int)tile.DirTypes[i], i, 1);
			}
		}
	}

	public float GetEntropy()
	{
		return Extentions.CalculateEntropy(possibleTiles.Select(t => t.Probability).ToList());
	}

	public void SetTile(TileDirs tile, bool solve)
	{
		bool[] result = new bool[tilesTypeAmount.Length];
		Array.Fill(result, true);
		Array.Fill(tilesTypeAmount, 0);

		for (int i = 0; i < tile.DirTypes.Length; i++)
		{
			int integerDirType = (int)tile.DirTypes[i];
			AddTileAmount(integerDirType, i, 1);
		
			if (GetTileAmount(integerDirType, i) <= 0)
			{
				SetDirectionChanged(result, i, integerDirType, false);
			}
		}

		SetSprite(tile.Sprite);
		transform.rotation = tile.Rotation;
		possibleTiles.Clear();
		possibleTiles.Add(tile);
		Solved = true;

		if(solve)
			Solve(result);
	}

	public void SetRandomPossibleTile()
	{
		if (possibleTiles == null)
			Debug.LogError("possibleTiles == null");
		if (possibleTiles.Count < 1)
		{
			Debug.LogError("possibleTiles.Count < 1");
			Debug.LogError("coord:"+coordinates);
			Solved = true;
			transform.parent = null;
			return;
		}

		SetTile(possibleTiles.GetRandomElement(), true);
	}

	public void SetLeastEntropyTile()
	{
		if (possibleTiles == null)
			Debug.LogError("possibleTiles == null");
		if (possibleTiles.Count < 1)
		{
			Debug.LogError("possibleTiles.Count < 1");
			Debug.LogError("coord:" + coordinates);
			Solved = true;
			transform.parent = null;
			return;
		}

		var maxProb = possibleTiles.Max(t => t.Probability);
		var element = possibleTiles.Where(t => t.Probability >= maxProb).GetRandomElement();

		SetTile(element, true);
	}

	private TileDir RepeatTileDir(TileDir currentDir, int amountToAdd)
	{
		return (TileDir)UnityEngine.Mathf.Repeat((int)currentDir + amountToAdd, Enums.EnumsValues.TileDirAmount);
	}

	public void NeighborChanged(GridSingleTile neighbor, TileDir neighborDir)
	{
		var removeList = new List<TileDirs>();

		foreach (var possibleTile in possibleTiles)
		{
			if (neighbor.GetTileAmount((int)possibleTile.DirTypes[(int)neighborDir], (int)RepeatTileDir(neighborDir, 2)) <= 0)
			{
				removeList.Add(possibleTile);
			}
		}
		
		bool[] result = new bool[tilesTypeAmount.Length];

		foreach (var tileToRemove in removeList)
		{
			for (int i = 0; i < tileToRemove.DirTypes.Length; i++)
			{
				int integerDirType = (int)tileToRemove.DirTypes[i];
				int beforeRemoveAmount = GetTileAmount(integerDirType, i);
				
				AddTileAmount((int)tileToRemove.DirTypes[i], i, -1);

				if (beforeRemoveAmount > 0 && GetTileAmount(integerDirType, i) <= 0)
				{
					SetDirectionChanged(result, i, integerDirType, true);
				}
			}
			possibleTiles.Remove(tileToRemove);
		}

		Solve(result);
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

	public void Solve(bool[] resultArray)
	{
		_pendingCells.Add(this);
		foreach (TileDir dir in Enum.GetValues(typeof(TileDir)))
		{
			if (IsDirectionChanged(resultArray, dir) && Grid.IsExistTile(dir, 1, coordinates))
			{
				var cell = _grid.GetTile(dir, 1, coordinates);
				if (!_pendingCells.Contains(cell) && !cell.Solved)
				{
					cell.NeighborChanged(this, RepeatTileDir(dir, 2));
				}
			}
		}
		_pendingCells.Remove(this);
	}

	public static bool IsDirectionChanged(bool[] result, TileDir direction)
	{
		return result[((int)TileDirType.Grass * EnumsValues.TileDirAmount) + (int)direction] ||
		       result[((int)TileDirType.Road * EnumsValues.TileDirAmount) + (int)direction];
	}

	public static bool IsDirectionChanged(bool[] result, int direction)
	{
		return result[((int)TileDirType.Grass * EnumsValues.TileDirAmount) + direction] ||
		       result[((int)TileDirType.Road * EnumsValues.TileDirAmount) + direction];
	}

	public static void SetDirectionChanged(bool[] result, TileDir direction, TileDirType dirType, bool value)
	{
		result[((int)dirType * EnumsValues.TileDirAmount) + (int)direction] = value;
	}

	public static void SetDirectionChanged(bool[] result, int direction, int dirType, bool value)
	{
		result[(dirType * EnumsValues.TileDirAmount) + direction] = value;
	}

}
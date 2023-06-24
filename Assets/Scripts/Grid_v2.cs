using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering;
using EasyButtons;
using UnityEngine.Playables;

public class Grid_v2 : MonoBehaviour
{
	public static int X = 3;
	public static int Y = 3;

	public int XSize = 3;
	public int YSize = 3;
	public List<TileData> _tiles = new List<TileData>();

	System.Random _random = new System.Random();

	public float WaitTimeToRender;

	[Space] public GridSingleTile_v2 _tilePrefab;

	private List<TileDirs> allPossibleTiles = new List<TileDirs>();

	private GridSingleTile_v2[] _gridTiles;

	private void Awake()
	{
		X = XSize;
		Y = YSize;

		foreach (var tiledata in _tiles)
		{
			allPossibleTiles.AddRange(tiledata.GetAllTileDirs());
		}

		_gridTiles = new GridSingleTile_v2[XSize * YSize];

		for (int x = 0; x < XSize; x++)
		{
			for (int y = 0; y < YSize; y++)
			{
				var instance = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
				instance.Initialize(allPossibleTiles, new Vector2Int(x, y), this);
				_gridTiles[x * XSize + y] = instance;
			}
		}
	}

	void Start()
	{
		StartCoroutine(SolveTiles());
	}

	private void SetTileChanged(Vector2Int coord)
	{
		var tileChanged = _gridTiles[coord.x * XSize + coord.y];
		//top
		if (!(coord.y + 1 >= YSize))
			_gridTiles[coord.x * XSize + coord.y + 1].NeighborChanged(tileChanged, TileDir.Bot);
		//right
		if (!(coord.x + 1 >= XSize))
			_gridTiles[(coord.x + 1) * XSize + coord.y].NeighborChanged(tileChanged, TileDir.Left);
		//bot
		if (!(coord.y - 1 < 0))
			_gridTiles[coord.x * XSize + coord.y - 1].NeighborChanged(tileChanged, TileDir.Top);
		//left
		if (!(coord.x - 1 < 0))
			_gridTiles[(coord.x - 1) * XSize + coord.y].NeighborChanged(tileChanged, TileDir.Right);
	}

	private void SetFirstTile()
	{
		//_gridTiles[0].SetTile(_gridTiles[0].possibleTiles.GetRandomElement(_random));
		_gridTiles[0].SetRandomPossibleTile();
		SetTileChanged(new Vector2Int(0, 0));
	}

	private void SetTileChanged_2(TileChangeAnswer result, Vector2Int coord)
	{
		var tileChanged = _gridTiles[coord.x * XSize + coord.y];
		//top
		if (!(coord.y + 1 >= YSize) && IsDirectionChanged(result.ChangeInfoArray, TileDir.Top))
			_gridTiles[coord.x * XSize + coord.y + 1].NeighborChanged(tileChanged, TileDir.Bot);
		//right
		if (!(coord.x + 1 >= XSize) && IsDirectionChanged(result.ChangeInfoArray, TileDir.Right))
			_gridTiles[(coord.x + 1) * XSize + coord.y].NeighborChanged(tileChanged, TileDir.Left);
		//bot
		if (!(coord.y - 1 < 0) && IsDirectionChanged(result.ChangeInfoArray, TileDir.Bot))
			_gridTiles[coord.x * XSize + coord.y - 1].NeighborChanged(tileChanged, TileDir.Top);
		//left
		if (!(coord.x - 1 < 0) && IsDirectionChanged(result.ChangeInfoArray, TileDir.Left))
			_gridTiles[(coord.x - 1) * XSize + coord.y].NeighborChanged(tileChanged, TileDir.Right);
	}

	public GridSingleTile_v2 GetGridTile(int x, int y)
	{
		return _gridTiles[x * XSize + y];
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

	public static bool IsExistTile(TileDir dir, int distance, Vector2Int currentCoord)
	{
		if (dir == TileDir.Top)
			return currentCoord.y + distance < Y;
		if (dir == TileDir.Right)
			return currentCoord.x + 1 < X;
		if (dir == TileDir.Bot)
			return currentCoord.y - 1 >= 0;
		if (dir == TileDir.Left)
			return currentCoord.x - 1 >= 0;
		return false;
	}

	public GridSingleTile_v2 GetTile(TileDir dir, int distance, Vector2Int currentCoord)
	{
		if (dir == TileDir.Top)
			return GetGridTile(currentCoord.x, currentCoord.y + 1);
		if (dir == TileDir.Right)
			return GetGridTile(currentCoord.x + 1, currentCoord.y);
		if (dir == TileDir.Bot)
			return GetGridTile(currentCoord.x, currentCoord.y - 1);
		if (dir == TileDir.Left)
			return GetGridTile(currentCoord.x - 1, currentCoord.y);

		return null;
	}


	private void SetFirstTile_2()
	{
		_gridTiles[0].SetRandomPossibleTile();
	}


	private IEnumerator SolveTiles()
	{
		SetFirstTile_2();
		while (_gridTiles.Any(t => !t.Solved))
		{
			foreach (var tile in _gridTiles)
			{
				if (tile.PossibleTilesAmount == 1 && !tile.Solved)
				{
					tile.SetTile(tile.possibleTiles[0], false);
				}
			}

			yield return new WaitForSeconds(WaitTimeToRender);

			_gridTiles.Where(t => !t.Solved).OrderBy(t => t.PossibleTilesAmount).First().SetRandomPossibleTile();

			foreach (var tile in _gridTiles)
			{
				if (tile.PossibleTilesAmount == 1 && !tile.Solved)
				{
					tile.SetTile(tile.possibleTiles[0], false);
				}
			}
		}

	}

	void CheckCell(Vector2 cell)
	{
		/*
		foreach direction in directions
		{
			if(tile changed in direction of any type changed && tile in that direction is ok[exists + not collapsed + not pending])
				
		}

		 */
	}
}

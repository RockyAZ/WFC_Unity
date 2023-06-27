using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class Grid : MonoBehaviour
{
	public static int X = 3;
	public static int Y = 3;

	public int XSize = 3;
	public int YSize = 3;
	public List<TileData> _tiles = new List<TileData>();

	System.Random _random = new System.Random();

	public float WaitTimeToRender;

	[Space] public GridSingleTile _tilePrefab;

	private List<TileDirs> allPossibleTiles = new List<TileDirs>();

	private GridSingleTile[] _gridTiles;

	private void Awake()
	{
		X = XSize;
		Y = YSize;
	}

	void Start()
	{
		foreach (var tiledata in _tiles)
		{
			allPossibleTiles.AddRange(tiledata.GetAllTileDirs());
		}

		_gridTiles = new GridSingleTile[XSize * YSize];

		for (int x = 0; x < XSize; x++)
		{
			for (int y = 0; y < YSize; y++)
			{
				var instance = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity, transform);
				instance.Initialize(allPossibleTiles, new Vector2Int(x, y), this);
				_gridTiles[x * XSize + y] = instance;
			}
		}

		StartCoroutine(SolveTiles());
	}

	public GridSingleTile GetGridTile(int x, int y)
	{
		return _gridTiles[x * XSize + y];
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

	public GridSingleTile GetTile(TileDir dir, int distance, Vector2Int currentCoord)
	{
		if (dir == TileDir.Top)
			return GetGridTile(currentCoord.x, currentCoord.y + distance);
		if (dir == TileDir.Right)
			return GetGridTile(currentCoord.x + distance, currentCoord.y);
		if (dir == TileDir.Bot)
			return GetGridTile(currentCoord.x, currentCoord.y - distance);
		if (dir == TileDir.Left)
			return GetGridTile(currentCoord.x - distance, currentCoord.y);

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
}

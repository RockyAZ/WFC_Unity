using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine.Rendering;
using EasyButtons;

public class Grid : MonoBehaviour
{
	public int XSize = 3;
	public int YSize = 3;
	public int SidesAmount = 0;
	public List<TileData> _tiles = new List<TileData>();

	System.Random _random = new System.Random();

	public float WaitTimeToRender;

	[Space] public GridSingleTile _tilePrefab;

	//private int[] speedSearchArray;

	private List<TileDirs> allPossibleTiles = new List<TileDirs>();

	private GridSingleTile[] _gridTiles;

	private void Awake()
	{
		//speedSearchArray = new int[XSize * YSize * _tiles.Count * SidesAmount];
		//Array.Fill(speedSearchArray, 4);

		print("TileDirAmount:" + Enums.Enums.TileDirAmount);
		print((TileDir)UnityEngine.Mathf.Repeat((int)TileDir.Top + 2, Enums.Enums.TileDirAmount));
		print((TileDir)UnityEngine.Mathf.Repeat((int)TileDir.Right + 2, Enums.Enums.TileDirAmount));
		print((TileDir)UnityEngine.Mathf.Repeat((int)TileDir.Bot + 2, Enums.Enums.TileDirAmount));
		print((TileDir)UnityEngine.Mathf.Repeat((int)TileDir.Left + 2, Enums.Enums.TileDirAmount));

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
				instance.Initialize(allPossibleTiles, new Vector2Int(x, y));
				instance.ON_CHANGED += SetTileChanged;
				_gridTiles[x * XSize + y] = instance;
			}
		}

		print("Grass|Grass = " + (byte)(TileDirType.Grass | TileDirType.Grass));
		print("Grass&Grass = " + (byte)(TileDirType.Grass & TileDirType.Grass));
		print("Grass^Grass = " + (byte)(TileDirType.Grass ^ TileDirType.Grass));
		print("Grass|Road = " + (byte)(TileDirType.Grass | TileDirType.Road));
		print("Grass&Road = " + (byte)(TileDirType.Grass & TileDirType.Road));
		print("Grass^Road = " + (byte)(TileDirType.Grass ^ TileDirType.Road));
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

	private IEnumerator SolveTiles()
	{
		SetFirstTile();
		while (_gridTiles.Any(t => !t.Solved))
		{
			yield return new WaitForSeconds(WaitTimeToRender);
			for (int x = 0; x < XSize; x++)
			{
				for (int y = 0; y < YSize; y++)
				{
					if (!_gridTiles[x * XSize + y].Solved)
					{
						_gridTiles[x * XSize + y].SetRandomPossibleTile();
						x = XSize;
						y = YSize;
					}
				}
			}
		}

	}
}
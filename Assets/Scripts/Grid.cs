using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using UnityEngine.Rendering;

public class Grid : MonoBehaviour
{
	public int XSize = 3;
	public int YSize = 3;
	public int SidesAmount = 0;
	public List<TileData> _tiles = new List<TileData>();

	public float WaitTimeToRender;

	[Space]
	public GridSingleTile _tilePrefab;

	private int[] speedSearchArray;

	private List<TileDirs> allPossibleTiles;

	private GridSingleTile[] _gridTiles;

	private void Awake()
	{
		//speedSearchArray = new int[XSize * YSize * _tiles.Count * SidesAmount];
		//Array.Fill(speedSearchArray, 4);

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
				_gridTiles[x*XSize + y] = instance;
			}
		}

		print("Grass|Grass = " + (byte)(TileDirType.Grass | TileDirType.Grass));
		print("Grass&Grass = " + (byte)(TileDirType.Grass & TileDirType.Grass));
		print("Grass^Grass = " + (byte)(TileDirType.Grass ^ TileDirType.Grass));
		print("Grass|Road = " + (byte)(TileDirType.Grass | TileDirType.Road));
		print("Grass&Road = " + (byte)(TileDirType.Grass & TileDirType.Road));
		print("Grass^Road = " + (byte)(TileDirType.Grass ^ TileDirType.Road));
	}

	private void SetTileChanged(Vector2Int cord, GridSingleTile)
	{

	}

	private IEnumerator SolveTiles()
	{

		yield return new WaitForSeconds(WaitTimeToRender);
		yield return null;
	}

}

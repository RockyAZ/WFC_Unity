using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

[RequireComponent(typeof(SpriteRenderer))]
public class GridSingleTile : MonoBehaviour
{
	private SpriteRenderer _spriteRenderer;

	public List<TileDirs> possibleTiles;
	public Vector2Int coordinates;

	//foreach int[] 0-top|1-right|2-bot|3-left

	public Dictionary<TileDirType, int[]> tilesTypeAmount = new Dictionary<TileDirType, int[]>();

	void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Initialize(List<TileDirs> startTiles, Vector2Int coordinate)
	{
		possibleTiles = startTiles;
		coordinates = coordinate;

		tilesTypeAmount[TileDirType.Grass] = new int [4];
		tilesTypeAmount[TileDirType.Road] = new int [4];

		foreach (var tile in startTiles)
		{
			for (int i = 0; i < tile.DirTypes.Length; i++)
			{
				tilesTypeAmount[tile.DirTypes[i]][i]++;
			}
		}
	}

	void SetTile()
	{

	}

	private void SetSprite(Sprite sprite)
	{
		_spriteRenderer.sprite = sprite;
	}
}

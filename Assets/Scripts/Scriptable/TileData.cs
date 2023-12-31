using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;
using System;
using UnityEngine.PlayerLoop;

[Serializable]
public class TileDirs
{
	[Header("Top,Right,Bot,Left")]
	public TileDirType[] DirTypes = new TileDirType[4];

	public TileDirType Top => DirTypes[0];
	public TileDirType Right => DirTypes[1];
	public TileDirType Bot => DirTypes[2];
	public TileDirType Left => DirTypes[3];

	[HideInInspector]
	public Sprite Sprite;
	[HideInInspector]
	public Quaternion Rotation;
	[HideInInspector] 
	public float Probability;
}

[CreateAssetMenu(fileName = "TileData", menuName = "ScriptableObjects/Tile", order = 1)]
public class TileData : ScriptableObject
{
	public Sprite TileSprite;
	[SerializeField] private TileDirs _tileDirs;
	[SerializeField] private float _probability = 1;
	[SerializeField] private bool _doNotRotate = false;

	public List<TileDirs> GetAllTileDirs()
	{
		List<TileDirs> resultList = new List<TileDirs>();
		_tileDirs.Sprite = TileSprite;
		_tileDirs.Probability = _probability;
		resultList.Add(_tileDirs);
		if(_doNotRotate)
			return resultList;

		float rotation = 90;
		for (int i = 1; i < 4; i++)
		{
			TileDirs tmpTileDir = new TileDirs();

			for (int j = 0; j < 4; j++)
			{
				tmpTileDir.DirTypes[j] = _tileDirs.DirTypes[(int)Mathf.Repeat(j + i, _tileDirs.DirTypes.Length)];
			}

			resultList.Add(tmpTileDir);
			tmpTileDir.Rotation = Quaternion.Euler(0, 0, rotation);
			tmpTileDir.Probability = _probability;
			tmpTileDir.Sprite = TileSprite;
			rotation += 90;
		}
		return resultList;
	}
}

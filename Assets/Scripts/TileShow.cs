using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TileShow : MonoBehaviour
{
	public TileData[] tileDatas;
	public float gizmosTextLenMult = 0.2f;

	List<TileView> tiles = new List<TileView>();

	private void Start()
	{
		int index = 0;
		foreach (var tileData in tileDatas)
		{
			var tmpTiles = tileData.GetAllTileDirs();

			for (int i = 0; i < tmpTiles.Count; i++)
			{
				var tmp = new GameObject("Tile_" + i);
				tmp.AddComponent<SpriteRenderer>().sprite = tileData.TileSprite;
				var tmpTileView = tmp.AddComponent<TileView>();
				tmpTileView.Init(tmpTiles[i].DirTypes);
				tmp.transform.rotation = tmpTiles[i].Rotation;
				tmp.transform.position = transform.position + Vector3.right * i + Vector3.down * index;
				tiles.Add(tmpTileView);
			}
			index++;
		}
	}

	void OnDrawGizmos()
	{
		foreach (var tile in tiles)
		{
			Handles.Label(tile.transform.position + Vector3.up * gizmosTextLenMult, tile.tileDirs[0].ToString());
			Handles.Label(tile.transform.position + Vector3.right * gizmosTextLenMult, tile.tileDirs[1].ToString());
			Handles.Label(tile.transform.position + Vector3.down * gizmosTextLenMult, tile.tileDirs[2].ToString());
			Handles.Label(tile.transform.position + Vector3.left * gizmosTextLenMult, tile.tileDirs[3].ToString());
		}
	}
}

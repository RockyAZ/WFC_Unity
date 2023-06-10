using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enums;

public class TileView : MonoBehaviour
{
	public TileDirType[] tileDirs;

	public void Init(TileDirType[] dirs)
	{
		tileDirs = dirs;
	}
}

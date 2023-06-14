using System;

namespace Enums
{
	public enum TileDirType : byte
	{
		Road,
		Grass
	}

	public enum TileDir
	{
		Top=0,
		Right=1,
		Bot=2,
		Left=3
	}

	public static class Enums
	{
		public static int TileDirAmount = Enum.GetValues(typeof(TileDir)).Length;
	}
}
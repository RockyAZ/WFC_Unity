using System;
using System.Collections.Generic;

public static class Extentions
{
	private static Random _random = new Random();

	public static T GetRandomElement<T>(this List<T> list)
	{
		if (list == null)
		{
			throw new ArgumentNullException(nameof(list));
		}

		if (list.Count == 0)
		{
			throw new InvalidOperationException("The list is empty.");
		}

		int randomIndex = _random.Next(0, list.Count);
		return list[randomIndex];
	}

	public static T GetRandomElement<T>(this List<T> list, Random random)
	{
		if (list == null)
		{
			throw new ArgumentNullException(nameof(list));
		}

		if (list.Count == 0)
		{
			throw new InvalidOperationException("The list is empty.");
		}

		int randomIndex = random.Next(0, list.Count);
		return list[randomIndex];
	}

	public static bool IsNumberInRange(this int number, int min, int max)
	{
		return number >= min && number <= max;
	}

}

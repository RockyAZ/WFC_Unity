using System;
using System.Collections.Generic;
using System.Linq;

public static class Extentions
{
	private static Random _random = new Random();

	public static T GetRandomElement<T>(this IEnumerable<T> list)
	{
		if (list == null)
		{
			throw new ArgumentNullException(nameof(list));
		}

		int count = list.Count();

		if (count == 0)
		{
			throw new InvalidOperationException("The list is empty.");
		}

		int randomIndex = _random.Next(0, count);
		return list.ElementAt(randomIndex);
	}

	public static T GetRandomElement<T>(this IEnumerable<T> list, Random random)
	{
		if (list == null)
		{
			throw new ArgumentNullException(nameof(list));
		}
		int count = list.Count();

		if (count == 0)
		{
			throw new InvalidOperationException("The list is empty.");
		}

		int randomIndex = random.Next(0, count);
		return list.ElementAt(randomIndex);
	}

	public static bool IsNumberInRange(this int number, int min, int max)
	{
		return number >= min && number <= max;
	}

	public static float CalculateEntropy(List<float> probabilities)
	{
		float entropy = 0;

		// Check if probabilities list is empty
		if (probabilities.Count == 0)
		{
			return entropy;
		}

		// Normalize probabilities to ensure their sum is 1
		float sum = probabilities.Sum();
		List<float> normalizedProbabilities = probabilities.Select(p => p / sum).ToList();

		// Calculate entropy
		foreach (float probability in normalizedProbabilities)
		{
			if (probability > 0)
			{
				entropy += probability * (float)Math.Log(probability, 2);
			}
		}

		entropy *= -1; // Reverse sign

		return entropy;
	}

}

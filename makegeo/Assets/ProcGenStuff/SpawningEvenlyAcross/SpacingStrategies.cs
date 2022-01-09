using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker

public static class SpacingStrategies
{
	public enum Distribution
	{
		NAIVE_ZERO_TO_N,
		NAIVE_ZERO_TO_N_MINUS_ONE,
		CENTERED_ZERO_TO_N,
		MAXIMUM,
	}

	public static float Distribute( Distribution dist, float left, float right, int i, int max)
	{
		float span = right - left;

		switch( dist)
		{
		default :
		case Distribution.NAIVE_ZERO_TO_N :
			return left + (span * i) / max;

		case Distribution.NAIVE_ZERO_TO_N_MINUS_ONE :
			return left + (span * i) / (max - 1);

		case Distribution.CENTERED_ZERO_TO_N :
			return left + (span * i + 0.5f) / max;
		}
	}
}

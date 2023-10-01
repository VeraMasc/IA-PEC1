using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Permite meter rangos de valores aleatorios de ints en el inspector
/// </summary>
[Serializable]
public class RandomBetweenInt{
	public int min;
	public int max;
	public int value {
		get => Random.Range(min, max);

	}

	public RandomBetweenInt(int _min, int _max){
		min = _min;
		max = _max;
	}

	public static implicit operator int (RandomBetweenInt x){
    	return x.value;
	}
}

/// <summary>
/// Permite meter rangos de valores aleatorios de floats en el inspector
/// </summary>
[Serializable]
public class RandomBetween{
	public float min;
	public float max;
	public float value {
		get => Random.Range(min, max);

	}

	public RandomBetween(float _min, float _max){
		min = _min;
		max = _max;
	}

	public static implicit operator float (RandomBetween x){
    	return x.value;
	}
}
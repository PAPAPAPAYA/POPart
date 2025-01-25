using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Linq;

public class UtilityFunctions : MonoBehaviour
{
	public static UtilityFunctions me;
	private void Awake()
	{
		me = this;
	}
	
	// used to shuffle given GameObject list
	public List<GameObject> ShuffleList(List<GameObject> list)
	{
		List<GameObject>shuffled = new();
		shuffled = list.OrderBy(x => Random.value).ToList();
		return shuffled;
	}
}

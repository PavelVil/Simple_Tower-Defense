using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour  where T : MonoBehaviour{

	private static T instance;

	public static T Instance {

		get { 		
		if (instance == null) {
			instance = GameObject.FindObjectOfType<T>();
		} else if (instance!=GameObject.FindObjectOfType<T>()){
			Destroy(FindObjectOfType<T>());
		}
			DontDestroyOnLoad(FindObjectOfType<T>());
			return instance;
			}		
	}

}

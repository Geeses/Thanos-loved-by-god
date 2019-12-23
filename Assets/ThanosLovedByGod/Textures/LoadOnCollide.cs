using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadOnCollide : MonoBehaviour
{

	public int scene;
	
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag.CompareTo("Thanos") == 1)
		{

			SceneManager.LoadScene(scene);
		}

		Debug.Log("lol");
	}
}

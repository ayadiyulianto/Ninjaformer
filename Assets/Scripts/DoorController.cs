using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
	public AudioClip audioUnlock;
	AudioSource audioSource;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Player") && GameController.GetInstance().hasKey) {
			Destroy(this.gameObject, 1);
            GameController.GetInstance().RemoveKey();
			if (audioUnlock != null) audioSource.PlayOneShot(audioUnlock);
		}
	}
}

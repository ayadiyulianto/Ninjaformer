using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BrokenPlatform : MonoBehaviour
{
    Animator animator;
    TilemapCollider2D tilemapCollider2D;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        tilemapCollider2D = GetComponent<TilemapCollider2D>();
    }

    void BrokeEvent()
    {
        tilemapCollider2D.enabled = false;
        StartCoroutine(AutoRespawn());
    }

    IEnumerator AutoRespawn()
    {
        animator.SetTrigger("Respawn");
		yield return new WaitForSeconds(10);
		tilemapCollider2D.enabled = true;
	}

    void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Player") || col.gameObject.CompareTag("Enemy")) {
			animator.SetTrigger("Destroy");
		}
	}
}

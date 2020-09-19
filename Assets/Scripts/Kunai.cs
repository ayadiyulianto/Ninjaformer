using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kunai : MonoBehaviour
{
    public int attackDamage = 1;

    void OnTriggerEnter2D(Collider2D col)
	{
        if (col.gameObject.CompareTag("Enemy")) {
            col.gameObject.GetComponent<ZombieController>().TakeDamage(attackDamage);
            Destroy(this.gameObject);
        }
        if (col.gameObject.CompareTag("Ground")) {
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    List<GameObject> listEnemy;

    void Start()
    {
        listEnemy = GameObject.FindWithTag("Player").GetComponent<PlayerController>().nearbyEnemies;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy")) {
            listEnemy.Add(col.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Enemy")) {
            listEnemy.Remove(col.gameObject);
        }
    }
}

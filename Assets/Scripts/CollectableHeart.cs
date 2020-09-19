using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableHeart : MonoBehaviour
{
    public int hpValue = 3;

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player")) {
            GameController.GetInstance().AddHP(hpValue);
            gameObject.SetActive(false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public enum Type 
    {
        Coin, Diamond, Key
    }
    public Type type;

    void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Player")) {
            switch(type){
                case Type.Coin:
                    GameController.GetInstance().AddCoin();
                    break;
                case Type.Diamond:
                    GameController.GetInstance().AddDiamond();
                    break;
                case Type.Key:
                    GameController.GetInstance().AddKey();
                    break;
            }
            gameObject.SetActive(false);
		}
	}
}

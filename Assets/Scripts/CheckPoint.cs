using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Sprite inactiveSprite;
    public Sprite activeSprite;
    public AudioClip audioCheckSave;
    public List<GameObject> CPGameObject;
    bool isActive = false;
    GameObject barrier;
    AudioSource audioSource;
    int prevCoin, prevDiamond;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = inactiveSprite;
        barrier = transform.GetChild(0).gameObject;
        barrier.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        foreach (GameObject go in CPGameObject) {
            go.SetActive(false);
        }
    }

    void StartCheckpoint()
    {
        GameController gc = GameController.GetInstance();
        prevCoin = gc.total_coin;
        prevDiamond = gc.total_diamond;
        GetComponent<SpriteRenderer>().sprite = activeSprite;
        barrier.SetActive(true);
        foreach (GameObject go in CPGameObject) {
            go.SetActive(true);
        }
        if (audioCheckSave != null) audioSource.PlayOneShot(audioCheckSave);
    }

    public void RestartCheckpoint()
    {
        GameController gc = GameController.GetInstance();
        gc.SetCoin(prevCoin);
        gc.SetDiamond(prevDiamond);
        foreach (GameObject go in CPGameObject) {
            go.SetActive(true);
            if (go.CompareTag("Enemy")) {
                go.GetComponent<ZombieController>().Respawn();
            }
        }
    }

    public void DestroyCPGameObject()
    {
        foreach (GameObject go in CPGameObject) {
            Destroy(go);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && !isActive) {
            isActive = true;
            StartCheckpoint();
        }
    }
}

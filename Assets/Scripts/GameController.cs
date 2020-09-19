using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int total_coin = 0; // digunakan untuk mencatat coin
    public int total_diamond = 0; // digunakan untuk mencatat diamond
    public bool hasKey = false; // digunakan untuk mencatat key
    int playerHP = 0;
	public Text coinLabel; // Digunakan untuk menampilkan coin
	public Text diamondLabel; // Digunakan untuk menampilkan coin
    public Slider hpSlider;
    public Image keyImage;
    public Sprite keyEmpty, keyFill;
    public GameObject completedPanel;
	public Text coinCompletedPanel, diamondCompletedPanel;
	public AudioClip audioWin;
    public AudioClip audioCollect;
    private static GameController instance;
    AudioSource audioSource;
    PlayerController pc;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        keyImage.sprite = keyEmpty;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerHP != pc.HP) {
            playerHP = pc.HP;
            hpSlider.value = playerHP;
        }
    }

    public void AddCoin()
    {
		total_coin++;
		coinLabel.text = total_coin.ToString ();
        audioSource.PlayOneShot(audioCollect);
	}
    public void SetCoin(int coin)
    {
		total_coin = coin;
		coinLabel.text = total_coin.ToString ();
	}

    public void AddDiamond()
    {
		total_diamond++;
        diamondLabel.text = total_diamond.ToString();
        audioSource.PlayOneShot(audioCollect);
	}
    public void SetDiamond(int diamond)
    {
		total_diamond = diamond;
        diamondLabel.text = total_diamond.ToString();
	}

    public void AddKey()
    {
		hasKey = true;
        keyImage.sprite = keyFill;
        audioSource.PlayOneShot(audioCollect);
	}
    public void RemoveKey()
    {
		hasKey = false;
        keyImage.sprite = keyEmpty;
	}

    public void AddHP(int hp)
    {
		pc.AddHP(hp);
        audioSource.PlayOneShot(audioCollect);
	}

	public static GameController GetInstance(){
		return instance;
	}
    
	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Player")) {
			coinCompletedPanel.text = GameController.GetInstance().total_coin.ToString();
			diamondCompletedPanel.text = GameController.GetInstance().total_diamond.ToString();
            completedPanel.SetActive(true);
			audioSource.PlayOneShot(audioWin);
		}
	}
}

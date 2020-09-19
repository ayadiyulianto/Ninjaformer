using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
	Rigidbody2D rigid;
	Animator anim;
	GameObject player;
	public GameObject killReward;
	public bool isPatrol = true;
	public bool isFacingRight = true;
	public float speed = 2; // kecepatan enemy bergerak
	public float range = 3; // jarak patrol
	public int maxHP = 3;
	public int attackDamage = 1;
	public float cooldown = 1.5f;
	int HP;
    bool isGrounded = false; // untuk mengecek karakter berada di ground
	bool isAttackMode = false;
	bool playerInAttackRange = false;
	bool isCanAttack = true;
	bool isDead = false;
	Vector3 startPosition; // posisi awal untuk respawn
	Vector3 batas1; //digunakan untuk batas gerak ke kiri
	Vector3 batas2; // digunakan untuk batas gerak ke kanan
	public AudioClip audioAttack, audioDie;
	AudioSource audioSource;

	// Use this for initialization
	void Start() {
		rigid = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		HP = maxHP;
		startPosition = transform.position;
		if (isFacingRight) {
			batas1 = transform.position;
			batas2 = transform.position + new Vector3(range, 0, 0);
		} else {
			batas1 = transform.position - new Vector3(range, 0, 0);
			batas2 = transform.position;
		}
		player = GameObject.FindWithTag("Player");
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update() {
		if (isGrounded && !isDead) {
			if (!isAttackMode && isPatrol) {
				anim.SetBool("isWalk", true);
				if(isFacingRight)
					MoveRight();
				else
					MoveLeft();
			} else if(isAttackMode) {
				anim.SetBool("isWalk", true);
				ChasePlayer();
			}
		}
	}

	void MoveRight()
	{
		Vector3 pos = transform.position;
		pos.x += speed * Time.deltaTime;
		transform.position = pos;
		if (transform.position.x >= batas2.x && isFacingRight) {
			Flip();
		}
	}

	void MoveLeft()
	{
		Vector3 pos = transform.position;
		pos.x -= speed * Time.deltaTime;
		transform.position = pos;
		if (transform.position.x <= batas1.x && !isFacingRight) {
			Flip();
		}
	}

	void Flip()
	{
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		isFacingRight = !isFacingRight;
	}

	void ChasePlayer()
	{
		Vector3 theScale = transform.localScale;
		// player on left or right
		if (transform.position.x > player.transform.position.x && isFacingRight) {
			Flip();
		} else if (transform.position.x < player.transform.position.x && !isFacingRight) {
			Flip();
		}
		// chase player
		transform.position = Vector2.MoveTowards(transform.position, 
			new Vector2(player.transform.position.x, transform.position.y), 
			speed * Time.deltaTime);
		// attack
		if (!audioSource.isPlaying) {
			audioSource.clip = audioAttack;
			audioSource.Play();
		}
		if (playerInAttackRange && isCanAttack){
			anim.SetTrigger("attack");
			StartCoroutine(CooldownAttack());
		}
	}
	
	IEnumerator CooldownAttack()
    {
		isCanAttack = false;
		yield return new WaitForSeconds(cooldown);
		isCanAttack = true;
	}

	public void TakeDamage(int damage)
	{
		if (HP > 0) HP -= damage;
		if (HP <= 0 && !isDead) Death();
	}

	void AttackEvent()
	{
		if (playerInAttackRange)
			player.GetComponent<PlayerController>().TakeDamage(attackDamage);
	}

	void Death()
	{
		isDead = true;
		rigid.velocity = Vector2.zero;
		anim.SetTrigger("dead");
		audioSource.clip = audioDie;
		audioSource.Play();
	}

	void DeathEvent()
	{
		GameObject go = (GameObject)Instantiate(killReward, (Vector2)transform.position, Quaternion.identity);
		gameObject.SetActive(false);
	}

	public void Respawn()
	{
		HP = maxHP;
		transform.position = startPosition;
		audioSource.Stop();
		if (isDead) {
			anim.SetTrigger("respawn");
			isDead = false;
		}
		isAttackMode = false;
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Ground")) {
			isGrounded = true;
		}
		if (col.gameObject.CompareTag("Player") && isAttackMode) {
			playerInAttackRange = true;
		}
		if (col.gameObject.CompareTag("Spike") && !isDead) {
			Death();
		}
	}

	//digunakan untuk mengecek apakah Player masih diatas tanah atau tidak
	void OnCollisionStay2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Ground")) {
			isGrounded = true;
		}
	}

	//digunakan untuk memberi tahu Player bahwa sudah tidak diatas tanah
	void OnCollisionExit2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Ground")) {
			isGrounded = false;
		}
		if (col.gameObject.CompareTag("Player")) {
			playerInAttackRange = false;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Player")) {
			isAttackMode = true;
		}
	}
}

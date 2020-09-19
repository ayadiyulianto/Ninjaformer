using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	Animator anim; // animator dari player
	Rigidbody2D rigid; // rigidbody 2d dari player
	bool isGrounded; // untuk menyimpan jarak player ke ground
	bool isDead = false; //untuk menyimpan state apakah karakter masih hidup
	bool isClimbing = false; // untuk mengetahui apakah terdapat tangga untuk memanjat
	int speed;

	public int maxHP = 10;
	public int HP;
	public bool isFacingRight = true; // untuk mengetahui arah hadap pada player
	public float jumpForce = 200f; // besar gaya untuk mengangkat karakter ke atas
	public float walkForce = 15f; // besar gaya untuk mendorong karakter ke samping
	public float maxSpeed = 5f; // kecepatan maksimum dari karakter utama
	public GameObject kunai; // object kunai
	public Vector2 kunaiVelocity = new Vector2(50, 0); // kecepatan kunai
	public Vector2 kunaiOffset = new Vector2(0.75f, -0.104f); // jarak posisi kunai dari posisi player
	public float cooldown = 1f; // jeda waktu untuk melempar
	public int attackDamage = 3;
	bool isCanClimb = false;
	bool isCanThrow = true; // memastikan untuk kapan dapat melempar
	bool isCanAttack = true; // memastikan untuk kapan dapat menyerang
	public List<GameObject> nearbyEnemies = new List<GameObject>();
	GameObject lastCheckpoint;
	public AudioClip audioHurt, audioDie, audioShoot;
	AudioSource audioSource;
	public GameObject respawnPanel;

	// Use this for initialization
	void Start()
	{
		anim = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody2D>();
		HP = maxHP;
		audioSource = GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
		if (!isDead) {
			InputHandler();
		}
		if (rigid.velocity.x > 0) {
			speed = (int)Mathf.Ceil(rigid.velocity.x);
		} else {
			speed = (int)Mathf.Floor(rigid.velocity.x);
		}
		if (isCanClimb && isClimbing) {
			Climb();
		} else {
			anim.SetInteger("Speed", speed);
		}
	}

	void InputHandler()
	{
		// Left right movement
		if (Input.GetKey(KeyCode.A)) {
			MoveLeft();
		}
		if (Input.GetKey(KeyCode.D)) {
			MoveRight();
		}
        if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

		// up movement
		if (Input.GetKeyDown(KeyCode.W)) {
			if(isCanClimb) {
				isClimbing = true;
				gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
				Climb();
			} else if (isGrounded && !isClimbing) {
				Jump();
			}
		}

		// attack trigger
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
			Throw();
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			Attack();
		}
	}

	void MoveLeft()
	{
		if (rigid.velocity.x * -1 < maxSpeed)
			rigid.AddForce(Vector2.left * walkForce);
		// membalik arah karakter apabila tidak menghadap ke kanan
		if (isFacingRight) {
			Flip();
		}
	}

	void MoveRight()
	{
		if (rigid.velocity.x * 1 < maxSpeed)
			rigid.AddForce(Vector2.right * walkForce);
		// membalik arah karakter apabila tidak menghadap ke kiri
		if (!isFacingRight) {
			Flip();
		}
	}

	void Jump()
	{
		rigid.AddForce(Vector2.up * jumpForce);
	}

	void Climb()
	{
		anim.SetBool("isClimbing", true);
		Vector3 pos = transform.position;
		pos.y += 1 * Time.deltaTime;
		transform.position = pos;
	}

	void Flip()
	{
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		isFacingRight = !isFacingRight;
	}

	void Throw()
    {
		// jika karakter dapat melempar
		if (isCanThrow) {
			anim.SetTrigger("Throw");
			//Membuat kunai baru
			GameObject bullet = (GameObject)Instantiate(kunai,
				(Vector2)transform.position+kunaiOffset*transform.localScale.x,
				Quaternion.identity);
			// mengatur kecepatan dari kunai
			Vector2 velocity = new Vector2(kunaiVelocity.x * transform.localScale.x,
				kunaiVelocity.y);
			bullet.GetComponent<Rigidbody2D>().velocity = velocity;
			//Menyesuaikan scale dari kunai dengan scale karakter
			Vector3 scale = transform.localScale;
			bullet.transform.localScale = scale;
			audioSource.PlayOneShot(audioShoot);
			StartCoroutine(CooldownThrow());
		}
	}

	IEnumerator CooldownThrow()
    {
		isCanThrow = false;
		yield return new WaitForSeconds(cooldown);
		isCanThrow = true;
	}

    void Attack()
    {
		// jika karakter dapat menyerang
		if (isCanAttack) {
            anim.SetTrigger("Attack");
			StartCoroutine(CooldownAttack());
        }
    }

	void AttackEvent()
	{
		for (int i = 0; i < nearbyEnemies.Count; i++) {
			nearbyEnemies[i].GetComponent<ZombieController>().TakeDamage(attackDamage);
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
		if (HP > 0) {
			HP -= damage;
			audioSource.PlayOneShot(audioHurt);
		}
		if (HP <= 0 && !isDead) Death();
	}
	
	public void AddHP(int hp)
	{
		HP += hp;
		if (HP > 10) {
			HP = 10;
		}
	}

	void Death()
	{
		anim.SetTrigger("isDead");
		isDead = true;
		rigid.velocity = Vector2.zero;
		audioSource.PlayOneShot(audioDie);
		StartCoroutine(ShowRespawnPanel());
	}

	IEnumerator ShowRespawnPanel()
	{
		yield return new WaitForSeconds(3f);
		respawnPanel.SetActive(true);
	}
	
	public void Respawn()
    {
		lastCheckpoint.GetComponent<CheckPoint>().RestartCheckpoint();
		transform.position = lastCheckpoint.transform.position;
		anim.SetTrigger("Respawn");
		HP = maxHP;
		if (isDead) isDead = false;
		respawnPanel.SetActive(false);
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Ground")) {
			anim.SetBool("isGrounded", true);
			isGrounded = true;
		}
		if (col.gameObject.CompareTag("Spike") && !isDead) {
			Death();
		}
	}

	//digunakan untuk mengecek apakah Player masih diatas tanah atau tidak
	void OnCollisionStay2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Ground")) {
			anim.SetBool("isGrounded", true);
			isGrounded = true;
		}
	}

	//digunakan untuk memberi tahu Player bahwa sudah tidak diatas tanah
	void OnCollisionExit2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Ground")) {
			anim.SetBool("isGrounded", false);
			isGrounded = false;
		}
	}

	// enemies masuk jarak serang
	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Checkpoint") && 
			!GameObject.ReferenceEquals(lastCheckpoint, col.gameObject)) {
			if (lastCheckpoint != null) {
				lastCheckpoint.GetComponent<CheckPoint>().DestroyCPGameObject();
			}
			lastCheckpoint = col.gameObject;
		}
		if (col.gameObject.CompareTag("Ladder")) {
			isCanClimb = true;
		}
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Ladder")) {
			isCanClimb = true;
		}
	}
	
	// enemies diluar jarak serang
	void OnTriggerExit2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Ladder")) {
			if (isClimbing) {
				Vector3 pos = transform.position;
				pos.y += 1;
				transform.position = pos;
				isClimbing = false;
			}
			isCanClimb = false;
			gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
			anim.SetBool("isClimbing", false);
		}
	}
}

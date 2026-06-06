using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	PlayerControls controls;
	Animator anim;
	Rigidbody2D rigid;
	bool isGrounded;
	bool isDead = false;
	bool isClimbing = false;
	int speed;

	public int maxHP = 10;
	public int HP;
	public bool isFacingRight = true;
	public float jumpForce = 200f;
	public float walkForce = 15f;
	public float maxSpeed = 5f;
	public GameObject kunai;
	public Vector2 kunaiVelocity = new Vector2(50, 0);
	public Vector2 kunaiOffset = new Vector2(0.75f, -0.104f);
	public float cooldown = 1f;
	public int attackDamage = 3;
	bool isCanClimb = false;
	bool isCanThrow = true;
	bool isCanAttack = true;
	public List<GameObject> nearbyEnemies = new List<GameObject>();
	GameObject lastCheckpoint;
	public AudioClip audioHurt, audioDie, audioShoot;
	AudioSource audioSource;
	public GameObject respawnPanel;

	void Awake()
	{
		controls = new PlayerControls();

		controls.Player.Jump.performed += ctx =>
		{
			if (isDead) return;
			if (isCanClimb)
			{
				isClimbing = true;
				rigid.gravityScale = 0;
				Climb();
			}
			else if (isGrounded && !isClimbing)
			{
				Jump();
			}
		};

		controls.Player.Throw.performed += ctx =>
		{
			if (!isDead) Throw();
		};

		controls.Player.Attack.performed += ctx =>
		{
			if (!isDead) Attack();
		};
	}

	void OnEnable()
	{
		controls.Enable();
	}

	void OnDisable()
	{
		controls.Disable();
	}

	void Start()
	{
		anim = GetComponent<Animator>();
		rigid = GetComponent<Rigidbody2D>();
		HP = maxHP;
		audioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		speed = rigid.linearVelocity.x > 0
			? (int)Mathf.Ceil(rigid.linearVelocity.x)
			: (int)Mathf.Floor(rigid.linearVelocity.x);

		if (!isDead)
		{
			float moveX = controls.Player.Move.ReadValue<Vector2>().x;
			if (moveX < 0)
				MoveLeft();
			else if (moveX > 0)
				MoveRight();
			else
				rigid.linearVelocity = new Vector2(0, rigid.linearVelocity.y);
		}

		if (isCanClimb && isClimbing)
			Climb();
		else
			anim.SetInteger("Speed", speed);
	}

	void MoveLeft()
	{
		if (rigid.linearVelocity.x * -1 < maxSpeed)
			rigid.AddForce(Vector2.left * walkForce);
		if (isFacingRight)
			Flip();
	}

	void MoveRight()
	{
		if (rigid.linearVelocity.x < maxSpeed)
			rigid.AddForce(Vector2.right * walkForce);
		if (!isFacingRight)
			Flip();
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
		if (isCanThrow)
		{
			anim.SetTrigger("Throw");
			GameObject bullet = (GameObject)Instantiate(kunai,
				(Vector2)transform.position + kunaiOffset * transform.localScale.x,
				Quaternion.identity);
			Vector2 velocity = new Vector2(kunaiVelocity.x * transform.localScale.x, kunaiVelocity.y);
			bullet.GetComponent<Rigidbody2D>().linearVelocity = velocity;
			bullet.transform.localScale = transform.localScale;
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
		if (isCanAttack)
		{
			anim.SetTrigger("Attack");
			StartCoroutine(CooldownAttack());
		}
	}

	void AttackEvent()
	{
		for (int i = 0; i < nearbyEnemies.Count; i++)
		{
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
		if (HP > 0)
		{
			HP -= damage;
			audioSource.PlayOneShot(audioHurt);
		}
		if (HP <= 0 && !isDead) Death();
	}

	public void AddHP(int hp)
	{
		HP += hp;
		if (HP > 10)
			HP = 10;
	}

	void Death()
	{
		anim.SetTrigger("isDead");
		isDead = true;
		rigid.linearVelocity = Vector2.zero;
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
		if (col.gameObject.CompareTag("Ground"))
		{
			anim.SetBool("isGrounded", true);
			isGrounded = true;
		}
		if (col.gameObject.CompareTag("Spike") && !isDead)
			Death();
	}

	void OnCollisionStay2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Ground"))
		{
			anim.SetBool("isGrounded", true);
			isGrounded = true;
		}
	}

	void OnCollisionExit2D(Collision2D col)
	{
		if (col.gameObject.CompareTag("Ground"))
		{
			anim.SetBool("isGrounded", false);
			isGrounded = false;
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Checkpoint") &&
			!GameObject.ReferenceEquals(lastCheckpoint, col.gameObject))
		{
			lastCheckpoint?.GetComponent<CheckPoint>().DestroyCPGameObject();
			lastCheckpoint = col.gameObject;
		}
		if (col.gameObject.CompareTag("Ladder"))
			isCanClimb = true;
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Ladder"))
			isCanClimb = true;
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("Ladder"))
		{
			if (isClimbing)
			{
				Vector3 pos = transform.position;
				pos.y += 1;
				transform.position = pos;
				isClimbing = false;
			}
			isCanClimb = false;
			rigid.gravityScale = 1;
			anim.SetBool("isClimbing", false);
		}
	}
}

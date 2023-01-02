using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
  Vector2 moveInput;
  CapsuleCollider2D myBodyCollider;
  BoxCollider2D myFeetCollider;
  Rigidbody2D myRigidBody;
  Animator myAnimator;
  float gravityScaleAtStart;

  [SerializeField] float moveSpeed = 10f;
  [SerializeField] float jumpSpeed = 5f;
  [SerializeField] float climbSpeed = 5f;
  [SerializeField] bool isAlive = true;
  [SerializeField] Vector2 deathKick = new Vector2(10f, 10f);
  [SerializeField] GameObject bulletInstanceSprite;
  [SerializeField] Transform gun;

  void Start()
  {
    myRigidBody = GetComponent<Rigidbody2D>();
    myAnimator = GetComponent<Animator>();
    myBodyCollider = GetComponent<CapsuleCollider2D>();
    myFeetCollider = GetComponent<BoxCollider2D>();
    gravityScaleAtStart = myRigidBody.gravityScale;
  }

  void Update()
  {
    if (!isAlive)
    {
      return;
    }
    Run();
    FlipSprite();
    ClimbLadder();
    Die();
  }

  void OnMove(InputValue value)
  {
    if (!isAlive)
    {
      return;
    }
    moveInput = value.Get<Vector2>();
  }

  void OnJump(InputValue value)
  {
    if (!isAlive)
    {
      return;
    }

    if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
    {
      return;
    }

    if (value.isPressed)
    {
      myRigidBody.velocity += new Vector2(0f, jumpSpeed);
    }
  }

  void OnFire(InputValue value)
  {
    if (!isAlive)
    {
      return;
    }

    if(value.isPressed)
    {
      Instantiate(bulletInstanceSprite, gun.position, transform.rotation);
    }
  }

  void Run()
  {
    Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, myRigidBody.velocity.y);
    myRigidBody.velocity = playerVelocity;

    bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

    if (playerHasHorizontalSpeed)
    {
      myAnimator.SetBool("isRunning", true);
    }
    else
    {
      myAnimator.SetBool("isRunning", false);
    }
  }

  void FlipSprite()
  {
    bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;

    if (playerHasHorizontalSpeed)
    {
      transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
    }

  }

  void ClimbLadder()
  {
    if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
    {
      myRigidBody.gravityScale = gravityScaleAtStart;
      myAnimator.SetBool("isClimbing", false);
      return;
    }

    myRigidBody.gravityScale = 0f;
    myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, moveInput.y * climbSpeed);
    
    bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
    
    if(playerHasVerticalSpeed)
    {
      myAnimator.SetBool("isClimbing", true);
    }
    else
    {
      myAnimator.SetBool("isClimbing", false);
    }
  }

  void Die()
  {
    if(myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")) 
    || myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
    {
      myAnimator.SetTrigger("Dying");
      isAlive = false;
      myRigidBody.velocity = deathKick;
      FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }

  }
}

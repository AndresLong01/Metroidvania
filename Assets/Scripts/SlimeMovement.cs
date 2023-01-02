using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeMovement : MonoBehaviour
{
  Rigidbody2D myRigidBody;
  BoxCollider2D myEdgeCollider;

  [SerializeField] float moveSpeed = 1f;

  void Start()
  {
    myRigidBody = GetComponent<Rigidbody2D>();
    myEdgeCollider = GetComponent<BoxCollider2D>();
  }

  void Update()
  {
    myRigidBody.velocity = new Vector2(moveSpeed, 0f);
  }

  void OnTriggerExit2D(Collider2D other) {
    moveSpeed = -moveSpeed;
    FlipSprite();
  }

  void FlipSprite ()
  {
    transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);
  }
}

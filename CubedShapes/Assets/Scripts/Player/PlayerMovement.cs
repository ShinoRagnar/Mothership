using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float moveSpeed;
    public float jumpForce;
    public float gravityScale;
 //   public float fallMultiplier;
 //   public float lowFallMultiplier;

    public bool canMove;

    private CharacterController playerController;
    private Vector3 playerMoveDirection;
    private Transform player;



    // Use this for initialization
    void Start()
    {
      //  if(fallMultiplier < 1) { fallMultiplier = 2.5f; }
      //  if (lowFallMultiplier < 1) { lowFallMultiplier = 4f; }
        if (moveSpeed == 0) { moveSpeed = 10; }
        if(jumpForce == 0) { jumpForce = 20; }
        if(gravityScale == 0) { gravityScale = 5; }

        canMove = true;
        playerController = GetComponent<CharacterController>();
        //playerBody = GetComponent<Rigidbody>();
        //playerController.Move(new Vector3(1,0));
	}


    float x = 0f;
    float y = 0f;
    float z = 0f;

    // Update is called once per frame
    void Update() {

        x = 0f;
        y = playerMoveDirection.y;
        z = 0f; //player.position.z;

        float heading = Input.GetAxis("Horizontal");

        if (canMove){
            x = heading * moveSpeed;
        }

        if (    Input.GetButton("Jump") && playerController.isGrounded){
            y = jumpForce;
        }

       
		
	}
    private void FixedUpdate(){

        float yChange = (Physics.gravity.y * gravityScale * Time.deltaTime);

        /*if (playerController.velocity.y < 0)
        {
            yChange = yChange * (fallMultiplier - 1);
        }
        else if (playerController.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            yChange = yChange * (lowFallMultiplier - 1);
        }*/
        y += yChange;

        playerMoveDirection = new Vector3(x, y, z);
        playerController.Move(playerMoveDirection * Time.deltaTime);
    }


}

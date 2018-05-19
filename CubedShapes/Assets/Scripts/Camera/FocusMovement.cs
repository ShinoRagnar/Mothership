using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusMovement : MonoBehaviour {

    //Tracking
    public Transform playerBody;

    public float speed;

    public bool visible;

    // turn to private after testing
    public float timeOffScreen;
    public float toTheRightX;
    public float toTheLeftX;
    public float upY;
    public float downY;

    public float innerFocusToTheRightX;
    public float innerFocusToTheLeftX;
    public float innerFocusUpY;
    public float innerFocusDownY;



    private Renderer rend;


    // Use this for initialization
    void Start()
    {
        if(speed <= 0)
        {
            speed = 1;
        }
        rend = GetComponent<Renderer>();
        timeOffScreen = 0;
        if (!visible)
        {
            GetComponent<MeshRenderer>().enabled = false;
        }

    }

    // Update is called once per frame
    void Update() {
        bool outOfBounds = false;
        bool outOfFocusBounds = false;

        float width = transform.localScale.x;
        float height = transform.localScale.y;
        float playerWidth = playerBody.transform.localScale.x;
        float playerHeight = playerBody.transform.localScale.y;

        toTheRightX = (playerBody.position.x - (playerWidth / 2))
                        - (transform.position.x + (width / 2));
        toTheLeftX = (transform.position.x - (width / 2)) -
                      (playerBody.position.x + (playerWidth / 2));
        upY = (playerBody.position.y - (playerHeight / 2))
                        - (transform.position.y + (height / 2));
        downY = (transform.position.y - (height / 2)) -
                      (playerBody.position.y + (playerHeight / 2));

        innerFocusToTheRightX = (playerBody.position.x - (playerWidth / 2))
                        - (transform.position.x + (playerWidth / 2));
        innerFocusToTheLeftX = (transform.position.x - (playerWidth / 2)) -
                      (playerBody.position.x + (playerWidth / 2));
        innerFocusUpY = (playerBody.position.y - (playerHeight / 2))
                        - (transform.position.y + (playerHeight / 2));
        innerFocusDownY = (transform.position.y - (playerHeight / 2)) -
                      (playerBody.position.y + (playerHeight / 2));

        Vector3 moveVector = Vector3.zero;

        if (toTheRightX > 0) {
            outOfBounds = true;
            moveVector += Vector3.right * (Mathf.Pow(timeOffScreen, 2) * toTheRightX * speed);
        } if (toTheLeftX > 0) {
            outOfBounds = true;
            moveVector += Vector3.left * (Mathf.Pow(timeOffScreen, 2) * toTheLeftX * speed);
        } if (upY > 0) {
            outOfBounds = true;
            moveVector += Vector3.up * (Mathf.Pow(timeOffScreen, 2) * upY * speed);
        } if (downY > 0) {
            outOfBounds = true;
            moveVector += Vector3.down * (Mathf.Pow(timeOffScreen, 2) * downY * speed);
        }
        if (innerFocusDownY > 0){
            outOfFocusBounds = true;
            moveVector += Vector3.down * (Time.deltaTime * innerFocusDownY * speed);
        }if (innerFocusUpY > 0)
        {
            outOfFocusBounds = true;
            moveVector += Vector3.up * (Time.deltaTime * innerFocusUpY * speed);
        }if (innerFocusToTheLeftX > 0)
        {
            outOfFocusBounds = true;
            moveVector += Vector3.left * (Time.deltaTime * innerFocusToTheLeftX * speed);
        }if (innerFocusToTheRightX > 0)
        {
            outOfFocusBounds = true;
            moveVector += Vector3.right * (Time.deltaTime * innerFocusToTheRightX * speed);
        }

        if (outOfBounds)
        {
            timeOffScreen += Time.deltaTime;
            setRed();
        }
        else
        {
            if (outOfFocusBounds){
                setYellow();
            }else{
                setGreen();
            }
            timeOffScreen = 0;
        }
        this.transform.Translate(moveVector);
        //Debug.Log(moveVector);
        //transform.position = playerPlane.position - offset;
    }
    void setGreen() {
        rend.material.shader = Shader.Find("_Color");
        rend.material.SetColor("_Color", Color.green);
        rend.material.shader = Shader.Find("Specular");
        rend.material.SetColor("_SpecColor", Color.green);
    }
    void setRed(){
        rend.material.shader = Shader.Find("_Color");
        rend.material.SetColor("_Color", Color.red);
        rend.material.shader = Shader.Find("Specular");
        rend.material.SetColor("_SpecColor", Color.red);
    }
    void setYellow()
    {
        rend.material.shader = Shader.Find("_Color");
        rend.material.SetColor("_Color", Color.yellow);
        rend.material.shader = Shader.Find("Specular");
        rend.material.SetColor("_SpecColor", Color.yellow);
    }
}

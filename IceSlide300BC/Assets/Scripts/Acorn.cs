using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acorn : MonoBehaviour {
    public bool isMoving;
    public bool broken;
    public bool isOut;

    private SpriteRenderer spriteRenderer;
    private SpriteRenderer spriteRendererN;
    public Sprite[] numbers;

    public Vector2Int arrayPosition;
    public Vector2Int direction;
    private Vector2Int destination;

    private static float MOVE_TIME = 0.35f;
    private float moveTimer;
    private int hits;

    private bool dis;
    private float alpha;

	// Use this for initialization
	void Start () {
        arrayPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        destination = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        direction = Vector2Int.zero;

        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        spriteRendererN = transform.GetChild(0).transform.GetComponent<SpriteRenderer>(); //Sprite Renderer for number of hits
        spriteRendererN.sprite = numbers[hits];

        moveTimer = 0;
        isMoving = false;
        broken = false;
        isOut = false;

        dis = false;
        alpha = 100;
    }

    //Sets number of hits
    public void setHits(int hit)
    {
        hits = hit;
    }

    //Set destination / future position
    public void SetDestination(Vector2Int dir, Vector2Int dest)
    {
        direction = dir;
        destination = dest;
    }

    //Makes acorn Fade out over time
    void Disappear()
    {
        if (dis && broken)
        {
            Color aColor = spriteRenderer.color;
            alpha -= 2;
            aColor.a = alpha;
            spriteRenderer.color = aColor;
            spriteRendererN.color = aColor;

            if (alpha <= 0) dis = false;
        }
    }

    void Move ()
    {


        if (isMoving)
        {
            LERP();
            return;
        }
        else if (arrayPosition != destination)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
            direction = Vector2Int.zero;
            Disappear();
        }
        
    }

    //Reduce hits acorn can take
    public void TakeHit()
    {
        hits--;
        if (hits <= 0)
        {
            broken = true;
            return;
        }
    }

    // Update is called once per frame
    void Update () {
        Move();
	}

    //Same lerp from Player
    private void LERP()
    {
        //get the direction vector
        Vector2 currentLerp = destination - arrayPosition;
        //get the percent of that
        currentLerp *= (moveTimer / MOVE_TIME);
        //add the current pos to get what the position should be right now
        currentLerp += arrayPosition;
        //set the position
        transform.position = new Vector3(currentLerp.x, currentLerp.y, 0);
        //increment timer
        moveTimer += Time.deltaTime;

        //if the timer is up
        if (moveTimer > MOVE_TIME)
        {
            //reset
            moveTimer = 0;
            isMoving = false;
            //set the arrayPos
            arrayPosition = destination;
            //set the transform
            transform.position = new Vector3(arrayPosition.x, arrayPosition.y, 0);

            //change the number sprite
            spriteRendererN.sprite = numbers[hits];
            //check if acorn is broken after moving
            if (broken) { dis = true; }
        }

    }
}

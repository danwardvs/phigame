﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class GameScript : MonoBehaviour
{
    // References to gameObjects that are used by this gameObject
    public GameObject gameBallPrefab;

    private GameObject arrowObject;
    private GameObject initialClickIndicator;
    private GameObject distanceIndicator;

    // We store a reference to this so we can change colour without re-getting the object
    private SpriteRenderer gameStartSpriteRenderer;

    private Text gameText;
    private float gameTimer = 0;
    private float finishTime = 0;
    private int ballCount = 0;
    // Limit power to abritary value given here
    private const float MAX_FORCE = 7;


    // Variables for the click and drag
    private bool isClicked = false;
    private bool levelFinish = false;
    private bool invertControls = false;
    private Vector2 calculatedForce = new Vector2(0,0);
    private Vector2 mouseLocation = new Vector2(0,0);
    private Vector2 initialClick = new Vector2(0,0);
    
    public Vector2 GetStats()
    {   

        return new Vector2(finishTime, ballCount);
    }

    public void SetLevelFinish(bool newFinish)
    {   
        // Write progress to file
        if(!levelFinish){
            int level_num = SceneManager.GetActiveScene().buildIndex - 2;
            PlayerPrefs.SetInt("Level_" + level_num.ToString(), 1);
            print("Level_" + level_num.ToString());
            PlayerPrefs.Save();

        }
        levelFinish = newFinish;
    }

    void HandleClick(){
        isClicked = true;
        initialClick = mouseLocation;
        initialClickIndicator.transform.position = mouseLocation;
        distanceIndicator.transform.position = mouseLocation;

    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Start the game timer
        gameTimer = Time.time;

        // Load inverted controls from PlayerPrefs, which is stored in a local
        // file and is set by the settings menu. 
        if(PlayerPrefs.GetInt("Control", 1)==0){
            invertControls = true;
        }
        //Grab a reference to the sprite renderer so we can manipulate the colour later on
        gameStartSpriteRenderer = GetComponent<SpriteRenderer>();

        // Snag a reference to the children for later use
        arrowObject = GameObject.Find("arrow");
        initialClickIndicator = GameObject.Find("InitialTouch");
        distanceIndicator = GameObject.Find("DistanceIndicator");



        // The text object that should be included with every level
        gameText = GameObject.Find("ShotText").GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {   
        // Little thing to test out inverted control scheme
        if(Input.GetKeyDown("space")){
            invertControls=!invertControls;
        }

        if (!levelFinish)
        {
            // Update the timer so the EndgameGUI can grab it for the stats displayed
            finishTime = Time.time - gameTimer;

            // Update mouse location, this is used later on in the update loop
            mouseLocation = Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));

            // Check if a touch has happened
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // Ignore touch if pressing on a UI element
                if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    HandleClick();

                }
            }

            // Check if a mouse click has happened
            else if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                HandleClick();
            }

            // Handles left click being released
            if (Input.GetMouseButtonUp(0))
            {
                if (isClicked)
                {
                    // If game start object was being clicked and dragged, spawn a game ball
                    GameObject game_ball_instance = Instantiate(gameBallPrefab, transform);
                    game_ball_instance.GetComponent<Rigidbody2D>().AddForce(calculatedForce * 100);

                    // Add one to our count for endgame stats
                    ballCount++;
                }

                isClicked = false;

            }
            if (isClicked)
            {
                // Calculate a force based on mouse location and game start object position
                calculatedForce = new Vector2(initialClick.x, initialClick.y) - mouseLocation;

                if(!invertControls){
                    calculatedForce = calculatedForce*-1;
                }

                if (calculatedForce.magnitude > MAX_FORCE)
                    calculatedForce = calculatedForce * (MAX_FORCE / calculatedForce.magnitude);

                // We use MATH and FUNCTIONS to find angles
                float new_angle = 90 + -57 * Mathf.Atan2(calculatedForce.x, calculatedForce.y);

                // Set the rotation of the arrow to show current trajectory of ball
                arrowObject.transform.eulerAngles = new Vector3(0, 0, new_angle);

                // Set scale of arrow to show power
                float min_scale = 0.3f;
                float scale_rate = 0.15f;
                arrowObject.transform.localScale = new Vector2(min_scale + calculatedForce.magnitude * scale_rate, min_scale + calculatedForce.magnitude * scale_rate);

                // Properly scale the image to match the mouse location
                float distance_scale = 0.2f; 
                distanceIndicator.transform.localScale = new Vector2(calculatedForce.magnitude*distance_scale,calculatedForce.magnitude*distance_scale);


                // Set colour of arrow to show power
                arrowObject.GetComponent<SpriteRenderer>().color = new Color((1f / 7f) * calculatedForce.magnitude, 1f - (1f / 7f) * calculatedForce.magnitude, 0f);

                // Set visual cues on the base object (triangle)
                gameStartSpriteRenderer.color = Color.red;
                arrowObject.SetActive(true);
                initialClickIndicator.SetActive(true);
                distanceIndicator.SetActive(true);

                gameText.enabled = true;

                // Update GUI to reflect the current shot vector
                gameText.text = "Power:" + calculatedForce.magnitude.ToString("#.#") + "\nAngle:" + new_angle.ToString("#");


            }
            else
            {
                // Reset visual cues
                gameStartSpriteRenderer.color = Color.green;
                arrowObject.SetActive(false);
                initialClickIndicator.SetActive(false);
                distanceIndicator.SetActive(false);


                gameText.enabled = false;

            }
        }
        // If level is finished
        else
        {
            arrowObject.SetActive(false);
            gameText.enabled = false;
            gameStartSpriteRenderer.enabled = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class joyControl : MonoBehaviour
{
    private Rigidbody spaceship;
    public float upSpeed;
    public FixedJoystick joystick;
    public Button boostButtom;
    public Button resetButton;
    private bool isBoosting = false;
    private float boostSpeed;
    private float score;
    private int life;

    public TextMeshProUGUI price;
    public TextMeshProUGUI timeLeft;

    public GameObject outBounds;

    public GameObject ptrash;
    public GameObject ltrash;
    public GameObject gameScreen;
    public GameObject startGameScreen;

    private float timeToAppear = 10f;
    private float timeWhenDisappear;

    private float startTime;
    private int numTags;
    public GameObject heart;


    private GameObject forend;

    // Happens at start 
    private void Start()
    {
        life = 5;
        spaceship = transform.GetComponent<Rigidbody>();
        boostButtom.onClick.AddListener(() => isBoosting = true);
        resetButton.onClick.AddListener(actualReset);
        boostSpeed = (float)(upSpeed * 1.4);
        gameScreen.SetActive(false);

        forend = startGameScreen.transform.GetChild(1).gameObject;
        forend.SetActive(false);

        Time.timeScale = 0;
        startTime = Time.time;

        deductLife();
        
    }

    //This resets the postion and orientation 
    private void rstBtn()
    {
        transform.position = new Vector3(137f, 0.25f, 0f);
        transform.LookAt(new Vector3(0f, 0f, 2.43f));
        
    }


    // This will happen when the actual resetButton is pressed
    private void actualReset()
    {
        life = 5;
        score = 0;
        rstBtn();
        startTime = Time.time;
        bringBack();
        deductLife();
    }


    // On each restart all the planetary trash and lunar trash should come back
    // That's what this function does

    private void bringBack()
    {
        GameObject pt = GameObject.FindGameObjectWithTag("Celestials");

        for (int i = 0; i < 8; i++)
        {
            pt.transform.GetChild(i).gameObject.SetActive(true);
        }

        GameObject[] m = GameObject.FindGameObjectsWithTag("Moons");
        foreach (GameObject item in m)
        {
            for (int i = 0; i < item.transform.childCount; i++)
            {
                item.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    //Orginial purpose was to deduct life but ended up being used in a lot of places 
    private void deductLife()
    {
       if (life==0)
        {
            endGame();
        }
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Life"))
        {
            g.SetActive(false);
        }
        for (int i=0;i<life;i++)
        {
            heart.transform.GetChild(i).gameObject.SetActive(true);
        }

    }

    //This is also needed for the boundary
    private void activate()
    {
        outBounds.SetActive(true);
        timeWhenDisappear = Time.time + timeToAppear;
    }

    //This implements boundary
    private void checkDistance()
    {
        Vector3 currPos = transform.position;
        Vector3 center = new Vector3(0f, 0f, 2.43f);
        float dis = Vector3.Distance(currPos, center);
        if (dis>200)
        {
            score -= 50;
            rstBtn();
            activate();
        }

    }

    // Called every frame
    void Update()
    {
        float yaw = joystick.Horizontal * upSpeed * Time.deltaTime*2;
        float pitch = joystick.Vertical * upSpeed * Time.deltaTime*2;

        // Applying yaw and pitch rotations
        transform.Rotate(Vector3.up, yaw, Space.Self);
        transform.Rotate(Vector3.right, pitch, Space.Self);
        spaceship.velocity = transform.forward * 15;
        if (isBoosting)
        {
            transform.position += transform.forward * boostSpeed;
            isBoosting = false;
        }
        checkDistance();
        
        if (outBounds.activeSelf && (Time.time >= timeWhenDisappear))
        {
            outBounds.SetActive(false);
        }
        raycast();
        setStats();
        numTags = GameObject.FindGameObjectsWithTag("Trash").Length + GameObject.FindGameObjectsWithTag("PTrash").Length;

        if (numTags == 0)
        {
            forend.transform.GetChild(2).gameObject.SetActive(true);
            endGame();
        }
    }

    // This will check when the game ends and what to do in that case 
    void endGame()
    {
        if (life == 0)
        {
            forend.transform.GetChild(2).gameObject.SetActive(false);
        }
        numTags = GameObject.FindGameObjectsWithTag("Trash").Length + GameObject.FindGameObjectsWithTag("PTrash").Length;
        if (life==0||numTags==0)
        {
            gameScreen.SetActive(false);
            startGameScreen.SetActive(true);
            Time.timeScale = 0;
        }
    }

    //When collison happens this executes
    void OnTriggerEnter(Collider other)
    {
        print("Hit");
        if (other.gameObject.CompareTag("Trash"))
        {
            score += 100;
            other.gameObject.SetActive(false);

        }
        else if (other.gameObject.CompareTag("PTrash"))
        {
            score += 150;
            other.gameObject.SetActive(false);
        }
        else
        {
            score -= 50;
            life -= 1;
 
            Vector3 currPos = transform.position + Vector3.down;
            rstBtn();
            if (other.gameObject.CompareTag("Moons"))
            {
                GameObject g = Instantiate(ltrash, currPos, Quaternion.identity);
                g.transform.SetParent(other.transform);
                g.AddComponent<Rotate>();
                g.transform.tag = "Trash";
            }
            else
            {
                GameObject g = Instantiate(ptrash, currPos + 2 * Vector3.right, Quaternion.identity);
                g.transform.SetParent(other.transform);
                g.AddComponent<Rotate>();
                g.transform.tag = "PTrash";
            }
            deductLife();
        }
        setStats();
    }
    //Increnent time in seconds more naturally
    private void FixedUpdate()
    {
        timeLeft.text = "Time Left: " + ((int)(120-(Time.time - startTime))).ToString();
        
    }

    //Sets the value 
    void setStats()
    {
        price.text = "Value of collected Trash: $" + score.ToString();

    }
    // This executes to play the game 
    public void playGame()
    {
        gameScreen.SetActive(true);
        startGameScreen.SetActive(false);
        Time.timeScale = 1;
        rstBtn();
        score = 0;
        life = 5;
        startTime = Time.time;
        deductLife();
        bringBack();


    }
    //This function raycasts the touches 
    void raycast()
    {
        if (Input.touchCount > 0 && (Input.GetTouch(0).phase == UnityEngine.TouchPhase.Stationary || Input.GetTouch(0).phase == UnityEngine.TouchPhase.Moved))
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Vector3 pos = Input.GetTouch(i).position;
                pos.z = 0;
                Ray ray = Camera.main.ScreenPointToRay(pos);
                RaycastHit obj;
                print(Camera.main);

                if (Physics.Raycast(ray, out obj))
                {
                    print(obj.transform.tag);
                    if (obj.collider.CompareTag("PTrash"))
                    {
                        score += 150;
                        obj.collider.gameObject.SetActive(false);
                    }
                    else if (obj.collider.CompareTag("Trash"))
                    {
                        score += 100;
                        obj.collider.gameObject.SetActive(false);
                    }
                    else
                    {
                        life -= 1;
                        deductLife();
                    }
                }
            }
        }
        setStats();
    }


}

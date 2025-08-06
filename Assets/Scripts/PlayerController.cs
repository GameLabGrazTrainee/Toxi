using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public enum ToxicType
{
    Not, Insult, Disruption, Threat, IllWish
}

[Serializable]
public class Messages
{
    public string messageText;
    public string username;
    public ToxicType isToxic;
}

public class PlayerController : MonoBehaviour
{
    private float speed = 0;
    public TextMeshProUGUI countText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI usernameText;
    public TextMeshProUGUI goodMessage;
    public TextMeshProUGUI badMessage; 
    public TextMeshProUGUI livesText;
    public GameObject winTextObject;
    private Rigidbody rb;
    private int count;
    private int livesCount;
    private float movementX;
    private float movementY;
    public List<Messages> messages2;
    
    private int currentMessage = 0;
    private int currentUsername = 0;
    public float messageInterval = 10f;
    private float messageTimer;
    public float feedbackMessageInterval = 5f;
    private float feedbackMessageTimer;
    private bool isFeedbackMessageShown = false;
    private bool isToxiFollowing = true;
    public float baseSpeed = 0;
    public GameObject Enemy;
    public AudioClip coin;
    public AudioClip ribbit;
    public AudioSource audioSource;
    public Transform respawnPoint;
    public Transform quizLocation;

    //0 = not toxic
    //1 = Insult
    //2 = Disruption of Gameplay
    //3 = Threat
    //4 = Ill Wish

    // Start is called before the first frame update
    void Start()
    {
        speed = baseSpeed;
        rb = GetComponent<Rigidbody>();
        count = 0;
        livesCount = 3;
        SetCountText();
        SetLivesText();
        winTextObject.SetActive(false);
        ShowNextMessage();
        goodMessage.gameObject.SetActive(false);
        badMessage.gameObject.SetActive(false);
        //this.transform.position = quizLocation.position;
    }

    void ShowNextMessage()
    {
        currentMessage = UnityEngine.Random.Range(0, messages2.Count);
        messageText.GetComponent<TextMeshProUGUI>().text = messages2[currentMessage].messageText;
        usernameText.GetComponent<TextMeshProUGUI>().text = messages2[currentUsername].username;
        if (messages2[currentMessage].isToxic != ToxicType.Not)
        {
            isToxiFollowing = false;
        }
    }

    void OnReport()
    {
        isFeedbackMessageShown = true;
        feedbackMessageTimer = 0f;
        if (messages2[currentMessage].isToxic == ToxicType.Not)
        {
            speed = baseSpeed * 0.6f;
            goodMessage.gameObject.SetActive(false);
            badMessage.gameObject.SetActive(true);
        }
        else
        {
            speed = baseSpeed * 1.2f;
            goodMessage.gameObject.SetActive(true);
            badMessage.gameObject.SetActive(false);
            Enemy.GetComponent<EnemyMovement>().stopChasingPlayer();
            messageInterval = 15f;
        }
    }



    private void Update()
    {
        messageTimer = messageTimer + Time.deltaTime;
        if (messageTimer > messageInterval)
        {
            ShowNextMessage();
            messageTimer = 0f;
            messageInterval = 7f;
           
        }
        if (isFeedbackMessageShown == true)
        {
            feedbackMessageTimer = feedbackMessageTimer + Time.deltaTime;
            if (feedbackMessageTimer > feedbackMessageInterval)
            {
                goodMessage.gameObject.SetActive(false);
                badMessage.gameObject.SetActive(false);
                isFeedbackMessageShown = false;
                feedbackMessageTimer = 0f;

            }
        }
    }
    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString() + "/20";
        if (count >= 20)
        {
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
            goodMessage.gameObject.SetActive(false);
            badMessage.gameObject.SetActive(false);
        }
    }

    void SetLivesText()
    {
        livesText.text = "Lives: " + livesCount.ToString();
        if (livesCount <= 0)
        {
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
            Destroy(gameObject);
            goodMessage.gameObject.SetActive(false);
            badMessage.gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
            audioSource.clip = coin;
            audioSource.Play();
        }

        if (other.gameObject.CompareTag("Insult") && messages2[currentMessage].isToxic == ToxicType.Insult)
        {
            livesCount = livesCount + 1;
        }

        if (other.gameObject.CompareTag("Disruption of Gameplay") && messages2[currentMessage].isToxic == ToxicType.Disruption)
        {
            livesCount = livesCount + 1;
        }

        if (other.gameObject.CompareTag("Threat") && messages2[currentMessage].isToxic == ToxicType.Threat)
        {
            livesCount = livesCount + 1;
        }

        if (other.gameObject.CompareTag("Ill Wish") && messages2[currentMessage].isToxic == ToxicType.IllWish)
        {
            livesCount = livesCount + 1;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            bool isFollowing = Enemy.GetComponent<EnemyMovement>().isFollowingPlayer;
            if (isFollowing == true)
            {
                livesCount = livesCount - 1;
                SetLivesText();
                this.transform.position = respawnPoint.position;
            }
            else
            {
                winTextObject.SetActive(true);
                audioSource.clip = ribbit;
                audioSource.Play();
                Enemy.GetComponent<EnemyMovement>().Die();
                
                goodMessage.gameObject.SetActive(false);
                badMessage.gameObject.SetActive(false);
            }
            
        }
    }
}

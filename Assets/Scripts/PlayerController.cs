using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;

public enum ToxicType
{
    Not, Insult, DisruptionOfGameplay, Threat, IllWish
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
    public TextMeshProUGUI rightCategoryMessage;
    public TextMeshProUGUI wrongCategoryMessage;
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
    private bool isFeedbackMessageShown = false;
    public float categoryMessageInterval = 5f;
    private bool isCategoryMessageShown = false;
    private bool isToxiFollowing = true;
    public float baseSpeed = 0;
    public GameObject Enemy;
    public AudioClip coin;
    public AudioClip ribbit;
    public AudioSource audioSource;
    public Transform respawnPoint;
    public Transform quizLocation;
    public bool isGameRunning;


    
    // Start is called before the first frame update
    void Start()
    {
        isGameRunning = true;
        speed = baseSpeed;
        rb = GetComponent<Rigidbody>();
        count = 0;
        livesCount = 3;
        SetCountText();
        SetLivesText();
        winTextObject.SetActive(false);

        messageInterval = 0;
        StartCoroutine(ShowNextMessage());
        
        goodMessage.gameObject.SetActive(false);
        badMessage.gameObject.SetActive(false);
        rightCategoryMessage.gameObject.SetActive(false);
        wrongCategoryMessage.gameObject.SetActive(false);
        //this.transform.position = quizLocation.position;
    }

   

    void OnReport()
    {
        isFeedbackMessageShown = true;
        
        if (messages2[currentMessage].isToxic == ToxicType.Not)
        {
            speed = baseSpeed * 0.6f;
            BadMessage();
            Enemy.GetComponent<EnemyMovement>().ToxicGas();
        }
        else
        {
            speed = baseSpeed * 1.2f;
            GoodMessage();
            Enemy.GetComponent<EnemyMovement>().stopChasingPlayer();
            messageInterval = 15f;
            this.transform.position = quizLocation.position;
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
            isGameRunning = false;
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
            goodMessage.gameObject.SetActive(false);
            badMessage.gameObject.SetActive(false);
            rightCategoryMessage.gameObject.SetActive(false);
            wrongCategoryMessage.gameObject.SetActive(false);
        }
    }

    void SetLivesText()
    {
        livesText.text = "Lives: " + livesCount.ToString();
        if (livesCount <= 0)
        {
            isGameRunning = false;
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
            Destroy(gameObject);
            goodMessage.gameObject.SetActive(false);
            badMessage.gameObject.SetActive(false);
            rightCategoryMessage.gameObject.SetActive(false);
            wrongCategoryMessage.gameObject.SetActive(false);
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
                isGameRunning = false;
                winTextObject.SetActive(true);
                audioSource.clip = ribbit;
                audioSource.Play();
                Enemy.GetComponent<EnemyMovement>().Die();
                goodMessage.gameObject.SetActive(false);
                badMessage.gameObject.SetActive(false);
                rightCategoryMessage.gameObject.SetActive(false);
                wrongCategoryMessage.gameObject.SetActive(false);
            }
            
        }
    }

    public Messages GetCurrentMessage()
    {
        return messages2[currentMessage];
    }
    public void IncreaseLife()
    {
        livesCount++;
        SetLivesText();
    }
    public void DecreaseSpeed()
    {
        speed = baseSpeed * 0.6f;
    }

    public void ResetPosition()
    {
        this.transform.position = respawnPoint.position;
    }

    public void RightCategory()
    {
        StartCoroutine(ShowMessageCategory(true));
    }
    public void WrongCategory()
    {
        StartCoroutine(ShowMessageCategory(false));
    }

    private IEnumerator ShowMessageCategory(bool isRight)
    {
        rightCategoryMessage.gameObject.SetActive(isRight);
        wrongCategoryMessage.gameObject.SetActive(!isRight);
        goodMessage.gameObject.SetActive(false);
        badMessage.gameObject.SetActive(false);

        yield return new WaitForSeconds(categoryMessageInterval);

        rightCategoryMessage.gameObject.SetActive(false);
        wrongCategoryMessage.gameObject.SetActive(false);
    }

    public void GoodMessage()
    {
        StartCoroutine(ShowFeedbackMessage(true));
    }
    public void BadMessage()
    {
        StartCoroutine(ShowFeedbackMessage(false));
    }
    private IEnumerator ShowFeedbackMessage(bool isRight)
    {
        goodMessage.gameObject.SetActive(isRight);
        badMessage.gameObject.SetActive(!isRight);
        rightCategoryMessage.gameObject.SetActive(false);
        wrongCategoryMessage.gameObject.SetActive(false);

        yield return new WaitForSeconds(feedbackMessageInterval);

        goodMessage.gameObject.SetActive(false);
        badMessage.gameObject.SetActive(false);
    }
    private IEnumerator ShowNextMessage()
    {
        if (!isGameRunning)
            yield return null;

        yield return new WaitForSeconds(messageInterval);
        messageInterval = 7;

        currentMessage = UnityEngine.Random.Range(0, messages2.Count);
        currentUsername = UnityEngine.Random.Range(0, messages2.Count);
        messageText.GetComponent<TextMeshProUGUI>().text = messages2[currentMessage].messageText;
        usernameText.GetComponent<TextMeshProUGUI>().text = messages2[currentUsername].username;
        if (messages2[currentMessage].isToxic != ToxicType.Not)
        {
            isToxiFollowing = false;
        }

        yield return new WaitForEndOfFrame();
        yield return ShowNextMessage();
    }
}

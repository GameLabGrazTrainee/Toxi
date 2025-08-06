using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizArea : MonoBehaviour
{
    
    public ToxicType Type;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter: {other.gameObject.name}");

        if (!other.gameObject.CompareTag("Player"))
            return;
        var playerController = other.gameObject.GetComponent<PlayerController>();
        var messageType = playerController.GetCurrentMessage().isToxic;
        if (messageType == Type)
        { 
            playerController.IncreaseLife();
            playerController.ResetPosition();
            playerController.RightCategory();
        }
        else
        {
            playerController.DecreaseSpeed();
            playerController.ResetPosition();
            playerController.WrongCategory();
        }
    }


}

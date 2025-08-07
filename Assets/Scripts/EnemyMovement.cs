using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;
    public bool isFollowingPlayer = true;
    public List<Transform> targets = new List<Transform>();
    private Transform currentTarget;
    private float runawayTimer;
    private float runawayInterval = 15f;
    private Animator animator;
    public AudioClip startClip;
    public AudioClip stopClip;
    public AudioClip ribbit;
    public AudioClip coin;
    public AudioSource audioSource;
    public ParticleSystem toxicCloudParticles;


    // Start is called before the first frame update
    void Start()
    {
        
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        animator.SetBool("IsRunning", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && isFollowingPlayer == true)
        {
            navMeshAgent.SetDestination(player.position);
        }
        else 
        {
            navMeshAgent.SetDestination(currentTarget.position);
        }
        if (isFollowingPlayer == false)
        {
            runawayTimer = runawayTimer + Time.deltaTime;
            if (runawayTimer > runawayInterval)
            {
                runawayTimer = 0f;
                startChasingPlayer();
            }
        }
    }

    public void chooseTarget()
    {
        int newTargetIndex = Random.Range(0, targets.Count);
        Transform newTarget = targets[newTargetIndex];
        if (newTarget == currentTarget)
        {
            chooseTarget();
        }
        else
        {
            currentTarget = newTarget;
        }
    }

    public void stopChasingPlayer()
    {
        chooseTarget();
        isFollowingPlayer = false;
        navMeshAgent.speed = 8f;
        audioSource.clip = stopClip;
        audioSource.Play();
    }

    public void startChasingPlayer()
    {
        isFollowingPlayer = true;
        navMeshAgent.speed = 3f;
        audioSource.clip = startClip;
        audioSource.Play();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ToxiTarget") && isFollowingPlayer == false)
        {
            chooseTarget();
        }   
    }
    public void Die()
    {
        navMeshAgent.speed = 0;
        //velocity
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        animator.SetTrigger("FallDown");
        Destroy(this.gameObject, 5f);
    }

    public void ToxicGas()
    {
        toxicCloudParticles.Play();
    }
}

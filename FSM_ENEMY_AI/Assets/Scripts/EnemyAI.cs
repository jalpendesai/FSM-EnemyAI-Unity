﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

    // General state machine variables 
    private Animator animator;
    private Ray ray;
    private RaycastHit hit;
    private float maxDistanceToCheck = 30.0f;
    private float currentDistance;
    private Vector3 checkDirection;

    // Patrol state variables
    public Transform pointA;
    public Transform pointB;
    [HideInInspector]public GameObject player;

    // Chase State
    public float chaseTime = 0;

    // Fireing buller
    public float timer = 2;
    public GameObject fireBall;
    public Transform fireBallSpawn;

    public UnityEngine.AI.NavMeshAgent navMeshAgent;

    private int currentTarget;
    private float distanceFromTarget;
    private Transform[] waypoints = null;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        animator = gameObject.GetComponent<Animator>();
        pointA = GameObject.Find("w1").transform;
        pointB = GameObject.Find("w2").transform;
        navMeshAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        waypoints = new Transform[2] {
            pointA,
            pointB
        };
        currentTarget = 0;
        navMeshAgent.SetDestination(waypoints[currentTarget].position);
    }

    private void FixedUpdate()
    {
        // First we check distance from the player
        currentDistance = Vector3.Distance(player.transform.position, transform.position);
        animator.SetFloat("distanceFromPlayer", currentDistance);

        // Then we check for visibility
        checkDirection = (player.transform.position - transform.position).normalized;
        ray = new Ray(transform.position, checkDirection);
        
        
        Vector3 forward = transform.TransformDirection(Vector3.forward) * maxDistanceToCheck;
        Debug.DrawRay(transform.position, forward, Color.red);

        if (Physics.Raycast(ray, out hit, maxDistanceToCheck))
            {
                if (hit.transform.gameObject.tag == player.gameObject.tag)
                {
                    animator.SetBool("isPlayerVisible", true);
                }
                else
                {
                    animator.SetBool("isPlayerVisible", false);
                }
            }
            else
            {
                animator.SetBool("isPlayerVisible", false);
            }

            // Lastly, we get the distance to the next waypoint target
            distanceFromTarget = Vector3.Distance(waypoints[currentTarget].position, transform.position);
        animator.SetFloat("distanceFromWaypoint", distanceFromTarget);

        
        
    }
    
    public void SetNextPoint()
    {
        switch (currentTarget)
        {
            case 0:
                currentTarget = 1;
                break;
            case 1:
                currentTarget = 0;
                break;
        }
        navMeshAgent.SetDestination(waypoints[currentTarget].position);
    }

    // Chase State
    public void ChaseState()
    {
        StartCoroutine(Chase());
        StartCoroutine(StartCountdown());
    }

    // --- Chasing Player ---
    IEnumerator Chase()
    {
        navMeshAgent.SetDestination(player.transform.position);
        navMeshAgent.speed *= 2;
        yield return new WaitForFixedUpdate();

    }

    public IEnumerator StartCountdown(float cTime = 7)
    {
        chaseTime = cTime;
        while (chaseTime > 0)
        {
            Debug.Log("Countdown: " + chaseTime);
            yield return new WaitForSeconds(1.0f);
            chaseTime--;
        }
        // Check if player is chased
        if (currentDistance < 10.0f & chaseTime < 2.0f)
        {
            animator.SetBool("isChased", true);
            animator.gameObject.GetComponent<EnemyAI>().SetNextPoint();
        }
    }

    // Fire State
    public void FireState()
    {
        StartCoroutine(Fire());
              
    }
    IEnumerator Fire()
    {
        transform.LookAt(player.transform);
        //Spawing bullet
        var bullet = (GameObject)Instantiate(fireBall, fireBallSpawn.position, fireBallSpawn.rotation);

        //Adding Velocity to bullet
        bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 20;

        //Destroy bullet after 1 sec
        Destroy(bullet, 1.5f);
        yield return new WaitForSeconds(0.001f);
    }

    // Flee if player is chased
    public void Flee()
    {

    }


    // Stops and Investigate

    public void LookAround()
    {
        
        //navMeshAgent.updatePosition = false;
    }
}

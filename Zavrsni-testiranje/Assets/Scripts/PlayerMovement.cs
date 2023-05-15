using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // player movement speed
    public float stoppingDistance = 0.1f; // distance from the goal at which the player stops moving
    public float rotationSpeed = 5f; // rotation speed of the player
    public List<Vector3> path; // path generated by the algorithms
    private int currentPathIndex = 0; // index of the current position in the path

    [SerializeField]
    private Animator playerAnimator;

    private GameObject breadthFS;
    private GameObject dijsktra;

    private GameControl gameControl;

    public void Start()
    {
        gameControl = GameObject.FindGameObjectWithTag("GameControl").GetComponent<GameControl>();
    }

    public void Update()
    {
        if(gameControl.isRunning)
        {
            // Check if the player has reached the goal position
            if (currentPathIndex >= path.Count)
            {
                playerAnimator.SetInteger("Run", 0);
                gameControl.isRunning = false;
                return;
            }

            playerAnimator.SetInteger("Run", 1);

            // Move the player towards the current position in the path
            Vector3 targetPosition = path[currentPathIndex];
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget > stoppingDistance)
            {
                // Calculate the direction of movement
                Vector3 directionToMove = (targetPosition - transform.position).normalized;

                // Move the player
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

                // Rotate the player towards the direction of movement
                Quaternion targetRotation = Quaternion.LookRotation(directionToMove);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Move to the next position in the path
                currentPathIndex++;
            }
        }
    }
}

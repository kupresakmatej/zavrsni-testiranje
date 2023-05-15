using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject player;
    private GameObject exit;

    void Start()
    {
        StartCoroutine(GetPositions());
    }

    private IEnumerator GetPositions()
    {
        yield return new WaitForSeconds(5f);

        player = GameObject.FindGameObjectWithTag("Player");
        exit = GameObject.FindGameObjectWithTag("Exit");

        Debug.Log($"Spawned player position is {player.transform.position}");
        Debug.Log($"Spawned exit position is {exit.transform.position}");

        List<Vector3> walkableTilePositions = WalkableTiles.instance.walkableTilePositions;

        if (walkableTilePositions.Contains(player.transform.position))
        {
            Debug.Log("Player spawned correctly");
        }
        else
        {
            Debug.Log("Player spawned incorrectly");
        }

        if (walkableTilePositions.Contains(exit.transform.position))
        {
            Debug.Log("Exit spawned correctly");
        }
        else
        {
            Debug.Log("Exit spawned incorrectly");
        }
    }
}

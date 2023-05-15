using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    [SerializeField]
    private GameObject breadthFS;
    [SerializeField]
    private GameObject dijsktra;

    public bool isRunning;

    private PlayerMovement playerMovement;

    private void Start()
    {
        breadthFS.SetActive(false);
        dijsktra.SetActive(false);

        isRunning = false;
    }

    public void StartBFS()
    {
        breadthFS.SetActive(true);
    }

    public void StartDijkstra()
    {
        dijsktra.SetActive(true);
    }

    public void StartRunning()
    {
        isRunning = true;

        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

        if (breadthFS.activeSelf && !dijsktra.activeSelf)
        {
            playerMovement.path = BreadthFirstSearch.instance.path;
        }
        else if (!breadthFS.activeSelf && dijsktra.activeSelf)
        {
            playerMovement.path = Dijkstra.instance.path;
        }

        playerMovement.enabled = true;
    }

    public void Reset()
    {
        SceneManager.LoadSceneAsync(0);
    }
}

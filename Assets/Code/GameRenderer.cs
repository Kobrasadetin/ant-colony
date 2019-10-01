using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRenderer : MonoBehaviour
{
    public GameState gameState;
    private Dictionary<AntModel, Ant> antsMap;
    public GameObject playerAnthillPrefab;
    private Anthill playerAnthill;
    // Start is called before the first frame update
    void Start()
    {
        gameState = new GameState();
        playerAnthill = Instantiate(playerAnthillPrefab, this.transform).GetComponent<Anthill>();
        Debug.Log(playerAnthill);
        playerAnthill.transform.position = gameState.PlayerAnthill.Position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

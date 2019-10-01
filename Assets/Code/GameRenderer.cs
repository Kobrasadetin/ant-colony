using System.Collections.Generic;
using UnityEngine;

public class GameRenderer : MonoBehaviour
{
    public GameState gameState;
    private Dictionary<BasicModel, Ant> antsMap;
    public GameObject playerAnthillPrefab;
    public GameObject antPrefab;
    private Anthill playerAnthill;
    private List<Ant> ants;
    // Start is called before the first frame update
    void Start()
    {
        antsMap = new Dictionary<BasicModel, Ant>();
        ants = new List<Ant>();
        gameState = new GameState();
        playerAnthill = Instantiate(playerAnthillPrefab, this.transform).GetComponent<Anthill>();
        Debug.Log(playerAnthill);
        playerAnthill.transform.position = gameState.PlayerAnthill.Position;
    }

    // Update is called once per frame
    void Update()
    {
        gameState.update();
        updateVisuals();

    }
    void updateVisuals()
    {
        HashSet<Ant> garbageCollected = new HashSet<Ant>(ants);
        Debug.Log(gameState.Ants);
        foreach (AntModel antModel in gameState.Ants) {
            Ant outAnt;
            if (antsMap.TryGetValue(antModel, out outAnt))
            {
                Debug.Log(outAnt);
                outAnt.updatePosition(antModel);
                garbageCollected.Remove(outAnt);
            }
            else
            {
                // Fond a new ant!
                Ant newAnt = Instantiate(antPrefab, this.transform).GetComponent<Ant>();
                ants.Add(newAnt);
                antsMap.Add(antModel, newAnt);
                newAnt.updatePosition(antModel);
            }
        }
        //TODO remove garbagecollected;
    }
}

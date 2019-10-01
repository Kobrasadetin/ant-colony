using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameRenderer : MonoBehaviour
{
    public GameState gameState;
    private Dictionary<BasicModel, RenderObject> modelMap;
    public GameObject playerAnthillPrefab;
    public GameObject antPrefab;
    public GameObject[] foodObjects;
    private Anthill playerAnthill;
    private List<RenderObject> renderObjects;

    // Start is called before the first frame update
    void Start()
    {
        modelMap = new Dictionary<BasicModel, RenderObject>();
        renderObjects = new List<RenderObject>();
        gameState = new GameState();
        playerAnthill = Instantiate(playerAnthillPrefab, this.transform).GetComponent<Anthill>();
        Debug.Log(playerAnthill);
        playerAnthill.transform.position = gameState.PlayerAnthill.Position;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == null) { Start(); };
        gameState.update();
        updateVisuals();

    }
    void updateVisuals()
    {
        Dictionary<RenderObject, bool> garbageCollected = renderObjects.ToDictionary(x => x, x => true);

        //draw ants
        foreach (AntModel antModel in gameState.Ants) {
            RenderObject outAnt;
            if (modelMap.TryGetValue(antModel, out outAnt))
            {
                outAnt.updatePosition(antModel);
                garbageCollected[outAnt]=false;
            }
            else
            {
                // Found a new ant!
                Ant newAnt = Instantiate(antPrefab, this.transform).GetComponent<Ant>();
                renderObjects.Add(newAnt);
                modelMap.Add(antModel, newAnt);
                newAnt.updatePosition(antModel);
            }
        }

        //draw food
        foreach (FoodModel foodModel in gameState.Foods)
        {
            RenderObject outFood;
            if (modelMap.TryGetValue(foodModel, out outFood))
            {
                outFood.updatePosition(foodModel);
                garbageCollected[outFood] = false;
            }
            else
            {
                // Found a new ant!
                Food newFood = Instantiate(foodObjects[0], this.transform).GetComponent<Food>();
                renderObjects.Add(newFood);
                modelMap.Add(foodModel, newFood);
                newFood.updatePosition(foodModel);
            }
        }
        //TODO remove garbagecollected;
    }


}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameRenderer : MonoBehaviour
{
    public GameState gameState;
    private Dictionary<EntityModel, RenderObject> modelMap;
    private Dictionary<PheromoneModel, Pheromone> pheroMap;
    public GameObject playerAnthillPrefab;
    public GameObject antPrefab;
    public GameObject[] foodObjects;
    public GameObject pheromonePrefab;
    private Anthill playerAnthill;
    private List<RenderObject> renderObjects;
    private List<Pheromone> pheromoneObjects;

	private void gameStateInitializer(GameState gameState)
	{
		FoodGenerator[] foodGenerators = GetComponentsInChildren<FoodGenerator>();
		foreach(FoodGenerator generator in foodGenerators){
			Vector2 pos = new Vector2(generator.transform.position.x, generator.transform.position.y);
			gameState.AddFoodSource(pos + new Vector2(0.15f, 0.0f));
			gameState.AddFoodSource(pos - new Vector2(0.15f, 0.0f));
		}
	}

    // Start is called before the first frame update
    void Start()
    {
        modelMap = new Dictionary<EntityModel, RenderObject>();
        pheroMap = new Dictionary<PheromoneModel, Pheromone> ();
        renderObjects = new List<RenderObject>();
        pheromoneObjects = new List<Pheromone>();
        gameState = new GameState();
		gameStateInitializer(gameState);
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

		//anthill
		playerAnthill.updateVisual(gameState.PlayerAnthill);

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

        //draw phero
        Dictionary<Pheromone, bool> garbagePhero = pheromoneObjects.ToDictionary(x => x, x => true);
        foreach (PheromoneModel pheroModel in gameState.Pheromones)
        {
            Pheromone outPhero;
            if (pheroMap.TryGetValue(pheroModel, out outPhero))
            {
                garbagePhero[outPhero] = false;
            }
            else
            {
                // Found a new ant!
                Pheromone newPhero = Instantiate(pheromonePrefab, this.transform).GetComponent<Pheromone>();
                pheromoneObjects.Add(newPhero);
                pheroMap.Add(pheroModel, newPhero);
                newPhero.updatePositionBasic(pheroModel);
            }
        }

		//TODO remove garbagecollected render&phero;
		foreach (KeyValuePair<RenderObject, bool> entry in garbageCollected)
		{
			if (entry.Value == true){
				renderObjects.Remove(entry.Key);
				entry.Key.remove();
			}
		}

	}


}

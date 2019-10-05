using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameRenderer : MonoBehaviour
{
	public GameState gameState;
	private Dictionary<EntityModel, VisualStatus> modelMap;
	private Dictionary<PheromoneModel, VisualStatus> pheroMap;
	public GameObject playerAnthillPrefab;
	public GameObject antPrefab;

	public GameObject[] foodObjects;
	public GameObject pheromonePrefab;
	private Anthill playerAnthill;
	private List<RenderObject> renderObjects;
	private List<Pheromone> pheromoneObjects;
	private PheroParticleRender pheromoneRenderer;
	private int clickDuration = 0;
	private AudioSource musicPlayer;
	private AudioSource sprayPlayer;
	public UnityEngine.UI.Text debugInfo;
	private DebugInfoHandler debugInfoHandeler = new DebugInfoHandler();
	private bool paused = false;
	private GameOptions options = new GameOptions();
	public int simulationMultiplier = 1;

	public GameOptions Options { get => options; set => options = value; }

	private void gameStateInitializer(GameState gameState)
	{
		FoodGenerator[] foodGenerators = GetComponentsInChildren<FoodGenerator>();
		foreach (FoodGenerator generator in foodGenerators)
		{
			Vector2 pos = new Vector2(generator.transform.position.x, generator.transform.position.y);
			gameState.AddFoodSource(pos + new Vector2(0.15f, 0.0f), 1.0f, generator.poisonValue);
			gameState.AddFoodSource(pos - new Vector2(0.15f, 0.0f), 1.0f, generator.poisonValue);
		}
	}

	// Start is called before the first frame update
	private void Start()
	{
		pheromoneRenderer = gameObject.GetComponent<PheroParticleRender>();
		musicPlayer = gameObject.AddComponent<AudioSource>();
		sprayPlayer = gameObject.AddComponent<AudioSource>();
		musicPlayer.clip = Resources.Load("sound/music") as AudioClip;
		sprayPlayer.clip = Resources.Load("sound/spray1") as AudioClip;
		musicPlayer.Play();
		modelMap = new Dictionary<EntityModel, VisualStatus>();
		pheroMap = new Dictionary<PheromoneModel, VisualStatus>();
		renderObjects = new List<RenderObject>();
		pheromoneObjects = new List<Pheromone>();
		gameState = new GameState();
		gameStateInitializer(gameState);
		playerAnthill = Instantiate(playerAnthillPrefab, transform).GetComponent<Anthill>();
		playerAnthill.transform.position = gameState.PlayerAnthill.Position;
	}

	internal void Pause()
	{
		Debug.Log("paused");
		paused = true;
	}
	internal void Resume()
	{
		paused = false;
	}

	private class VisualStatus
	{
		public bool removed = false;
		public RenderObject renderObject;

		public VisualStatus(bool removed, RenderObject renderObject)
		{
			this.removed = removed;
			this.renderObject = renderObject;
		}
	}

	private void WriteDebugInfo(bool renderBlocked)
	{
		if (debugInfo != null)
		{
			debugInfoHandeler.writeDebugInfo(debugInfo, renderBlocked, gameState);
		}
	}

	// Update is called once per frame
	private void Update()
	{
		if (paused)
		{
			return;
		}
		pheromoneRenderer.RenderThreaded(gameState.Pheromones.AsList());
		if (!musicPlayer.isPlaying)
		{
			musicPlayer.Play();
		}
		if (Input.GetKeyDown("1"))
		{
			simulationMultiplier = 1;
		}
		if (Input.GetKeyDown("2"))
		{
			simulationMultiplier = 2;
		}
		if (Input.GetKeyDown("3"))
		{
			simulationMultiplier = 4;
		}
		if (Input.GetMouseButton(0))
		{
			clickDuration += 1;
		}
		if (Input.GetMouseButtonUp(0))
		{
			if (clickDuration < 8)
			{
				Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				gameState.AddRepellantPheromone(pz);
				sprayPlayer.Play();
			}
			clickDuration = 0;
		}
		if (gameState == null) { Start(); };
		for (int i = 0; i < simulationMultiplier; i++)
		{
			gameState.update();
		}
		updateVisuals();

	}
	private void LateUpdate()
	{
		pheromoneRenderer.RenderPheromones(options);
	}

	private void updateVisuals()
	{
		foreach (KeyValuePair<EntityModel, VisualStatus> kvPair in modelMap)
		{
			kvPair.Value.removed = true;
		}

		//anthill
		playerAnthill.updateVisual(gameState.PlayerAnthill);

		//draw ants
		foreach (AntModel antModel in gameState.Ants)
		{
			if (modelMap.TryGetValue(antModel, out VisualStatus outAnt))
			{
				Ant ant = (Ant)outAnt.renderObject;

				ant.updateAntPosition(antModel);
				ant.updateAntFeelings(antModel);
				outAnt.removed = false;
			}
			else
			{
				// Found a new ant!
				Ant newAnt = Instantiate(antPrefab, transform).GetComponent<Ant>();
				renderObjects.Add(newAnt);
				modelMap.Add(antModel, new VisualStatus(false, newAnt));
				newAnt.updateAntPosition(antModel);
			}
		}

		//draw food
		foreach (FoodModel foodModel in gameState.Foods)
		{
			if (modelMap.TryGetValue(foodModel, out VisualStatus outFood))
			{
				outFood.renderObject.updatePosition(foodModel);
				outFood.removed = false;
			}
			else
			{
				// Found a new ant!
				Food newFood = Instantiate(foodObjects[0], transform).GetComponent<Food>();
				renderObjects.Add(newFood);
				modelMap.Add(foodModel, new VisualStatus(false, newFood));
				newFood.updatePosition(foodModel);
			}
		}

		foreach (KeyValuePair<EntityModel, VisualStatus> entry in modelMap)
		{
			if (entry.Value.removed == true)
			{
				renderObjects.Remove(entry.Value.renderObject);
				entry.Value.renderObject.remove();
			}
		}
		modelMap = modelMap.Where(x => x.Value.removed == false).ToDictionary(x => x.Key, x => x.Value);

		WriteDebugInfo(pheromoneRenderer.Blocked);
	}

}

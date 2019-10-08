using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameRenderer : MonoBehaviour
{
	public const int PHERO_SPRAY_SOUND_DELAY_FRAMES = 6;

	public GameState gameState;
	private Dictionary<EntityModel, VisualStatus> modelMap;
	private Dictionary<PheromoneModel, VisualStatus> pheroMap;

	public GameObject playerAnthillPrefab;
	public GameObject antPrefab;
	public GameObject foodPrefab;
	//public GameObject pheromonePrefab;
	public GameObject sourcePrefab;

	public AudioClip hatchingSound;
	public AudioClip spraySound;

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
	private PheromoneSprayControl pheromoneSprayControl;
	private int SpraySoundDelay = 0;
	private GameInfo gameInfo;
	private CameraDrag cameraDragControl;

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
		cameraDragControl = FindObjectOfType<CameraDrag>();
		gameInfo = gameObject.GetComponentInChildren<GameInfo>();
		pheromoneSprayControl = gameObject.GetComponentInChildren<PheromoneSprayControl>();
		pheromoneRenderer = gameObject.GetComponent<PheroParticleRender>();
		musicPlayer = gameObject.AddComponent<AudioSource>();
		sprayPlayer = gameObject.AddComponent<AudioSource>();
		musicPlayer.clip = Resources.Load("sound/music") as AudioClip;
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
		cameraDragControl.DragEnabled = false;
		if (pheromoneSprayControl.ActiveButton == PheromoneSprayControl.SprayButton.NONE){
			cameraDragControl.DragEnabled = true;
		}
		SpraySoundDelay++;
		if (paused)
		{
			return;
		}
		pheromoneRenderer.RenderThreaded(gameState.Pheromones.AsList());
		if (!musicPlayer.isPlaying)
		{
			musicPlayer.Play();
		}
		DebugGameSpeedControls();
		if (Input.GetMouseButton(0))
		{
			PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
			GameObject clicked = EventSystem.current.currentSelectedGameObject;
			if (clicked != null && clicked.GetComponent<UIClickable>() != null)
			{
				//ui button click
			}
			else
			{
				if (pheromoneSprayControl.ActiveButton == PheromoneSprayControl.SprayButton.REPEL)
				{
					Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					gameState.AddRepellantPheromone(pz);
					if (SpraySoundDelay > PHERO_SPRAY_SOUND_DELAY_FRAMES)
					{
						SpraySoundDelay = 0;
						sprayPlayer.clip = spraySound;
						sprayPlayer.pitch = Random.Range(0.98f, 1.02f);
						sprayPlayer.Play();
					}
				}
				if (pheromoneSprayControl.ActiveButton == PheromoneSprayControl.SprayButton.ATTRACT)
				{
					Vector3 pz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
					gameState.AddAttractPheromone(pz);
					if (SpraySoundDelay > PHERO_SPRAY_SOUND_DELAY_FRAMES)
					{
						SpraySoundDelay = 0;
						sprayPlayer.clip = spraySound;
						sprayPlayer.pitch = Random.Range(0.98f, 1.02f);
						sprayPlayer.Play();
					}
				}
			}
		}
		if (Input.GetMouseButtonUp(0))
		{
			clickDuration = 0;
		}
		if (gameState == null) { Start(); };
		for (int i = 0; i < simulationMultiplier; i++)
		{
			gameState.update();
		}
		updateVisuals();

	}

	private void DebugGameSpeedControls()
	{
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
	}

	private void LateUpdate()
	{
		pheromoneRenderer.RenderPheromones(options);
	}

	public void AddAnt(){
		if (gameState.AddAnt())
		{
			sprayPlayer.clip = hatchingSound;
			sprayPlayer.Play();
		};
	}

	private void updateVisuals()
	{
		gameInfo.setGameInfo(gameState);
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
				outFood.renderObject.updateState(foodModel);
				outFood.removed = false;
			}
			else
			{
				// Found a new food!
				Food newFood = Instantiate(foodPrefab, transform).GetComponent<Food>();
				newFood.initializeState(foodModel);
				renderObjects.Add(newFood);
				modelMap.Add(foodModel, new VisualStatus(false, newFood));
			}
		}

		//draw sources
		foreach (SourceModel sourceModel in gameState.Sources)
		{
			if (modelMap.TryGetValue(sourceModel, out VisualStatus outSource))
			{
				outSource.renderObject.updateState(sourceModel);
				outSource.removed = false;
			}
			else
			{
				// Found a new source!
				Source newSource = Instantiate(sourcePrefab, transform).GetComponent<Source>();
				newSource.initializeState(sourceModel);
				renderObjects.Add(newSource);
				modelMap.Add(sourceModel, new VisualStatus(false, newSource));
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

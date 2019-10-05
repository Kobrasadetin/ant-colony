using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	private GameRenderer gameRenderer;
    // Start is called before the first frame update
    void OnEnable()
    {
		Debug.Log("menu open");
		gameRenderer = GetComponentInParent<GameRenderer>();
		gameRenderer.Pause();
	}
	void OnDisable()
	{
		gameRenderer = GetComponentInParent<GameRenderer>();
		gameRenderer.Resume();
	}

	public void SetBlurryPheromones(bool setting){
		gameRenderer.Options.blurryPheromones = setting;
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}

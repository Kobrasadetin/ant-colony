using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PheromoneSprayControl : MonoBehaviour
{
	public Image RepelButton;
	public Sprite RepelActive;
	public Sprite repelInactive;
	public Image AttractButton;
	public Sprite AttracActive;
	public Sprite attractInactive;
	private SprayButton activeButton;

	public SprayButton ActiveButton { get => activeButton; set
		{
			activeButton = value;
			RepelButton.sprite = repelInactive;
			AttractButton.sprite = attractInactive;
			if (activeButton == SprayButton.REPEL)
			{
				RepelButton.sprite = RepelActive;
			}
			if (activeButton == SprayButton.ATTRACT)
			{
				AttractButton.sprite = AttracActive;
			}
		}
	}

	public enum SprayButton {
		NONE,
		REPEL,
		ATTRACT
	}
	// Start is called before tse first frame update
	void Start()
    {
		ActiveButton = SprayButton.NONE;
	}

    // Update is called once per frame
    void Update()
    {
	}
	public void Toggle(SprayButton button){
		if (ActiveButton ==button){
			ActiveButton = SprayButton.NONE;
		} else {
			ActiveButton = button;
		}
	}

	public void ToggleAttract()
	{
		Toggle(SprayButton.ATTRACT);
	}
	public void ToggleRepel()
	{
		Toggle(SprayButton.REPEL); 
	}
}

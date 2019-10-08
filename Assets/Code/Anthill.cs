using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anthill : RenderObject
{
	int goodCounter = 20;
	int badCounter = 20;
	int animationLength = 20;
	//int soundqueue
	AudioSource audioSource;
	public GameObject GoodSprite;
	public GameObject BadSprite;
	GameObject animatedFloat = null;
	int soundDelay = 0;
	Stack<AudioClip> soundQueue = new Stack<AudioClip> ();
	AudioClip good;
	AudioClip bad;

	public void Start()
	{
		good = Resources.Load("sound/positive") as AudioClip;
		bad = Resources.Load("sound/negative") as AudioClip;
		audioSource = gameObject.AddComponent<AudioSource>();
	}



	public void updateVisual(AnthillModel model) {
		
		if (model.HealthChanged >0 ){
			goodCounter = 0;
			GoodSprite.SetActive(false);
			GoodSprite.SetActive(true);
			if (soundQueue.Count < 2)
			{
				soundQueue.Push(good);
			}
		
		}
		if (model.HealthChanged < 0)
		{
			badCounter = 0;
			BadSprite.SetActive(false);
			BadSprite.SetActive(true);
			soundQueue.Push(bad);
		}
		if (goodCounter >= animationLength){
			GoodSprite.SetActive(false);
		}
		if (badCounter >= animationLength)
		{
			BadSprite.SetActive(false);
		}
		goodCounter++;
		badCounter++;
		if (soundDelay <= 0 && soundQueue.Count > 0){
			audioSource.clip = soundQueue.Pop();
			audioSource.Play();
			soundDelay = 7;
		}
		if (soundQueue.Count > 4){
			soundQueue.Pop();
		}
		soundDelay -= 1;
	}
}

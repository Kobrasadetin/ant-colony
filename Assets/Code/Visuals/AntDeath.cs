using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntDeath : MonoBehaviour
{
	public const int ANIMATION_LENGTH = 105;
	public readonly int[] ANIMATION_PHASES = { 30, 65 };
	public SpriteRenderer antBody;
	int animationCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (animationCounter < ANIMATION_LENGTH)
		{
			if (animationCounter == ANIMATION_PHASES[0]) antBody.flipX = true;
			if (animationCounter == ANIMATION_PHASES[1]) antBody.flipX = false;
			if (animationCounter == ANIMATION_LENGTH) antBody.flipX = true;
			antBody.color = Color.Lerp(Color.white, new Color(0.85f, 0.95f, 1.0f), 1f/ANIMATION_LENGTH* animationCounter);
			animationCounter++;
		}	

	}
}

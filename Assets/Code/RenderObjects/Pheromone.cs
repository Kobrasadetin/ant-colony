using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pheromone : RenderObject
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setRepellant(bool repellant)
    {
        if (repellant)
        {
            this.GetComponent<SpriteRenderer>().color = new Color(1f, 0f, 0f, .8f);
            this.transform.localScale *= 4f;
        }
    }
}

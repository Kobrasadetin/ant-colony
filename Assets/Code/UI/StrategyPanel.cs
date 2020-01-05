using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyPanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        GameRenderer renderer = GameObject.FindObjectOfType<GameRenderer>();
        renderer.AddActivePanel(this);
    }

    private void OnDisable()
    {
        GameRenderer renderer = GameObject.FindObjectOfType<GameRenderer>();
        if (renderer != null) renderer.RemoveActivePanel(this);
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}

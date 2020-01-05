using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrategyPanel : MonoBehaviour
{
    public GameObject sliderPrefab;
    public string[] sliderNames;

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

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
        Transform content = transform.Find("Scroll View").Find("Viewport").Find("Content");
        Debug.Log(content);
        //clear previous children
        ClearChildren(content);
        AddSliders(content);


    }

    private void OnDisable()
    {
        GameRenderer renderer = GameObject.FindObjectOfType<GameRenderer>();
        if (renderer != null) renderer.RemoveActivePanel(this);
    }

    private void AddSliders(Transform transform)
    {
        float pos = 0;
        foreach (string sliderName in sliderNames)
        {
            GameObject slider = CreateSlider(sliderName, transform);
            RectTransform rt = slider.GetComponent<RectTransform>();
            Debug.Log(rt);
            Vector3 p = rt.position;
            rt.position = new Vector3(p.x, p.y - pos, p.z);
            pos += rt.sizeDelta.y;
        }
        Debug.Log("cacassc");
    }

    private GameObject CreateSlider(string name, Transform transform)
    {
        GameObject go = Instantiate(sliderPrefab, transform);
        Transform textTrans = go.transform.Find("Text");
        Text text = textTrans.GetComponent<Text>();
        text.text = name;
        return go;
    }

    private void ClearChildren(Transform transform)
    {
        int i = 0;

        //Array to hold all child obj
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Find all child obj and store to that array
        foreach (Transform child in transform)
        {
            allChildren[i] = child.gameObject;
            i += 1;
        }

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}

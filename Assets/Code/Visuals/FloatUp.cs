using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatUp : MonoBehaviour
{
	float timer;
	Vector3 origin;
	SpriteRenderer r;
	bool firstEnable = true;
	float cosOffset = 0f;

	void OnEnable()
	{
		if (firstEnable){
			r = this.GetComponent<SpriteRenderer>();
			origin = this.transform.position;
			firstEnable = false;
		}
		timer = 0f;
		cosOffset = Random.Range(-Mathf.PI, Mathf.PI);
		this.transform.position = origin;
	}

    // Update is called once per frame
    void Update()
    {
		timer += 0.03f;
		Vector3 fpos = new Vector3(Mathf.Cos(timer+ cosOffset) *Mathf.Min(timer* timer, 1f), timer, 0f);
		this.transform.position = origin + fpos;
		r.color = new Color(1f, 1f, 1f, 1f - timer);
	}
}

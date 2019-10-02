using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectModel
{
	float Radius { get; set; }
	Vector2 Position { get; set; }
	bool IsInside(IObjectModel largerEntity);
}

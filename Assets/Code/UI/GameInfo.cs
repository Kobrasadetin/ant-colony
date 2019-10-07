using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour
{
	public Text antCount;
	public Text foodReserve;
	public void setGameInfo(GameState gameState){
		antCount.text = gameState.GetAntCount().ToString();
		foodReserve.text = gameState.GetPlayerFood().ToString("0.0");
	}
}

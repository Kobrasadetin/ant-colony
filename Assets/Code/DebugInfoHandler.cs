﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugInfoHandler
{
	const int BUFFER_FRAMES = 60;
	private FrameInfo[] frameTimeBuffer = new FrameInfo[BUFFER_FRAMES];
	private int frameTimeIndex = 0;

	private struct FrameInfo {
		public bool renderIdle;
		public float deltaTimeReciproc;
		public bool recorded;
	};

	public struct BufferInfo
	{
		public float RenderIdleTime;
		public float AverageFramerate;
		public float LowestFramerate;
	};


	private FrameInfo calculateFrameInfo(bool renderBlocked) {
		return new FrameInfo
		{
			renderIdle = renderBlocked,
			deltaTimeReciproc = 1.0f / Time.deltaTime,
			recorded = true,
		};
	}

	public BufferInfo CalculateBufferStats(){
		float sum = 0;
		float min = float.MaxValue;
		int renderIdleSum = 0;
		int total = 0; ;
		foreach (FrameInfo frameInfo in frameTimeBuffer)
		{
			if (frameInfo.recorded)
			{
				renderIdleSum += frameInfo.renderIdle ? 1 : 0;
				sum += frameInfo.deltaTimeReciproc;
				min = Mathf.Min(frameInfo.deltaTimeReciproc, min);
				total++;
			}
		}
		return new BufferInfo
		{
			RenderIdleTime = (float)renderIdleSum / total,
			AverageFramerate = sum / total,
			LowestFramerate = min
		};
	}

    public void writeDebugInfo(Text debugInfo, bool renderBlocked, GameState gameState)
	{
		frameTimeBuffer[frameTimeIndex] = calculateFrameInfo(renderBlocked);
		frameTimeIndex = (frameTimeIndex + 1) % BUFFER_FRAMES;
		BufferInfo info = CalculateBufferStats();
		debugInfo.text = "FPS avr " + info.AverageFramerate.ToString("00.00") +
			", min " + info.LowestFramerate.ToString("00.00") +
			"	thread finished "+ (info.RenderIdleTime*100).ToString("00.") + "%\n"+
			"Pheromones:"+ gameState.Pheromones.AsList().Count;

	}
}

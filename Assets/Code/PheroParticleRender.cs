using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class PheroParticleRender : MonoBehaviour, System.IDisposable
{
	public const int MAX_PARTICLE_COUNT = 16384;
	public const int MIN_FRAME_PARTICLES = 512;
	public const int MAX_FRAME_PARTICLES = 16384;
	public const bool COLORS = true;

	public int ParticlesThisFrame = 16384;
	private readonly int frameParticeCount;
	private ParticleSystem pheroParticleSystem;
	private ParticleSystemRenderer renderer;
	private JobHandle jobHandle;
	private readonly PheroStruct[] pheroInputBuffer = new PheroStruct[MAX_PARTICLE_COUNT];
	private NativeArray<Settings> settingsNativeArray;
	private NativeArray<PheroStruct> nativeInputBuffer;
	private NativeArray<ParticleSystem.Particle> jobResult;
	private readonly ParticleSystem.Particle[] jobResultOutput = new ParticleSystem.Particle[MAX_PARTICLE_COUNT];
	public volatile RenderState CurrentState = RenderState.IDLE;
	private int pheroCount = 0;
	public bool Blocked = false;
	private readonly int startingIndex = 0;
	public Material blurryMaterial;
	public Material solidMaterial;
	private Settings currentSettings = new Settings
	{
		particleSize = 0.3f,
		time = 0f
	};

	//private ParticleSyste

	public enum RenderState
	{
		IDLE,
		QUEUED,
		RENDERING,
	}

	private NativeArray<PheroStruct> PheromoneInputBuffer
	{
		get
		{
			if (!nativeInputBuffer.IsCreated)
			{
				nativeInputBuffer = new NativeArray<PheroStruct>(MAX_PARTICLE_COUNT, Allocator.Persistent);
			}
			return nativeInputBuffer;
		}
	}
	public NativeArray<ParticleSystem.Particle> JobResult
	{
		get
		{
			if (!jobResult.IsCreated)
			{
				jobResult = new NativeArray<ParticleSystem.Particle>(MAX_PARTICLE_COUNT, Allocator.Persistent);
			}

			return jobResult;
		}
	}

	private NativeArray<Settings> SettingsNativeArray
	{
		get
		{
			if (!settingsNativeArray.IsCreated)
			{
				settingsNativeArray = new NativeArray<Settings>(1, Allocator.Persistent);
			}

			return settingsNativeArray;
		}
	}

	private static void UpdateParticles(NativeArray<PheroStruct> pheromones, NativeArray<ParticleSystem.Particle> target)
	{

		int count = pheromones.Length;
		for (int i = 0; i < count; i++)
		{
			PheroStruct p = pheromones[i];
			target[i] = new ParticleSystem.Particle
			{
				position = p.position,
				startColor = new Color
				{
					r = (p.FoodDistance + p.HomeDistance) * .5f + p.Confusion,
					g = p.FoodDistance * 0.1f,
					b = (p.HomeDistance * 0.1f) + p.Confusion,
					a = p.Strength * 0.5f,
				},
				startSize = 0.3f,
				remainingLifetime = 5,
			};
		}
	}


	private static ParticleSystem.Particle PheroToParticle(PheromoneModel phero)
	{
		ParticleSystem.Particle p = new ParticleSystem.Particle
		{
			startSize = 0.3f,
			remainingLifetime = 5,
			position = phero.Position,
			startColor = new Color(1f, 1f, 1f, 0.5f)
		};
		return p;
	}

	// Start is called before the first frame update
	private void Start()
	{
		pheroParticleSystem = GetComponentInChildren<ParticleSystem>();
		renderer = pheroParticleSystem.GetComponent<ParticleSystemRenderer>();
		Debug.Log(renderer);
	}

	private struct Settings
	{
		public float particleSize;
		public float time;
	}

	private struct ParallelUpdate : IJobParallelFor
	{
		[ReadOnly]
		public NativeArray<Settings> settings;
		[ReadOnly]
		public NativeArray<PheroStruct> input;
		[WriteOnly]
		public NativeArray<ParticleSystem.Particle> result;
		void IJobParallelFor.Execute(int i)
		{
			PheroStruct p = input[i];
			if (input[i].SpecialType == 1) //repel
			{
				result[i] = new ParticleSystem.Particle
				{
					position = p.position,
					startColor = new Color //rgb(91%,34%,98%)
					{
						r = .91f,
						g = .34f,
						b = .98f,
						a = p.Strength * 3f,
					},
					startSize = settings[0].particleSize * 2.2f,
					remainingLifetime = 0,
				};
			}
			else if (input[i].SpecialType == 2) //attract
			{
				float m = Mathf.Abs((settings[0].time + p.Strength*150f) % 1f - 0.5f)*.5f + 0.75f;
				result[i] = new ParticleSystem.Particle
				{
					position = p.position,					
					startColor = new Color //rgb(34%,98%,52%)
					{
						r = .34f * m,
						g = .98f *m,
						b = .52f *m,
						a = p.Strength * 4f,
					},
					startSize = settings[0].particleSize * 1.4f,
					remainingLifetime = 0,
				};
			} else
			{
				result[i] = new ParticleSystem.Particle
				{
					position = p.position,
					startColor = new Color
					{
						r = (p.FoodDistance + p.HomeDistance) * .5f + p.Confusion,
						g = p.FoodDistance * 0.1f,
						b = (p.HomeDistance * 0.1f) + p.Confusion,
						a = p.Strength * 0.6f,
					},
					startSize = settings[0].particleSize,
					remainingLifetime = 0,
				};
			}
		}
	}

	private static int AssignSpecial(PheromoneModel model){
		if (model.IsAttract){
			return 2;
		}
		if (model.IsRepellant)
		{
			return 1;
		}
		return 0;
	}

	private struct PheroStruct
	{
		public Vector2 position;
		public float HomeDistance;
		public float FoodDistance;
		public float Strength;
		public float Confusion;
		public int SpecialType;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PheroStruct FromModel(PheromoneModel model)
		{
			return new PheroStruct
			{
				HomeDistance = model.HomeDistance,
				FoodDistance = model.FoodDistance,
				position = model.Position,
				Strength = model.Strength,
				Confusion = model.Confusion,
				SpecialType = AssignSpecial(model),
			};
		}
	}

	public void RenderThreaded(List<PheromoneModel> pheromones)
	{
		currentSettings.time = Time.time % 1f;
		if (CurrentState == RenderState.IDLE)
		{
			pheroCount = pheromones.Count;
			NativeArray<PheroStruct> input = PheromoneInputBuffer;
			NativeArray<Settings> settings = SettingsNativeArray;
			settings[0] = currentSettings;
			for (int i = 0; i < pheroCount; i++)
			{
				input[i] = PheroStruct.FromModel(pheromones[i]);
			}
			ParallelUpdate job = new ParallelUpdate
			{
				settings = SettingsNativeArray,
				input = input,
				result = JobResult,
			};
			jobHandle = job.Schedule(pheroCount, 1024); //maybe smaller batch size for mobile??
			CurrentState = RenderState.RENDERING;
		}
	}
	public void RenderPheromones(GameOptions options)
	{
		Blocked = false;
		currentSettings.particleSize = options.blurryPheromones ? 0.25f : 0.05f;
		if (CurrentState == RenderState.RENDERING && jobHandle.IsCompleted)
		{
			renderer.material = options.blurryPheromones ? blurryMaterial : solidMaterial;
			if (options.blurryPheromones){
				var am = pheroParticleSystem.textureSheetAnimation;
				am.enabled = false;
			} else {
				var am = pheroParticleSystem.textureSheetAnimation;
				am.enabled = true;
			}
			jobHandle.Complete();
			NativeArray<ParticleSystem.Particle>.Copy(jobResult, 0, jobResultOutput, 0, pheroCount);
			pheroParticleSystem.SetParticles(jobResultOutput, pheroCount);
			CurrentState = RenderState.IDLE;
			//startingIndex += PARTICLES_PER_FRAME;
		}
		else
		{
			if (!jobHandle.IsCompleted)
			{
				Blocked = true;
			}
		}
	}

	private void OnDestroy()
	{
		jobHandle.Complete();
		Dispose();
	}

	public void Dispose()
	{
		jobHandle.Complete();
		if (jobResult.IsCreated)
		{
			((IDisposable)jobResult).Dispose();
		}
		if (nativeInputBuffer.IsCreated)
		{
			((IDisposable)nativeInputBuffer).Dispose();
		}
		if (settingsNativeArray.IsCreated)
		{
			((IDisposable)settingsNativeArray).Dispose();
		}
	}
}

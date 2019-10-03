using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class PheroParticleRender : MonoBehaviour
{
	public const int MAX_PARTICLE_COUNT = 4096;
	public const bool COLORS = true;
	private ParticleSystem.Particle[] particles;
	private readonly PheroStruct[] pheroArr = new PheroStruct[MAX_PARTICLE_COUNT];
	private int particleCount;
	private ParticleSystem pheroParticleSystem;
	private JobHandle jobHandle;
	private NativeArray<PheroStruct> nativePheromones;
	private NativeArray<ParticleSystem.Particle> jobResult;
	private readonly ParticleSystem.Particle[] jobResultOutput = new ParticleSystem.Particle[MAX_PARTICLE_COUNT];
	private bool incompleteFrame = false;

	private enum state
	{
		IDLE,
		QUEUED,
		RENDERING,
	}
	private state currentState = state.IDLE;

	private NativeArray<PheroStruct> NativePheromones
	{
		get
		{
			if (!nativePheromones.IsCreated)
			{
				nativePheromones = new NativeArray<PheroStruct>(MAX_PARTICLE_COUNT, Allocator.Persistent);
			}

			return nativePheromones;
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

	private static void UpdateParticles(PheromoneModel[] pheromones, ParticleSystem.Particle[] target)
	{
		int count = pheromones.Length;
		for (int i = 0; i < count; i++)
		{
			PheromoneModel p = pheromones[i];
			target[i].position = p.Position;
			if (COLORS)
			{
				target[i].startColor = new Color(
					(p.FoodDistance + p.HomeDistance) * .5f,
					p.FoodDistance * 0.1f,
					p.HomeDistance * 0.1f,
					p.Strength * 0.5f);
			}
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
					r = (p.FoodDistance + p.HomeDistance) * .5f,
					g = p.FoodDistance * 0.1f,
					b = p.HomeDistance * 0.1f,
					a = p.Strength * 0.5f
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
		particles = new ParticleSystem.Particle[MAX_PARTICLE_COUNT];
		for (int i = 0; i < MAX_PARTICLE_COUNT; i++)
		{
			ParticleSystem.Particle p = new ParticleSystem.Particle
			{
				startSize = 0.3f,
				remainingLifetime = 5,
				position = Vector3.zero,
				startColor = new Color(1f, 1f, 1f, 0.5f)
			};
			particles[i] = p;
		}

	}

	private struct MyJob : IJob
	{
		[ReadOnly]
		public NativeArray<PheroStruct> input;
		[WriteOnly]
		public NativeArray<ParticleSystem.Particle> result;
		void IJob.Execute()
		{
			int count = input.Length;
			for (int i = 0; i < count; i++)
			{
				PheroStruct p = input[i];
				result[i] = new ParticleSystem.Particle
				{
					position = p.position,
					startColor = new Color
					{
						r = (p.FoodDistance + p.HomeDistance) * .5f,
						g = p.FoodDistance * 0.1f,
						b = p.HomeDistance * 0.1f,
						a = p.Strength * 0.5f
					},
					startSize = 0.3f,
					remainingLifetime = 5,
				};
			}
		}
	}

	private struct PheroStruct
	{
		public Vector2 position;
		public float HomeDistance;
		public float FoodDistance;
		public float Strength;
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PheroStruct FromModel(PheromoneModel model)
		{
			return new PheroStruct
			{
				HomeDistance = model.HomeDistance,
				FoodDistance = model.FoodDistance,
				position = model.Position,
				Strength = model.Strength
			};
		}
	}

	public void PrepareUpdate(List<PheromoneModel> pheromones)
	{
		if (currentState == state.IDLE)
		{
			//jobHandle.Complete();
			particleCount = Mathf.Min(pheromones.Count, MAX_PARTICLE_COUNT);
			for (int i = 0; i < particleCount; i++)
			{
				pheroArr[i] = PheroStruct.FromModel(pheromones[i]);
			}
			NativeArray<PheroStruct>.Copy(pheroArr, 0, NativePheromones, 0, particleCount);
			MyJob job = new MyJob
			{
				input = NativePheromones,
				result = JobResult,
			};
			jobHandle = job.Schedule();
			currentState = state.RENDERING;
		}
	}
	public void RenderPheromones()
	{
		//if (jobHandle.IsCompleted){
		if (currentState == state.RENDERING && jobHandle.IsCompleted)
		{
			jobHandle.Complete();
			incompleteFrame = false;
			JobResult.CopyTo(jobResultOutput);
			pheroParticleSystem.SetParticles(jobResultOutput, particleCount);
			currentState = state.IDLE;
		}
		else
		{
			//render previous?		
		}
	}

	private void OnDestroy()
	{
		jobHandle.Complete();
		if (nativePheromones.IsCreated)
		{
			nativePheromones.Dispose();
		}

		if (jobResult.IsCreated)
		{
			jobResult.Dispose();
		}
	}
}

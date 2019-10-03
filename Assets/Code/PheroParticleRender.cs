using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheroParticleRender : MonoBehaviour
{
	public const int MAX_PARTICLE_COUNT = 4096;
	public const bool COLORS = true;
	private ParticleSystem.Particle[] particles;

	private void UpdateParticles(List<PheromoneModel> pheromones)
	{
		int count = pheromones.Count;
		for (int i = 0; i < count; i++)
		{
			PheromoneModel p = pheromones[i];
			particles[i].position = p.Position;
			if (COLORS)
			{
				particles[i].startColor = new Color(
					(p.FoodDistance + p.HomeDistance) * .5f,
					p.FoodDistance * 0.1f,
					p.HomeDistance * 0.1f,
					0.5f);
			}
		}
	}

	private static ParticleSystem.Particle PheroToParticle(PheromoneModel phero)
	{
		ParticleSystem.Particle p = new ParticleSystem.Particle();
		p.startSize = 0.3f;
		p.remainingLifetime = 5;
		p.position = phero.Position;
		p.startColor = new Color(1f, 1f, 1f, 0.5f);
		return p;
	}

	private ParticleSystem particleSystem;
    // Start is called before the first frame update
    void Start()
    {
		particleSystem = GetComponentInChildren<ParticleSystem>();
		particles = new ParticleSystem.Particle[MAX_PARTICLE_COUNT];
		for (int i = 0; i< MAX_PARTICLE_COUNT; i++){
			ParticleSystem.Particle p = new ParticleSystem.Particle();
			p.startSize = 0.3f;
			p.remainingLifetime = 5;
			p.position = Vector3.zero;
			p.startColor = new Color(1f, 1f, 1f, 0.5f);
			particles[i] = p;
		}

	}

	public void PrepareUpdate(List<PheromoneModel> pheromones)
	{
		UpdateParticles(pheromones);
		particleSystem.SetParticles(particles, pheromones.Count);
	}
	public void RenderPheromones(List<PheromoneModel> pheromones)
	{
		UpdateParticles(pheromones);
		particleSystem.SetParticles(particles, pheromones.Count);
	}
}

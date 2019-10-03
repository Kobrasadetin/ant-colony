using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{

    public class SpatialTest
    {
		public const float MAX_TEST_RANGE = 0.3f;

		private class Search<T> where T : IObjectModel
		{
			public static List<T> ObjectsInRange(List<T> fromList, Vector2 position, float range)
			{
				List<T> list = new List<T>();
				if (fromList.Count == 0)
				{
					return list;
				}
				float treshold = range * range;
				foreach (T model in fromList)
				{
					//TODO space partition
					Vector2 v = position - model.Position;
					float d2 = (v.x * v.x + v.y * v.y);
					if (d2 < treshold)
					{
						list.Add(model);
					}
				}
				return list;
			}
			public static T Nearest(List<T> fromList, Vector2 position)
			{
				if (fromList.Count == 0)
				{
					return default(T);
				}
				T closest = default(T);
				float dist2 = float.MaxValue;
				foreach (T model in fromList)
				{
					Vector2 v = position - model.Position;
					float d2 = (v.x * v.x + v.y * v.y);
					if (d2 < dist2)
					{
						dist2 = d2;
						closest = model;
					}
				}
				return closest;
			}
		}

		// A Test behaves as an ordinary method
		[Test]
        public void SpatialTestSimplePasses()
        {
			System.Random Rand = new System.Random(1234);

			SpacePartitionList<PheromoneModel> pheromonesSP = new SpacePartitionList<PheromoneModel>();
			PheromoneModel p = new PheromoneModel();
			//p.Position = new Vector2((float)Rand.NextDouble()*40-20, (float)Rand.NextDouble() * 40 - 20);
			p.Position = new Vector2(19.9f, 19.9f);
			pheromonesSP.Add(p);
			List<PheromoneModel> result = pheromonesSP.FindInRange(p.Position, MAX_TEST_RANGE);
			Assert.AreEqual(1, result.Count);
		
		}

		[Test]
		public void SpatialTestSimpleCachePasses()
		{
			System.Random Rand = new System.Random(1234);

			SpacePartitionList<PheromoneModel> pheromonesSP = new SpacePartitionList<PheromoneModel>();
			PheromoneModel p = new PheromoneModel();
			//p.Position = new Vector2((float)Rand.NextDouble()*40-20, (float)Rand.NextDouble() * 40 - 20);
			p.Position = new Vector2(19.9f, 19.9f);
			pheromonesSP.Add(p);
			pheromonesSP.FindInRange(p.Position, MAX_TEST_RANGE);
			p = new PheromoneModel();
			p.Position = new Vector2(20.1f, 20.1f);
			List<PheromoneModel> result = pheromonesSP.FindInRange(p.Position, MAX_TEST_RANGE);
			Assert.AreEqual(2, result.Count);

		}

		// Test out of bounds behaviour
		[Test]
		public void SpatialTestOOBPasses()
		{
			SpacePartitionList<PheromoneModel> pheromonesSP = new SpacePartitionList<PheromoneModel>();

			//too small
			PheromoneModel a = new PheromoneModel();
			a.Position = new Vector2(-39.9f, -39.9f);
			pheromonesSP.Add(a);
			List<PheromoneModel> result = pheromonesSP.FindInRange(a.Position, MAX_TEST_RANGE);
			Assert.AreEqual(1, result.Count);

			//too large
			PheromoneModel b = new PheromoneModel();
			b.Position = new Vector2(39.9f, 39.9f);
			pheromonesSP.Add(b);
			result = pheromonesSP.FindInRange(b.Position, MAX_TEST_RANGE);
			Assert.AreEqual(1, result.Count);
		}

		[Test]
		public void SpatialTestComplexePasses()
		{
			System.Random Rand = new System.Random(1234);

			List<PheromoneModel> pheromones = new List<PheromoneModel>();
			SpacePartitionList<PheromoneModel> pheromonesSP = new SpacePartitionList<PheromoneModel>();
			for (int i = 0; i < 2000; i++)
			{
				PheromoneModel p = new PheromoneModel();
				p.Position = new Vector2((float)Rand.NextDouble() * 50 - 25, (float)Rand.NextDouble() * 50 - 25);
				pheromones.Add(p);
				pheromonesSP.Add(p);
			}
			for (int i = 0; i < 200; i++)
			{
				PheromoneModel p = new PheromoneModel();
				Vector2 randomPos = new Vector2((float)Rand.NextDouble() * 50 - 25, (float)Rand.NextDouble() * 50 - 25);
				List<PheromoneModel> inRange = Search<PheromoneModel>.ObjectsInRange(pheromones, randomPos, MAX_TEST_RANGE);
				List<PheromoneModel> result;
				if (i == 5)
				{
					result = pheromonesSP.FindInRange(randomPos, MAX_TEST_RANGE);
				}
				result = pheromonesSP.FindInRange(randomPos, MAX_TEST_RANGE);
				Assert.AreEqual(inRange.Count, result.Count);
				foreach(PheromoneModel model in inRange){
					Assert.That(result.Contains(model));
				}
			}
			for (int i = 0; i < 2000; i++)
			{
				PheromoneModel p = new PheromoneModel();
				p.Position = new Vector2((float)Rand.NextDouble() * 50 - 25, (float)Rand.NextDouble() * 50 - 25);
				pheromones.Add(p);
				pheromonesSP.Add(p);
			}
			for (int i = 0; i < 200; i++)
			{
				PheromoneModel p = new PheromoneModel();
				Vector2 randomPos = new Vector2((float)Rand.NextDouble() * 50 - 25, (float)Rand.NextDouble() * 50 - 25);
				List<PheromoneModel> inRange = Search<PheromoneModel>.ObjectsInRange(pheromones, randomPos, MAX_TEST_RANGE);
				List<PheromoneModel> result;
				if (i == 5)
				{
					result = pheromonesSP.FindInRange(randomPos, MAX_TEST_RANGE);
				}
				result = pheromonesSP.FindInRange(randomPos, MAX_TEST_RANGE);
				Assert.AreEqual(inRange.Count, result.Count);
				foreach (PheromoneModel model in inRange)
				{
					Assert.That(result.Contains(model));
				}
			}
		}

		// A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
		// `yield return null;` to skip a frame.
		/*[UnityTest]
        public IEnumerator SpatialTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }*/
	}
}

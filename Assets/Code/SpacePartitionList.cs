using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpacePartitionList<T> : IEnumerable where T : IObjectModel
{
	private const float X_HALF_SIZE = 20.0f;
	private const float Y_HALF_SIZE = 20.0f;
	private const int X_BUCKETS = 32;
	private const int Y_BUCKETS = 32;
	private const uint OVERFLOW_BITS = 0b1111_1111_1111_1111_1111_1110_0000;
	private const float BUCKET_SIZE_X = X_HALF_SIZE * 2 / X_BUCKETS;
	private const float BUCKET_SIZE_Y = Y_HALF_SIZE * 2 / Y_BUCKETS;
	private const float X_STAGGER = BUCKET_SIZE_X / 2;
	private const float Y_STAGGER = BUCKET_SIZE_Y / 2;
	private const float BS_RECIPROCAL_X = 1 / BUCKET_SIZE_X;
	private const float BS_RECIPROCAL_Y = 1 / BUCKET_SIZE_Y;

	private readonly List<T>[] buckets;
	private List<T> outOfBounds = new List<T>();
	private readonly List<T>[][] grid_buckets;
	private List<T> globalList = new List<T>();

	private HashSet<List<T>> searchBuckets = new HashSet<List<T>>();

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private uint find(float pos, float rangeStart, float stepReciproc, out bool outOfBound)
	{
		//we want (pos - rangeStart) / stepSize;
		outOfBound = false;
		uint result = unchecked((uint)((pos - rangeStart) * stepReciproc));
		if ((result & OVERFLOW_BITS) != 0)
		{
			outOfBound = true;
		}
		return result;
	}

	private bool valid(int x, int y)
	{
		return x >= 0 && x < X_BUCKETS && y >= 0 && y < Y_BUCKETS;
	}

	public SpacePartitionList()
	{
		buckets = new List<T>[X_BUCKETS * Y_BUCKETS];
		for (int i = 0; i < buckets.Length; i++)
		{
			buckets[i] = new List<T>();
		}

		//build grid
		grid_buckets = new List<T>[(X_BUCKETS + 1) * (Y_BUCKETS + 1)][];

		for (int y = 0; y < Y_BUCKETS + 1; y++)
		{
			for (int x = 0; x < X_BUCKETS + 1; x++)
			{
				//grid_buckets[y * (X_BUCKETS + 1) + x];
				List<List<T>> neighbors = new List<List<T>>();
				bool outOfBoundsBucket = false;
				for (int dx = -1; dx < 1; dx++)
				{
					for (int dy = -1; dy < 1; dy++)
					{
						int cx = x + dx;
						int cy = y + dy;
						if (valid(cx, cy))
						{
							neighbors.Add(buckets[cy * X_BUCKETS + cx]);
						}
						else
						{
							outOfBoundsBucket = true;
						}
					}
				}
				if (outOfBoundsBucket)
				{
					neighbors.Add(outOfBounds);
				}
				grid_buckets[y * (X_BUCKETS + 1) + x] = neighbors.ToArray();
			}
		}
	}

	public void Add(T model)
	{
		Vector2 pos = model.Position;
		uint xBucket = find(pos.x, -X_HALF_SIZE, BS_RECIPROCAL_X, out bool outOfBoundsX);
		uint yBucket = find(pos.y, -Y_HALF_SIZE, BS_RECIPROCAL_Y, out bool outOfBoundsY);
		if (outOfBoundsX || outOfBoundsY)
		{
			outOfBounds.Add(model);
		}
		else
		{
			buckets[yBucket * X_BUCKETS + xBucket].Add(model);
		}
		globalList.Add(model);
	}

	public void Remove(T model)
	{
		Vector2 pos = model.Position;
		uint xBucket = find(pos.x, -X_HALF_SIZE, BS_RECIPROCAL_X, out bool outOfBoundsX);
		uint yBucket = find(pos.y, -Y_HALF_SIZE, BS_RECIPROCAL_Y, out bool outOfBoundsY);
		if (outOfBoundsX || outOfBoundsY)
		{
			outOfBounds.Remove(model);
		}
		else
		{
			buckets[yBucket * X_BUCKETS + xBucket].Remove(model);
		}
		globalList.Remove(model);
	}


	public List<T> FindInRange(Vector2 position, float range)
	{
		searchBuckets.Clear();
		List<T> result = new List<T>();
		//TODO implement large ranges
		if (range > Mathf.Min(BUCKET_SIZE_X, BUCKET_SIZE_Y))
		{
			Debug.LogError("Too long range search, max " + BUCKET_SIZE_X + ", " + BUCKET_SIZE_Y);
		}
		uint xGrid = find(position.x + X_STAGGER, -X_HALF_SIZE, BS_RECIPROCAL_X, out bool outOfBoundsX);
		uint yGrid = find(position.y + Y_STAGGER, -Y_HALF_SIZE, BS_RECIPROCAL_Y, out bool outOfBoundsY);

		if (outOfBoundsX || outOfBoundsY)
		{
			searchBuckets.Add(outOfBounds);
		}
		else
		{
			foreach (List<T> l in grid_buckets[yGrid * (X_BUCKETS + 1) + xGrid])
			{
				searchBuckets.Add(l);
			}
		}
		float treshold = range * range;
		foreach (List<T> l in searchBuckets)
		{
			foreach (T model in l)
			{
				Vector2 modelPosition = model.Position;
				float x = position.x - modelPosition.x;
				float y = position.y - modelPosition.y;
				float d2 = (x * x + y * y);
				if (d2 < treshold)
				{
					result.Add(model);
				}
			}

		}
		return result;
	}

	public IEnumerator GetEnumerator()
	{
		return ((IEnumerable)globalList).GetEnumerator();
	}
}

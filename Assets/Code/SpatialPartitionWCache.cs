using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpatialPartitionWCache<T> : IEnumerable where T : IObjectModel
{
	private const float X_HALF_SIZE = 20.0f;
	private const float Y_HALF_SIZE = 20.0f;
	private const int X_BUCKETS = 64;
	private const int Y_BUCKETS = 64;
	private const uint OVERFLOW_BITS = 0b1111_1111_1111_1111_1111_1100_0000;
	private const float BUCKET_SIZE_X = X_HALF_SIZE * 2 / X_BUCKETS;
	private const float BUCKET_SIZE_Y = Y_HALF_SIZE * 2 / Y_BUCKETS;
	private const float X_STAGGER = BUCKET_SIZE_X / 2;
	private const float Y_STAGGER = BUCKET_SIZE_Y / 2;
	private const float BS_RECIPROCAL_X = 1 / BUCKET_SIZE_X;
	private const float BS_RECIPROCAL_Y = 1 / BUCKET_SIZE_Y;
	private const int OOB_BUCKET = X_BUCKETS * Y_BUCKETS;

	private readonly Bucket[] buckets;
	private List<T> returnBuffer = new List<T>();
	private readonly List<int>[] bucket_grid_cache_indexes = new List<int>[X_BUCKETS * Y_BUCKETS + 1];
	private readonly Bucket[][] grid_buckets = new Bucket[(X_BUCKETS + 1) * (Y_BUCKETS + 1)][];
	private readonly List<T>[] grid_cache = new List<T>[(X_BUCKETS + 1) * (Y_BUCKETS + 1)];
	private bool[] grid_cache_valid = new bool[(X_BUCKETS + 1) * (Y_BUCKETS + 1)];
	private List<T> globalList = new List<T>();

	List<T> outOfBoundsCache = new List<T>();

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

	private struct Bucket
	{
		public List<T> val;
		public List<int> gridCacheIndexes;
	}

	private bool valid(int x, int y)
	{
		return x >= 0 && x < X_BUCKETS && y >= 0 && y < Y_BUCKETS;
	}

	public void RemoveAll(System.Predicate<T> predicate)
	{
		List<T> toRemove = globalList.FindAll(predicate);
		globalList.RemoveAll(predicate);
		foreach (T model in toRemove)
		{
			FindBucket(model).val.Remove(model);
		}
	}

	public SpatialPartitionWCache()
	{
		buckets = new Bucket[X_BUCKETS * Y_BUCKETS + 1];
		for (int i = 0; i < buckets.Length; i++)
		{
			buckets[i] = new Bucket
			{
				val = new List<T>(),
				gridCacheIndexes = new List<int>(),
			};
		}

		//build grid

		for (int y = 0; y < Y_BUCKETS + 1; y++)
		{
			for (int x = 0; x < X_BUCKETS + 1; x++)
			{
				//grid_buckets[y * (X_BUCKETS + 1) + x];
				List<Bucket> neighbors = new List<Bucket>();
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
							buckets[cy * X_BUCKETS + cx].gridCacheIndexes.Add(y * (X_BUCKETS + 1) + x);
						}
						else
						{
							neighbors.Add(buckets[OOB_BUCKET]);
							buckets[OOB_BUCKET].gridCacheIndexes.Add(y * (X_BUCKETS + 1) + x);
						}
					}
				}
				int gridIndex = y * (X_BUCKETS + 1) + x;
				grid_buckets[gridIndex] = neighbors.ToArray();
				grid_cache[gridIndex] = new List<T>();
				grid_cache_valid[gridIndex] = true;
			}
		}
	}

	public List<T> AsList()
	{
		return globalList;
	}

	public void Add(T model)
	{
		Bucket bucket = FindBucket(model);
		foreach (int index in bucket.gridCacheIndexes)
		{
			grid_cache_valid[index] = false;
		}
		bucket.val.Add(model);
		globalList.Add(model);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Bucket FindBucket(T model)
	{
		Vector2 pos = model.Position;
		uint xBucket = find(pos.x, -X_HALF_SIZE, BS_RECIPROCAL_X, out bool outOfBoundsX);
		uint yBucket = find(pos.y, -Y_HALF_SIZE, BS_RECIPROCAL_Y, out bool outOfBoundsY);
		if (outOfBoundsX || outOfBoundsY)
		{
			return buckets[OOB_BUCKET];
		}
		else
		{
			return buckets[yBucket * X_BUCKETS + xBucket];
		}
	}

	public void Remove(T model)
	{
		Bucket bucket = FindBucket(model);
		foreach (int index in bucket.gridCacheIndexes)
		{
			grid_cache_valid[index] = false;
		}
		bucket.val.Remove(model);
		globalList.Remove(model);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GridIndex(Vector2 position, out bool outOfBounds)
	{

		int xGrid = (int)((position.x + X_STAGGER + X_HALF_SIZE) * BS_RECIPROCAL_X);
		int yGrid = (int)((position.y + Y_STAGGER + Y_HALF_SIZE) * BS_RECIPROCAL_Y);
		outOfBounds = (xGrid < 0 || xGrid > X_BUCKETS || yGrid < 0 || yGrid > Y_BUCKETS);

		return yGrid * (X_BUCKETS + 1) + xGrid;
	}


	public List<T> FindInRange(Vector2 position, float range)
	{
		//TODO implement large ranges
		if (range > Mathf.Min(BUCKET_SIZE_X / 2, BUCKET_SIZE_Y / 2))
		{
			Debug.LogError("Too long range search, max " + BUCKET_SIZE_X + ", " + BUCKET_SIZE_Y);
		}
		float treshold = range * range;
		bool isOutOfBounds;
		int grid_index = GridIndex(position, out isOutOfBounds);
		returnBuffer.Clear();
		if (isOutOfBounds)
		{
			foreach (T model in buckets[OOB_BUCKET].val)
			{
				Vector2 modelPosition = model.Position;
				float x = position.x - modelPosition.x;
				float y = position.y - modelPosition.y;
				float d2 = (x * x + y * y);
				if (d2 < treshold)
				{
					returnBuffer.Add(model);
				}
			}
			return returnBuffer;
		}
		else
		{
			if (grid_cache_valid[grid_index])
			{
				foreach (T model in grid_cache[grid_index])
				{
					Vector2 modelPosition = model.Position;
					float x = position.x - modelPosition.x;
					float y = position.y - modelPosition.y;
					float d2 = (x * x + y * y);
					if (d2 < treshold)
					{
						returnBuffer.Add(model);
					}
				}
			}
			else
			{
				foreach (Bucket bucket in grid_buckets[grid_index])
				{
					if (bucket.val.Count > 0)
					{
						foreach (T model in bucket.val)
						{
							grid_cache[grid_index].Add(model);
							Vector2 modelPosition = model.Position;
							float x = position.x - modelPosition.x;
							float y = position.y - modelPosition.y;
							float d2 = (x * x + y * y);
							if (d2 < treshold)
							{
								returnBuffer.Add(model);
							}
						}
					}
				}
			}
			grid_cache_valid[grid_index] = true;
		}
		return returnBuffer;
	}

	public IEnumerator GetEnumerator()
	{
		return ((IEnumerable)globalList).GetEnumerator();
	}
}

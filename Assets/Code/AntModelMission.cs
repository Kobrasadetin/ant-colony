using UnityEngine;

public partial class AntModel
{
	public class Mission
	{
		public delegate float EvaluatePhero(PheromoneModel model);
		public EvaluatePhero evaluatePhero;
		public MissionType type;
		public bool SearchFood;
		public bool GoHome;
		internal static float HomeDistance(PheromoneModel model)
		{
			return -model.HomeDistance;
		}
		internal static float FoodDistance(PheromoneModel model)
		{
			return -model.FoodDistance;
		}
		internal static float HomeOrFood(PheromoneModel model)
		{
			return Mathf.Max(HomeDistance(model), FoodDistance(model));
		}
	}

	private static readonly Mission ToGoHome = new Mission
	{
		evaluatePhero = Mission.HomeDistance,
		type = MissionType.GO_HOME,
		SearchFood = false,
		GoHome = true,
	};
	private static Mission ToForage = new Mission
	{
		evaluatePhero = Mission.FoodDistance,
		type = MissionType.FORAGE,
		SearchFood = true,
		GoHome = false,
	};

	private static readonly Mission ToStopStarving = new Mission
	{
		evaluatePhero = Mission.HomeOrFood,
		type = MissionType.FOOD_OR_HOME,
		SearchFood = true,
		GoHome = true,
	};

	private static Mission MissionGoHome()
	{
		return new Mission
		{
			evaluatePhero = Mission.HomeDistance,
			type = MissionType.GO_HOME,
			SearchFood = false,
			GoHome = true,
		};
	}
}

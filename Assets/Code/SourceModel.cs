using UnityEngine;

public class SourceModel : EntityModel
{
	public const float BASE_RADIUS = 0.1f;
	public const float DEFAULT_RADIUS = 0.1f;
	public const float NEWSPAWN_RADIUS = 0.01f;
	public FoodModel FoodModelProto;
	private int spawnsLeft = 20;

	private FoodModel.Type[] buildingBlocks = { FoodModel.Type.BONE, FoodModel.Type.PINE, FoodModel.Type.STICK };
	private FoodModel.Type[] mystery = { 
		FoodModel.Type.DARK_PURPLE, FoodModel.Type.PURPLE,
		FoodModel.Type.LIGHT_PURPLE, FoodModel.Type.RED, FoodModel.Type.BLUE };
	private FoodModel.Type[] poisonous = { FoodModel.Type.WASTE, FoodModel.Type.WASTE2, FoodModel.Type.MUSHROOM };


	public SourceModel(Vector2 position, float nutrition, float poison) : base(DEFAULT_RADIUS)
	{
		FoodModel.Type randomType = (FoodModel.Type)Random.Range(2, 18);	
		FoodModelProto = new FoodModel(
			randomType,
			Vector2.zero,
			DEFAULT_RADIUS,
			1.0f,
			0.0f);
		if (System.Array.Exists(buildingBlocks, x=> x.Equals(randomType)))
		{
			FoodModelProto.NutritionValue = 0;
			FoodModelProto.BuildingBlockValue = 1;
		}
		if (System.Array.Exists(mystery, x => x.Equals(randomType)))
		{
			FoodModelProto.NutritionValue = Random.Range(0f, 2f);
			FoodModelProto.PoisonValue = Random.Range(0f, 2f);
		}
		if (System.Array.Exists(poisonous, x => x.Equals(randomType)))
		{
			FoodModelProto.PoisonValue = 2f;
		}
		Position = position;
	}
	public void update(GameState state)
	{
		FoodModel nearest = state.findNearestFood(Position);
		if (spawnsLeft > 0 && (nearest == null || (nearest.Position - Position).magnitude > NEWSPAWN_RADIUS))
		{
			FoodModel food = new FoodModel(Position, FoodModelProto);
			state.SpawnFood(food);
			spawnsLeft--;
		}
	}
	public bool IsExhausted(){
		return spawnsLeft <= 0;
	}
}

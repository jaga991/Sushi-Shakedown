using UnityEngine;

public class FoodManager : Singleton<FoodManager>
{
    public GameObject foodPrefab;
    Sprite[] sprites;

    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Food");
    }

    public Food GetRandomFood()
    {
        int randomIndex = Random.Range(0, sprites.Length);
        Sprite randomSprite = sprites[randomIndex];

        GameObject foodGO = Instantiate(foodPrefab, Vector3.zero, Quaternion.identity);
        Food foodComponent = foodGO.GetComponent<Food>();

        foodComponent.spriteRenderer.sprite = randomSprite;
        foodComponent.foodName = randomSprite.name;

        return foodComponent;
    }
}

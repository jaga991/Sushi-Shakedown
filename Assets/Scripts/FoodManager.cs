using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public GameObject foodPrefab;
    Sprite[] sprites;

    void Start()
    {
        sprites = Resources.LoadAll<Sprite>("Food");
    }

    public Food GetRandomFood(Vector3 spawnPos)
    {
        int randomIndex = Random.Range(0, sprites.Length);
        Sprite randomSprite = sprites[randomIndex];

        GameObject foodGO = Instantiate(foodPrefab, spawnPos, Quaternion.identity);
        Food foodComponent = foodGO.GetComponent<Food>();

        foodComponent.spriteRenderer.sprite = randomSprite;
        foodComponent.foodName = randomSprite.name;

        return foodComponent;
    }
}

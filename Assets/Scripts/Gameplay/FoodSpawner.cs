using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private Snake snake;

    [SerializeField] private int minIndex = -4;  
    [SerializeField] private int maxIndex = 3;
    private GameObject currentFood;
    public static FoodSpawner Instance;
     void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        SpawnFood();
    }

    public void SpawnFood()
    {
        if (currentFood != null)
            Destroy(currentFood);

        List<Vector3> occupied = snake.GetOccupiedPositions();

        Vector3 spawnPos;
        int safety = 0;

        do
        {
            int xIndex = Random.Range(minIndex, maxIndex + 1);
            int yIndex = Random.Range(minIndex, maxIndex + 1);

            spawnPos = new Vector3(xIndex + 0.25f, yIndex + 0.25f, 0);

            safety++;
            if (safety > 200)
            {
                Debug.LogWarning("No free space available.");
                return;
            }

        } while (occupied.Contains(spawnPos));

        currentFood = Instantiate(foodPrefab, spawnPos, Quaternion.identity);
    }
}

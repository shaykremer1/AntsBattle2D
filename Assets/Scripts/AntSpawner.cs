using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AntSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Transform[] spawnPoints; // נקודות Spawn לנמלים
    public Transform[] foodSpawnPoints; // נקודות Spawn לפירות
    public GameObject antPrefab;    // הפריפאב של הנמלה
    public GameObject[] foodPrefabs; // מערך פריפאבים של פירות

    [Header("Player Colors")]
    public Color[] playerColors = new Color[3] { Color.red, Color.blue, Color.green }; // צבעים שונים לנמלים

    [Header("Scene Settings")]
    public string nextLevelSceneName; // שם הסצנה הבאה

    public static List<int> remainingPlayers = new List<int>();

    // מיפוי של מקשים לפי שחקן
    private Dictionary<int, (KeyCode forward, KeyCode Backward, KeyCode left, KeyCode right, KeyCode pickup, KeyCode balanceRight, KeyCode balanceLeft)> playerKeys =
        new Dictionary<int, (KeyCode, KeyCode, KeyCode, KeyCode, KeyCode, KeyCode, KeyCode)>()
        {
            { 1, (KeyCode.W,KeyCode.S, KeyCode.A, KeyCode.D, KeyCode.G, KeyCode.R, KeyCode.E) },
            { 2, (KeyCode.UpArrow,KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.RightShift, KeyCode.O, KeyCode.P) },
            { 3, (KeyCode.I,KeyCode.K, KeyCode.J, KeyCode.L, KeyCode.K, KeyCode.U, KeyCode.Y) }
        };

    void Start()
    {
        if (remainingPlayers.Count == 0)
        {
            remainingPlayers = new List<int> { 1, 2, 3 }; // ברירת מחדל - שלושה שחקנים
        }

        SpawnRemainingAnts();
        SpawnFoodForPlayers();
    }

    void SpawnRemainingAnts()
    {
        int spawnIndex = 0;

        // Find the Canvas in the scene
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the scene!");
            return;
        }

        foreach (int playerID in remainingPlayers)
        {
            if (spawnIndex >= spawnPoints.Length)
            {
                Debug.LogWarning("Not enough spawn points for ants!");
                break;
            }

            Transform spawnPoint = spawnPoints[spawnIndex];
            spawnIndex++;

            // יצירת נמלה חדשה
            GameObject newAnt = Instantiate(antPrefab, spawnPoint.position, Quaternion.identity);
            // Set the parent of the instantiated ant to be the Canvas
            newAnt.transform.SetParent(canvas.transform, true);  // false keeps the local position and scale

            // הגדרת צבע לפי שחקן
            SpriteRenderer renderer = newAnt.GetComponent<SpriteRenderer>();
            if (renderer != null && playerID <= playerColors.Length)
            {
                renderer.color = playerColors[playerID - 1];
            }

            AntMovement antMovement = newAnt.GetComponent<AntMovement>();
            if (antMovement != null)
            {
                antMovement.playerID = playerID;

                if (playerKeys.ContainsKey(playerID))
                {
                    var keys = playerKeys[playerID];
                    antMovement.moveForwardKey = keys.forward;
                    antMovement.moveBackwardKey = keys.Backward;
                    antMovement.rotateLeftKey = keys.left;
                    antMovement.rotateRightKey = keys.right;
                    antMovement.pickupKey = keys.pickup;
                    antMovement.balanceRightKey = keys.balanceRight;
                    antMovement.balanceLeftKey = keys.balanceLeft;
                }
            }
        }
    }

    void SpawnFoodForPlayers()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas not found in the scene!");
            return;
        }

        for (int i = 0; i < remainingPlayers.Count; i++)
        {
            if (i >= foodSpawnPoints.Length || i >= foodPrefabs.Length)
            {
                Debug.LogWarning("Not enough food spawn points or food prefabs!");
                break;
            }

            Transform foodSpawnPoint = foodSpawnPoints[i];
            GameObject food = Instantiate(foodPrefabs[i], foodSpawnPoint.position, Quaternion.identity);
            food.transform.SetParent(canvas.transform, true);  // false keeps the local position and scale
            FoodCarry foodCarry = food.GetComponent<FoodCarry>();

            if (foodCarry != null)
            {
                foodCarry.playerID = remainingPlayers[i];
                Debug.Log("Spawned food for Player " + remainingPlayers[i]);
            }
        }
    }

    public static void RemoveLosingPlayer(int losingPlayerID)
    {
        if (remainingPlayers.Contains(losingPlayerID))
        {
            remainingPlayers.Remove(losingPlayerID);
            Debug.Log("Player " + losingPlayerID + " has been removed.");
        }
    }

    public static void LoadNextLevelStatic()
    {
        AntSpawner spawner = FindObjectOfType<AntSpawner>();
        if (spawner != null)
        {
            spawner.LoadNextLevel();
        }
        else
        {
            Debug.LogError("AntSpawner not found in the scene.");
        }
    }

    private void LoadNextLevel()
    {
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            Debug.Log("Loading next level: " + nextLevelSceneName);
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            Debug.LogWarning("Next level scene name not set in Inspector!");
        }
    }
}

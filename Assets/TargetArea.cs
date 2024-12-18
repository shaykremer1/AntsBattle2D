using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetArea : MonoBehaviour
{
    private static List<int> playersWhoDelivered = new List<int>(); // רשימה של השחקנים שהביאו את האוכל

    //wtf
    //test212213
    // כאשר האוכל נכנס לאזור המטרה
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food")) // בודק שהקוליידר שייך לאוכל
        {
            FoodCarry food = collision.GetComponent<FoodCarry>();
            if (food != null)
            {
                int playerID = food.playerID; // קבלת ה-ID של השחקן שהביא את הפרי
                Debug.Log($"Player {playerID} has delivered their food!");

                HandleFoodDelivery(playerID, food);
            }
        }
    }

    // טיפול בהבאת האוכל לאזור המטרה
    private void HandleFoodDelivery(int playerID, FoodCarry food)
    {
        // הוספת השחקן שהביא את האוכל לרשימה (אם הוא לא קיים כבר)
        if (!playersWhoDelivered.Contains(playerID))
        {
            playersWhoDelivered.Add(playerID);
        }

        // השמדת האוכל
        Destroy(food.gameObject);

        // קריאה לבדיקת מצב המשחק לאחר ההשמדה
        Invoke(nameof(CheckFoodCountAfterDestroy), 0.1f);
    }

    // בדיקה של מצב הפירות לאחר השמדה
    private void CheckFoodCountAfterDestroy()
    {
        Debug.Log("Remaining Food Count: " + FoodCarry.totalFoodCount);

        // אם נשארו רק שני שחקנים (במשחקים עם 2 שחקנים בלבד)
        if (AntSpawner.remainingPlayers.Count == 2)
        {
            int winnerID = playersWhoDelivered[0]; // השחקן הראשון שהביא את הפרי הוא המנצח
            Debug.Log($"Game Over! Player {winnerID} Wins!");

            AntSpawner.remainingPlayers.Clear(); // ניקוי הרשימה כדי לסיים את המשחק
        }
        // אם נשאר רק פרי אחד - יש למצוא את המפסיד
        else if (FoodCarry.totalFoodCount <= 1)
        {
            int losingPlayerID = FindLosingPlayer();
            if (losingPlayerID != -1)
            {
                Debug.Log($"Player {losingPlayerID} Lost!");
                AntSpawner.RemoveLosingPlayer(losingPlayerID);
            }

            // טעינת השלב הבא
            playersWhoDelivered.Clear(); // איפוס הרשימה
            AntSpawner.LoadNextLevelStatic();
        }
    }

    // מציאת השחקן שהפסיד (הפרי האחרון שייך לו)
    private int FindLosingPlayer()
    {
        foreach (int playerID in AntSpawner.remainingPlayers)
        {
            if (!playersWhoDelivered.Contains(playerID))
            {
                return playerID; // השחקן שלא הביא את הפרי שלו הוא המפסיד
            }
        }
        return -1; // לא אמור לקרות, מצב בטיחות
    }
}

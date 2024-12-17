using System.Collections;
using UnityEngine;

public class FoodCarry : MonoBehaviour
{
    [Header("Food Settings")]
    public int playerID; // מזהה השחקן שהאוכל שייך לו
    public static int totalFoodCount = 0; // ספירת כל הפירות במשחק

    private Transform leadAnt; // הנמלה המובילה שנושאת את האוכל
    private Transform secondAnt; // נמלה שנייה שמסייעת לנשיאה
    private bool isCarried = false; // האם האוכל מורם כרגע
    private Vector3 lastStablePosition; // המיקום היציב האחרון של האוכל
    private float balanceOffset = 0f; // חוסר האיזון של האוכל
    private float previousRotation = 0f; // הזווית הקודמת של הנמלה לניהול איזון

    [Header("Balance Settings")]
    public float maxBalanceOffset = 1.5f; // חוסר איזון מקסימלי לפני נפילה
    public float balanceSpeed = 1.5f; // מהירות האיזון של הנמלה הראשונה
    public float balanceSpeedWithSecondAnt = 0.5f; // איזון איטי יותר עם נמלה שנייה
    public float rotationImpact = 0.03f; // השפעת סיבוב הנמלה על האיזון
    public float tiltSpeed = 0.5f; // מהירות נטייה הדרגתית של האוכל
    public float rotationSpeed = 60f; // מהירות הסיבוב של האוכל

    [Header("Speed Settings")]
    public float speedReductionFactor = 0.5f; // הפחתת מהירות לנמלה הראשונה
    public float speedBoostWithSecondAnt = 1.2f; // תוספת מהירות עם נמלה שנייה

    void Start()
    {
        totalFoodCount++; // הגדלת הספירה של הפירות בתחילת המשחק
        lastStablePosition = transform.position; // שמירת המיקום ההתחלתי כמיקום יציב
    }

    void Update()
    {
        if (isCarried)
        {
            HandleCarriedMovement();
            HandleBalance();
        }
    }

    private void HandleCarriedMovement()
    {
        if (leadAnt != null)
        {
            transform.position = leadAnt.position + Vector3.up * 0.3f; // מיקום האוכל על גב הנמלה
        }
    }

    public bool HasSecondAnt()
    {
        return secondAnt != null; // האם יש נמלה שנייה
    }

    public bool IsSecondAnt(Transform ant)
    {
        return secondAnt == ant; // האם האובייקט שנשלח הוא הנמלה השנייה
    }
    private void HandleBalance()
    {
        if (leadAnt != null)
        {
            AntMovement leadAntMovement = leadAnt.GetComponent<AntMovement>();
            float currentBalanceSpeed = (secondAnt != null) ? balanceSpeedWithSecondAnt : balanceSpeed;

            // שליטה ידנית באיזון
            if (leadAntMovement.IsBalancingRight()) balanceOffset += currentBalanceSpeed * Time.deltaTime;
            if (leadAntMovement.IsBalancingLeft()) balanceOffset -= currentBalanceSpeed * Time.deltaTime;

            // השפעת הסיבוב של הנמלה
            float currentRotation = leadAnt.eulerAngles.z;
            float rotationChange = Mathf.DeltaAngle(previousRotation, currentRotation);
            balanceOffset += rotationChange * rotationImpact;
            previousRotation = currentRotation;

            // נטייה הדרגתית של האוכל
            balanceOffset = Mathf.Lerp(balanceOffset, balanceOffset + Mathf.Sign(balanceOffset) * tiltSpeed, Time.deltaTime);

            // עדכון סיבוב האוכל
            transform.rotation = Quaternion.Euler(0, 0, -balanceOffset * rotationSpeed);

            // אם האיזון חורג מהגבול המותר, האוכל נופל
            if (Mathf.Abs(balanceOffset) > maxBalanceOffset)
            {
                DropFood(false);
                Debug.Log("Food fell due to imbalance!");
            }
        }
    }

    public void PickUpFood(Transform ant, int antPlayerID)
    {
        if (!isCarried)
        {
            leadAnt = ant;
            isCarried = true;
            balanceOffset = 0f;
            previousRotation = leadAnt.eulerAngles.z;

            transform.position = leadAnt.position + Vector3.up * 0.3f;
            transform.rotation = Quaternion.identity;

            AntMovement antMovement = leadAnt.GetComponent<AntMovement>();
            antMovement.SetMoveSpeed(antMovement.moveSpeed * speedReductionFactor);
            Debug.Log($"Player {antPlayerID} picked up food of Player {playerID}");
        }
        else
        {
            DropFood(true);
        }
    }

    public void DropFood(bool manuallyPlaced)
    {
        isCarried = false;

        if (leadAnt != null)
        {
            AntMovement antMovement = leadAnt.GetComponent<AntMovement>();
            antMovement.ResetSpeed();
        }

        leadAnt = null;
        secondAnt = null;
        balanceOffset = 0f;

        if (manuallyPlaced)
        {
            lastStablePosition = transform.position; // עדכון מיקום יציב חדש
            Debug.Log("Food placed manually!");
        }
        else
        {
            transform.position = lastStablePosition; // חזרה למיקום היציב האחרון
            Debug.Log("Food fell!");
        }

        transform.rotation = Quaternion.identity;
    }

    public void JoinSecondAnt(Transform secondAntTransform)
    {
        if (secondAnt == null)
        {
            secondAnt = secondAntTransform;
            AntMovement leadAntMovement = leadAnt.GetComponent<AntMovement>();

            // הגדלת מהירות עם נמלה שנייה
            leadAntMovement.SetMoveSpeed(leadAntMovement.moveSpeed * speedBoostWithSecondAnt);
            Debug.Log("Second ant joined to help carry the food!");
        }
    }

    void OnDestroy()
    {
        totalFoodCount--; // עדכון הספירה כשפרי מושמד
        Debug.Log("Food destroyed! Remaining Food Count: " + totalFoodCount);
    }
}

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
    private bool isRepositioned = false; // האם מיקום הפרי הותאם לנמלה השנייה

    [Header("Position Settings")]
    public float fruitHeightOffset = 0.3f; // הטיה קלה למעלה

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
    public float heightValue = 0.5f;

    [Header("Rotation Settings")]
    public float foodRotationImpact = 0.03f; // שליטה בכמות התזוזה של האוכל בעת סיבוב הנמלים
    public float secondAntRotationLerpSpeed = 5f; // מהירות סיבוב חלקה של הנמלה השנייה (כמו אוטובוס אקורדיון)

    void Start()
    {
        totalFoodCount++; // הגדלת הספירה של הפירות בתחילת המשחק
        lastStablePosition = transform.position; // שמירת המיקום ההתחלתי כמיקום יציב
    }

    void Update()
    {
        if (isCarried)
        {
            HandleCarriedMovement(); // תזוזת האוכל
            HandleBalance();         // טיפול באיזון
        }

    }
    private void HandleCarriedMovement()
    {
        if (leadAnt != null)
        {
            if (secondAnt != null)
            {
                // הנמלה השנייה עוקבת אחרי הנמלה הראשונה
                Vector3 followPosition = leadAnt.position - leadAnt.up * 0.5f;
                secondAnt.position = Vector3.Lerp(secondAnt.position, followPosition, Time.deltaTime * 10f);

                // סיבוב חלק של הנמלה השנייה כמו אוטובוס אקורדיון
                Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, leadAnt.up);
                secondAnt.rotation = Quaternion.Lerp(secondAnt.rotation, targetRotation, Time.deltaTime * secondAntRotationLerpSpeed);

                // מיקום הפרי בין שתי הנמלים
                transform.position = (leadAnt.position + secondAnt.position) / 2 + Vector3.up * fruitHeightOffset;
            }
            else
            {
                // מיקום רגיל אם יש רק נמלה אחת
                transform.position = leadAnt.position + Vector3.up * 0.3f;
            }
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
        AntMovement balancingAnt = null;

        if (secondAnt != null) // אם יש נמלה שנייה, היא שולטת על האיזון
        {
            balancingAnt = secondAnt.GetComponent<AntMovement>();
        }
        else if (leadAnt != null) // אחרת, הנמלה הראשונה שולטת
        {
            balancingAnt = leadAnt.GetComponent<AntMovement>();
        }

        if (balancingAnt != null)
        {
            // שליטה ידנית באיזון
            if (balancingAnt.IsBalancingRight())
                balanceOffset += (balanceSpeed * 0.5f) * Time.deltaTime;
            if (balancingAnt.IsBalancingLeft())
                balanceOffset -= (balanceSpeed * 0.5f) * Time.deltaTime;
        }

        // כוח גרביטציה מדומה – נופל מהר יותר לצד הנוטה
        balanceOffset = Mathf.Lerp(balanceOffset, balanceOffset + Mathf.Sign(balanceOffset) * tiltSpeed, Time.deltaTime);

        // השפעת סיבוב הנמלה המובילה על האיזון (כעת עם שליטה)
        float currentRotation = leadAnt.eulerAngles.z;
        float rotationChange = Mathf.DeltaAngle(previousRotation, currentRotation);
        balanceOffset += rotationChange * foodRotationImpact;

        previousRotation = currentRotation;

        // עדכון סיבוב האוכל
        transform.rotation = Quaternion.Euler(0, 0, -balanceOffset * rotationSpeed);

        // בדיקת חריגה מהאיזון
        if (Mathf.Abs(balanceOffset) > maxBalanceOffset)
        {
            DropFood(false);
            Debug.Log("Food fell due to imbalance!");
        }
    }




    public void PickUpFood(Transform ant, int antPlayerID)
    {
        if (!isCarried)
        {
            // אם אין מובילה, זו הנמלה המובילה
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
        else if (leadAnt != null && secondAnt == null)
        {
            // אם יש כבר מובילה, הנמלה השנייה מצטרפת
            JoinSecondAnt(ant);
        }
    }

    public void DropFood(bool manuallyPlaced)
    {
        isCarried = false;

        if (leadAnt != null)
        {
            AntMovement leadMovement = leadAnt.GetComponent<AntMovement>();
            leadMovement.ResetSpeed();
            leadAnt = null;
        }

        if (secondAnt != null)
        {
            secondAnt.SetParent(null);
            AntMovement secondMovement = secondAnt.GetComponent<AntMovement>();
            secondMovement.UnlockMovement();
            secondAnt = null;
        }

        balanceOffset = 0f;
        previousRotation = 0f;

        if (manuallyPlaced)
        {
            lastStablePosition = transform.position;
            Debug.Log("Food placed manually.");
        }
        else
        {
            transform.position = lastStablePosition;
            Debug.Log("Food fell and reset.");
        }

        transform.rotation = Quaternion.identity;
    }


    public void JoinSecondAnt(Transform secondAntTransform)
    {
        if (secondAnt == null && leadAnt != null)
        {
            secondAnt = secondAntTransform;

            // חיבור הנמלה השנייה מאחורי הנמלה הראשונה
            Vector3 secondAntPosition = leadAnt.position - leadAnt.up * 0.5f;
            secondAnt.position = secondAntPosition;

            // סיבוב הנמלה השנייה בהתאם לנמלה הראשונה
            secondAnt.rotation = leadAnt.rotation;

            // מיקום הפרי באמצע בין שתי הנמלים
            Vector3 midPoint = (leadAnt.position + secondAnt.position) / 2;
            transform.position = midPoint + Vector3.up * fruitHeightOffset;

            // איפוס האיזון כאשר הנמלה השנייה מצטרפת
            balanceOffset = 0f;

            Debug.Log("Second ant joined successfully.");
        }
    }




    void OnDestroy()
    {
        totalFoodCount--; // עדכון הספירה כשפרי מושמד
        Debug.Log("Food destroyed! Remaining Food Count: " + totalFoodCount);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// fffsafsafsa
/// </summary>
public class AntMovement : MonoBehaviour
{
    [Header("Player Settings")]
    public int playerID;

    [Header("Input Settings")]
    public KeyCode moveForwardKey = KeyCode.W;
    public KeyCode rotateLeftKey = KeyCode.A;
    public KeyCode rotateRightKey = KeyCode.D;
    public KeyCode moveBackwardKey = KeyCode.S;
    public KeyCode pickupKey = KeyCode.Space;
    public KeyCode balanceRightKey = KeyCode.R;
    public KeyCode balanceLeftKey = KeyCode.E;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 200f;

    [Header("Animation")]
    [SerializeField]
    private Animator _animator;

    private FoodCarry currentFood;
    private float defaultSpeed;
    private bool isLocked = false;

    void Start()
    {
        defaultSpeed = moveSpeed;
    }

    void Update()
    {
        if (!isLocked)
        {
            HandleMovement();
            HandleRotation();
        }

        HandlePickupOrDrop();
        LockMovementIfHelping();
    }

    private void HandleMovement()
    {
        if (Input.GetKey(moveForwardKey))
        {
            Vector3 movement = transform.up * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);
            _animator.SetFloat("Direction", 1f);
            _animator.SetBool("IsWalking", true);
            Debug.Log("walking Forward!");
        }
        else
        {
            if (Input.GetKeyUp(moveForwardKey))
            {
                Debug.Log("STOPPED!");
                _animator.SetBool("IsWalking", false);
            }
        }
        if (Input.GetKey(moveBackwardKey))
        {
            Debug.Log("walking Back!");
            Vector3 movement = -0.5f * transform.up * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);
            _animator.SetFloat("Direction", -1f);
            _animator.SetBool("IsWalking", true);
        }
        else
        {
            if (Input.GetKeyUp(moveBackwardKey))
            {
                Debug.Log("STOPPED!");
                _animator.SetBool("IsWalking", false);
            }
        }


    }

    private void HandleRotation()
    {
        if (Input.GetKey(rotateLeftKey))
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(rotateRightKey))
        {
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }
    }

    private void HandlePickupOrDrop()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            if (currentFood == null)
            {
                TryPickupFood();
            }
            else
            {
                currentFood.DropFood(transform);
                currentFood = null;
                ResetSpeed();
            }
        }
    }

    private void TryPickupFood()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var collider in colliders)
        {
            FoodCarry food = collider.GetComponent<FoodCarry>();
            if (food != null)
            {
                food.PickUpFood(transform, playerID);
                currentFood = food;
                break;
            }
        }
    }

    public bool IsBalancingRight()
    {
        return Input.GetKey(balanceRightKey);
    }

    public bool IsBalancingLeft()
    {
        return Input.GetKey(balanceLeftKey);
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void ResetSpeed()
    {
        moveSpeed = defaultSpeed;
    }

    public void LockMovement()
    {
        isLocked = true;
    }

    public void UnlockMovement()
    {
        isLocked = false;
    }
    public void LockMovementIfHelping()
    {
        if (currentFood != null && currentFood.HasSecondAnt() && currentFood.IsSecondAnt(transform))
        {
            LockMovement(); // הנמלה השנייה לא יכולה לזוז
        }
    }
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerController : MonoBehaviour
//{
//    private Vector2Int targetPosition;

//    private void Awake()
//    {
//        // Set this so we don't wander off at the start
//        targetPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
//        transform.position = (Vector2)targetPosition;
//    }

//    private void Update()
//    {
//        var moving = (Vector2)transform.position != targetPosition;

//        if (moving)
//        {
//            MoveTowardsTargetPosition();
//        }
//        else
//        {
//            SetNewTargetPositionFromInput();
//        }
//    }

//    private void MoveTowardsTargetPosition()
//    {
//        transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
//    }

//    private void SetNewTargetPositionFromInput()
//    {
//        if (Input.GetKey(KeyCode.W))
//        {
//            targetPosition += Vector2Int.up;
//        }
//        else if ...
//    }
//}

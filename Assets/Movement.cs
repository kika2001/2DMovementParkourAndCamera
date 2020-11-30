using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider2D))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private BoxCollider2D collider2d;

    private Vector3 startScale;
    [SerializeField] private Vector2 maxVelocity2D;
    [SerializeField] private float stepSpeed;
    private bool canJump;
    [SerializeField] private float jumpForce;


    [SerializeField] private LayerMask walkable;

    [Space] [Header("Ground Checkers")] [SerializeField]
    private float sensorYOffset;

    [SerializeField] private float sensorYDistance;
    [SerializeField] private float sensorXThicknessReducer;

    [Space] [Header("WallCheckers")] [SerializeField]
    private float sensorXOffset;

    [SerializeField] private float sensorXDistance;

    [Header("Downwards Checker")] [SerializeField]
    private float downwardsEdgeXOffset;

    [SerializeField] private float downwardsEdgeYOffset;
    [SerializeField] private float downwardsEdgeDistance;

    private RaycastHit2D downwardsEdgeCheck;
    private RaycastHit2D groundCheck;
    private RaycastHit2D fowardCheck;
    private RaycastHit2D backCheck;

    private bool edgeClose;
    private Vector3 edgepos;

    private WalkingState playerState;
    private WallState playerWallState;

    enum WalkingState
    {
        Grounded,
        Air
    }

    enum WallState
    {
        None,
        FrontWall,
        BackWall
    }


    private void Awake()
    {
        startScale = transform.localScale;
        rb2d = GetComponent<Rigidbody2D>();
        collider2d = GetComponent<BoxCollider2D>();
    }


    private void OnDrawGizmos()
    {
        #region GizmosCheckersRegionFloorWall

        /*
         * groundCheck = Physics2D.BoxCast(
                new Vector2(transform.position.x,
                    transform.position.y - (sensorYDistance / 2) -
                    (transform.GetComponent<SpriteRenderer>().size.y / 2 * transform.localScale.y) - sensorYOffset),
                new Vector2(
                    (transform.GetComponent<SpriteRenderer>().size.x * transform.localScale.x) -
                    sensorXThicknessReducer, sensorYDistance),
                0,
                Vector2.down, 0, walkable);
         */
        if (transform.localScale.x > 0)
        {
            //GroundChecker
            /*
             * Gizmos.DrawWireCube(
                new Vector3(transform.position.x,
                    transform.position.y - (sensorYDistance / 2) -
                    (transform.GetComponent<BoxCollider2D>().size.y / 2 * transform.localScale.y) - sensorYOffset, 0),
                new Vector2(
                    (transform.GetComponent<BoxCollider2D>().size.x * transform.localScale.x) - sensorXThicknessReducer,
                    sensorYDistance)
            );

             */
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector2(transform.position.x,
                    transform.position.y - (sensorYDistance / 2) -
                    (transform.GetComponent<SpriteRenderer>().size.y / 2 * transform.localScale.y) - sensorYOffset),
                new Vector2(
                    (transform.GetComponent<SpriteRenderer>().size.x * transform.localScale.x) -
                    sensorXThicknessReducer, sensorYDistance));
            //FowardChecker
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(
                new Vector2(
                    transform.position.x +
                    (transform.GetComponent<BoxCollider2D>().size.x / 2) * transform.localScale.x + sensorXOffset,
                    transform.position.y),
                transform.right * sensorXDistance);
            //BackCheker
            Gizmos.color = Color.green;
            Gizmos.DrawRay(
                new Vector2(
                    transform.position.x -
                    (transform.GetComponent<BoxCollider2D>().size.x / 2) * transform.localScale.x - sensorXOffset,
                    transform.position.y),
                -transform.right * sensorXDistance);
        }
        else if (transform.localScale.x < 0)
        {
            //GroundChecker
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector2(transform.position.x,
                    transform.position.y - (sensorYDistance / 2) -
                    (transform.GetComponent<SpriteRenderer>().size.y / 2 * transform.localScale.y) - sensorYOffset),
                new Vector2(
                    (transform.GetComponent<SpriteRenderer>().size.x * transform.localScale.x) +
                    sensorXThicknessReducer, sensorYDistance));

            //FowardChecker
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(
                new Vector2(
                    transform.position.x +
                    (transform.GetComponent<BoxCollider2D>().size.x / 2) * transform.localScale.x - sensorXOffset,
                    transform.position.y),
                -transform.right * sensorXDistance);
            //BackCheker
            Gizmos.color = Color.green;
            Gizmos.DrawRay(
                new Vector2(
                    transform.position.x -
                    (transform.GetComponent<BoxCollider2D>().size.x / 2) * transform.localScale.x + sensorXOffset,
                    transform.position.y),
                transform.right * sensorXDistance);
        }

        #endregion

        #region GizmosCheckersEdge

        if (transform.localScale.x > 0)
        {
            //DownwardsPosition ray
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(
                new Vector3(
                    transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x +
                    downwardsEdgeXOffset,
                    transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
                    downwardsEdgeYOffset,
                    0),
                new Vector3(.2f, .2f, .2f)
            );
            //DownwardsRayDown
            Gizmos.DrawLine(new Vector3(
                transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x +
                downwardsEdgeXOffset,
                transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
                downwardsEdgeYOffset,
                0), new Vector3(
                transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x +
                downwardsEdgeXOffset,
                transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
                downwardsEdgeYOffset,
                0) + Vector3.down * downwardsEdgeDistance);
        }
        else if (transform.localScale.x < 0)
        {
            //DownwardsPosition ray
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(
                new Vector3(
                    transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x -
                    downwardsEdgeXOffset,
                    transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
                    downwardsEdgeYOffset,
                    0),
                new Vector3(.2f, .2f, .2f)
            );
            //DownwardsRayDown
            Gizmos.DrawLine(new Vector3(
                transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x -
                downwardsEdgeXOffset,
                transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
                downwardsEdgeYOffset,
                0), new Vector3(
                transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x -
                downwardsEdgeXOffset,
                transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
                downwardsEdgeYOffset,
                0) + Vector3.down * downwardsEdgeDistance);
        }

        //Edge
        if (edgeClose)
        {
            Gizmos.DrawWireCube(
                edgepos,
                new Vector3(.2f, .2f, .2f)
            );
        }

        //Foward Edge Ray
        /*
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Vector3(transform.position.x +(transform.GetComponent<BoxCollider2D>().size.x/2 *transform.localScale.x +fowardEdgeXOffset),
            transform.position.y +(transform.GetComponent<BoxCollider2D>().size.y/2 *transform.localScale.y +fowardEdgeYOffset),
            0),Vector3.right);
        */

        #endregion
    }


    private void FixedUpdate()
    {
        CheckersAndEdge();
    }

    void CheckersAndEdge()
    {
        #region CheckEdge

        if (transform.localScale.x > 0)
        {
            downwardsEdgeCheck = Physics2D.Raycast(new Vector3(
                transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x +
                downwardsEdgeXOffset,
                transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
                downwardsEdgeYOffset,
                0), Vector2.down, downwardsEdgeDistance);
        }
        else if (transform.localScale.x < 0)
        {
            downwardsEdgeCheck = Physics2D.Raycast(new Vector3(
                transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x -
                downwardsEdgeXOffset,
                transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
                downwardsEdgeYOffset,
                0), Vector2.down, downwardsEdgeDistance);
        }

        #endregion

        #region CheckersRegionFloorWall

        if (transform.localScale.x > 0)
        {
            groundCheck = Physics2D.BoxCast(
                new Vector2(transform.position.x,
                    transform.position.y - (sensorYDistance / 2) -
                    (transform.GetComponent<SpriteRenderer>().size.y / 2 * transform.localScale.y) - sensorYOffset),
                new Vector2(
                    (transform.GetComponent<SpriteRenderer>().size.x * transform.localScale.x) -
                    sensorXThicknessReducer, sensorYDistance),
                0,
                Vector2.down, 0, walkable);

            if (groundCheck.collider != null)
            {
                Debug.Log($"Floor name: {groundCheck.collider.name}");
            }

            fowardCheck = Physics2D.Raycast(
                new Vector2(
                    transform.position.x +
                    (transform.GetComponent<SpriteRenderer>().size.x / 2) * transform.localScale.x + sensorXOffset,
                    transform.position.y),
                Vector3.right, sensorXDistance, walkable);
            backCheck = Physics2D.Raycast(
                new Vector2(
                    transform.position.x -
                    (transform.GetComponent<SpriteRenderer>().size.x / 2) * transform.localScale.x - sensorXOffset,
                    transform.position.y),
                Vector3.left, sensorXDistance, walkable);
        }
        else if (transform.localScale.x < 0)
        {
            groundCheck = Physics2D.BoxCast(
                new Vector2(transform.position.x,
                    transform.position.y - (sensorYDistance / 2) -
                    (transform.GetComponent<SpriteRenderer>().size.y / 2 * transform.localScale.y) - sensorYOffset),
                new Vector2(
                    (transform.GetComponent<SpriteRenderer>().size.x * transform.localScale.x) -
                    sensorXThicknessReducer, sensorYDistance),
                0,
                Vector2.down, 1, walkable);


            if (groundCheck.collider != null)
            {
                Debug.Log($"Floor name: {groundCheck.collider.name}");
            }

            fowardCheck = Physics2D.Raycast(
                new Vector2(
                    transform.position.x +
                    (transform.GetComponent<SpriteRenderer>().size.x / 2) * transform.localScale.x - sensorXOffset,
                    transform.position.y),
                Vector3.left, sensorXDistance, walkable);
            backCheck = Physics2D.Raycast(
                new Vector2(
                    transform.position.x -
                    (transform.GetComponent<SpriteRenderer>().size.x / 2) * transform.localScale.x + sensorXOffset,
                    transform.position.y),
                Vector3.right, sensorXDistance, walkable);
        }

        #endregion


        Debug.Log($"size.x{transform.GetComponent<SpriteRenderer>().size.x * transform.localScale.x}");
        Debug.Log(groundCheck.collider);

        #region Walking States and Jump

        if (groundCheck.collider != null)
        {
            playerState = WalkingState.Grounded;
        }
        else
        {
            playerState = WalkingState.Air;
        }

        #endregion

        #region Walls States

        if ((fowardCheck && !backCheck) || (fowardCheck && backCheck))
        {
            playerWallState = WallState.FrontWall;
        }
        else if (!fowardCheck && backCheck)
        {
            playerWallState = WallState.BackWall;
        }
        else
        {
            playerWallState = WallState.None;
        }

        #endregion

        //---------------------------------EdgeChecker----------------------------------------------

        #region EdgeChecker

        if (downwardsEdgeCheck && fowardCheck && downwardsEdgeCheck.distance > 0.05f)
        {
            edgeClose = true;
            //edgepos = new Vector2(fowardCheck.point.x,downwardsEdgeCheck.point.y);
            edgepos = new Vector2(fowardCheck.point.x, downwardsEdgeCheck.point.y);
        }

        if (!downwardsEdgeCheck || !fowardCheck || downwardsEdgeCheck.distance < 0.05f)
        {
            edgeClose = false;
        }

        #endregion
    }

    public void Move(float horizontal)
    {
        //Add air control
        //Add slip movement and not instant
        if (horizontal < 0)
        {
            transform.localScale = new Vector3(-startScale.x, startScale.y, startScale.z);
        }
        else if (horizontal > 0)
        {
            transform.localScale = startScale;
        }

        //rb2d.AddForce(new Vector2(horizontal*speed,0),ForceMode2D.Force);
        if (playerState == WalkingState.Grounded)
        {
            if (Mathf.Abs(maxVelocity2D.x) >= Mathf.Abs(rb2d.velocity.x + horizontal * stepSpeed))
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x + horizontal * stepSpeed, rb2d.velocity.y);
            }
        }
        else if (playerState == WalkingState.Air)
        {
            if (Mathf.Abs(maxVelocity2D.x) >= Mathf.Abs(rb2d.velocity.x + horizontal * ((stepSpeed / 2))))
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x + horizontal * (stepSpeed / 2), rb2d.velocity.y);
            }
        }
    }


    public void Jump()
    {
        /*
        if (playerState== WalkingState.Grounded || (playerState == WalkingState.Air && (fowardCheck|| backCheck)) )
        {
            canJump = true;
        }else if (playerState == WalkingState.Air )
        {
            canJump = false;
        }
        */
        if (playerState == WalkingState.Grounded)
        {
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            canJump = false;
        }
        else if (playerState == WalkingState.Air)
        {
            if (fowardCheck)
            {
                //rb2d.AddForce((-transform.right) * jumpForce * 1.4f, ForceMode2D.Impulse);
                rb2d.AddForce(fowardCheck.normal * jumpForce * 1.4f, ForceMode2D.Impulse);
                Debug.Log("Jumped Wall");
                canJump = false;
            }
        }
    }
}
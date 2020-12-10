using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D))]
public class Movement : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private BoxCollider2D collider2d;

    private Vector3 startScale;
    [Header("Walking Variables")]
    [SerializeField] private Vector2 maxWalkingVelocity2D = new Vector2(4,0);
    [SerializeField] private float stepWalkingSpeed = 0.2f;
    [Header("Running Variables")]
    [SerializeField] private Vector2 maxRunningVelocity2D = new Vector2(7,0);
    [SerializeField] private float stepRunningSpeed = 0.5f;
    [Header("Air Variables")]
    [SerializeField] private float aircontrol = 0.7f;
    private bool canJump;
    [SerializeField] private float jumpForce = 5;
    [SerializeField] private float gravityNormal = 1;
        [SerializeField] private float gravityWall = 0.6f;


    [SerializeField] private LayerMask walkable;

    [Space] [Header("Ground Checkers")] [SerializeField]
    private float sensorYOffset = 0;

    [SerializeField] private float sensorYDistance = 0.05f;
    [SerializeField] private float sensorXThicknessReducer = 0.07f;
    private RaycastHit2D groundCheck;
    public RaycastHit2D GroundCheck
    {
        get => groundCheck;
    }
    
    [Space]
    [Header("WallCheckers")] 
    [SerializeField] private float sensorXOffset = 0.02f;
    private RaycastHit2D fowardCheck;
    public RaycastHit2D FowardCheck
    {
        get => fowardCheck;
    }
    private RaycastHit2D backCheck;
    public RaycastHit2D BackCheck => backCheck;
    [SerializeField]
    private float sensorXDistance = 0.08f;

    public float WallCheckDistance
    {
        get { return sensorXDistance; }
    }

    
    private RaycastHit2D downwardsEdgeCheck;
    
    public RaycastHit2D DownwardsEdgeCheck => downwardsEdgeCheck;
    
    

    
    [Space]
    [Header("Downwards Checker")] 
    [SerializeField] private float downwardsEdgeXOffset=0;
    [SerializeField] private float downwardsEdgeYOffset=0;
    [SerializeField] private float downwardsEdgeDistance= 1.49f;

    [Space]
    [Header("Edge/Climbing Stuff")]
    [SerializeField] private float climbingCooldown=0.5f;
    [SerializeField] private float hangingMaxTime=1.5f;
    private bool edgeClose;
    private Vector3 edgepos;
    private bool isHanging;
    private float hangingStartTime;
    private bool canClimb;
    
    [Space]
    [Header("Vault Checker. Its still not working")] 
    [SerializeField] private float vaultEdgeXOffset = -0.23f;
    [SerializeField] private float vaultEdgeYOffset = -0.24f;
    [SerializeField] private float vaultEdgeDistance = 0.6f;
    private bool vaultableObjectClose;
    private Vector2 vaultpos;
    private RaycastHit2D vaultCheck;
    public RaycastHit2D VaultCheck => vaultCheck;
    [Header("Other Stuff")]

    

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
        isHanging = false;
        canClimb = false;
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
        //GroundChecker
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector2(transform.position.x,
                transform.position.y - (sensorYDistance / 2) -
                (transform.GetComponent<BoxCollider2D>().size.y / 2 * transform.localScale.y) - sensorYOffset),
            new Vector2(
                (transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(transform.localScale.x)) -
                sensorXThicknessReducer, //+((transform.localScale.x > 0) ? -sensorXThicknessReducer : +sensorXThicknessReducer),
                sensorYDistance));
        //FowardChecker
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(
            new Vector2(
                transform.position.x +
                (transform.GetComponent<BoxCollider2D>().size.x / 2) * transform.localScale.x +
                ((transform.localScale.x > 0) ? +sensorXOffset : -sensorXOffset),
                transform.position.y),
            ((transform.localScale.x > 0) ? transform.right : -transform.right) * sensorXDistance);
        //BackCheker
        Gizmos.color = Color.green;
        Gizmos.DrawRay(
            new Vector2(
                transform.position.x -
                (transform.GetComponent<BoxCollider2D>().size.x / 2) * transform.localScale.x +
                ((transform.localScale.x > 0) ? -sensorXOffset : +sensorXOffset),
                transform.position.y),
            ((transform.localScale.x > 0) ? -transform.right : transform.right) * sensorXDistance);

        #endregion

        #region GizmosCheckersEdge

        //DownwardsPosition ray
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(
            new Vector3(
                transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x +
                ((transform.localScale.x > 0) ? +downwardsEdgeXOffset : -downwardsEdgeXOffset),
                transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
                downwardsEdgeYOffset,
                0),
            new Vector3(.2f, .2f, .2f)
        );
        //DownwardsRayDown
        Gizmos.DrawLine(new Vector3(
            transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x +
            ((transform.localScale.x > 0) ? +downwardsEdgeXOffset : -downwardsEdgeXOffset),
            transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
            downwardsEdgeYOffset,
            0), new Vector3(
            transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x +
            ((transform.localScale.x > 0) ? +downwardsEdgeXOffset : -downwardsEdgeXOffset),
            transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
            downwardsEdgeYOffset,
            0) + Vector3.down * downwardsEdgeDistance);


       

        //Foward Edge Ray
        /*
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Vector3(transform.position.x +(transform.GetComponent<BoxCollider2D>().size.x/2 *transform.localScale.x +fowardEdgeXOffset),
            transform.position.y +(transform.GetComponent<BoxCollider2D>().size.y/2 *transform.localScale.y +fowardEdgeYOffset),
            0),Vector3.right);
        */

        #endregion
        //Edge
        if (edgeClose)
        {
            Gizmos.DrawWireCube(
                edgepos,
                new Vector3(.2f, .2f, .2f)
            );
        }

        #region GizmosCheckerVault

        //VaultPosition ray
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(
            new Vector3(
                transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x +
                ((transform.localScale.x > 0) ? +vaultEdgeXOffset : -vaultEdgeXOffset),
                transform.position.y - (transform.GetComponent<BoxCollider2D>().size.y/4) * transform.localScale.y +
                vaultEdgeYOffset,
                0),
            new Vector3(.2f, .2f, .2f)
        );
        //VaultRayFoward
        Gizmos.DrawRay(new Vector3(
            transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x +
            ((transform.localScale.x > 0) ? +vaultEdgeXOffset : -vaultEdgeXOffset),
            transform.position.y - (transform.GetComponent<BoxCollider2D>().size.y/4) * transform.localScale.y +
            vaultEdgeYOffset,
            0), ((transform.localScale.x > 0) ? transform.right : -transform.right) * vaultEdgeDistance);

       
        Gizmos.DrawWireCube(vaultCheck.point,new Vector3(0.2f,0.2f,0.2f));
           
       

        #endregion
        



    }

    private void LateUpdate()
    {
        CheckHanging();
    }

    private void FixedUpdate()
    {
        CheckersAndEdge();
    }

    void CheckersAndEdge()
    {
        #region CheckEdge

        downwardsEdgeCheck = Physics2D.Raycast(new Vector3(
            transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x + ((transform.localScale.x > 0) ? +downwardsEdgeXOffset : -downwardsEdgeXOffset),
            transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
            downwardsEdgeYOffset,
            0), Vector2.down, downwardsEdgeDistance,walkable);
       
        #endregion

        #region CheckersRegionFloorWall

        groundCheck = Physics2D.BoxCast(
            new Vector2(transform.position.x,
                transform.position.y - (sensorYDistance / 2) -
                (transform.GetComponent<BoxCollider2D>().size.y / 2 * transform.localScale.y) - sensorYOffset),
            new Vector2(
                (transform.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(transform.localScale.x)) -
                sensorXThicknessReducer, //+((transform.localScale.x > 0) ? -sensorXThicknessReducer : +sensorXThicknessReducer),
                sensorYDistance),
            0,
            Vector2.down, 0, walkable);

        fowardCheck = Physics2D.Raycast(
            new Vector2(
                transform.position.x +
                (transform.GetComponent<BoxCollider2D>().size.x / 2) * transform.localScale.x +
                ((transform.localScale.x > 0) ? +sensorXOffset : -sensorXOffset),
                transform.position.y),
            ((transform.localScale.x > 0) ? Vector3.right : Vector3.left), sensorXDistance, walkable);

        backCheck = Physics2D.Raycast(
            new Vector2(
                transform.position.x -
                (transform.GetComponent<BoxCollider2D>().size.x / 2) * transform.localScale.x +
                ((transform.localScale.x > 0) ? -sensorXOffset : +sensorXOffset),
                transform.position.y),
            ((transform.localScale.x > 0) ? Vector3.left : Vector3.right), sensorXDistance, walkable);
      
        #endregion

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
        
        //---------------------------------VaultChecker---------------------------------------------
        //Still not done
        #region CheckerVault
        vaultCheck = Physics2D.Raycast(
            new Vector3(
                transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x +
                ((transform.localScale.x > 0) ? +vaultEdgeXOffset : -vaultEdgeXOffset),
                transform.position.y - (transform.GetComponent<BoxCollider2D>().size.y/4) * transform.localScale.y +
                vaultEdgeYOffset,
                0),
            ((transform.localScale.x > 0) ? Vector3.right : Vector3.left), vaultEdgeDistance, walkable);

        /*
         * downwardsEdgeCheck = Physics2D.Raycast(new Vector3(
            transform.position.x + (transform.GetComponent<BoxCollider2D>().size.x) * transform.localScale.x + ((transform.localScale.x > 0) ? +downwardsEdgeXOffset : -downwardsEdgeXOffset),
            transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y +
            downwardsEdgeYOffset,
            0), Vector2.down, downwardsEdgeDistance,walkable);
         */
        
        //I did the position.y - position.y just to understand when reading
       // Debug.Log($"Downwards Distance: {downwardsEdgeCheck.distance}| Depois de: {(transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y + downwardsEdgeYOffset)- transform.position.y}");
        if (downwardsEdgeCheck && vaultCheck && downwardsEdgeCheck.distance >  (transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y + downwardsEdgeYOffset)- transform.position.y)
        {
            vaultableObjectClose = true;
            vaultpos = new Vector2(vaultCheck.point.x, downwardsEdgeCheck.point.y);
            // edgepos = new Vector2(fowardCheck.point.x, downwardsEdgeCheck.point.y);
        }

        if (!downwardsEdgeCheck || !vaultCheck || downwardsEdgeCheck.distance <=  (transform.position.y + (transform.GetComponent<BoxCollider2D>().size.y) * transform.localScale.y + downwardsEdgeYOffset)- transform.position.y)
        {
            vaultableObjectClose = false;
        }
       

        #endregion

        if (playerState == WalkingState.Air && playerWallState != WallState.None)
        {
            rb2d.gravityScale = gravityWall;
        }
        else
        {
            rb2d.gravityScale = gravityNormal;
        }
    }

    /// <summary>
    /// Makes the character move on the X axis, with the walking speed or running speed
    /// </summary>
    /// <param name="horizontal"></param> Variable responsible for the input
    /// <param name="running"></param> Variable responsible for the state of movement
    public void Move(float horizontal, bool running =false)
    {
        if (!isHanging)
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
                if (Mathf.Abs( (running) ? maxRunningVelocity2D.x : maxWalkingVelocity2D.x ) >= Mathf.Abs(rb2d.velocity.x + horizontal * ((running) ? stepRunningSpeed :  stepWalkingSpeed)))
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x + horizontal * ((running) ? stepRunningSpeed :  stepWalkingSpeed), rb2d.velocity.y);
                }

                if (vaultableObjectClose && running && CompareNormalWithVector(vaultCheck.normal,((horizontal>0) ? Vector2.left : Vector2.right),0.3f) )
                {
                    transform.position = new Vector3(
                        vaultpos.x + ((vaultpos.x - transform.position.x > 0) ? -(transform.GetComponent<BoxCollider2D>().size.x / 4) : +(transform.GetComponent<BoxCollider2D>().size.x / 4)),
                        vaultpos.y + transform.GetComponent<BoxCollider2D>().size.y / 1.8f,
                        transform.position.z
                    );
                }
            }
            else if (playerState == WalkingState.Air)
            {
                if (Mathf.Abs(maxWalkingVelocity2D.x) >= Mathf.Abs(rb2d.velocity.x + horizontal * ((stepWalkingSpeed * aircontrol))))
                {
                    rb2d.velocity = new Vector2(rb2d.velocity.x + horizontal * (stepWalkingSpeed * aircontrol), rb2d.velocity.y);
                    //rb2d.velocity = new Vector2(rb2d.velocity.x + horizontal * ((running) ? stepRunningSpeed :  stepWalkingSpeed), rb2d.velocity.y);
                }
            }
        }
    }

    

    public void Jump(Vector2 input)
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
            if (CompareInputWithVector(input,Vector2.up,0.8f) && fowardCheck && edgeClose && !isHanging)
            {
                Debug.Log("Grabbed Edge");
                isHanging = true;
                hangingStartTime = Time.time;
                rb2d.velocity = Vector2.zero;
                rb2d.isKinematic = true;
                transform.position = new Vector3(
                    edgepos.x + ((edgepos.x - transform.position.x > 0) ? -(transform.GetComponent<BoxCollider2D>().size.x / 4) : +(transform.GetComponent<BoxCollider2D>().size.x / 4)),
                    edgepos.y - transform.GetComponent<BoxCollider2D>().size.y / 2,
                    transform.position.z
                );
                /*
                Debug.Log("Climbed Wall");
                rb2d.velocity = Vector2.zero;
                //rb2d.AddForce(transform.up * jumpForce * 1.4f, ForceMode2D.Impulse);
                transform.position = new Vector3(
                    edgepos.x + ((edgepos.x - transform.position.x > 0) ? +(transform.GetComponent<BoxCollider2D>().size.x / 2) : -(transform.GetComponent<BoxCollider2D>().size.x / 2)),
                    edgepos.y + transform.GetComponent<BoxCollider2D>().size.y / 2,
                    transform.position.z
                    );
                */
            }else if (CompareInputWithVector(input,Vector2.up,0.8f) && isHanging && canClimb)
            {
                Debug.Log("Climbed Wall");
                rb2d.velocity = Vector2.zero;
                isHanging = false;
                canClimb = false;
                rb2d.isKinematic = false;
                //rb2d.AddForce(transform.up * jumpForce * 1.4f, ForceMode2D.Impulse);
                transform.position = new Vector3(
                    edgepos.x + ((edgepos.x - transform.position.x > 0) ? +(transform.GetComponent<BoxCollider2D>().size.x / 2) : -(transform.GetComponent<BoxCollider2D>().size.x / 2)),
                    edgepos.y + transform.GetComponent<BoxCollider2D>().size.y / 2,
                    transform.position.z
                );
            }
            else if (fowardCheck)
            {
                Debug.Log("Jumped Wall");
                //rb2d.AddForce((-transform.right) * jumpForce * 1.4f, ForceMode2D.Impulse);
                rb2d.AddForce(fowardCheck.normal * jumpForce * 1.4f, ForceMode2D.Impulse);
                rb2d.AddForce(transform.up * jumpForce * 0.9f, ForceMode2D.Impulse);
                
                canJump = false;
            }
        }
    }

    private void CheckHanging()
    {
        if (isHanging)
        {
            if (climbingCooldown+hangingStartTime<Time.time)
            {
                canClimb = true;
            }
            if (hangingStartTime+hangingMaxTime<Time.time)
            {
                rb2d.velocity = Vector2.zero;
                isHanging = false;
                rb2d.isKinematic = false;
                
            }
        }
        else
        {
            canClimb = false;
        }
        
    }
    public bool CompareInputWithVector(Vector2 input,Vector2 vector,float offset)
    {
        input = input.normalized;
        if ((input.x >=vector.x-offset && input.x <= vector.x+offset) && (input.y >= vector.y - offset && input.y <=vector.y + offset))
        {
            return true;
        }

        return false;
    }
    public bool CompareNormalWithVector(Vector2 normal,Vector2 vector,float offset)
    {
        normal = normal.normalized;
        if ((normal.x >=vector.x-offset && normal.x <= vector.x+offset) && (normal.y >= vector.y - offset && normal.y <=vector.y + offset))
        {
            return true;
        }

        return false;
    }
}
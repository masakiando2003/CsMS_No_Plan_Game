using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using TMPro;

public class Player : MonoBehaviour {

    // Config
    [Header("Player")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] float dashSpeed = 12f;
    [SerializeField] float walkStairSpeed = 1f;
    [SerializeField] Vector2 deathKick = new Vector2(0f, 25f);
    [SerializeField] Vector2 knoutOutKick = new Vector2(80f, 30f);
    [SerializeField] int playerID = 1;
    [SerializeField] string playerType = "Adventurer";
    [SerializeField] bool canShoot = true;
    [SerializeField] float rangeAttackTime = 0.8f;
    [SerializeField] float rangeAttackDelay = 0.8f;
    [SerializeField] bool walkingStairs = false;
    [SerializeField] bool respawnState = false;
    [SerializeField] float respawnInvicibleTime = 5f;
    [SerializeField] float maxRespawnInvicibleTime = 5f;
    [SerializeField] float spriteBlinkingTimer = 0.0f;
    [SerializeField] float spriteBlinkingMiniDuration = 0.1f;
    [SerializeField] float spriteBlinkingTotalTimer = 0.0f;
    [SerializeField] float spriteBlinkingTotalDuration = 2.0f;

    [Header("ProjectTile")]
    [SerializeField] GameObject fireBallPrefab;
    [SerializeField] float projectTileSpeed = 10f;

    [Header("Prefabs")]
    [SerializeField] GameObject staminaPrefab;
    [SerializeField] GameObject playerPrefab;

    Coroutine firingCorutine;

    // State
    bool isAlive = true;

    // Melee Attack State
    int meleeKeyPressedTime = 0;
    [SerializeField] int totalMeleeKeyPressedTime = 0;
    //Time when last key was pressed
    float lastKeyPressedTime = 0f;
    //Delay between presses for which presses will be considered as combo
    float maxComboDelay = 1f;

    // Cached component references
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    DamageDealer myDamageDealer;
    Transform myTransform;
    float gravityScaleAtStart;
    SpriteRenderer mySpriteRenderer;

    public animationC_rio aniCsc;
    public animationC_rio02 aniCsc2;

    // Messagers then methods
    void Start ()
    {
        playerID = (playerType == "Adventurer") ? 1 : 2;
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        myDamageDealer = GetComponent<DamageDealer>();
        myTransform = GetComponent<Transform>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        gravityScaleAtStart = myRigidbody.gravityScale;
    }
	
	// Update is called once per frame
	void Update () {
        if (!isAlive) { return; }
        if (FindObjectsOfType<GameSession>().Length < 1) { return; }

        Run();
        Dash();
        WalkStairs();
        ClimbLadder();
        Jump();
        FlipSprite();
        HurtOrDie();
        RespawnInvicible();
        MeleeAttack();
        /*switch (playerType)
        {
            case "Adventurer":
                MeleeAttack();
                break;
            case "Player":
                FireSpell();
                break;
        }*/
        SPAttack();
        if (FindObjectsOfType<GameSession>().Length > 0)
        {
            if (FindObjectOfType<GameSession>().GetDebugModeFlag())
            {
                ControlHP();    // For Debugging HP bar
                ControlMP();    // For Debugging MP bar
                ControlSP();    // For Debugging SP bar
            }
        }
    }

    private void Run()
    {
        string Horizontal = (playerID == 1) ? "Player1Horizontal" : "Player2Horizontal" ;
        float controlFlow = CrossPlatformInputManager.GetAxis(Horizontal); // value is between -1 and 1
        Vector2 playerVelocity = new Vector2(controlFlow * runSpeed, myRigidbody.velocity.y * walkStairSpeed);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Math.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Running", playerHasHorizontalSpeed);
    }

    private void Dash()
    {
        string Horizontal = (playerID == 1) ? "Player1Horizontal" : "Player2Horizontal";
        string Dash = (playerID == 1) ? "Player1Dash" : "Player2Dash";
        float controlFlow = CrossPlatformInputManager.GetAxis(Horizontal); // value is between -1 and 1
        if (CrossPlatformInputManager.GetButton(Dash) && controlFlow!=0)
        {
            if (FindObjectsOfType<GameSession>().Length > 0)
            {
                if (FindObjectOfType<GameSession>().GetCurrentStamina(playerID) > FindObjectOfType<GameSession>().GetMinDashStamina() &&
                    controlFlow != 0)
                {
                    if(playerID == 1)
                    {
                        GameObject.Find("P1 Stamina Label").GetComponent<UnityEngine.UI.Text>().enabled = true;
                        GameObject.Find("P1 Stamina Bar").GetComponent<UnityEngine.UI.Image>().enabled = true;
                    }
                    else
                    {
                        GameObject.Find("P2 Stamina Label").GetComponent<UnityEngine.UI.Text>().enabled = true;
                        GameObject.Find("P2 Stamina Bar").GetComponent<UnityEngine.UI.Image>().enabled = true;
                    }
                    Vector2 playerVelocity = new Vector2(controlFlow * dashSpeed, myRigidbody.velocity.y);
                    myRigidbody.velocity = playerVelocity;
                    FindObjectOfType<GameSession>().ProcessReducePlayerStamina(playerID);
                }
            }
        }
        else
        {
            if (FindObjectsOfType<GameSession>().Length > 0)
            {
                FindObjectOfType<GameSession>().RegenStamina(playerID);
                if (FindObjectOfType<GameSession>().GetCurrentStamina(playerID) >= FindObjectOfType<GameSession>().GetMaxStamina(playerID))
                {
                    if (playerID == 1)
                    {
                        GameObject.Find("P1 Stamina Label").GetComponent<UnityEngine.UI.Text>().enabled = false;
                        GameObject.Find("P1 Stamina Bar").GetComponent<UnityEngine.UI.Image>().enabled = false;
                    }
                    else
                    {
                        GameObject.Find("P2 Stamina Label").GetComponent<UnityEngine.UI.Text>().enabled = false;
                        GameObject.Find("P2 Stamina Bar").GetComponent<UnityEngine.UI.Image>().enabled = false;
                    }
                }
            }
        }
    }

    private void WalkStairs()
    {
        if(walkingStairs == true)
        {
            myRigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            string Horizontal = (playerID == 1) ? "Player1Horizontal" : "Player2Horizontal";
            float controlFlow = CrossPlatformInputManager.GetAxis(Horizontal); // value is between -1 and 1
            if (controlFlow != 0){
                myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                //walkStairSpeed = 1.1f;
            }
        }
        else
        {
            myRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            walkStairSpeed = 1f;
        }
    }

    private void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myAnimator.SetBool("Climbing", false);
            myRigidbody.gravityScale = gravityScaleAtStart;
            return;
        }

        string Vertical = (playerID == 1) ? "Player1Vertical" : "Player2Vertical";
        float controlFlow = CrossPlatformInputManager.GetAxis(Vertical);// value is between -1 and 1
        Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, controlFlow * climbSpeed);
        myRigidbody.velocity = climbVelocity;
        myRigidbody.gravityScale = 0;

        bool playerHasVerticalSpeed = Math.Abs(myRigidbody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);
    }

    private void Jump()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Box")))
        {
            return;
        }

        string Jump = (playerID == 1) ? "Player1Jump" : "Player2Jump";
        if (CrossPlatformInputManager.GetButtonDown(Jump))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidbody.velocity += jumpVelocityToAdd;
        }
    }

    private void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Math.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }
    
    private void HurtOrDie()
    {
        int currentHP = FindObjectOfType<GameSession>().GetCurrentHP(playerID);
        if(currentHP <= 0)
        {
            DieAnimation(playerID);
            if(playerID == 1)
            {
                //gameObject.GetComponentInChildren<animationC_rio2>().SetDie(true);
            }
            else
            {
                //gameObject.GetComponentInChildren<animationC_rio02>().SetDie(true);
            }
        }
        else
        {
            if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Hazards")))
            {
                DieAnimation(playerID);
            }
            else if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Traps")))
            {
                if (FindObjectOfType<Trap>().GetIsHitState() == false)
                {
                    FindObjectOfType<Trap>().IsHit();
                    int trapDamage = FindObjectOfType<DamageDealer>().GetDamage();
                    FindObjectOfType<GameSession>().ProcessReducePlayerHP(trapDamage);
                }
            }
        }
    }

    private void DieAnimation(int playerID)
    {
        isAlive = false;
        Debug.Log("Player" + playerID + "Dying");
        myAnimator.SetBool("Player" + playerID + "Dying", true);
        if (playerID == 1)
        {
            aniCsc.Die();
        }
        else
        {
            aniCsc2.Die();
        }
        //GetComponent<Rigidbody2D>().velocity = transform.localScale.x * deathKick;
    }

    public void SetRespawnPosition()
    {
        myTransform.position = new Vector2(myTransform.position.x, myTransform.position.y + 2f);
    }

    private void FireSpell()
    {
        rangeAttackTime += Time.deltaTime;
        if (CrossPlatformInputManager.GetButtonDown("Range Attack"))
        {
            if(rangeAttackTime >= rangeAttackDelay)
            {
                FireBallAttack();
                myAnimator.SetBool("Firing", true);
                rangeAttackTime = 0f;
            }
        }
        else
        {
            myAnimator.SetBool("Firing", false);
        }
    }

    private void MeleeAttack()
    {
        string playerMeleeAttack = "Player" + playerID + " Melee Attack";
        Debug.Log(playerMeleeAttack);

        if (CrossPlatformInputManager.GetButtonDown(playerMeleeAttack))
        {
            //Debug.Log("Animator Name: "+myAnimator.name);
            myAnimator.SetBool("Attacking", true);
        }
        else
        {
            //EndMeleeAttack();
        }
    }
    
    public void EndMeleeAttack()
    {
        myAnimator.SetBool("Attack", false);
    }


    private void FireBallAttack()
    {
        GameObject fireBall = Instantiate(
            fireBallPrefab,
            new Vector2(transform.position.x, transform.position.y - 0.1f),
            Quaternion.identity) as GameObject;
        fireBall.GetComponent<Rigidbody2D>().velocity = new Vector2(transform.localScale.x * projectTileSpeed, 0f);
        fireBall.GetComponent<Transform>().localScale = transform.localScale;
        fireBall.GetComponent<RangeAttack>().SetBelongToPlayerID(playerID);
        //SetShooted(true);
    }

    private void SPAttack()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (FindObjectOfType<GameSession>().GetCurrentSP(playerID) >= FindObjectOfType<GameSession>().GetMaxSP(playerID))
            {
                FindObjectOfType<GameSession>().ProcessUseSP(playerID);
                GameObject.Find("SPMax Text").GetComponent<TextMeshProUGUI>().enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision Tag: " + other.gameObject.tag);
        if(FindObjectsOfType<GameSession>().Length < 1) { return; }
        DamageDealer damageDealer = other.GetComponent<DamageDealer>();
        if (!damageDealer) { return; }
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy")))
        {
            //other.GetComponentInParent<Enemy>().ProcessHit(other.GetComponentInParent<DamageDealer>(), playerID, 0);
            //FindObjectOfType<GameSession>().ProcessReducePlayerHP(damageDealer.GetDamage(), playerID);
        }
        if((other.gameObject.tag == "Enemy Melee Attack" || other.gameObject.tag == "Enemy Range Attack") && respawnState==false)
        {
            if (GameObject.Find("Player" + playerID).GetComponentsInChildren<SkinnedMeshRenderer>().Length > 0)
            {
                foreach (SkinnedMeshRenderer smr in GameObject.Find("Player" + playerID).GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    Debug.Log(smr.gameObject.name);
                    if (smr.enabled == false)
                    {
                        smr.enabled = true;
                    }
                }
            }

            if (isAlive)
            {
                FindObjectOfType<GameSession>().ProcessReducePlayerHP(damageDealer.GetDamage(), playerID);
            }
            if(other.gameObject.tag == "Enemy Melee Attack")
            {
                KnockOut();
            }
        }
    }

    private void KnockOut()
    {
        Debug.Log("Knock Out!");
        //myRigidbody.velocity = knoutOutKick;
    }

    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy Melee Attack" && respawnState == false)
        {
            KnockOut();
        }
    }

    IEnumerator ResetHitStatus()
    {
        myBodyCollider.isTrigger = false;
        yield return new WaitForSecondsRealtime(1f);
        myBodyCollider.isTrigger = true;
    }
    

    public int GetPlayerID()
    {
        return playerID;
    }

    private void ControlHP()
    {
        int damage = myDamageDealer.GetDamage();
        int addHP = myDamageDealer.GetAddHP();
        if (FindObjectsOfType<GameSession>().Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if ((FindObjectOfType<GameSession>().GetCurrentHP(playerID) - damage) <= 0)
                {
                    DieAnimation(playerID);
                }
                else
                {
                    FindObjectOfType<GameSession>().ProcessReducePlayerHP(damage);
                }
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                FindObjectOfType<GameSession>().ProcessIncreasePlayerHP(addHP, playerID);
            }
        }
    }

    private void ControlMP()
    {
        float reduceMP = myDamageDealer.GetReduceMP();
        float addMP = myDamageDealer.GetAddMP();
        if (FindObjectsOfType<GameSession>().Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                if ((FindObjectOfType<GameSession>().GetCurrentMP() - reduceMP) > 0)
                {
                    FindObjectOfType<GameSession>().ProcessReducePlayerMP(reduceMP, playerID);
                }
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                FindObjectOfType<GameSession>().ProcessIncreasePlayerMP(addMP, playerID);
            }
        }
    }

    private void ControlSP()
    {
        float addSP = myDamageDealer.GetAddSP();
        if (FindObjectsOfType<GameSession>().Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                FindObjectOfType<GameSession>().ProcessIncreasePlayerSP(addSP, playerID);
                if (FindObjectOfType<GameSession>().GetCurrentSP(playerID) >= FindObjectOfType<GameSession>().GetMaxSP(playerID))
                {
                    GameObject.Find("SPMax Text").GetComponent<TextMeshProUGUI>().enabled = true;
                }
                else
                {
                    GameObject.Find("SPMax Text").GetComponent<TextMeshProUGUI>().enabled = false;
                }
            }
        }
    }

    public void SetAttackMeleeKeyPressedTime(int meleeKeyPressedTime)
    {
        totalMeleeKeyPressedTime = meleeKeyPressedTime;
    }

    public int GetAttackMeleeKeyPressedTime()
    {
        return totalMeleeKeyPressedTime;
    }

    public void SetAlive(bool isAlive)
    {
        this.isAlive = isAlive;
        myAnimator.SetBool("Player"+playerID+"Alive", true);
        if (playerID == 1)
        {
            aniCsc.Allive();
        }
        else
        {
            aniCsc2.Allive();
        }
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    public void SetShooted(bool isShooted)
    {
        canShoot = (isShooted) ? false : true;
    }

    public void SetWalkingStairState(bool isWalkingStairs)
    {
        walkingStairs = isWalkingStairs;
    }

    public void RespawnInvicible()
    {
        string respawnInvicibleObj = "Player" + playerID + " Respawn Invicible Area";
        if (respawnState == true)
        {
            GameObject.Find(respawnInvicibleObj).GetComponent<BoxCollider2D>().enabled = true;
            myBodyCollider.enabled = false;
            respawnInvicibleTime -= Time.deltaTime;
            RespawnBlinkingEffect();
            if (respawnInvicibleTime <= 0.0f)
            {
                SetRespawnState(false);
                GameObject.Find(respawnInvicibleObj).GetComponent<BoxCollider2D>().enabled = false;
                myBodyCollider.enabled = true;

                CheckSkinnedMeshRendered();
            }
        }
    }

    public void SetRespawnState(bool isRespawn)
    {
        respawnState = isRespawn;
        respawnInvicibleTime = maxRespawnInvicibleTime;
        spriteBlinkingTotalTimer = 0.0f;
    }

    private void RespawnBlinkingEffect()
    {
        spriteBlinkingTotalTimer += Time.deltaTime;
        if (spriteBlinkingTotalTimer >= spriteBlinkingTotalDuration)
        {
            if (GameObject.Find("Player" + playerID).GetComponentsInChildren<SkinnedMeshRenderer>().Length > 0)
            {
                foreach (SkinnedMeshRenderer smr in GameObject.Find("Player" + playerID).GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    Debug.Log(smr.gameObject.name);
                    if (smr.enabled == false)
                    {
                        smr.enabled = true;
                    }
                }
            }
        }

        spriteBlinkingTimer += Time.deltaTime;
        if (spriteBlinkingTimer >= spriteBlinkingMiniDuration)
        {

            spriteBlinkingTimer = 0.0f;
            if (GameObject.Find("Player" + playerID).GetComponentsInChildren<SkinnedMeshRenderer>().Length > 0)
            {
                foreach (SkinnedMeshRenderer smr in GameObject.Find("Player" + playerID).GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    Debug.Log(smr.gameObject.name);
                    if (smr.enabled == false)
                    {
                        smr.enabled = true;
                    }
                    else
                    {
                        smr.enabled = false;
                    }
                }
            }
        }
    }

    private void CheckSkinnedMeshRendered()
    {
        foreach(SkinnedMeshRenderer smr in GameObject.Find("Player" + playerID).GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            if (smr.enabled == false)
            {
                smr.enabled = true;
            }
        }
    }
}

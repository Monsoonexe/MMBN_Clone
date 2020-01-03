using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class NaviController_Battle : MonoBehaviour
{
    private static PanelArray panelArray;

    [SerializeField]
    private NaviAsset naviAsset;

    public NaviAsset NaviAsset { get { return naviAsset; } } //readonly


    [SerializeField]
    private Animator chargeAuraAnim;

    /// <summary>
    /// Cached transform
    /// </summary>
    private Transform myTransform;

    /// <summary>
    /// Animates the body sprite
    /// </summary>
    private Animator naviAnimator;

    //health and element stuff
    private int maxHealth = 100;//arbitrary defaults

    public int MaxHealth { get { return maxHealth; } } // readonly

    [SerializeField]
    private int currentHealth = 100;//arbitrary default

    public int CurrentHealth { get { return currentHealth; } } //readonly

    //buster stuff
    private bool busterIsCharging = false;
    private float busterCharge = 0.0f;

    //sword stuff
    private bool swordIsCharging = false;
    private float swordCharge = 0.0f;

    //attack stuff
    /// <summary>
    /// The next time that this is able to make an attack.
    /// </summary>
    private float nextAttackTime = 0.0f;

    //current coordinates on board
    private int currentPanelX = -1; //init coordinates of current panel to invalid panel
    private int currentPanelY = -1; //init coordinates of current panel to invalid panel

    private int movementX = 0;
    private int movementY = 0;

    //movement delay stuff
    private readonly float movementDelay = 0.15f;//.15f seems good
    private float movementDelayTimeSince;
    private bool movementDelayCoolingDown = false;
    private bool isMoving = false;

    //disable controls stuff
    private bool controlsDisabled = false;
    private Coroutine coroutine_controlsDisabled;
    
    /// <summary>
    /// Points navi towards enemy.
    /// </summary>
    private int targetOrientation = 1;
    
    [SerializeField]
    private Panel startingPanel; //panel that the Player WANTS to start at.  GameManager or BoardManager will actually determine where to start

    private Panel currentPanel = null;

    [SerializeField]
    private BattleTeam battleTeam = BattleTeam.BLUE;//default

    [SerializeField]
    private bool isPlayer = false;

    [SerializeField]
    private int owningPlayer = 0;

    [SerializeField]
    private List<StatusEffect> statusAilments = new List<StatusEffect>();

    [Header("---Events---")]
    public UnityEvent takeDamage = new UnityEvent();

    public UnityEvent die = new UnityEvent();

    public UnityEvent recoverHealth = new UnityEvent();

    void Awake()
    {
        var playerManager = GameObject.Find("PlayerManager");

        if (playerManager)//look for naviAssets from previous scenes
        {
            Debug.Log("PlayerManager found. Navi Assets loaded from previous scene.");
            var pm = playerManager.GetComponent<PlayerManager>() as PlayerManager;
            pm.GetNaviStats(ref this.battleTeam, ref this.naviAsset, ref this.maxHealth, ref this.currentHealth);

        }
        else
        {
            Debug.LogWarning("PlayerManager not found. Using default navi Assets and Inspector values.");
            //uses default health values or values set in Inspector
        }

        //init components
        myTransform = transform;//cache for performance
        naviAnimator = gameObject.GetComponent<Animator>();
        naviAnimator.runtimeAnimatorController = naviAsset.animatorOverrideController; //set overrides
        
        if (!panelArray)
        {
            panelArray = GameObject.FindGameObjectWithTag("PanelArray").GetComponent<PanelArray>();
        }

        if (!startingPanel)
        {
            SetStartingPanel();
        }
    }
    
    // Use this for initialization
    void Start ()
    {
        ConfigureSpriteOffset();//based on battle team, fixes sprite offset
        UpdateCurrentPanelCoordinates();
        //set starting values
        controlsDisabled = false;
        currentPanel = startingPanel;
    }

    // Update is called once per frame
    void Update()
    {
        if (controlsDisabled) return;
        isMoving = false;

        //handle input
        if (isPlayer)
        {
            HandleMovement_HumanPlayer();
            HandleActionInput_HumanPlayer();

        }//end if isPlayer

        else
        {
            HandleMovement_AIPlayer();
            HandleActionInput_AIPlayer();
        }

        //remove old status ailments
    }//end Update()
    

    private void InitializeDelays()//this function sets the delays
    {
        movementDelayTimeSince = movementDelay;//player should be able to move right away
        nextAttackTime = 0.0f;
    }

    public Panel GetDesiredStartingPanel()
    // gives where the Navi WANTS to start when asked. GameManager / Board Manager makes the call.
    {
        if (!startingPanel)
        {
            SetStartingPanel();
        }
        return startingPanel;
    }

    public Element GetElement()
    {
        return naviAsset.element;
    }

    /// <summary>
    /// After waiting the given time, controls will no longer be disabled.  coroutine ensures only 1 running at a time.
    /// </summary>
    /// <param name="disableTime"></param>
    /// <returns></returns>
    private IEnumerator DisableControls(float disableTime)
    {
        yield return new WaitForSeconds(disableTime);

        controlsDisabled = false;
        naviAnimator.SetBool("Damaged", false);
    }

    /// <summary>
    /// Make the sprite face left or right depending on team and original orientation.
    /// </summary>
    private void ConfigureSpriteOffset()
    {
        if (battleTeam == BattleTeam.BLUE && naviAsset.orientation != 1)
        {
            targetOrientation = -1;
        }
        if (battleTeam == BattleTeam.RED && naviAsset.orientation != -1)
        {
            targetOrientation = 1;
        }
    }

    /// <summary>
    /// Safe way of moving navi to a new panel.
    /// </summary>
    /// <param name="targetPanel">Panel that this navi desireth to move to.</param>
    /// <param name="allowTresspass">Is this navi allowed to move onto a panel that does not belong to the same team?</param>
    public void MoveNavi(Panel targetPanel, bool allowTresspass = false)
    {
        if (targetPanel.GetOccupant()) return;//can't enter occupied panels

        var canMoveToPanel = true;
        
        //if missing or broken, need to fly or float over holes.
        if(!(targetPanel.panelType == PanelType.MISSING || targetPanel.panelType == PanelType.BROKEN))
        {
            if (!allowTresspass)
            {
                //check for team
                canMoveToPanel = targetPanel.GetPanelTeam() == battleTeam;
            }
        }
        else
        {
            //can step on hole panels if element is wind
            canMoveToPanel = naviAsset.element == Element.WIND;
        }

        if(canMoveToPanel)
        {
            //Debug.Log(currentPanel.panelOccupant.name);//print test
            myTransform.position = targetPanel.GetPosition() + naviAsset.spriteOffset * targetOrientation;//move sprite to new location, offset the sprite to be at center of board
            currentPanel.LeavePanel();//clear current Panel's of any occupants
            targetPanel.OccupyPanel(this);//target Panel now has this object as an occupant
            currentPanel = targetPanel; //this object now knows which Panel it is on
            UpdateCurrentPanelCoordinates();//update coordinates of current panel
            movementDelayTimeSince = 0.0f;//reset movement delay
            
            naviAnimator.SetTrigger("Move");//trigger animation
            //Debug.Log("Move Completed!");//print test
            //TODO
            //OnPanelEnter(); //activate panel effects
        }
        else{
        //Debug.Log("Cannot move to target panel.");//print test
        }
        
    }//end MoveNavi()
    
    private void GetMovement(){
        if (owningPlayer == 1 && isPlayer == true)
        {
            GetMoveInput_Letters();
        }//end if

        else if (owningPlayer == 2 && isPlayer == true)
        {
            GetMoveInput_Keypad();
        }//end if

        else
        {
            Debug.Log("Warning: AI Player Detected. No AI Set up. staying at  current spot.");//error debug
            Debug.Log("owningPlayer = " + owningPlayer + " | isplayer = " + isPlayer);//print test
            movementX = 0;//set no movement
            movementY = 0;
            
        }
    }

    private void GetMoveInput_Letters()
    {
        if (Input.GetKeyDown(KeyCode.W))//move up
        {
            movementY = 1;
        }
        if (Input.GetKeyDown(KeyCode.S))//move down
        {
            movementY = -1;
        }
        if (Input.GetKeyDown(KeyCode.A))//move left
        {
            movementX = -1;
        }
        if (Input.GetKeyDown(KeyCode.D))//move right
        {
            movementX = 1;
        }
    }//end GetMoveInput_Letters()

    private void GetMoveInput_Keypad()
    {
        if (Input.GetKeyDown(KeyCode.Keypad8))//move up
        {
            movementY = 1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))//move down
        {
            movementY = -1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))//move left
        {
            movementX = -1;
        }
        if (Input.GetKeyDown(KeyCode.Keypad6))//move right
        {
            movementX = 1;
        }
    }//end GetMoveInput_Numbers()

    [System.Obsolete("I don't know why this doesn't work as well.  Use GetMoveInput_Keypad()")]
    /// <summary>
    /// 
    /// </summary>
    private void GetMovementInput_Axes()
    {
        //I don't understand why this doesn't work as well
        movementX = (int)Input.GetAxisRaw("Horizontal");//this version causes the character to move 3 times for some reason
        movementY = (int)Input.GetAxisRaw("Vertical");//this version of movement causes the character to move 3 times, causing problems
    }

    private void HandleMovement_HumanPlayer()
    {
        var skipMovement = false;
        
        //check movementDelay
        if (movementDelayTimeSince >= movementDelay)//if enough time has passed between moves, ie move cooldown is over
        {
            movementDelayCoolingDown = false;
        }
        else//set flags and increment time
        {
            movementDelayTimeSince += Time.deltaTime * naviAsset.naviMoveSpeed;
            movementDelayCoolingDown = true;
            isMoving = false;
        }

        GetMovement();//rely on this method to determine best method of getting input, ie human vs ai, p1 vs p2

        //Debug.Log("MovementX, MovementY: " + movementX + ", " + movementY); //print test
        if (movementX == 0 && movementY == 0)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }

        //check if movement is on the board
        if (currentPanelX + movementX >= PanelArray.GetBoardColumnsCount() || currentPanelX + movementX < 0 ||
            currentPanelY + movementY >= PanelArray.GetBoardRowsCount() || currentPanelY + movementY < 0)//test if movement is out of bounds
        {
            //Debug.Log("Move cancelled: next move would be off board. Current coords: " + currentPanelX + ", " + currentPanelY);//print test
            skipMovement = true;
            isMoving = false;
        }
        //check status ailments or other movement restrictions here

        //if can move, then move
        if (isMoving && !skipMovement && !movementDelayCoolingDown)
        {
            isMoving = false;//clear flag as movement is happening
            MoveNavi(panelArray.GetPanel(currentPanelX + movementX, currentPanelY + movementY));//move this navi to target panel //all panel and movement effects should be handled in MoveNavi
        }
        movementX = 0;//reset these values after move phase
        movementY = 0;//reset these values after move phase
    }//end HandleMovement_HumanPlayer()

    /// <summary>
    /// Handles movement if player is AI. Not yet defined.
    /// </summary>
    private void HandleMovement_AIPlayer()
    {
        //TODO 
        Debug.Log("AIMovement not yet set up.");
    }//end HandleMovement_AIPlayer()

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nowTime">Current time is used to determine cooldown.</param>
    private void HandleBuster(float nowTime)
    {
        var fireBuster = false;
        var busterCooledDown = nowTime > nextAttackTime;

        //get buster input for player1
        if(owningPlayer == 1)
            //TODO controls are SO with easy-to-change variables
        {
            if (Input.GetKeyDown(KeyCode.Space))//was buster button pressed down?
            {
                if (busterCooledDown)
                {
                    fireBuster = true;
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))//was buster button released?
            {
                fireBuster = true;
            }

            if (Input.GetKey(KeyCode.Space))//is buster button held down?
            {
                if (!swordIsCharging)
                {
                    busterIsCharging = true;
                }
            }
            else
            {
                busterIsCharging = false;
            }
        }
        //get buster input for player2
        if (owningPlayer == 2)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))//was buster button pressed down?
            {
                if (busterCooledDown)
                {
                    fireBuster = true;
                }
            }

            if (Input.GetKeyUp(KeyCode.RightArrow))//was buster button released?
            {
                fireBuster = true;
            }  

            if (Input.GetKey(KeyCode.RightArrow))//is buster button held down?
            {
                if (!swordIsCharging)
                {
                    busterIsCharging = true;
                }
            }
            else
            {
                busterIsCharging = false;
            }
        }

        if (fireBuster && busterCooledDown)//FIRE! regular buster
        {
            //handle charge attack if fully charged
            if (busterCharge > naviAsset.chargedBusterAttack.chargeTime)//if fully charged
            {
                StartCoroutine(naviAsset.chargedBusterAttack.TriggerAttack(this, naviAnimator));//do attack logic
                nextAttackTime = nowTime + naviAsset.chargedBusterAttack.delay;//always a delay between attacks

            }
            else//regular buster shot
            {
                StartCoroutine(naviAsset.busterAttack.TriggerAttack(this, naviAnimator));//do attack logic
                nextAttackTime = nowTime + naviAsset.busterAttack.delay;//always a delay between attacks

            }
            //reset values after shot
            busterCharge = 0;//reset charge amount when button is released
            movementDelayTimeSince = 0.0f;//reset movement delay after firing
        }

        if (busterIsCharging)//charge up buster numbers
        {
            busterCharge += Time.deltaTime;
            chargeAuraAnim.SetFloat("BusterCharge", busterCharge / naviAsset.chargedBusterAttack.chargeTime);//play charging animation

        }
        else
        {
            chargeAuraAnim.SetFloat("BusterCharge", 0.0f);//no charge
        }
    }//end HandleBuster()

    /// <summary>
    /// Handle Input and logic of sword attack.
    /// </summary>
    /// <param name="nowTime">Current time is used to determine cooldown.</param>
    private void HandleSword(float nowTime)
    {
        var fireSword = false;
        var swordCooledDown = nowTime > nextAttackTime;

        if (owningPlayer == 1)
        {
            if (Input.GetKeyDown(KeyCode.C))//was sword button pressed down?
            {
                if (swordCooledDown)
                {
                    fireSword = true;
                }
            }

            if (Input.GetKeyUp(KeyCode.C))//was sword button released?
            {
                fireSword = true;
            }

            if (Input.GetKey(KeyCode.C))//is sword button held down?
            {
                if (!busterIsCharging)
                {
                    swordIsCharging = true;
                }
            }
            else
            {
                swordIsCharging = false;
            }
        }
        else if (owningPlayer == 2)
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))//was sword button pressed down?
            {
                if (swordCooledDown)
                {
                    fireSword = true;
                }
            }

            if (Input.GetKeyUp(KeyCode.Keypad0))//was sword button released?
            {
                fireSword = true;
            }

            if (Input.GetKey(KeyCode.Keypad0))//is sword button held down?
            {
                if (!busterIsCharging)
                {
                    swordIsCharging = true;
                }
            }
            else
            {
                swordIsCharging = false;
            }
        }

        if (fireSword && swordCooledDown)//FIRE!
        {
            if (swordCharge > naviAsset.chargedSwordAttack.chargeTime)//if fully charged
            {
                StartCoroutine(naviAsset.chargedSwordAttack.TriggerAttack(this, naviAnimator));
                nextAttackTime = nowTime + naviAsset.chargedSwordAttack.delay;//reset time since last sword shot
            }
            else//regular sword swing
            {
                StartCoroutine(naviAsset.swordAttack.TriggerAttack(this, naviAnimator));
                nextAttackTime = nowTime + naviAsset.swordAttack.delay;//reset time since last sword shot

            }
            //reset values after shot
            swordCharge = 0;//reset charge amount when button is released
            movementDelayTimeSince = 0.0f;//reset movement delay after firing
        }

        if (swordIsCharging)//charge up buster numbers
        {
            swordCharge += Time.deltaTime;
            chargeAuraAnim.SetFloat("SwordCharge", swordCharge / naviAsset.chargedSwordAttack.chargeTime);//play animation, sending charge percent
        }
        else
        {
            chargeAuraAnim.SetFloat("SwordCharge", 0.0f);
        }
    }//end HandleSword_Letters()

    /// <summary>
    /// Handle Input and Logic of Chip Attack.
    /// </summary>
    /// <param name="nowTime">Current time is used to determine cooldown.</param>
    private void HandleChip(float nowTime)
    {
        var chipAttack = false;
        var attackCooledDown = Time.time > nextAttackTime;

        if (owningPlayer == 1)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                chipAttack = true;
            }

        }
        if (owningPlayer == 2)
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                chipAttack = true;
            }
        }

        //TODO check if has a chip available
        if (chipAttack && attackCooledDown)
        {
            if (!naviAsset.specialAttack)
            {
                Debug.Log("Attack not implemented. Animating", this.naviAsset);
                naviAnimator.SetTrigger("Special");
                return;
            }
            movementDelayTimeSince = -1.0f;//reset movement

            naviAsset.specialAttack.TriggerAttack(this, naviAnimator);

            nextAttackTime = nowTime + naviAsset.specialAttack.delay;//set delay
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nowTime">Current time is used to determine cooldown.</param>
    private void HandleThrow(float nowTime)
    {
        var chipAttack = false;
        var attackCooledDown = Time.time > nextAttackTime;

        if (owningPlayer == 1)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                chipAttack = true;
            }

        }
        if (owningPlayer == 2)
        {
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                chipAttack = true;
            }
        }

        //TODO check if has a chip available
        if (chipAttack && attackCooledDown)
        {
            if (!naviAsset.throwAttack)
            {
                Debug.Log("Attack not implemented. Animating", this.naviAsset);
                naviAnimator.SetTrigger("Throw");
                return;
            }
            movementDelayTimeSince = -1.0f;//reset movement

            naviAsset.throwAttack.TriggerAttack(this, naviAnimator);
            nextAttackTime = nowTime + naviAsset.throwAttack.delay;//set delay
        }

    }
       
    private void HandleActionInput_HumanPlayer()
    {
        //get the current time
        var nowTime = Time.time;

        HandleBuster(nowTime);
        HandleSword(nowTime);
        HandleChip(nowTime);
        HandleThrow(nowTime);

        //check for enter custom gauge
    }//end function

    private void HandleActionInput_AIPlayer()
    {
        Debug.Log("AI not yet set up. Actions skipped");
    }//end function

    private void StartDeath()
    {
        //play death sound
        //disable controls
        controlsDisabled = true;
        naviAnimator.SetTrigger("Death");
        //chargeAuraAnim.SetTrigger("DeathExplosion");//trigger explosion effect
        StartCoroutine(FadeToNothing());
        BattleManager.OnCombatantDeath(this);//call event or send message or somehow let everyone know that the game might be over
    }

    private IEnumerator FadeToNothing()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>() as SpriteRenderer;
        yield return new WaitForSeconds(1f);
        var color = spriteRenderer.color;
        var fadeRate = .06f;
        while(color.a > 0)
        {
            color.a /= 2.0f;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(fadeRate);
        }
    }

    public void TakeDamage(int damageAmount, Element damageElement = Element.NONE)
    {
        //animator

        //play sound

        //modify based on elemental str and weakness
        damageAmount = (int)(damageAmount * ElementalDamageManager.DetermineDamageModifier(damageElement, naviAsset.element));

        //subtract damage
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);//prevent damage falling below 0 and healing raising above max

        //keep in bounds
        if(currentHealth == 0)
        {
            StartDeath();//you have died
        }

        if(damageAmount >=5)
        {
            naviAnimator.SetBool("Damaged", true);
            controlsDisabled = true;
            coroutine_controlsDisabled = StartCoroutine(DisableControls(1.0f));//this will re-enable controls after time passes
        }
    }
    
    public void SetStartingPanel()//sets starting panel to center panel of team
    {
        if (battleTeam == BattleTeam.BLUE)
        {
            //TODO set with code instead of hard numbers
            startingPanel = panelArray.GetPanel(1, 1); // center panel of blue side
        }
        else if (battleTeam == BattleTeam.RED)
        {
            //TODO set with code instead of hard numbers
            startingPanel = panelArray.GetPanel(4, 1); //center panel of red side
        }
    }//end SetStartingPanel()

    /// <summary>
    /// hard start point select.  DOES NOT TAKE INTO ACCOUNT BATTLE TEAM
    /// </summary>
    /// <param name="x">X-coord of panel</param>
    /// <param name="y">Y-coord of panel</param>
    public void SetStartingPanel(int x, int y)
    {
        startingPanel = panelArray.GetPanel(x, y);
    }

    /// <summary>
    /// returns +1 or -1.  Used to help determine direction and facing
    /// </summary>
    /// <returns>+1 or -1</returns>
    public int GetOrientation()
    {
        return naviAsset.orientation;
    }

    /// <summary>
    /// Get the coordinates of the panel that this navi is currently occupying.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void GetCurrentPanelCoordinates(out int x, out int y)
    {
        x = currentPanelX;
        y = currentPanelY;
    }

    public Panel GetCurrentPanel()
    {
        return currentPanel;
    }

    /// <summary>
    /// Syncs coordinates to panel currently occupying.
    /// </summary>
    public void UpdateCurrentPanelCoordinates()
    {
        panelArray.GetPanelCoordinates(currentPanel, ref currentPanelX, ref currentPanelY);//get the coordinates for this panel
    }
}

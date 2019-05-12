using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class NaviController_Battle : MonoBehaviour
{
    private static PanelArray panelArray;
    [SerializeField]
    private NaviAsset naviAsset;
    [SerializeField]
    private HealthColors healthColorsAsset;
    [SerializeField]
    private Animator bodyAnim;
    [SerializeField]
    private Animator chargeAuraAnim;

    //health and element stuff
    private Element element = Element.NONE;
    private int maxHealth = 9999;//arbitrary defaults
    [SerializeField]
    private int currentHealth = 4444;//arbitrary default
    private bool healthFlashing = false;

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
    
    //Attacks
    private BaseAttack busterAttack;
    private BaseAttack chargedBusterAttack;
    private BaseAttack swordAttack;
    private BaseAttack chargedSwordAttack;
    private BaseAttack throwAttack;
    private BaseAttack specialAttack;

    //current coordinates on board
    private int currentPanelX = -1; //init coordinates of current panel to invalid panel
    private int currentPanelY = -1; //init coordinates of current panel to invalid panel

    private int movementX = 0;
    private int movementY = 0;

    private int orientation = 0;// +1 for facing right, -1 for facing left -- used for attacks

    //movement delay stuff
    private const float movementDelay = 0.15f;//.15f seems good
    private float movementDelayTimeSince;
    private bool movementDelayCoolingDown = false;
    private bool isMoving = false;

    //sprite stuff
    private Vector3 spriteOffset;
    private bool flipSpriteX = true;

    private float naviMoveSpeed = 1.0f;//can increase rate of move cooldown -- speed < 1 (.95) is faster than 1.05

    private Panel currentPanel = null;

    public Image emotionWindow;
    public Text healthText;

    public Panel startingPanel; //panel that the Player WANTS to start at.  GameManager or BoardManager will actually determine where to start

    public BattleTeam battleTeam = BattleTeam.BLUE;//default
    public bool isPlayer = false;
    public int owningPlayer = 0;

    public List<StatusEffect> statusAilments = new List<StatusEffect>();

    void Awake()
    {

        if (GameObject.Find("PlayerManager") != null)//look for naviAssets from previous scenes
        {
            Debug.Log("PlayerManager found. Navi Assets loaded from previous scene.");
            var pm = GameObject.Find("PlayerManager").GetComponent<PlayerManager>() as PlayerManager;
            pm.GetNaviStats(ref this.battleTeam, ref this.naviAsset, ref this.maxHealth, ref this.currentHealth);

        }
        else
        {
            Debug.Log("PlayerManager not found. Using default navi Assets and Inspector values.");
            //uses default health values or values set in Inspector
        }

        InitializeNavi();//initializes from naviAsset

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
	void Start () {
        SetOrientation();
        currentPanel = startingPanel;
        UpdateCurrentPanelCoordinates();
        InitializeDelays();
        InitializeHealth();
        UpdateHealthVisuals();

    }

    // Update is called once per frame
    void Update()
    {
        var stunned = false;
        isMoving = false;

        //handle stun
        foreach (var ailment in statusAilments)
        {
            if(Time.time < ailment.effectEndTime)
            {
                //effect triggers
                if(ailment.type == StatusAilmentType.STUN)
                {
                    stunned = true;
                }
            }
        }

        if (stunned)
        {
            return;
        }

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

    private void OnValidate()
    {
        UpdateHealthVisuals();

        if (naviAsset)
        {
            if(emotionWindow) emotionWindow.sprite = naviAsset.emotionWindow;
            spriteOffset = naviAsset.spriteOffset;
            orientation = naviAsset.orientation;

            //TODO sprite renderer updates to idle animation
        }

    }

    private void UpdateHealthVisuals()
    {
        if (healthText)
        {
            healthText.text = currentHealth.ToString();
            if (healthColorsAsset) healthColorsAsset.SetHealthColor(currentHealth, maxHealth, healthText);
        }

    }

    private void InitializeDelays()//this function sets the delays
    {
        movementDelayTimeSince = movementDelay;//player should be able to move right away
        nextAttackTime = 0.0f;
    }

    private void InitializeHealth()
    {
        healthText.text = currentHealth.ToString();
        //Debug.Log("CurrentHealth: " + currentHealth.ToString());//print test
        healthColorsAsset.SetHealthColor(currentHealth, maxHealth, healthText);
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

    private void ConfigureSpriteOffset()
    {
        if(battleTeam == BattleTeam.BLUE)
        {
            spriteOffset.x *= -1;
            if(orientation == -1)
            {
                this.flipSpriteX = true;
            }
        }
        if(battleTeam == BattleTeam.RED && orientation == 1)
        {
            this.flipSpriteX = true;
        }
    }

    private void InitializeNavi()
    {
        //load animator components
        this.bodyAnim = this.GetComponent<Animator>();
        this.bodyAnim.runtimeAnimatorController = naviAsset.runtimeAnimController;

        //load visuals
        this.emotionWindow.sprite = naviAsset.emotionWindow;
        this.spriteOffset = naviAsset.spriteOffset;
        this.orientation = naviAsset.orientation;

        ConfigureSpriteOffset();//based on battle team, fixes sprite offset

        //load battle stuff
        this.naviMoveSpeed = naviAsset.naviMoveSpeed;
        this.element = naviAsset.element;

        //load scriptable object abilities
        this.busterAttack = naviAsset.busterAttack;
        this.chargedBusterAttack = naviAsset.chargedBusterAttack;
        this.swordAttack = naviAsset.swordAttack;
        this.chargedSwordAttack = naviAsset.chargedSwordAttack;
        this.throwAttack = naviAsset.throwAttack;
        this.specialAttack = naviAsset.specialAttack;
    }

    public void MoveNavi(Panel targetPanel)
    {
        if(this.CanMoveToPanel(targetPanel))
        {
            //Debug.Log(currentPanel.panelOccupant.name);//print test
            this.transform.position = targetPanel.GetPosition() + spriteOffset;//move sprite to new location, offset the sprite to be at center of board
            currentPanel.LeavePanel(this.gameObject);//clear current Panel's of any occupants // THIS LINE CAUSES NULLREFERENCEEXCEPTION!
            targetPanel.OccupyPanel(this.gameObject);//target Panel now has this object as an occupant
            currentPanel = targetPanel; //this object now knows which Panel it is on
            UpdateCurrentPanelCoordinates();//update coordinates of current panel
            movementDelayTimeSince = 0.0f;//reset movement delay
            
            bodyAnim.SetTrigger("Move");//trigger animation
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
            GetMoveInput_Numbers();
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

    private void GetMoveInput_Numbers()
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

    [System.Obsolete("I don't know why this doesn't work as well.  Use GetMoveInput_Numbers()")]
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
            movementDelayTimeSince += Time.deltaTime * naviMoveSpeed;
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

            if (busterCharge >= busterAttack.chargeTime)//if fully charged
            {
                bodyAnim.SetTrigger(busterAttack.GetAnimatorMessage(true));//show animation
                //specialAttack.DoAttack();
                //TODO play sound
                chargedBusterAttack.TriggerAttack(this, true);

            }
            else//regular buster shot
            {
                bodyAnim.SetTrigger(busterAttack.GetAnimatorMessage());//fire the buster
                //busterAttack.DoAttack();
                //TODO play sound
                //TODO Send damage or something combat related
                busterAttack.TriggerAttack(this);

            }
            //reset values after shot
            busterCharge = 0;//reset charge amount when button is released
            nextAttackTime = nowTime + busterAttack.delay;//always a delay between attacks
            movementDelayTimeSince = 0.0f;//reset movement delay after firing
        }

        if (busterIsCharging)//charge up buster numbers
        {
            busterCharge += Time.deltaTime;
            chargeAuraAnim.SetFloat("BusterCharge", busterCharge / busterAttack.chargeTime);//play charging animation
            //swordChargeAmount = 0.0f; //TODO Buster and Sword cannot charge simultaneously

        }
        else
        {
            chargeAuraAnim.SetFloat("BusterCharge", 0.0f);//no charge
            //chargeAuraAnim.SetTrigger("ResetCharge"); //reset animation
        }
    }//end HandleBuster()

    /// <summary>
    /// 
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
            if (swordCharge >= swordAttack.chargeTime)//if fully charged
            {
                bodyAnim.SetTrigger("ChargeSword");//show animation
                //TODO play sound
                //TODO Send damage or something combat related
                chargedSwordAttack.TriggerAttack(this, true);
            }
            else//regular sword swing
            {
                bodyAnim.SetTrigger("Sword");//swing the sword
                //TODO play sound
                //TODO Send damage or something combat related
                swordAttack.TriggerAttack(this);

            }
            //reset values after shot
            swordCharge = 0;//reset charge amount when button is released
            nextAttackTime = nowTime + swordAttack.delay;//reset time since last sword shot
            movementDelayTimeSince = 0.0f;//reset movement delay after firing

        }

        if (swordIsCharging)//charge up buster numbers
        {
            swordCharge += Time.deltaTime;
            chargeAuraAnim.SetFloat("SwordCharge", swordCharge / swordAttack.chargeTime);//play animation, sending charge percent
            //busterChargeAmount = 0.0f; //TODO Buster and Sword cannot charge simultaneously
        }
        else
        {
            chargeAuraAnim.SetFloat("SwordCharge", 0.0f);
        }
    }//end HandleSword_Letters()

    /// <summary>
    /// 
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
            bodyAnim.SetTrigger("Special");
            movementDelayTimeSince = -1.0f;//reset movement
            
            specialAttack.TriggerAttack(this);

            nextAttackTime = nowTime + specialAttack.delay;//set delay
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
            if (Input.GetKeyDown(KeyCode.V))
            {
                chipAttack = true;
            }

        }
        if (owningPlayer == 2)
        {
            if (Input.GetKeyDown(KeyCode.RightShift))
            {
                chipAttack = true;
            }
        }

        //TODO check if has a chip available
        if (chipAttack && attackCooledDown)
        {
            bodyAnim.SetTrigger("Throw");
            movementDelayTimeSince = -1.0f;//reset movement

            throwAttack.TriggerAttack(this);
            nextAttackTime = nowTime + throwAttack.delay;//set delay
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

    public void TakeDamage(int damageAmount, StatusEffect effect, Element damageElement = Element.NONE)
    {
        //animator

        //play sound

        //modify based on elemental str and weakness
        damageAmount = (int)(damageAmount * ElementalDamageManager.DetermineDamageModifier(damageElement, element));

        //subtract damage
        currentHealth -= damageAmount;

        //keep in bounds
        currentHealth = currentHealth < 0 ? 0 : currentHealth;

        //update visuals 
        UpdateHealthVisuals();

        //handle status effect (poison, stun, pushback, etc)
        //if (effect.duration > 0)
        //{
        //    statusAilments.Add(effect);
        //}

    }

    public Vector3 GetSpriteOffset()
    {
        return spriteOffset;
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

    public void SetStartingPanel(Panel desiredPanelToStartOn)
    {
        for (var column = 0; column < PanelArray.GetBoardColumnsCount(); ++column)//iterate through all Panels in PanelArray
        {
            for (var row = 0; row < PanelArray.GetBoardRowsCount(); ++row)
            {
                if (panelArray.GetPanel(column, row) == desiredPanelToStartOn)//if panel being searched for matches one
                {
                    startingPanel = desiredPanelToStartOn;//make this the panel to start on
                    return;
                }//end if 
            }//end for rows
        }//end for columns
        Debug.Log("ERROR: This panel could not be found. WTF???");
    }//end SetStartingPanel()

    public void SetStartingPanel(int x, int y)
        //hard start point select.  DOES NOT TAKE INTO ACCOUNT BATTLE TEAM
    {
        startingPanel = panelArray.GetPanel(x, y);
    }

    public bool CanMoveToPanel(Panel desiredPanel)
    {
        var canMove = true;

        //these conditions cause check to fail
        if (desiredPanel.GetPanelTeam() != battleTeam)//if panel is not on same team
        {
            canMove = false;
        }
        else if (desiredPanel.GetPanelType() == PanelType.BROKEN)//if panel is BROKEN
        {
            //TODO check if Navi can fly or float
            canMove = false;
        }
        else if (desiredPanel.GetPanelType() == PanelType.MISSING)// if panel is MISSING
        {
            //TODO check if Navi can fly or float
            canMove = false;
        }

        return canMove;
    }//end NaviCanMoveToPanel()

    public void SetOrientation(){
        //sets NaviFacing left or right
        switch(battleTeam){
            case BattleTeam.BLUE:
                orientation = 1;
                break;

            case BattleTeam.RED:
                orientation = -1;
                //this.gameObject.GetComponent<SpriteRenderer>().flipX = flipSpriteX;
                break;
        }//end switch
    }//end SetOrientation()

    /// <summary>
    /// returns +1 or -1.  Used to help determine direction and facing
    /// </summary>
    /// <returns>+1 or -1</returns>
    public int GetOrientation()
    {
        return orientation;
    }

    /// <summary>
    /// Syncs coordinates to panel currently occupying.
    /// </summary>
    public void UpdateCurrentPanelCoordinates()
    {
        panelArray.GetPanelCoordinates(currentPanel, ref currentPanelX, ref currentPanelY);//get the coordinates for this panel
    }
}

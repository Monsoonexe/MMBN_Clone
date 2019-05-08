using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class NaviController_Battle : MonoBehaviour {
    public NaviAsset naviAsset;
    public HealthColors healthColorsAsset;
    public Animator bodyAnim;
    public Animator chargeAuraAnim;

    //health and element stuff
    private Element element = Element.NONE;
    private int maxHealth = 9999;//arbitrary defaults
    private int currentHealth = 4444;//arbitrary default
    private bool healthFlashing = false;

    //buster stuff
    private bool busterIsCharging = false;
    private float busterDelay = 0.25f;
    private float busterDelayTimeSince;
    private float busterChargeRate = 1.0f;//multiplier
    private float busterChargeMax = 3.0f;//max power needed before charge shot
    private float busterChargeAmount = 0.0f;
    private float busterCooldownRate = 1.0f;
    private float busterDamageMod = 1.0f;
    private float busterChargeDamageMod = 1.0f;
    private float busterDefenseMod = 1.0f;

    //sword stuff
    private bool swordIsCharging = false;
    private float swordDelay = 0.45f;
    private float swordDelayTimeSince;
    private float swordChargeRate = 0.5f;
    private float swordChargeMax = 3.0f;
    private float swordChargeAmount = 0.0f;
    private float swordCooldownRate = 1.0f;
    private float swordDamageMod = 1.0f;
    private float swordChargeDamageMod = 1.0f;
    private float swordDefenseMod = 1.0f;

    //Chip Stuff
    private float chipDelay = 1.0f;
    private float chipDelayTimeSince;

    //Attacks
    private BaseAttack busterAttack;
    private BaseAttack chargedBusterAttack;
    private BaseAttack swordAttack;
    private BaseAttack chargedSwordAttack;
    private BaseAttack throwAttack;
    private BaseAttack specialAttack;

    private bool isMoving = false;
    private bool flipSpriteX = true;

    //current coordinates on board
    private int currentPanelX = -1; //init coordinates of current panel to invalid panel
    private int currentPanelY = -1; //init coordinates of current panel to invalid panel

    private int movementX = 0;
    private int movementY = 0;

    private int orientation = 0;// +1 for facing right, -1 for facing left -- used for attacks

    //hide these values when properly set up
    private const float movementDelay = 0.15f;//.15f seems good
    private float movementDelayTimeSince;
    private bool movementDelayCoolingDown = false;

    private Vector3 spriteOffset;

    private float naviMoveSpeed = 1.0f;//can increase rate of move cooldown -- speed < 1 (.95) is faster than 1.05

    private Panel currentPanel = null;

    public Image emotionWindow;
    public Text healthText;

    public PanelArray panelArray;
    public Panel startingPanel; //panel that the Player WANTS to start at.  GameManager or BoardManager will actually determine where to start

    public BattleTeam battleTeam = BattleTeam.BLUE;//default
    public bool isPlayer = false;
    public int owningPlayer = 0;

    public List<StatusEffect> statusAilments = new List<StatusEffect>();

    public Panel GetDesiredStartingPanel()
    // gives where the Navi WANTS to start when asked. GameManager / Board Manager makes the call.
    {
        if (startingPanel == null)
        {
            SetStartingPanel();
        }
        return startingPanel;
    }

    void Awake()
    {

        if (GameObject.Find("PlayerManager") != null)//look for naviAssets from previous scenes
        {
            Debug.Log("PlayerManager found. Navi Assets loaded from previous scene.");
            PlayerManager pm = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
            pm.GetNaviStats(ref this.battleTeam, ref this.naviAsset, ref this.maxHealth, ref this.currentHealth);

        }
        else
        {
            Debug.Log("PlayerManager not found. Using default navi Assets and Inspector values.");
            //uses default health values or values set in Inspector
        }
        InitializeNavi();//initializes from naviAsset

        if (panelArray == null)
        {
            panelArray = GameObject.FindGameObjectWithTag("PanelArray").GetComponent<PanelArray>();
        }

        if (startingPanel == null)
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

    }

    private void InitializeDelays()//this function sets the delays
    {
        movementDelayTimeSince = movementDelay;//player should be able to move right away
        busterDelayTimeSince = 0.0f; //buster should not be used right away
        swordDelayTimeSince = 0.0f; //sword should not be usuable immediately
        chipDelayTimeSince = 0.0f; //chips unavailable for a time
    }

    private void InitializeHealth()
    {
        healthText.text = currentHealth.ToString();
        //Debug.Log("CurrentHealth: " + currentHealth.ToString());//print test
        healthColorsAsset.SetHealthColor(currentHealth, maxHealth, healthText);
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
        this.bodyAnim = this.GetComponent<Animator>();
        this.bodyAnim.runtimeAnimatorController = naviAsset.runtimeAnimController;

        this.emotionWindow.sprite = naviAsset.emotionWindow;
        this.spriteOffset = naviAsset.spriteOffset;
        this.orientation = naviAsset.orientation;
        ConfigureSpriteOffset();//based on battle team, fixes sprite offset
        this.naviMoveSpeed = naviAsset.naviMoveSpeed;
        this.element = naviAsset.element;

        this.busterDelay = naviAsset.busterDelay;
        this.busterChargeRate = naviAsset.busterChargeRate;
        this.busterChargeMax = naviAsset.busterChargeMax;
        this.busterCooldownRate = naviAsset.busterCooldownRate;
        this.busterDamageMod = naviAsset.busterDamageMod;
        this.busterChargeDamageMod = naviAsset.busterChargeDamageMod;
        this.busterDefenseMod = naviAsset.busterDefenseMod;

        this.swordDelay = naviAsset.swordDelay;
        this.swordChargeRate = naviAsset.swordChargeRate;
        this.swordChargeMax = naviAsset.swordChargeMax;
        this.swordCooldownRate = naviAsset.swordCooldownRate;
        this.swordDamageMod = naviAsset.swordDamageMod;
        this.swordChargeDamageMod = naviAsset.swordChargeDamageMod; 
        this.swordDefenseMod = naviAsset.swordDefenseMod;
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
            //get movement


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

    private void GetMovementInput_Axes()
    {
        //I don't understand why this doesn't work as well
        movementX = (int)Input.GetAxisRaw("Horizontal");//this version causes the character to move 3 times for some reason
        movementY = (int)Input.GetAxisRaw("Vertical");//this version of movement causes the character to move 3 times, causing problems
    }

    private void HandleMovement_HumanPlayer()
    {
        bool skipMovement = false;


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
            Debug.Log("Move cancelled: next move would be off board. Current coords: " + currentPanelX + ", " + currentPanelY);//print test
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

    private void HandleMovement_AIPlayer()
    {
        //TODO 
        Debug.Log("AIMovement not yet set up.");
    }//end HandleMovement_AIPlayer()

    private void HandleBuster()
    {
        bool fireBuster = false;
        //get buster input for player1
        if(owningPlayer == 1)
            //TODO controls are SO with easy-to-change variables
        {
            if (Input.GetKeyDown(KeyCode.Space))//was buster button pressed down?
            {
                if (busterDelayTimeSince >= busterDelay)
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
                if (busterDelayTimeSince >= busterDelay)
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

        if (fireBuster && busterDelayTimeSince >= busterDelay)//FIRE! regular buster
        {
            if (busterChargeAmount >= busterChargeMax)//if fully charged
            {
                bodyAnim.SetTrigger("ChargeShot");//show animation
                //specialAttack.DoAttack();
                //TODO play sound
                //TODO Send damage or something combat related
            }
            else//regular buster shot
            {
                bodyAnim.SetTrigger("Buster");//fire the buster
                //busterAttack.DoAttack();
                //TODO play sound
                //TODO Send damage or something combat related

            }
            //reset values
            busterChargeAmount = 0.0f;//reset charge amount when button is released
            busterDelayTimeSince = 0.0f;//reset time since last buster shot
            swordDelayTimeSince = 0.0f;//reset time since last sword shot
            movementDelayTimeSince = 0.0f;//reset movement delay after firing
        }

        else
        {
            busterDelayTimeSince += Time.deltaTime * busterCooldownRate;//buster cooldown
        }

        if (busterIsCharging)//charge up buster numbers
        {
            busterChargeAmount += Time.deltaTime * busterChargeRate;//begin adding to charge
            chargeAuraAnim.SetFloat("BusterCharge", busterChargeAmount / busterChargeMax);//play charging animation
            //swordChargeAmount = 0.0f; //TODO Buster and Sword cannot charge simultaneously

        }
        else
        {
            chargeAuraAnim.SetFloat("BusterCharge", 0.0f);//no charge
            //chargeAuraAnim.SetTrigger("ResetCharge"); //reset animation
        }
    }//end HandleBuster()

    private void HandleSword()
    {
        bool fireSword = false;


        if(owningPlayer == 1)
        {
            if (Input.GetKeyDown(KeyCode.C))//was sword button pressed down?
            {
                if (swordDelayTimeSince >= swordDelay)
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
                if (swordDelayTimeSince >= swordDelay)
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

        if (fireSword && swordDelayTimeSince >= swordDelay)//FIRE!
        {
            if (swordChargeAmount >= swordChargeMax)//if fully charged
            {
                bodyAnim.SetTrigger("ChargeSword");//show animation
                //TODO play sound
                //TODO Send damage or something combat related
            }
            else//regular sword swing
            {
                bodyAnim.SetTrigger("Sword");//swing the sword
                //TODO play sound
                //TODO Send damage or something combat related

            }
            //reset values
            swordChargeAmount = 0.0f;//reset charge amount when button is released
            swordDelayTimeSince = 0.0f;//reset time since last sword shot
            busterDelayTimeSince = 0.0f;//reset buster attack as well
            movementDelayTimeSince = 0.0f;//reset movement delay after firing

        }

        else
        {
            swordDelayTimeSince += Time.deltaTime * swordCooldownRate;//buster cooldown
        }

        if (swordIsCharging)//charge up buster numbers
        {
            swordChargeAmount += Time.deltaTime * swordChargeRate;//begin adding to charge
            chargeAuraAnim.SetFloat("SwordCharge", swordChargeAmount / swordChargeMax);//play animation, sending charge percent
            //busterChargeAmount = 0.0f; //TODO Buster and Sword cannot charge simultaneously
        }
        else
        {
            chargeAuraAnim.SetFloat("SwordCharge", 0.0f);
            //chargeAuraAnim.SetTrigger("ResetCharge"); //reset animation
        }
    }//end HandleSword_Letters()

    private void HandleChip()
    {
        bool chipAttack = false;
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
        if (chipAttack && chipDelayTimeSince >= chipDelay)
        {
            bodyAnim.SetTrigger("Special");
            movementDelayTimeSince = -1.0f;//reset movement
            swordDelayTimeSince = -1.0f;
            busterDelayTimeSince = -1.0f;
        }

        chipDelayTimeSince += Time.deltaTime;
    }

    private void HandleThrow()
    {
        bool chipAttack = false;
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
        if (chipAttack && chipDelayTimeSince >= chipDelay)
        {
            bodyAnim.SetTrigger("Throw");
            movementDelayTimeSince = -1.0f;//reset movement
            swordDelayTimeSince = -1.0f;
            busterDelayTimeSince = -1.0f;
        }

        chipDelayTimeSince += Time.deltaTime;
    }
       
    private void HandleActionInput_HumanPlayer()
    {
        if(owningPlayer == 1)
        {
            HandleBuster();
            HandleSword();
            HandleChip();
            HandleThrow();
            //TODO pause
        }
        else if(owningPlayer == 2)
        {
            HandleBuster();
            HandleSword();
            HandleChip();
            HandleThrow();


        }

        //check for enter custom gauge
    }//end function

    private void HandleActionInput_AIPlayer()
    {
        Debug.Log("AI not yet set up. Actions skipped");
    }//end function
	
	// Update is called once per frame
	void Update () {
        isMoving = false;
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
        //update health text and color with current health
        healthText.text = currentHealth.ToString();
        healthColorsAsset.SetHealthColor(currentHealth, maxHealth, healthText);
	}//end Update()

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
    }

    public void SetStartingPanel(Panel desiredPanelToStartOn)
    {
        for (int column = 0; column < PanelArray.GetBoardColumnsCount(); ++column)//iterate through all Panels in PanelArray
        {
            for (int row = 0; row < PanelArray.GetBoardRowsCount(); ++row)
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
        //these conditions cause check to fail
        if (desiredPanel.GetPanelTeam() != battleTeam)//if panel is not on same team
        {
            return false;
        }
        else if (desiredPanel.GetPanelType() == PanelType.BROKEN)//if panel is BROKEN
        {
            //TODO check if Navi can fly or float
            return false;
        }
        else if (desiredPanel.GetPanelType() == PanelType.MISSING)// if panel is MISSING
        {
            //TODO check if Navi can fly or float
            return false;
        }

        else return true;
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

    public int GetOrientation()
    {
        return orientation;
    }

    public void UpdateCurrentPanelCoordinates()
    {
        panelArray.GetPanelCoordinates(currentPanel, ref currentPanelX, ref currentPanelY);//update coordinates of current panel
    }
}

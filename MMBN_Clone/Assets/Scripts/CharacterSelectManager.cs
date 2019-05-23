using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public struct Coords
{
    public int x;
    public int y;
}

public class CharacterSelectManager : MonoBehaviour
{
    private Coords blueCursorCurrentPosition;
    private Coords redCursorCurrentPosition;

    private NaviAsset blueSelectedNaviAsset;
    private NaviAsset redSelectedNaviAsset;

    private bool blueSelectionUpdated = false;
    private bool redSelectionUpdated = false;
    private bool blueSelectionConfirmed = false;
    private bool redSelectionConfirmed = false;
    private bool sceneIsLoading = false;

    private readonly string battleSceneName = "MegaMan Battle Scene";//name of battle scene.unity

    private CharacterProfile[,] characterProfiles;

    [SerializeField]
    private CharacterProfile[] listOfCharacterProfiles;

    [SerializeField]
    private PlayerManager playerManager;//singleton'd and loads between scenes, hopefully

    [SerializeField]
    private CharacterProfile[] charListRow0;
    [SerializeField]
    private CharacterProfile[] charListRow1;

    [SerializeField]
    private GameObject blueCursor;
    [SerializeField]
    private GameObject redCursor;

    [SerializeField]
    private CharacterSelectPET bluePet;
    [SerializeField]
    private CharacterSelectPET redPet;

    [SerializeField]
    private Animator screenTransitionAnimator;
    [SerializeField]
    private TMP_InputField healthText;

    private void Awake()
    {
        //TODO
        //initialize characterProfiles to be 1 list. 
        //possible to move up and down in character selection screen
    }


    // Use this for initialization
    void Start () {
        //set default cursor starting positions
        blueCursorCurrentPosition.x = 0;
        blueCursorCurrentPosition.y = 0;
        redCursorCurrentPosition.x = listOfCharacterProfiles.Length - 1;
        redCursorCurrentPosition.y = 0;//TODO first value should be number of rows - 1
        //move cursors to starting positions

        MoveCursor(blueCursor, blueCursorCurrentPosition);
        blueSelectedNaviAsset = listOfCharacterProfiles[blueCursorCurrentPosition.x].naviAssetSO;
        bluePet.SetNaviPreview(blueSelectedNaviAsset.runtimeAnimController, blueSelectedNaviAsset.orientation, blueSelectedNaviAsset.READY);

        MoveCursor(redCursor, redCursorCurrentPosition);
        redSelectedNaviAsset = listOfCharacterProfiles[redCursorCurrentPosition.x].naviAssetSO;
        redPet.SetNaviPreview(redSelectedNaviAsset.runtimeAnimController, redSelectedNaviAsset.orientation, redSelectedNaviAsset.READY);

        if (playerManager == null)
        {
            playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        }
        //TODO count children and add to list
    }
	
	// Update is called once per frame
	void Update () {
        //get input from each player
        if(blueSelectionConfirmed && redSelectionConfirmed && !sceneIsLoading)
        {
            //Debug.Log("CharacterSelection is complete! Now load the scene.");//print test
            playerManager.SetNavi(BattleTeam.BLUE, blueSelectedNaviAsset);
            playerManager.SetNavi(BattleTeam.RED, redSelectedNaviAsset);
            playerManager.SetNaviHealthForMatch(healthText.text);
            StartCoroutine(LoadBattleScene());//run animation and load next screen
        }
        //blueCursor.transform.localPosition = new Vector3(0, 0, 0);

        GetMoveInput();
        GetSelection();
    }

    public IEnumerator LoadBattleScene()
    {
        int waitForNaviAnimationsToFinishDelay = 1;
        int waitForScreenAnimationToFinishDelay = 3;//why not just use the length of the animation clip? you ask?

        sceneIsLoading = true;
        yield return new WaitForSeconds(waitForNaviAnimationsToFinishDelay);
        screenTransitionAnimator.SetTrigger("JackIn_Blue");//do the animation

        //Debug.Log("Clip Length: " + screenTransitionAnimator.GetCurrentAnimatorStateInfo(0).length);// print test
        //yield return new WaitForSeconds(screenTransitionAnimator.GetCurrentAnimatorStateInfo(0).length); //WTF?!?! why doesn't this work??
        yield return new WaitForSeconds(waitForScreenAnimationToFinishDelay);
        //change scene
        //TODO screen animation is a load screen in and of itself
        SceneManager.LoadScene(battleSceneName);
    }

    private void MoveCursor(GameObject cursor, Coords newCoords)
    {
        if (newCoords.y != 0)
        {
            Debug.Log("ERROR: Multi-Row character selection not yet integrated. skipping");//
        }

        //set transform to next or previous game object
        //this is the last way I would have imagined, but it finally works, dammit
        cursor.transform.SetParent(listOfCharacterProfiles[newCoords.x].transform);
        cursor.transform.localPosition = new Vector3(0, 0, 0);
    }

    private void GetMoveInput()
    {
        //p1 blue
        if (Input.GetKeyDown(KeyCode.W))//move up
        {
            blueSelectionUpdated = true;
            blueCursorCurrentPosition.y += 1;
        }
        else if (Input.GetKeyDown(KeyCode.S))//move down
        {
            blueSelectionUpdated = true;
            blueCursorCurrentPosition.y -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.A))//move left
        {
            if (blueCursorCurrentPosition.x - 1 >= 0)
            {
                blueSelectionUpdated = true;
                blueCursorCurrentPosition.x -= 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.D))//move right
        {
            if (blueCursorCurrentPosition.x + 1 < listOfCharacterProfiles.Length)
            {
                blueSelectionUpdated = true;
                blueCursorCurrentPosition.x += 1;
            }
        }

        //p2 red
        if (Input.GetKeyDown(KeyCode.Keypad8))//move up
        {
            redSelectionUpdated = true;
            redCursorCurrentPosition.y += 1;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5))//move down
        {
            redSelectionUpdated = true;
            redCursorCurrentPosition.y -= 1;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4))//move left
        {
            if (redCursorCurrentPosition.x - 1 >= 0)
            {
                redSelectionUpdated = true;
                redCursorCurrentPosition.x -= 1;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6))//move right
        {
            if (redCursorCurrentPosition.x + 1 < listOfCharacterProfiles.Length)
            {
                redSelectionUpdated = true;
                redCursorCurrentPosition.x += 1;
            }
        }

        if (redSelectionUpdated) //update cursor visual positions
        {
            MoveCursor(redCursor, redCursorCurrentPosition);//TODO integrate vertical movement
            if (!redSelectionConfirmed)
            {
                redSelectedNaviAsset = listOfCharacterProfiles[redCursorCurrentPosition.x].naviAssetSO;
                redPet.SetNaviPreview(redSelectedNaviAsset.runtimeAnimController, redSelectedNaviAsset.orientation, redSelectedNaviAsset.READY);
            }


        }
        if (blueSelectionUpdated)
        {
            MoveCursor(blueCursor, blueCursorCurrentPosition);//TODO integrate vertical movement
            if (!blueSelectionConfirmed)
            {
                blueSelectedNaviAsset = listOfCharacterProfiles[blueCursorCurrentPosition.x].naviAssetSO;
                bluePet.SetNaviPreview(blueSelectedNaviAsset.runtimeAnimController, blueSelectedNaviAsset.orientation, blueSelectedNaviAsset.READY);

            }

        }
        //reset flags
        redSelectionUpdated = false;
        blueSelectionUpdated = false;
    }//end GetMoveInput()

    private void GetSelection()
    {
        if(Input.GetKeyDown(KeyCode.Space))//blue player select
        {
            if (!blueSelectionConfirmed)
            {
                if (blueSelectedNaviAsset.READY)
                {
                    blueSelectionConfirmed = true;
                    bluePet.OnSelectionConfirm();
                    //TODO animate cursor
                }
                else
                {
                    Debug.Log("Navi not selected. Not yet Integrated! Do work, Rich!");//print test
                                                                                       //TODO make navi flash or something
                }
            }
            else
            {
                blueSelectionConfirmed = false;
                blueSelectionUpdated = true;
            }

            
        }

        if (Input.GetKeyDown(KeyCode.Keypad0) )//blue player select
        {
            if (!redSelectionConfirmed)
            {
                if (redSelectedNaviAsset.READY)
                {
                    redSelectionConfirmed = true;
                    redPet.OnSelectionConfirm();
                    //TODO animate cursor
                }
                else
                {
                    Debug.Log("Navi not selected. Not yet Integrated! Do work, Rich!");//print test
                                                                                       //TODO make navi flash or something
                }
            }
            else
            {
                redSelectionConfirmed = false;
                redSelectionUpdated = true;
            }
            

        }
    }

}

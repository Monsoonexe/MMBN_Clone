using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPET : MonoBehaviour {
    public BattleTeam battleTeam;

    public Image screenImage;
    public Animator naviPreviewAnim;
    public SpriteRenderer spriteRenderer;

    private Color grayScreenColor = new Color(0, 0, 0, .2f);
    private Color naviSelectedScreenColor = new Color(81 / 255, 2255 / 255, 89 / 255, .4f);
    private Color invalidNaviScreenColor = new Color(255 / 255, 90 / 255, 40 / 255, .3f);

    private Coroutine coroutine_randomAnimator;

    private readonly string[] animatorTriggerStrings = {"Buster", "ChargeShot", "ChargeSword", "Special", "Sword", "Throw"};
    
    private void Start()
    {
        screenImage.color = grayScreenColor;
        coroutine_randomAnimator = StartCoroutine(RandomlyAnimate());
    }

    private void Update()
    {
        
    }

    private IEnumerator RandomlyAnimate()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2.0f, 5));

            var randomIndex = Random.Range(0, animatorTriggerStrings.Length);//get a random index of triggers
            naviPreviewAnim.SetTrigger(animatorTriggerStrings[randomIndex]);//send trigger to animator
            //pick a random trigger
        }
    }

    public void SetNaviPreview(RuntimeAnimatorController newController, int orientation, bool readyToPlay)
    {
        
        if(battleTeam == BattleTeam.RED && orientation == 1)
        {
            spriteRenderer.flipX = true;
        }
        else if(battleTeam == BattleTeam.RED && orientation == -1)
        {
            spriteRenderer.flipX = false;
        }
        if(battleTeam == BattleTeam.BLUE && orientation == 1)
        {
            spriteRenderer.flipX = false;
        }
        else if (battleTeam == BattleTeam.BLUE && orientation == -1)
        {
            spriteRenderer.flipX = true;
        }

        naviPreviewAnim.runtimeAnimatorController = newController;
        naviPreviewAnim.SetTrigger("Move");
        if (!readyToPlay)//change screen color to red if not ready
        {
            screenImage.color = invalidNaviScreenColor;
            //Debug.Log("HERE!");//print test
        }
        else
        {
            screenImage.color = grayScreenColor;
        }
    }

    public void OnSelectionConfirm()
    {
        screenImage.color = naviSelectedScreenColor;
        naviPreviewAnim.SetTrigger("Sword");
        //play selection sound
    }
}

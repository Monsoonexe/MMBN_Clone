using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentCombatant : MonoBehaviour {
    public Image combatantPortrait;
    public Image playerColorHalo;
    public Image defeatedGray;

    public void Start()
    {
        playerColorHalo.gameObject.SetActive(false);
    }

    public void SetPlayerColor(Color newColor)
    {
        playerColorHalo.color = newColor;
        playerColorHalo.gameObject.SetActive(true);
    }

    public void Defeated()
    {
        defeatedGray.gameObject.SetActive(true);
    }

}

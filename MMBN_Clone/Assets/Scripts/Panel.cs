﻿using UnityEngine;
using UnityEngine.UI;

[SelectionBase]
public class Panel : MonoBehaviour
{
    //private PanelType lastUpdatePanelType;
    private NaviController_Battle panelOccupant;
    //private BattleTeam lastUpdatePanelTeam;

    public PanelType panelType = PanelType.NULL;
    public BattleTeam panelTeam = BattleTeam.BLUE;
    public Image panelImage;
    public Image panelTypeImage;
    public Vector3 panelImageOffset;

    [SerializeField]
    private Transform occupantAnchorPoint;

    public Vector3 OccupantAnchorPoint { get => occupantAnchorPoint.position; } // readonly

    [Header("Panel Sprites")]
    [Header("*Blue Team")]
    public Sprite nullPanel_B;
    public Sprite broken_B;
    public Sprite cracked_B;
    public Sprite hole_B;

    [Header("*Red Team")]
    public Sprite nullPanel_R;
    public Sprite broken_R;
    public Sprite cracked_R;
    public Sprite hole_R;

    [Header("*Types")]
    public Sprite grass;
    public Sprite holy;
    public Sprite ice;
    public Sprite lava;
    public Sprite metal;
    public Sprite poison;
    public Sprite sand;
    public Sprite sea;
    public Sprite warning;
    public Sprite noType;

    private void OnValidate()
    {
        UpdatePanelType();//check to see if anything has changed

    }

    private void UpdatePanelType()
    {
        SetPanelImage(panelType, panelTeam);
        panelTypeImage.transform.position = panelImage.transform.position + panelImageOffset;
        
    }//end UpdatePanelType()

    private void SetPanelImage(PanelType panelType, BattleTeam battleTeam){
        switch (panelType)
        {
            case PanelType.BROKEN:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = broken_B;
                }
                else
                {
                    panelImage.sprite = broken_R;
                }
                panelTypeImage.sprite = noType;
                break;
            case PanelType.CRACKED:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = cracked_B;
                }
                else
                {
                    panelImage.sprite = cracked_R;
                }
                panelTypeImage.sprite = noType;
                break;
            case PanelType.GRASS:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = nullPanel_B;
                }
                else
                {
                    panelImage.sprite = nullPanel_R;
                }
                panelTypeImage.sprite = grass;
                break;
            case PanelType.HOLY:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = nullPanel_B;
                }
                else
                {
                    panelImage.sprite = nullPanel_R;
                }
                panelTypeImage.sprite = holy;
                break;
            case PanelType.ICE:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = nullPanel_B;
                }
                else
                {
                    panelImage.sprite = nullPanel_R;
                }
                panelTypeImage.sprite = ice;
                break;
            case PanelType.LAVA:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = nullPanel_B;
                }
                else
                {
                    panelImage.sprite = nullPanel_R;
                }
                panelTypeImage.sprite = lava;
                break;
            case PanelType.METAL: 
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = hole_B;
                }
                else
                {
                    panelImage.sprite = hole_R;
                }
                panelTypeImage.sprite = metal;
                break;
            case PanelType.MISSING:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = hole_B;
                }
                else
                {
                    panelImage.sprite = hole_R;
                }
                panelTypeImage.sprite = noType;
                break;
            case PanelType.NULL:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = nullPanel_B;
                }
                else
                {
                    panelImage.sprite = nullPanel_R;
                }
                panelTypeImage.sprite = noType;
                break;
            case PanelType.POISON:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = nullPanel_B;
                }
                else
                {
                    panelImage.sprite = nullPanel_R;
                }
                panelTypeImage.sprite = poison;
                break;
            case PanelType.SAND:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = hole_B;
                }
                else
                {
                    panelImage.sprite = hole_R;
                }
                panelTypeImage.sprite = sand;
                break;
            case PanelType.SEA:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = nullPanel_B;
                }
                else
                {
                    panelImage.sprite = nullPanel_R;
                }
                panelTypeImage.sprite = sea;
                break;
            case PanelType.WARNING:
                if (battleTeam == BattleTeam.BLUE)
                {
                    panelImage.sprite = nullPanel_B;
                }
                else
                {
                    panelImage.sprite = nullPanel_R;
                }
                panelTypeImage.sprite = warning;
                break;
            default: Debug.Log("Default: fuck all. PanelType not found. Cannot set sprite.");
                break;

        }
    }

    public NaviController_Battle GetOccupant()
    {
        return panelOccupant;
    }

    public BattleTeam GetPanelTeam()
    {
        return panelTeam;
    }

    public PanelType GetPanelType()
    {
        return panelType;
    }

    public Image GetPanelImage()
    {
        return panelImage;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void OccupyPanel(NaviController_Battle occupant)
    {
        panelOccupant = occupant;
    }

    public void LeavePanel()
    {
        panelOccupant = null;
    }


}//end Class Definition
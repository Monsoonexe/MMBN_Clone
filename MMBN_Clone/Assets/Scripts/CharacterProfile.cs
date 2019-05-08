using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CharacterProfile : MonoBehaviour {

    public NaviAsset naviAssetSO;
    public Text naviName;
    public Image naviPortrait;

    private void Start()
    {
        //TODO  
    }

    private void Update()
    {
        naviName.text = naviAssetSO.naviName;
        naviPortrait.sprite = naviAssetSO.characterSelectionPortrait;
    }

}

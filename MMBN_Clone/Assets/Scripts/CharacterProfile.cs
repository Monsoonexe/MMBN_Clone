using UnityEngine;
using UnityEngine.UI;

public class CharacterProfile : MonoBehaviour {

    public NaviAsset naviAssetSO;
    public Text naviName;
    public Image naviPortrait;
    

    private void OnValidate()
    {
        if (naviAssetSO)//if there's a SO loaded
        {
            //update visuals
            if (naviName) naviName.text = naviAssetSO.naviName;
            if (naviPortrait) naviPortrait.sprite = naviAssetSO.characterSelectionPortrait;

        }
    }
}

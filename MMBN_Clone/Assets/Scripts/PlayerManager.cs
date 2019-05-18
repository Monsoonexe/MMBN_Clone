using UnityEngine;

public class PlayerManager : MonoBehaviour {
    private NaviAsset blueNavi;
    private int blueNaviHealth;
    private int blueNaviMaxHealth;
    private NaviAsset redNavi;
    private int redNaviHealth;
    private int redNaviMaxHealth;

    private static PlayerManager playerManagerSingleton;

    public void Start()
    {
        if (!playerManagerSingleton)
        {
            playerManagerSingleton = this;
        }
        else if(playerManagerSingleton != this)
        {

            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(playerManagerSingleton);//IMMORTALITY!!!
    }


    public void SetNavi(BattleTeam team, NaviAsset selectedNavi)
    {
        switch (team)
        {
            case BattleTeam.BLUE:
                blueNavi = selectedNavi;
                break;
            case BattleTeam.RED:
                redNavi = selectedNavi;
                break;
               
        }
    }

    public void SetNaviHealthForMatch(string healthForThisMatch)
    {
        if(int.TryParse(healthForThisMatch, out int health))
        {
            Debug.Log("Health Parsed successfully!");//print test
        }
        else
        {
            Debug.Log("Warning: HEALTH CAN ONLY BE NUMBERS! Setting health to 9999.");
           
            //fuck all
            //TODO input validation
        }

        blueNaviHealth = health;
        blueNaviMaxHealth = health;
        redNaviHealth = health;
        redNaviMaxHealth = health;
    }

    public void GetNaviStats(ref BattleTeam team, ref NaviAsset naviAsset, ref int maxHealth, ref int currentHealth)
    {
        if (team == BattleTeam.BLUE)
        {
            naviAsset = blueNavi;
            maxHealth = blueNaviMaxHealth;
            currentHealth = blueNaviHealth;
        }
        else if (team == BattleTeam.RED)
        {
            naviAsset = redNavi;
            maxHealth = redNaviMaxHealth;
            currentHealth = redNaviHealth;
        }
    }
}

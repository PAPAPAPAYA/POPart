using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeHolderScript : MonoBehaviour
{
    public string name_upgrade;
    public Material mat_upgrade;
    public Sprite icon_upgrade;
    public BubbleUpgrade.Upgrades thisUpgrade;
    
    public void ActivateUpgrade()
    {
        switch (thisUpgrade)
        {
            case BubbleUpgrade.Upgrades.fastSqueeze:
                break;
            case BubbleUpgrade.Upgrades.lineExplode:

                break;
            case BubbleUpgrade.Upgrades.boxExplode:
                break;
            case BubbleUpgrade.Upgrades.thornFan:
            default:
                break;
        }
    }
    
}
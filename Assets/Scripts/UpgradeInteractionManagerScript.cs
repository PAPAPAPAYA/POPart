using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UpgradeInteractionManagerScript : MonoBehaviour
{
    #region SINGLETON
    public static UpgradeInteractionManagerScript me;
    private void Awake()
    {
        me = this;
    }
    #endregion

    // button refs
    public GameObject option1;
    public GameObject option2;
    public GameObject option3;

    // available upgrades
    public List<GameObject> upgradePool;
    private List<GameObject> upgradeListToShuffle;

    // upgrade queue
    public int showButtonStack;
    public bool showingButtons = false;

    private void Update()
    {
        if (showButtonStack > 0 &&
            !showingButtons)
        {
            showButtonStack--;
            showingButtons = true;
            ShowButtons();
        }
        // if showing buttons, muffle music
        if (showingButtons)
        {
            AudioManager.Instance.ApplyMuffleEffect(true);
        }
        else
        {
            AudioManager.Instance.ApplyMuffleEffect(false);
        }
    }

    public void ShowButtons()
    {
        RollUpgradesToButtons();
        option1.SetActive(true);
        option2.SetActive(true);
        option3.SetActive(true);
    }

    private void RollUpgradesToButtons()
    {
        List<GameObject> upgrades = new List<GameObject>();
        upgrades.AddRange(upgradePool);
        Debug.Log(upgrades.Count);
        if(GameManager.me.upgradedCount < 3)
        {
            foreach (GameObject ug in upgradePool) 
            {
                UpgradeHolderScript uhs = ug.GetComponent<UpgradeHolderScript>();
                if(uhs.bubbleUpgrade == BubbleUpgrade.Upgrades.none && uhs.handUpgrade != HandUpgrade.HandUpgrades.none)
                {
                    upgrades.Remove(ug);
                    Debug.Log("Removed " + ug.name);
                }
            }
        }
        upgradeListToShuffle = UtilityFunctions.me.ShuffleList(upgrades);
        
        // get the first three shuffled upgrades
        UpgradeHolderScript uhs1 = upgradeListToShuffle[0].GetComponent<UpgradeHolderScript>();
        UpgradeHolderScript uhs2 = upgradeListToShuffle[1].GetComponent<UpgradeHolderScript>();
        UpgradeHolderScript uhs3 = upgradeListToShuffle[2].GetComponent<UpgradeHolderScript>();
        // check to see if bomb upgrade is obtained, if not, no [more bomb] upgrade
        uhs1 = CheckMoreBombUpgradeDependency(uhs1);
        uhs2 = CheckMoreBombUpgradeDependency(uhs2);
        uhs3 = CheckMoreBombUpgradeDependency(uhs3);
        // get the three buttons
        Button button1 = option1.GetComponent<Button>();
        Button button2 = option2.GetComponent<Button>();
        Button button3 = option3.GetComponent<Button>();
        // detect which upgrade is allocated to each button, assign corresponding OnClick() events to buttons
        DetectUpgrade(uhs1, button1);
        DetectUpgrade(uhs2, button2);
        DetectUpgrade(uhs3, button3);
    }

    private void DetectUpgrade(UpgradeHolderScript uhs, Button button)
    {
        button.onClick.AddListener(() => ActivateBubbleUpgrade(uhs.bubbleUpgrade));
        button.onClick.AddListener(() => ActivateHandUpgrade(uhs.handUpgrade));
        button.GetComponentInChildren<TextMeshProUGUI>().text = uhs.name_upgrade;
        button.GetComponentInChildren<TextMeshProUGUI>().color = uhs.textColor;
        button.GetComponent<Image>().sprite = uhs.icon_upgrade;
    }
    private void ActivateHandUpgrade(HandUpgrade.HandUpgrades upgrade)
    {
        switch (upgrade)
        {
            case HandUpgrade.HandUpgrades.none:
                break;
            case HandUpgrade.HandUpgrades.hand_fastSqueeze:
                HandUpgrade.me.SqueezeSpdUp();
                break;
            case HandUpgrade.HandUpgrades.hand_lineHand:
                if (HandUpgrade.me.lineHand)
                {
                    HandUpgrade.me.lineHandLevel++;
                }
                else
                {
                    HandUpgrade.me.lineHand = true;
                }
                break;
            case HandUpgrade.HandUpgrades.hand_xxHand:
                if (HandUpgrade.me.xxHand)
                {
                    HandUpgrade.me.xxHandLevel++;
                }
                else
                {
                    HandUpgrade.me.xxHand = true;
                }
                break;
            case HandUpgrade.HandUpgrades.hand_box:
                if (HandUpgrade.me.boxHand)
                {
                    HandUpgrade.me.boxHandLevel++;
                }
                else
                {
                    HandUpgrade.me.boxHand = true;
                }
                break;
            case HandUpgrade.HandUpgrades.moreBomb:
                HandUpgrade.me.MoreBombUpgrade();
                break;
            default:
                break;
        }
    }
    private void ActivateBubbleUpgrade(BubbleUpgrade.Upgrades upgrade)
    {
        switch (upgrade)
        {
            case BubbleUpgrade.Upgrades.none:
                break;
            case BubbleUpgrade.Upgrades.lineExplode:
                if (BubbleUpgrade.me.lineExplosion)
                {
                    BubbleUpgrade.me.lineExplodeLevel++;
                }
                else
                {
                    BubbleUpgrade.me.lineExplosion = true;
                }
                break;
            case BubbleUpgrade.Upgrades.boxExplode:
                if (BubbleUpgrade.me.boxExplosion)
                {
                    BubbleUpgrade.me.boxExplodeLevel++;
                }
                else
                {
                    BubbleUpgrade.me.boxExplosion = true;
                }
                break;
            case BubbleUpgrade.Upgrades.thornFan:
                if (BubbleUpgrade.me.thornFan)
                {
                    BubbleUpgrade.me.thornFanLevel++;
                }
                else
                {
                    BubbleUpgrade.me.thornFan = true;
                }
                break;
            case BubbleUpgrade.Upgrades.fastSqueeze:
                if (BubbleUpgrade.me.fastSqueeze)
                {
                    BubbleUpgrade.me.fastSqueezeLevel++;
                }
                else
                {
                    BubbleUpgrade.me.fastSqueeze = true;
                }
                break;
            default:
                break;
        }
        // close all buttons
        option1.SetActive(false);
        option2.SetActive(false);
        option3.SetActive(false);
        // remove all listeners(onClick events)
        option1.GetComponent<Button>().onClick.RemoveAllListeners();
        option2.GetComponent<Button>().onClick.RemoveAllListeners();
        option3.GetComponent<Button>().onClick.RemoveAllListeners();
        // if there are other squeezed chest waiting in the queue, show buttons again
        if (showButtonStack > 0)
        {
            ShowButtons();
        }
        showingButtons = false;
        // Count up upgradedCount in GameManager
        GameManager.me.upgradedCount++;
    }
    private UpgradeHolderScript CheckMoreBombUpgradeDependency(UpgradeHolderScript uhs)
    {
        if (uhs.handUpgrade == HandUpgrade.HandUpgrades.moreBomb) // check if this upgrade is [more bomb] upgrade
        {
            if (!BubbleUpgrade.me.boxExplosion && // check if any bubble bomb upgrade is obtained
                !BubbleUpgrade.me.lineExplosion &&
                !BubbleUpgrade.me.thornFan)
            {
                return upgradeListToShuffle[3].GetComponent<UpgradeHolderScript>(); // if not, return another upgrade
            }
            return uhs; // if yes, return the same upgrade that is passed in
        }
        return uhs; // if this upgrade is not [more bomb] upgrade, return the same upgrade
    }
}
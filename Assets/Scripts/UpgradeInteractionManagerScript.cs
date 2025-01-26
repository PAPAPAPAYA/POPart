using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public List<GameObject> upgrades;
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
        upgradeListToShuffle = UtilityFunctions.me.ShuffleList(upgrades);
        UpgradeHolderScript uhs1 = upgradeListToShuffle[0].GetComponent<UpgradeHolderScript>();
        UpgradeHolderScript uhs2 = upgradeListToShuffle[1].GetComponent<UpgradeHolderScript>();
        UpgradeHolderScript uhs3 = upgradeListToShuffle[2].GetComponent<UpgradeHolderScript>();
        Button button1 = option1.GetComponent<Button>();
        Button button2 = option2.GetComponent<Button>();
        Button button3 = option3.GetComponent<Button>();
        DetectUpgrade(uhs1, button1);
        DetectUpgrade(uhs2, button2);
        DetectUpgrade(uhs3, button3);
    }

    private void DetectUpgrade(UpgradeHolderScript uhs, Button button)
    {
        button.onClick.AddListener(() => ActivateUpgrade(uhs.thisUpgrade));
        button.GetComponentInChildren<TextMeshProUGUI>().text = uhs.name_upgrade;
    }
    private void ActivateUpgrade(BubbleUpgrade.Upgrades upgrade)
    {
        switch (upgrade)
        {
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
    }
}
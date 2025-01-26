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
        switch (uhs.thisUpgrade)
        {
            case BubbleUpgrade.Upgrades.lineExplode:
                button.onClick.AddListener(ActivateLineExplosion);
                button.GetComponentInChildren<TextMeshProUGUI>().text = uhs.name_upgrade;
                break;
            case BubbleUpgrade.Upgrades.boxExplode:
                button.onClick.AddListener(ActivateBoxExplosion);
                button.GetComponentInChildren<TextMeshProUGUI>().text = uhs.name_upgrade;
                break;
            case BubbleUpgrade.Upgrades.thornFan:
                button.onClick.AddListener(ActivateThornFan);
                button.GetComponentInChildren<TextMeshProUGUI>().text = uhs.name_upgrade;
                break;
            case BubbleUpgrade.Upgrades.fastSqueeze:
                button.onClick.AddListener(ActivateFastSqueeze);
                button.GetComponentInChildren<TextMeshProUGUI>().text = uhs.name_upgrade;
                break;
            default:
                break;
        }
    }

    #region ButtonFunctions
    public void ActivateLineExplosion()
    {
        BubbleUpgrade.me.lineExplosion = true;
        // close all buttons
        option1.SetActive(false);
        option2.SetActive(false);
        option3.SetActive(false);
        if (showButtonStack > 0)
        {
            //StartCoroutine(WaitABit());
            ShowButtons();
        }
        showingButtons = false;
    }
    public void ActivateBoxExplosion()
    {
        BubbleUpgrade.me.boxExplosion = true;
        // close all buttons
        option1.SetActive(false);
        option2.SetActive(false);
        option3.SetActive(false);
        if (showButtonStack > 0)
        {
            //StartCoroutine(WaitABit());
            ShowButtons();
        }
        showingButtons = false;
    }
    public void ActivateThornFan()
    {
        BubbleUpgrade.me.thornFan = true;
        // close all buttons
        option1.SetActive(false);
        option2.SetActive(false);
        option3.SetActive(false);
        if (showButtonStack > 0)
        {
            //StartCoroutine(WaitABit());
            ShowButtons();
        }
        showingButtons = false;
    }
    public void ActivateFastSqueeze()
    {
        BubbleUpgrade.me.fastSqueeze = true;
        //close all buttons
        option1.SetActive(false);
        option2.SetActive(false);
        option3.SetActive(false);
        if (showButtonStack > 0)
        {
            //StartCoroutine(WaitABit());
            ShowButtons();
        }
        showingButtons = false;
    }
    #endregion
    IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(.1f);
    }
}

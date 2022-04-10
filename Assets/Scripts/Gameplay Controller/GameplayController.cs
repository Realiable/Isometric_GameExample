using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public struct SkillModifierCardElement
{
    public GameObject Object_Card;
    public Animator Animator_Card;
    public TMP_Text UIText_SkillModifier_Name;
    public TMP_Text UIText_SkillModifier_Restriction;
    public TMP_Text UIText_SkillModifier_Description;
    public Button UIButton_SkillModifier_SetToMelee;
    public Button UIButton_SkillModifier_SetToRange;
    public Button UIButton_SkillModifier_SetToMobility;
}

public class GameplayController : MonoBehaviour
{
    public GameObject playerCharacter;
    public GameObject enemySpawner;

    [Header("Skill Modifier Data")]
    public SkillModifier_BaseProperties[] avaliableSkillModifierList;

    [Header("Skill Modifier UI")]
    public GameObject UIPanel_SkillModifier;
    public GameObject[] UI_SkillModifierCard;
    public GameObject UIPanel_CurrentSkillModifier;
    public TMP_Text UIText_CurrentMeleeModifierList;
    public TMP_Text UIText_CurrentRangeModifierList;
    public TMP_Text UIText_CurrentMobilityModifierList;

    [Header("Gameover UI")]
    public GameObject UIPanel_Gameover;
    public Animator Animator_GameOver;
    public TMP_Text UIText_PlayerKillCount;
    public TMP_Text UIText_PlayerPlayTime;

    private List<SkillModifierCardElement> skillModifierCard;
    private Animator Animator_CurrentSkillModifier;

    private PlayerInformation playerInformation;

    private List<SkillModifier_BaseProperties> currentSkillModifierSelected;
    private bool onPlayUIAnimation = false;

    private bool onInitializeGameplay = false;
    private float totalPlayTime = 0.0f;

    private void Start()
    {
        Time.timeScale = 1.0f;
        UIPanel_SkillModifier.SetActive(false);
        UIPanel_Gameover.SetActive(false);

        playerInformation = playerCharacter.GetComponent<PlayerInformation>();

        Pause_Gameplay();
        Setup_SkillModifierCardElement();
        Initial_GameStartSequence();
    }

    private void FixedUpdate()
    {
        if(Time.timeScale != 0.0f)
        {
            totalPlayTime += Time.deltaTime;
        }
    }

    void Pause_Gameplay()
    {
        Time.timeScale = 0.0f;
        enemySpawner.SetActive(false);
    }

    void Resume_Gameplay()
    {
        Time.timeScale = 1.0f;
        enemySpawner.SetActive(true);
    }

    void Setup_SkillModifierCardElement()
    {
        skillModifierCard = new List<SkillModifierCardElement>();

        for(int index = 0; index < UI_SkillModifierCard.Length; index++)
        {
            GameObject currentCard = UI_SkillModifierCard[index];

            SkillModifierCardElement cardStruct = new SkillModifierCardElement();
            cardStruct.Object_Card = currentCard;
            cardStruct.Animator_Card = currentCard.GetComponent<Animator>();
            cardStruct.UIText_SkillModifier_Name = currentCard.transform.Find("Skill Modifier Name").GetComponent<TMP_Text>();
            cardStruct.UIText_SkillModifier_Restriction = currentCard.transform.Find("Skill Modifier Restriction").GetComponent<TMP_Text>();
            cardStruct.UIText_SkillModifier_Description = currentCard.transform.Find("Skill Modifier Description").GetComponent<TMP_Text>();
            cardStruct.UIButton_SkillModifier_SetToMelee = currentCard.transform.Find("Button - Apply To Melee").GetComponent<Button>();
            cardStruct.UIButton_SkillModifier_SetToRange = currentCard.transform.Find("Button - Apply To Range").GetComponent<Button>();
            cardStruct.UIButton_SkillModifier_SetToMobility = currentCard.transform.Find("Button - Apply To Mobility").GetComponent<Button>();

            skillModifierCard.Add(cardStruct);
        }

        Animator_CurrentSkillModifier = UIPanel_CurrentSkillModifier.GetComponent<Animator>();
    }

    void Initial_GameStartSequence()
    {
        SetupAndShow_SkillModifierCardSelection();
    }

    public void SetupAndShow_SkillModifierCardSelection()
    {
        UIPanel_SkillModifier.SetActive(true);
        Pause_Gameplay();
        StartCoroutine(SetupAndShow_SkillModifierCard_Coroutine());
    }

    IEnumerator SetupAndShow_SkillModifierCard_Coroutine()
    {
        onPlayUIAnimation = true;
        Setup_CurrentSkillModifierList();
        Random_SkillModifierCard();

        if (onInitializeGameplay == false)
        {
            yield return new WaitForSecondsRealtime(0.5f);
        }

        for(int index = 0; index < skillModifierCard.Count; index++)
        {
            skillModifierCard[index].Animator_Card.SetTrigger("Active_ScaleUp");
            yield return new WaitForSecondsRealtime(0.1f);
        }

        yield return new WaitForSecondsRealtime(0.1f);
        Animator_CurrentSkillModifier.SetTrigger("Active_ScaleUp");
        onPlayUIAnimation = false;
    }

    void Setup_CurrentSkillModifierList()
    {
        List<SkillModifier_BaseProperties> meleeModifer = playerInformation.meleeSkillModifier;
        List<SkillModifier_BaseProperties> rangeModifer = playerInformation.rangeSkillModifier;
        List<SkillModifier_BaseProperties> mobilityModifer = playerInformation.mobilitySkillModifier;

        UIText_CurrentMeleeModifierList.text = "";
        UIText_CurrentRangeModifierList.text = "";
        UIText_CurrentMobilityModifierList.text = "";

        if (meleeModifer.Count == 0)
        {
            UIText_CurrentMeleeModifierList.text = "-";
        }
        else
        {
            for(int index = 0; index < meleeModifer.Count; index++)
            {
                SkillModifier_BaseProperties currentModifier = meleeModifer[index];
                UIText_CurrentMeleeModifierList.text += currentModifier.SkillMod_Name;

                if(index < meleeModifer.Count - 1)
                {
                    UIText_CurrentMeleeModifierList.text += ", ";
                }
            }
        }

        if (rangeModifer.Count == 0)
        {
            UIText_CurrentRangeModifierList.text = "-";
        }
        else
        {
            for (int index = 0; index < rangeModifer.Count; index++)
            {
                SkillModifier_BaseProperties currentModifier = rangeModifer[index];
                UIText_CurrentRangeModifierList.text += currentModifier.SkillMod_Name;

                if (index < rangeModifer.Count - 1)
                {
                    UIText_CurrentRangeModifierList.text += ", ";
                }
            }
        }

        if (mobilityModifer.Count == 0)
        {
            UIText_CurrentMobilityModifierList.text = "-";
        }
        else
        {
            for (int index = 0; index < mobilityModifer.Count; index++)
            {
                SkillModifier_BaseProperties currentModifier = mobilityModifer[index];
                UIText_CurrentMobilityModifierList.text += currentModifier.SkillMod_Name;

                if (index < mobilityModifer.Count - 1)
                {
                    UIText_CurrentMobilityModifierList.text += ", ";
                }
            }
        }
    }

    void Random_SkillModifierCard()
    {
        currentSkillModifierSelected = new List<SkillModifier_BaseProperties>();
        List<SkillModifier_BaseProperties> skillModifierPool = new List<SkillModifier_BaseProperties>();

        for(int index = 0; index < avaliableSkillModifierList.Length; index++)
        {
            skillModifierPool.Add(avaliableSkillModifierList[index]);
        }

        for(int index = 0; index < 3; index++)
        {
            int randomSkillModInPool = Random.Range(0, skillModifierPool.Count);
            SkillModifier_BaseProperties randomSkillMod = skillModifierPool[randomSkillModInPool];
            SkillModifierCardElement currentCardUI = skillModifierCard[index];

            currentCardUI.UIText_SkillModifier_Name.text = randomSkillMod.SkillMod_Name;
            currentCardUI.UIText_SkillModifier_Description.text = randomSkillMod.SkillMod_Description;
            currentCardUI.UIText_SkillModifier_Restriction.text = Get_SkillModifierRestrictionInString(randomSkillMod.ApplyRestriction);

            Set_SkillModifierSetupButton(currentCardUI, randomSkillMod);
            currentSkillModifierSelected.Add(randomSkillMod);

            skillModifierPool.RemoveAt(randomSkillModInPool);
        }
    }

    string Get_SkillModifierRestrictionInString(ApplyEffectRestriction restriction)
    {
        if(restriction == ApplyEffectRestriction.MeleeSkillOnly)
        {
            return "Melee Skill Modifier";
        }
        else if (restriction == ApplyEffectRestriction.RangeSkillOnly)
        {
            return "Range Skill Modifier";
        }
        else if (restriction == ApplyEffectRestriction.MobilitySkillOnly)
        {
            return "Mobility Skill Modifier";
        }

        return "Skill Modifier";
    }

    void Set_SkillModifierSetupButton(SkillModifierCardElement targetCard, SkillModifier_BaseProperties targetSkillModifier)
    {
        targetCard.UIButton_SkillModifier_SetToMelee.interactable = true;
        targetCard.UIButton_SkillModifier_SetToRange.interactable = true;
        targetCard.UIButton_SkillModifier_SetToMobility.interactable = true;

        if(targetSkillModifier.ApplyRestriction == ApplyEffectRestriction.MeleeSkillOnly)
        {
            targetCard.UIButton_SkillModifier_SetToRange.interactable = false;
            targetCard.UIButton_SkillModifier_SetToMobility.interactable = false;
        }
        else if (targetSkillModifier.ApplyRestriction == ApplyEffectRestriction.RangeSkillOnly)
        {
            targetCard.UIButton_SkillModifier_SetToMelee.interactable = false;
            targetCard.UIButton_SkillModifier_SetToMobility.interactable = false;
        }
        else if (targetSkillModifier.ApplyRestriction == ApplyEffectRestriction.MobilitySkillOnly)
        {
            targetCard.UIButton_SkillModifier_SetToMelee.interactable = false;
            targetCard.UIButton_SkillModifier_SetToRange.interactable = false;
        }

        if(playerInformation.Check_AvaliableToAddMeleeModifier(targetSkillModifier) == false)
        {
            targetCard.UIButton_SkillModifier_SetToMelee.interactable = false;
        }

        if (playerInformation.Check_AvaliableToAddRangeModifier(targetSkillModifier) == false)
        {
            targetCard.UIButton_SkillModifier_SetToRange.interactable = false;
        }

        if (playerInformation.Check_AvaliableToAddMobilityModifier(targetSkillModifier) == false)
        {
            targetCard.UIButton_SkillModifier_SetToMobility.interactable = false;
        }
    }

    public void Set_SkillModifierToMelee(int skillModifierCardIndex)
    {
        if (onPlayUIAnimation == false)
        {
            playerInformation.Add_SkillModifierToMeleeSkill(currentSkillModifierSelected[skillModifierCardIndex]);
            Close_SkillModifierCardSelection();
        }
    }

    public void Set_SkillModifierToRange(int skillModifierCardIndex)
    {
        if (onPlayUIAnimation == false)
        {
            playerInformation.Add_SkillModifierToRangeSkill(currentSkillModifierSelected[skillModifierCardIndex]);
            Close_SkillModifierCardSelection();
        }
    }

    public void Set_SkillModifierToMobility(int skillModifierCardIndex)
    {
        if (onPlayUIAnimation == false)
        {
            playerInformation.Add_SkillModifierToMobilitySkill(currentSkillModifierSelected[skillModifierCardIndex]);
            Close_SkillModifierCardSelection();
        }
    }

    public void Skip_SkillModifierSelection()
    {
        Close_SkillModifierCardSelection();
    }

    void Close_SkillModifierCardSelection()
    {
        StartCoroutine(Close_SkillModifierCardSelection_Coroutine());
    }

    IEnumerator Close_SkillModifierCardSelection_Coroutine()
    {
        onPlayUIAnimation = true;

        for (int index = 0; index < skillModifierCard.Count; index++)
        {
            skillModifierCard[index].Animator_Card.SetTrigger("Active_ScaleDown");
            Animator_CurrentSkillModifier.SetTrigger("Active_ScaleDown");
        }

        yield return new WaitForSecondsRealtime(0.5f);
        onPlayUIAnimation = false;
        UIPanel_SkillModifier.SetActive(false);

        if(onInitializeGameplay == false)
        {
            onInitializeGameplay = true;
            SetupAndShow_SkillModifierCardSelection();
        }
        else
        {
            Resume_Gameplay();
        }
    }

    public void Show_GameoverScreen(int playerKillCount)
    {
        UIText_PlayerKillCount.text = "" + playerKillCount;
        UIText_PlayerPlayTime.text = Get_PlayTimeAsString();

        UIPanel_Gameover.SetActive(true);
        Animator_GameOver.SetTrigger("Active_FadeIn");
    }

    string Get_PlayTimeAsString()
    {
        int totalPlayTimeInt = Mathf.RoundToInt(totalPlayTime);
        int totalPlayTimeHour = Mathf.FloorToInt(totalPlayTimeInt / 3600.0f);
        int totalPlayTimeMinute = Mathf.FloorToInt((totalPlayTimeInt % 3600) / 60.0f);
        int totalPlayTimeSecond = totalPlayTimeInt % 60;

        string playTimeText = "";

        if (totalPlayTimeHour < 10)
        {
            playTimeText += "0" + totalPlayTimeHour;
        }
        else
        {
            playTimeText += totalPlayTimeHour;
        }

        playTimeText += ":";

        if (totalPlayTimeMinute < 10)
        {
            playTimeText += "0" + totalPlayTimeMinute;
        }
        else
        {
            playTimeText += totalPlayTimeMinute;
        }

        playTimeText += ":";

        if(totalPlayTimeSecond < 10)
        {
            playTimeText += "0" + totalPlayTimeSecond;
        }
        else
        {
            playTimeText += totalPlayTimeSecond;
        }

        return playTimeText;
    }

    public void Restart_Game()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
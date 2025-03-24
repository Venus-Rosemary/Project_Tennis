using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject startPanel;

    public GameObject gamePanel;

    public GameObject endPanel;

    public GameObject tipsPanel;
    private float recordTime;
    public TMP_Text CountdownText;

    public GameObject player;

    public GameObject aiBot;

    void Start()
    {
        player.SetActive(false);
        aiBot.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            End_Game();
        }
        if (tipsPanel.activeSelf)
        {
            recordTime -= Time.deltaTime;
            CountdownText.text = string.Format("{0}秒后关闭", Mathf.CeilToInt(recordTime));
            if (recordTime <= 0)
            {
                recordTime = 0;
            }
        }
    }
    public void Start_Fallow()//开始休闲模式
    {
        player.GetComponent<PlayerController>().SetHintSystemEnabled(false);
        Start_Game();
    }

    public void Start_Challenge()//开启挑战模式
    {
        StartCoroutine(Set_TipsPanel());
    }
    IEnumerator Set_TipsPanel()
    {
        tipsPanel.SetActive(true);
        recordTime = 3;
        Manage_UI_ActiveSelf(false, false, false, false, false);
        yield return new WaitForSeconds(3.5f);
        player.GetComponent<PlayerController>().SetHintSystemEnabled(true);
        tipsPanel.SetActive(false);
        Start_Game();
    }
    private void Start_Game()
    {
        GoalController.Instance.Initialize_Data();
        GoalController.Instance.canControl = true;

        Manage_UI_ActiveSelf(true, true, false, true, false);
    }
    public void End_Game()//结束游戏
    {
        GoalController.Instance.Initialize_Data();
        Manage_UI_ActiveSelf(false, false, false, false, true);
    }
    public void Back_Main()//返回主页面
    {
        Manage_UI_ActiveSelf(false, false, true, false, false);
    }

    public void Restart()
    {
        Manage_UI_ActiveSelf(true, true, false, true, false);
    }

    public void Manage_UI_ActiveSelf(bool PL=false, bool AB = false, bool SP = false, bool GP = false, bool EP = false)
    {
        player.SetActive(PL);
        aiBot.SetActive(AB);
        startPanel.SetActive(SP);
        gamePanel.SetActive(GP);
        endPanel.SetActive(EP);
    }
    public void ExitClick()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

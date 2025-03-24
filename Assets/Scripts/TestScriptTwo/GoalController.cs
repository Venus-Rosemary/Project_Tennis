using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalController : Singleton<GoalController>
{
    [Header("球的预制体")]
    public GameObject Ball_Prefab;
    private GameObject Record_Now_Ball;

    [Header("发球设置")]
    public Transform playerSpawnPoint; // 玩家发球点
    public Transform aiSpawnPoint; // AI发球点
    public bool isPlayerServe = true; // 轮流发球标记
    public bool isPlayerServeState = false;//玩家处于发球状态,球在玩家手中
    public bool isAiServeState = false;//ai处于发球状态，球在ai手中

    private float ballBoundaryX = 10;
    private float ballBoundaryZ = 16;

    [Header("得分")]
    public int playerGoal = 0;
    public int aiGoal = 0;
    public TMP_Text ScoreSituation;

    [Header("发球UI")]
    public GameObject ServeUI;//用于看向摄像头
    public GameObject SliderUI;
    public Slider slider;
    public RectTransform Perfect_Radius;
    public float speed = 0.1f;

    private float minValue;
    private float maxValue;

    private float AiTime = 0f;

    [Header("初始化设置")]
    public bool canControl = false;

    public GameObject PlayerG;
    public GameObject AiG;
    public UIController UIC;
    private Vector3 PlayerT;
    private Vector3 AiT;
    public GameObject winPanel;
    public GameObject losePanel;

    private void Start()
    {
        PlayerT = PlayerG.transform.position;
        AiT = AiG.transform.position;
        Initialize_Data();
        Initialize_Slider();
    }

    void Update()
    {
        if (!canControl) return;
        Game_Over();
        Check_Boundaries();
        if (SliderUI.activeSelf)
        {
            UI_3d_Setting();
        }
        if (Input.GetMouseButtonDown(0)&& isPlayerServeState)
        {
            isPlayerServeState = false;
            Record_Now_Ball.gameObject.transform.SetParent(null);
            Record_Now_Ball.GetComponent<Collider>().enabled = true;
            Record_Now_Ball.GetComponent<TrailRenderer>().enabled = true;
            Record_Now_Ball.GetComponent<Rigidbody>().useGravity = true;
            if (slider.value > minValue && slider.value < maxValue)
            {
                Record_Now_Ball.GetComponent<BallController>().OnHit(Record_Now_Ball.transform.position, true, true);
            }
            else
            {
                Record_Now_Ball.GetComponent<BallController>().OnHit(Record_Now_Ball.transform.position, true);
            }
            SliderUI.SetActive(false);
            Initialize_Slider();
        }
        if (isAiServeState && Time.time - AiTime > 1.5f)
        {
            isAiServeState = false;
            Record_Now_Ball.GetComponent<Collider>().enabled = true;
            Record_Now_Ball.GetComponent<Rigidbody>().useGravity = true;

        }

        ServeUI.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }

    void Check_Boundaries()//判断出界
    {
        if (Record_Now_Ball.transform.position.x < -ballBoundaryX || Record_Now_Ball.transform.position.x > ballBoundaryX ||
            Record_Now_Ball.transform.position.z < -ballBoundaryZ || Record_Now_Ball.transform.position.z > ballBoundaryZ)
        {
            if (Record_Now_Ball.transform.position.z > 0 && Record_Now_Ball.transform.position.z > ballBoundaryZ || Record_Now_Ball.transform.position.x < -ballBoundaryX
                || Record_Now_Ball.transform.position.x > ballBoundaryX)
            {
                Score(true);
            }
            else if (Record_Now_Ball.transform.position.z < 0 && Record_Now_Ball.transform.position.z < -ballBoundaryZ || Record_Now_Ball.transform.position.x < -ballBoundaryX
                || Record_Now_Ball.transform.position.x > ballBoundaryX)
            {
                Score(false);
            }

            Reset_Ball();
        }
    }

    void Score(bool playerScored)
    {
        if (Record_Now_Ball == null) return;
        Record_Now_Ball.GetComponent<BallController>().Set_FallVFX();
        Destroy(Record_Now_Ball);
        if (playerScored)
        {
            playerGoal += 1;
            Set_ScoreSituationText();
        }
        else
        {
            aiGoal += 1;
            Set_ScoreSituationText();
        }
    }

    public void Set_ScoreSituationText()
    {
        ScoreSituation.text=
        string.Format("得分情况:\n我的得分：{0}      对方得分：{1}", playerGoal, aiGoal);
    }

    #region 重新发球的设置
    void Reset_Ball()
    {

        Record_Now_Ball=Instantiate(Ball_Prefab);

        // 根据当前是玩家还是AI发球来选择发球点
        if (isPlayerServe)
        {
            isPlayerServeState = true;
            SliderUI.SetActive(true);

            Record_Now_Ball.transform.position = playerSpawnPoint.position;
            Record_Now_Ball.gameObject.transform.SetParent(playerSpawnPoint.gameObject.transform);
            Record_Now_Ball.GetComponent<Collider>().enabled = false;
            Record_Now_Ball.GetComponent<TrailRenderer>().enabled = false;
            Record_Now_Ball.GetComponent<Rigidbody>().useGravity = false;
            Record_Now_Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {
            AiTime = Time.time;
            isAiServeState = true;
            AIBotController.Instance.targetPosition = AiG.gameObject.transform.position;
            Record_Now_Ball.GetComponent<Collider>().enabled = false;
            Record_Now_Ball.GetComponent<Rigidbody>().useGravity = false;
            Record_Now_Ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Record_Now_Ball.transform.position = aiSpawnPoint.position;
        }

        // 切换下一次发球方
        isPlayerServe = !isPlayerServe;

    }
    #endregion

    public void Initialize_Data()//初始化数据
    {
        playerGoal = 0;
        aiGoal = 0;
        Set_ScoreSituationText();
        isPlayerServe = true;
        canControl = false;
        isAiServeState = false;
        PlayerG.transform.position = PlayerT;
        AiG.transform.position = AiT;
        AIBotController.Instance.targetPosition = AiG.gameObject.transform.position;
        if (Record_Now_Ball!=null)
        {
            Record_Now_Ball.GetComponent<BallController>().Set_FallVFX();
            Destroy(Record_Now_Ball);
        }
        Reset_Ball();
    }
    public void Game_Restart()//重新游玩
    {
        UIC.Restart();
        canControl = true;
    }
    public void Game_Over()//结束游戏
    {
        if (playerGoal>=10||aiGoal>=10)
        {
            if (playerGoal >= 10)
            {
                winPanel.SetActive(true);
                losePanel.SetActive(false);
            }
            else if (aiGoal >= 10)
            {
                winPanel.SetActive(false);
                losePanel.SetActive(true);
            }
            UIC.End_Game();
        }
    }

    #region 滑动条UI
    void Initialize_Slider()
    {
        float randomY = Random.Range(-60f, 60f);
        Perfect_Radius.anchoredPosition = new Vector2(Perfect_Radius.anchoredPosition.x, randomY);

        float leftPos = randomY - 20;
        float rightPos = randomY + 20;

        // Slider长度为160，中心点为0，所以滑轨范围是-80到80
        // 将图片左右边界位置映射到Slider的value范围（0-1）
        minValue = Mathf.Clamp01((leftPos + 80) / 160f);
        maxValue = Mathf.Clamp01((rightPos + 80) / 160f);

        Debug.Log("min: "+minValue +"; max:"+maxValue); 
    }
    private void UI_3d_Setting()
    {
        if (SliderUI != null)
        {
            slider.value = Mathf.PingPong(Time.time * speed, 1f);
        }
    }
    #endregion
}

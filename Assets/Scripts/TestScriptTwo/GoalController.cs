using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoalController : Singleton<GoalController>
{
    [Header("���Ԥ����")]
    public GameObject Ball_Prefab;
    private GameObject Record_Now_Ball;

    [Header("��������")]
    public Transform playerSpawnPoint; // ��ҷ����
    public Transform aiSpawnPoint; // AI�����
    public bool isPlayerServe = true; // ����������
    public bool isPlayerServeState = false;//��Ҵ��ڷ���״̬,�����������
    public bool isAiServeState = false;//ai���ڷ���״̬������ai����

    private float ballBoundaryX = 10;
    private float ballBoundaryZ = 16;

    [Header("�÷�")]
    public int playerGoal = 0;
    public int aiGoal = 0;
    public TMP_Text ScoreSituation;

    [Header("����UI")]
    public GameObject ServeUI;//���ڿ�������ͷ
    public GameObject SliderUI;
    public Slider slider;
    public RectTransform Perfect_Radius;
    public float speed = 0.1f;

    private float minValue;
    private float maxValue;

    private float AiTime = 0f;

    [Header("��ʼ������")]
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

    void Check_Boundaries()//�жϳ���
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
        string.Format("�÷����:\n�ҵĵ÷֣�{0}      �Է��÷֣�{1}", playerGoal, aiGoal);
    }

    #region ���·��������
    void Reset_Ball()
    {

        Record_Now_Ball=Instantiate(Ball_Prefab);

        // ���ݵ�ǰ����һ���AI������ѡ�����
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

        // �л���һ�η���
        isPlayerServe = !isPlayerServe;

    }
    #endregion

    public void Initialize_Data()//��ʼ������
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
    public void Game_Restart()//��������
    {
        UIC.Restart();
        canControl = true;
    }
    public void Game_Over()//������Ϸ
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

    #region ������UI
    void Initialize_Slider()
    {
        float randomY = Random.Range(-60f, 60f);
        Perfect_Radius.anchoredPosition = new Vector2(Perfect_Radius.anchoredPosition.x, randomY);

        float leftPos = randomY - 20;
        float rightPos = randomY + 20;

        // Slider����Ϊ160�����ĵ�Ϊ0�����Ի��췶Χ��-80��80
        // ��ͼƬ���ұ߽�λ��ӳ�䵽Slider��value��Χ��0-1��
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

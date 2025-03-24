using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("玩家移动参数")]
    public float moveSpeed = 5f;
    public float boundaryX = 6f; // 左右边界限制
    public float boundaryZ = 16f;

    [Header("击球参数")]
    public bool is_canHit = false;
    public GameObject hitTrigger;
    
    [Header("左右判断")]
    public bool is_rightHit=false;
    public bool is_leftHit = false;

    [Header("场景提示物体")]
    public bool enableHintSystem = false; // 是否启用提示系统
    public GameObject leftHintObject; // 左侧提示物体
    public GameObject rightHintObject; // 右侧提示物体
    public GameObject falseLeftHintObject; // 假的左侧提示物体
    public GameObject falserightHintObject; // 假的右侧提示物体
    private bool isLeftHintActive = false; // 左侧提示是否激活
    private bool isRightHintActive = false; // 右侧提示是否激活
    private float hintTimer = 0f; // 提示计时器
    public float hintDuration = 3f; // 提示持续时间

    private void Start()
    {
        if (enableHintSystem)
        {
            if (leftHintObject != null) leftHintObject.SetActive(false);
            if (rightHintObject != null) rightHintObject.SetActive(false);
            if (falserightHintObject != null) falserightHintObject.SetActive(false);
            if (falseLeftHintObject != null) falseLeftHintObject.SetActive(false);
        }
    }
    
    void Update()
    {
        Player_Move();
        
        if (!hitTrigger.activeSelf)
        {
            is_rightHit = false;
            is_leftHit = false;
            is_canHit = false;
        }
        
        if (!is_canHit)
        {
            Player_Set_Trigger();
        }
        
        // 处理提示系统
        if (enableHintSystem)
        {
            UpdateHintSystem();
        }
    }
    
    // 更新提示系统
    private void UpdateHintSystem()
    {
        if (GoalController.Instance.isPlayerServeState == true)
        {
            DeactivateHints();
            return;
        }
        // 如果当前有提示激活，更新计时器
        if (isLeftHintActive || isRightHintActive)
        {
            hintTimer -= Time.deltaTime;
            
            // 如果计时器结束，关闭提示并随机生成新提示
            if (hintTimer <= 0)
            {
                DeactivateHints();
                GenerateRandomHint();
            }
        }
        else if (GoalController.Instance.isPlayerServeState == false)
        {
            // 如果没有提示激活，且不是发球状态，随机生成新提示
            GenerateRandomHint();
        }
    }
    
    // 随机生成提示
    private void GenerateRandomHint()
    {
        float random = Random.value;
        DeactivateHints();
        if (GoalController.Instance.playerGoal < 5)
        {
            if (random < 0.5f)
            {
                // 激活左侧提示
                isLeftHintActive = true;
                isRightHintActive = false;
                if (leftHintObject != null) leftHintObject.SetActive(true);
            }
            else
            {
                // 激活右侧提示
                isLeftHintActive = false;
                isRightHintActive = true;
                if (rightHintObject != null) rightHintObject.SetActive(true);
            }
        }
        else
        {
            if (random < 0.5f)
            {
                // 激活左侧提示
                isLeftHintActive = true;
                isRightHintActive = false;
                if (leftHintObject != null) falserightHintObject.SetActive(true);
            }
            else
            {
                // 激活右侧提示
                isLeftHintActive = false;
                isRightHintActive = true;
                if (rightHintObject != null) falseLeftHintObject.SetActive(true);
            }
        }

        // 设置计时器
        hintTimer = hintDuration;
    }
    
    // 关闭所有提示
    private void DeactivateHints()
    {
        isLeftHintActive = false;
        isRightHintActive = false;
        if (leftHintObject != null) leftHintObject.SetActive(false);
        if (rightHintObject != null) rightHintObject.SetActive(false);
        if (falserightHintObject != null) falserightHintObject.SetActive(false);
        if (falseLeftHintObject != null) falseLeftHintObject.SetActive(false);
    }
    
    private void Player_Set_Trigger()
    {
        if (GoalController.Instance.isPlayerServeState == false)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                is_rightHit = false;
                is_leftHit = true;
                hitTrigger.transform.localPosition = new Vector3(-1, 0, 1);
                hitTrigger.SetActive(true);
                StartCoroutine(DisableHitTrigger());
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                is_rightHit = true;
                is_leftHit = false;
                hitTrigger.transform.localPosition = new Vector3(1, 0, 1);
                hitTrigger.SetActive(true);
                StartCoroutine(DisableHitTrigger());
            }
        }
    }

    private IEnumerator DisableHitTrigger()
    {
        is_canHit = true;
        yield return new WaitForSeconds(0.5f);
        is_canHit = false;
        hitTrigger.SetActive(false);
    }

    private void Player_Move()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;
        transform.position += movement * Time.deltaTime * moveSpeed;

        float clampedX = Mathf.Clamp(transform.position.x, -boundaryX, boundaryX);
        float clampedZ = Mathf.Clamp(transform.position.z, -boundaryZ, -4f);
        transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GoalController.Instance.isPlayerServeState) return;
        if (other.CompareTag("Ball"))
        {
            // 如果提示系统启用，检查玩家是否按照提示击球
            if (enableHintSystem)
            {
                // 如果左侧提示激活，但玩家使用右侧击球，或者右侧提示激活，但玩家使用左侧击球，则不执行击球
                if ((isLeftHintActive && is_rightHit) || (isRightHintActive && is_leftHit))
                {
                    return;
                }
                
                // 如果玩家按照提示击球，奖励玩家（可以添加得分或其他奖励）
                if ((isLeftHintActive && is_leftHit) || (isRightHintActive && is_rightHit))
                {
                }
            }
            
            other.GetComponent<BallController>().Set_FallVFX();
            other.GetComponent<BallController>().OnHit(other.transform.position, true);
            
            // 摄像机震动
            CameraController cameraController = Camera.main.GetComponent<CameraController>();
            if (cameraController != null)
            {
                cameraController.ShakeCamera();
            }
            
            // 击球后重置提示
            if (enableHintSystem)
            {
                DeactivateHints();
            }
        }
    }
    
    // 公共方法，用于外部启用/禁用提示系统
    public void SetHintSystemEnabled(bool enabled)
    {
        enableHintSystem = enabled;
        if (!enabled)
        {
            DeactivateHints();
        }
    }
}

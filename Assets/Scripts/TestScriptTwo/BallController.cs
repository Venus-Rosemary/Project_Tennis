using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public float maxBounceHeight = 2f; // 最大反弹高度
    public float gravity = 9.81f;
    public float bounceFactor = 0.5f; // 弹跳系数
    public float maxBounces = 3; // 最大弹跳次数

    public Vector3 targetPosition; // 目标位置
    public GameObject target_Prefab_test;
    private GameObject aa;
    private int bounceCount = 0; // 弹跳计数

    // 添加一个变量记录最后一次击球的时间
    private float lastHitTime = 0f;

    public void OnHit(Vector3 hitPosition, bool isPlayerHit = true, bool isPerfectServe=false)
    {
        // 记录击球时间
        lastHitTime = Time.time;
        
        // 重置弹跳计数
        bounceCount = 0;
        
        // 根据击球方向生成对方场地内的随机落点
        float randomX = Random.Range(-8f, 8f); // 对方场地X范围
        float randomZ;
        
        if (isPlayerHit)
        {
            // 玩家击球，目标在对方场地 (z: 3 到 12)
            randomZ = Random.Range(6f, 18f);
        }
        else
        {
            // AI/对手击球，目标在玩家场地 (z: -12 到 -3)
            randomZ = Random.Range(-18f, -6f);
        }

        if (isPerfectServe)
        {
            int randomInt= Random.Range(0, 2);
            if (randomInt==0)
            {
                targetPosition = new Vector3(-8f, 0.3f, 15f);
            }
            else
            {
                targetPosition = new Vector3(8f, 0.3f, 15f);
            }
        }
        else
        {
            targetPosition = new Vector3(randomX, 0.3f, randomZ);
        }
        if (isPlayerHit)
        {
            AIBotController.Instance.SetTarget(targetPosition);
        }
        // 计算抛物线轨迹
        LaunchBall(hitPosition, targetPosition);
    }

    private void LaunchBall(Vector3 startPos, Vector3 targetPos)
    {
        // 获取水平距离和方向
        Vector3 displacement = targetPos - startPos;
        float horizontalDistance = new Vector2(displacement.x, displacement.z).magnitude;
        Vector3 horizontalDirection = new Vector3(displacement.x, 0, displacement.z).normalized;
        
        // 使用物理公式计算初速度
        // 使用Unity的物理重力而不是自定义重力
        float actualGravity = Physics.gravity.magnitude;
        
        // 计算需要的初始速度
        // 使用抛物线公式: x = v0 * t, y = v0 * t - 0.5 * g * t^2
        // 我们知道在落点时 y = 0，可以求解出所需的时间和初速度
        
        // 如果想要球达到特定的最大高度，可以使用这个公式
        float verticalSpeed = Mathf.Sqrt(2 * actualGravity * maxBounceHeight);

        // 调整时间和水平速度以匹配所需的垂直速度
        float timeToTarget = 2 * verticalSpeed / actualGravity; // 上升时间
        float horizontalSpeed = horizontalDistance / timeToTarget;
        
        // 设置最终速度向量
        Vector3 velocity = horizontalDirection * horizontalSpeed;
        velocity.y = verticalSpeed;
        
        // 应用速度
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = velocity;
        
        Debug.Log($"目标位置: {targetPos}, 初始速度: {velocity}, 预计时间: {timeToTarget}秒");
        aa=Instantiate(target_Prefab_test);
        aa.transform.position = targetPos+ horizontalDirection;
    }
    private void Update()
    {

    }

    public void Set_FallVFX()
    {
        if (aa!=null)
        {
            Destroy(aa);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // 检查是否在击球后的0.5秒内
            if (Time.time - lastHitTime < 0.5f)
            {
                Debug.Log("球在击打后0.5秒内触地，忽略弹跳");
                return;
            }

            if (bounceCount==0)
            {
                Destroy(aa);
            }
            bounceCount++;

            // 如果超过最大弹跳次数，可以在这里处理（例如重置球或得分）
            if (bounceCount >= maxBounces)
            {
                // 游戏逻辑：球停止弹跳，可能需要重置或计分
                // ResetBall(); 或 GameManager.Instance.ScorePoint();
                if (gameObject.transform.position.z>0)
                {
                    Debug.Log("在对面弹2次，我得分");
                    GoalController.Instance.Score(true);
                    GoalController.Instance.Reset_Ball();
                }
                else if (gameObject.transform.position.z < 0)
                {
                    Debug.Log("在我方弹2次，对面得分");
                    GoalController.Instance.Score(false);
                    GoalController.Instance.Reset_Ball();
                }

                return;
            }
            
            // 计算反弹速度
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 newVelocity = rb.velocity;
            // 反向Y轴速度并应用弹跳系数
            //newVelocity.y = -newVelocity.y * bounceFactor;
            newVelocity.y = Mathf.Sqrt(2 * Physics.gravity.magnitude * bounceFactor);
            rb.velocity = newVelocity;
            
            // 每次弹跳后水平速度也略微减小
            newVelocity.x *= 0.95f;
            newVelocity.z *= 0.95f;
            
            rb.velocity = newVelocity;
        }
        
        // 可以添加与其他物体（如球拍）的碰撞逻辑
    }
    
    // 可以添加一个公共方法供球拍调用
    public void HitByRacket(Vector3 hitPosition, Vector3 hitDirection, float hitForce)
    {
        // 根据击球方向判断是玩家还是对手击球
        bool isPlayerHit = hitDirection.z > 0;
        
        // 调用OnHit方法处理击球
        OnHit(hitPosition, isPlayerHit);
    }
}

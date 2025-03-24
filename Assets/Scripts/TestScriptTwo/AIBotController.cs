using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBotController : Singleton<AIBotController>
{
    public float moveSpeed = 3f;
    public float smoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;
    public Vector3 targetPosition;

    void Update()
    {
        if (targetPosition != Vector3.zero)
        {
            // 平滑移动到目标点
            //Vector3 direction = (targetPosition - transform.position).normalized;
            //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            Vector3 direction = targetPosition - transform.position;

            float step = moveSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
    }

    public void SetTarget(Vector3 position)
    {
        //targetPosition = new Vector3(position.x,gameObject.transform.position.y, gameObject.transform.position.z);
        targetPosition = new Vector3(position.x, gameObject.transform.position.y, position.z+5f); ;
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            other.GetComponent<BallController>().Set_FallVFX();
            other.GetComponent<BallController>().OnHit(other.transform.position,false);

            //// 通知玩家控制器AI已击球，需要生成新提示
            //FindObjectOfType<PlayerController>()?.GenerateHintAfterAIHit();
        }
    }
}

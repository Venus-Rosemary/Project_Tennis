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
            // ƽ���ƶ���Ŀ���
            //Vector3 direction = (targetPosition - transform.position).normalized;
            //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            Vector3 direction = targetPosition - transform.position;

            float step = moveSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
    }

    public void SetTarget(Vector3 startPos, Vector3 position)
    {
        Vector3 displacement = position - startPos;
        Vector3 horizontalDirection = new Vector3(displacement.x, 0, displacement.z).normalized;
        //targetPosition = new Vector3(position.x,gameObject.transform.position.y, gameObject.transform.position.z);
        targetPosition = new Vector3(position.x, gameObject.transform.position.y, position.z)+ horizontalDirection*6; 
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            other.GetComponent<BallController>().Set_FallVFX();
            SoundManagement.Instance.PlaySFX(0);
            other.GetComponent<BallController>().OnHit(other.transform.position,false);

            //// ֪ͨ��ҿ�����AI�ѻ�����Ҫ��������ʾ
            //FindObjectOfType<PlayerController>()?.GenerateHintAfterAIHit();
        }
    }
}

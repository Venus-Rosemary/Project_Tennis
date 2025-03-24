using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("��������")]
    public Transform target; // �����Ŀ�꣨��ң�
    public Vector3 offset = new Vector3(0, 5, -7); // �����Ŀ���ƫ��
    public float smoothSpeed = 0.125f; // ƽ�������ٶ�

    [Header("��ת����")]
    public float maxRotationAngle = 10f; // �����ת�Ƕ�
    public float maxPlayerX = 8f; // ������X����

    [Header("������")]
    public float shakeDuration = 0.2f; // �𶯳���ʱ��
    public float shakeIntensity = 0.2f; // ��ǿ��

    private Vector3 originalPosition; // ��ǰ��ԭʼλ��
    private bool isShaking = false; // �Ƿ�������
    private float currentShakeTime = 0f; // ��ǰ��ʱ��

    void Start()
    {
        // ���û��ָ��Ŀ�꣬���Բ������
        if (target == null)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                target = player.transform;
            }
        }

        // ��ʼ�������λ��
        if (target != null)
        {
            UpdateCameraPosition(false);
        }
    }

    void LateUpdate()
    {
        // ���û��Ŀ�꣬��ִ�и���
        if (target == null)
            return;

        // ������Ч��
        if (isShaking)
        {
            HandleShake();
            return;
        }

        // ���������λ�ú���ת
        UpdateCameraPosition(true);
        UpdateCameraRotation();
    }

    // ���������λ��
    private void UpdateCameraPosition(bool smooth)
    {
        // ����Ŀ��λ��
        Vector3 desiredPosition = target.position + offset;

        if (smooth)
        {
            // ƽ���ƶ���Ŀ��λ��
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
        else
        {
            // ֱ������λ��
            transform.position = desiredPosition;
        }
    }

    // �����������ת
    private void UpdateCameraRotation()
    {
        // ��ȡ��ҵ�ǰX����
        float playerX = target.position.x;

        // ������ת�Ƕȣ�����ӳ�䣩
        // �������x=-maxPlayerXʱ����תy=maxRotationAngle
        // �������x=maxPlayerXʱ����תy=-maxRotationAngle
        float rotationY = Mathf.Lerp(maxRotationAngle, -maxRotationAngle,
                                     (playerX + maxPlayerX) / (2 * maxPlayerX));

        // Ӧ����ת
        Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotationY, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * 2);
    }

    // ������Ļ��
    public void ShakeCamera()
    {
        // ����Ѿ����𶯣����ظ�����
        if (isShaking)
            return;

        // ��¼ԭʼλ��
        originalPosition = transform.position;
        isShaking = true;
        currentShakeTime = 0f;
    }

    // ������Ч��
    private void HandleShake()
    {
        // ������ʱ��
        currentShakeTime += Time.deltaTime;

        // �����ʱ��������ָ�ԭλ
        if (currentShakeTime >= shakeDuration)
        {
            transform.position = originalPosition;
            isShaking = false;
            return;
        }

        // ������ǿ�ȣ���ʱ��˥����
        float currentIntensity = shakeIntensity * (1f - currentShakeTime / shakeDuration);

        // Ӧ�����ƫ��
        transform.position = originalPosition + Random.insideUnitSphere * currentIntensity;
    }
}

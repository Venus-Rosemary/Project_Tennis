using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target; // 跟随的目标（玩家）
    public Vector3 offset = new Vector3(0, 5, -7); // 相对于目标的偏移
    public float smoothSpeed = 0.125f; // 平滑跟随速度

    [Header("旋转设置")]
    public float maxRotationAngle = 10f; // 最大旋转角度
    public float maxPlayerX = 8f; // 玩家最大X坐标

    [Header("震动设置")]
    public float shakeDuration = 0.2f; // 震动持续时间
    public float shakeIntensity = 0.2f; // 震动强度

    private Vector3 originalPosition; // 震动前的原始位置
    private bool isShaking = false; // 是否正在震动
    private float currentShakeTime = 0f; // 当前震动时间

    void Start()
    {
        // 如果没有指定目标，尝试查找玩家
        if (target == null)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                target = player.transform;
            }
        }

        // 初始化摄像机位置
        if (target != null)
        {
            UpdateCameraPosition(false);
        }
    }

    void LateUpdate()
    {
        // 如果没有目标，不执行跟随
        if (target == null)
            return;

        // 处理震动效果
        if (isShaking)
        {
            HandleShake();
            return;
        }

        // 更新摄像机位置和旋转
        UpdateCameraPosition(true);
        UpdateCameraRotation();
    }

    // 更新摄像机位置
    private void UpdateCameraPosition(bool smooth)
    {
        // 计算目标位置
        Vector3 desiredPosition = target.position + offset;

        if (smooth)
        {
            // 平滑移动到目标位置
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
        else
        {
            // 直接设置位置
            transform.position = desiredPosition;
        }
    }

    // 更新摄像机旋转
    private void UpdateCameraRotation()
    {
        // 获取玩家当前X坐标
        float playerX = target.position.x;

        // 计算旋转角度（线性映射）
        // 当玩家在x=-maxPlayerX时，旋转y=maxRotationAngle
        // 当玩家在x=maxPlayerX时，旋转y=-maxRotationAngle
        float rotationY = Mathf.Lerp(maxRotationAngle, -maxRotationAngle,
                                     (playerX + maxPlayerX) / (2 * maxPlayerX));

        // 应用旋转
        Quaternion targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotationY, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * 2);
    }

    // 触发屏幕震动
    public void ShakeCamera()
    {
        // 如果已经在震动，不重复触发
        if (isShaking)
            return;

        // 记录原始位置
        originalPosition = transform.position;
        isShaking = true;
        currentShakeTime = 0f;
    }

    // 处理震动效果
    private void HandleShake()
    {
        // 更新震动时间
        currentShakeTime += Time.deltaTime;

        // 如果震动时间结束，恢复原位
        if (currentShakeTime >= shakeDuration)
        {
            transform.position = originalPosition;
            isShaking = false;
            return;
        }

        // 计算震动强度（随时间衰减）
        float currentIntensity = shakeIntensity * (1f - currentShakeTime / shakeDuration);

        // 应用随机偏移
        transform.position = originalPosition + Random.insideUnitSphere * currentIntensity;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLineController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 areaSize = new Vector3(6, 0, 10); // �������
    public Vector3 boundarySize = new Vector3(10, 0, 16); // ��������
    public Vector3 boundarySizeTWO = new Vector3(14, 0, 20);//����ʧ����
    void OnDrawGizmos()
    {
        DrawLine_Boundary(Color.blue, transform.position, areaSize);
        DrawLine_Boundary(Color.red, transform.position, boundarySize);
        DrawLine_Boundary(Color.green, transform.position, boundarySizeTWO);
    }
    
    //���ƾ�������
    private void DrawLine_Boundary(Color color,Vector3 center,Vector3 Radius)
    {
        Gizmos.color = color;

        // ���ƾ�������
        // ǰ��
        Gizmos.DrawLine(center + new Vector3(-Radius.x, 0, -Radius.z), center + new Vector3(Radius.x, 0, -Radius.z));
        // ���
        Gizmos.DrawLine(center + new Vector3(-Radius.x, 0, Radius.z), center + new Vector3(Radius.x, 0, Radius.z));
        // ���
        Gizmos.DrawLine(center + new Vector3(-Radius.x, 0, -Radius.z), center + new Vector3(-Radius.x, 0, Radius.z));
        // �ұ�
        Gizmos.DrawLine(center + new Vector3(Radius.x, 0, -Radius.z), center + new Vector3(Radius.x, 0, Radius.z));

    }
}

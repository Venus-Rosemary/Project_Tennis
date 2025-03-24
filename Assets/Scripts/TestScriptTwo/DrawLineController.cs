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
    public Vector3 areaSize = new Vector3(6, 0, 10); // 落点区域
    public Vector3 boundarySize = new Vector3(10, 0, 16); // 出界区域
    public Vector3 boundarySizeTWO = new Vector3(14, 0, 20);//球消失区域
    void OnDrawGizmos()
    {
        DrawLine_Boundary(Color.blue, transform.position, areaSize);
        DrawLine_Boundary(Color.red, transform.position, boundarySize);
        DrawLine_Boundary(Color.green, transform.position, boundarySizeTWO);
    }
    
    //绘制矩形区域
    private void DrawLine_Boundary(Color color,Vector3 center,Vector3 Radius)
    {
        Gizmos.color = color;

        // 绘制矩形区域
        // 前边
        Gizmos.DrawLine(center + new Vector3(-Radius.x, 0, -Radius.z), center + new Vector3(Radius.x, 0, -Radius.z));
        // 后边
        Gizmos.DrawLine(center + new Vector3(-Radius.x, 0, Radius.z), center + new Vector3(Radius.x, 0, Radius.z));
        // 左边
        Gizmos.DrawLine(center + new Vector3(-Radius.x, 0, -Radius.z), center + new Vector3(-Radius.x, 0, Radius.z));
        // 右边
        Gizmos.DrawLine(center + new Vector3(Radius.x, 0, -Radius.z), center + new Vector3(Radius.x, 0, Radius.z));

    }
}

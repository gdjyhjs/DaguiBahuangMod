using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(BarrierCtrl))]
public class BarrierCtrlEditor : Editor
{
    public const int mapWidthOneScene = 40;
    public const int mapHeightOneScene = 20;
    public const float mapSizeLength = 19.2f / mapWidthOneScene;           //格子长度
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BarrierCtrl barrier = target as BarrierCtrl;

        Repaint(); 
    }

    void OnSceneGUI()
    {
        Camera camera = GameObject.FindObjectOfType<Camera>();

        BarrierCtrl barrier = target as BarrierCtrl;

        Event e = Event.current;
        if (e.isKey)
        {
            bool isAdd = e.keyCode == KeyCode.A;
            bool isDel = !isAdd && (e.keyCode == KeyCode.A | e.keyCode == KeyCode.D);

            if (isAdd || isDel)
            {
                Vector3 worldPosition = Event.current.mousePosition;
                Ray r = Camera.current.ScreenPointToRay(new Vector3(worldPosition.x, -worldPosition.y + Camera.current.pixelHeight));
                worldPosition = r.origin;  //鼠标的世界坐标

                Vector3 posi = worldPosition;
                posi -= barrier.transform.position;
                Vector2Int point = new Vector2Int(Mathf.RoundToInt(posi.x / mapSizeLength), Mathf.RoundToInt(posi.y / mapSizeLength));

                if (isAdd && !barrier.points.Contains(point))
                {
                    barrier.points.Add(point);
                }
                if (isDel && barrier.points.Contains(point))
                {
                    barrier.points.Remove(point);
                }

                //更新预制体
                var prefabStage = PrefabStageUtility.GetPrefabStage(barrier.gameObject);
                if (prefabStage != null)
                {
                    EditorSceneManager.MarkSceneDirty(prefabStage.scene);
                }
            }
        }

        BarrierCtrlEditor.DrawPoint(barrier.points, barrier.transform.position);
    }

    public static void DrawPoint(List<Vector2Int> points, Vector3 posi)
    {
        for (int i = 0; i < points.Count; i++)
        {
            Handles.DrawWireCube(posi + new Vector3(mapSizeLength * points[i].x, mapSizeLength * points[i].y),
                new Vector3(mapSizeLength, mapSizeLength));
        }
    }
}

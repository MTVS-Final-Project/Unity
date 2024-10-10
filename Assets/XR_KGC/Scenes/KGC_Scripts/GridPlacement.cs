using System;
using UnityEngine;

public class GridPlacement : MonoBehaviour
{
    public float gridSize = 1.0f; // 격자의 크기 설정
    public Material highlightMaterial; // 하이라이트 효과를 위한 재질
    private GameObject highlightObject;
    private Camera mainCamera;
    private bool gridDrawn = false;

    [SerializeField]
    float gridDrawExtent = 5.0f; // 그리드의 최대 범위 설정

    private void Start()
    {
        mainCamera = Camera.main;
        DrawGrid();
        if (mainCamera != null)
        {
            print("카메라 있음");
        }
        else
        {
            print("없음");
        }
    }

    private void Update()
    {
        HighlightGrid();

        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 클릭을 감지
        {
            Vector3 mousePosition = Input.mousePosition;

            // 화면 좌표가 유효한지 확인합니다.
            if (mousePosition.x < 0 || mousePosition.y < 0 || mousePosition.x > Screen.width || mousePosition.y > Screen.height)
            {
                return; // 유효하지 않은 좌표인 경우 함수 종료
            }

            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                Vector3 targetPosition = hitInfo.point;
                Vector3 snappedPosition = SnapToGrid(targetPosition);

                // 물체 생성 또는 배치
                GameObject newObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newObject.transform.position = snappedPosition;
            }
        }
    }

    private void HighlightGrid()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning("Main camera is not assigned.");
            return; // 카메라가 없는 경우 함수 종료
        }

        Vector3 mousePosition = Input.mousePosition;

        // 마우스 좌표가 화면 범위 내에 있는지 확인
        if (mousePosition.x < 0 || mousePosition.y < 0 || mousePosition.x > Screen.width || mousePosition.y > Screen.height)
        {
            return; // 유효하지 않은 좌표인 경우 함수 종료
        }

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            Vector3 targetPosition = hitInfo.point;
            Vector3 snappedPosition = SnapToGrid(targetPosition);

            if (highlightObject == null)
            {
                highlightObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                highlightObject.GetComponent<Collider>().enabled = false; // 충돌 비활성화
                if (highlightMaterial != null)
                {
                    highlightObject.GetComponent<Renderer>().material = highlightMaterial;
                }
                else
                {
                    Debug.LogWarning("Highlight material is not assigned.");
                }
            }
            highlightObject.transform.position = snappedPosition;
        }
    }

    private void DrawGrid()
    {
        if (!gridDrawn && Application.isPlaying)
        {
            for (float x = -gridDrawExtent; x <= gridDrawExtent; x += gridSize)
            {
                Vector3 startPoint = new Vector3(x, 0, -gridDrawExtent);
                Vector3 endPoint = new Vector3(x, 0, gridDrawExtent);
                CreateLine(startPoint, endPoint);
            }

            for (float z = -gridDrawExtent; z <= gridDrawExtent; z += gridSize)
            {
                Vector3 startPoint = new Vector3(-gridDrawExtent, 0, z);
                Vector3 endPoint = new Vector3(gridDrawExtent, 0, z);
                CreateLine(startPoint, endPoint);
            }
            gridDrawn = true;
        }
    }

    private void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject line = new GameObject("GridLine");
        line.transform.position = start;
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    // 지정된 위치를 격자에 맞게 조정하는 함수
    Vector3 SnapToGrid(Vector3 originalPosition)
    {
        float x = Mathf.Round(originalPosition.x / gridSize) * gridSize + (gridSize / 2);
        float y = Mathf.Round(originalPosition.y / gridSize) * gridSize + (gridSize / 2);
        float z = Mathf.Round(originalPosition.z / gridSize) * gridSize + (gridSize / 2);

        return new Vector3(x, y, z);
    }
}
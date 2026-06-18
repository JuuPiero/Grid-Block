using DG.Tweening;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Material hightlight;
    public Material normal;

    public Block selectedBlock;

    GridManager gridManager;
    Camera mainCam;

    Vector3 dragOffset;
    Vector2Int originalGridPos;
    Vector3 originalWorldPos;
    Vector2Int lastValidGridPos;
    Vector3 lastValidWorldPos;
    float dragDepth;
    Tween movementTween;
    const float moveTweenDuration = 0.12f;

    void Start()
    {
        mainCam = Camera.main;
        gridManager = ServiceLocator.Get<GridManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                Block block = hit.collider.GetComponent<Block>();
                if (block != null)
                {
                    selectedBlock = block;
                    selectedBlock.SetHightLight(true, hightlight);

                    originalGridPos = block.position;
                    originalWorldPos = block.transform.position;
                    lastValidGridPos = originalGridPos;
                    lastValidWorldPos = originalWorldPos;
                    dragDepth = mainCam.WorldToScreenPoint(block.transform.position).z;
                    Vector3 worldPoint = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth));
                    dragOffset = block.transform.position - worldPoint;
                }
            }
        }

        if (Input.GetMouseButton(0) && selectedBlock != null)
        {
            Vector3 worldPoint = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, dragDepth));
            worldPoint.z = 0;
            Vector3 targetWorld = worldPoint + dragOffset;

            Vector2Int desiredGrid = gridManager.GetGridPosition(targetWorld);
            Vector2Int delta = desiredGrid - lastValidGridPos;
            Vector2Int step = new Vector2Int(Mathf.Clamp(delta.x, -1, 1), Mathf.Clamp(delta.y, -1, 1));

            if (step.x != 0 && step.y != 0)
            {
                Vector3 dir = targetWorld - gridManager.GetWorldPositionForGrid(lastValidGridPos);
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    step.y = 0;
                else
                    step.x = 0;
            }

            Vector2Int candidateGrid = lastValidGridPos + step;
            candidateGrid = gridManager.ClampToLevel(candidateGrid);
            if (gridManager.CanPlaceBlock(selectedBlock, candidateGrid))
            {
                lastValidGridPos = candidateGrid;
                lastValidWorldPos = gridManager.GetWorldPositionForGrid(candidateGrid);
                movementTween?.Kill();
                movementTween = selectedBlock.transform.DOMove(lastValidWorldPos, moveTweenDuration).SetEase(Ease.OutQuad);
            }
            else
            {
                movementTween?.Kill();
                selectedBlock.transform.position = lastValidWorldPos;
            }
        }

        if (Input.GetMouseButtonUp(0) && selectedBlock != null)
        {
            // Commit move through GridManager so occupancy maps are updated
            gridManager.TryMoveBlock(selectedBlock, lastValidGridPos);
            movementTween?.Kill();
            selectedBlock?.SetHightLight(false, normal);
            selectedBlock = null;
        }
    }
    


}

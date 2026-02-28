using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoryGridUI : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup gridLayout;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private RectTransform gridRect;

    private List<MemoryCellUI> cells = new List<MemoryCellUI>();
    private int currentGridSize;

    public void UpdateGrid(
        MemoryQuestion memQuestion,
        bool showPhase,
        HashSet<int> selectedCells,
        bool done,
        bool answered,
        Action<int> onCellClicked)
    {
        int gridSize = memQuestion.gridSize;
        int totalCells = gridSize * gridSize;

        // Rebuild grid if size changed
        if (gridSize != currentGridSize)
        {
            RebuildGrid(gridSize, totalCells);
            currentGridSize = gridSize;
        }

        // Update each cell
        for (int i = 0; i < cells.Count && i < totalCells; i++)
        {
            bool isHighlighted = memQuestion.highlighted.Contains(i);
            bool isSelected = selectedCells.Contains(i);
            bool isCorrectPick = isSelected && isHighlighted;
            bool isWrongPick = isSelected && !isHighlighted;
            bool showAsHighlighted = showPhase && isHighlighted;
            bool revealMissed = done && isHighlighted && !isSelected;

            Color bgColor;
            Color borderColor;

            if (showAsHighlighted)
            {
                bgColor = AppColors.Purple60;
                borderColor = AppColors.Purple80;
            }
            else if (isCorrectPick)
            {
                bgColor = AppColors.EasyGreen;
                borderColor = AppColors.EasyGreen;
            }
            else if (isWrongPick)
            {
                bgColor = AppColors.HardRed;
                borderColor = AppColors.HardRed;
            }
            else if (revealMissed)
            {
                bgColor = new Color(AppColors.Purple60.r, AppColors.Purple60.g, AppColors.Purple60.b, 0.5f);
                borderColor = AppColors.BorderLight;
            }
            else
            {
                bgColor = AppColors.SurfaceLight;
                borderColor = AppColors.BorderLight;
            }

            bool interactable = !showPhase && !answered;
            int capturedIndex = i;
            cells[i].SetState(bgColor, borderColor, interactable, () => onCellClicked(capturedIndex));
        }
    }

    private void RebuildGrid(int gridSize, int totalCells)
    {
        // Clear existing cells
        foreach (var cell in cells)
        {
            if (cell != null) Destroy(cell.gameObject);
        }
        cells.Clear();

        // Configure grid layout
        if (gridLayout != null)
        {
            gridLayout.constraintCount = gridSize;

            // Calculate cell size based on available space
            float availableWidth = gridRect != null ? gridRect.rect.width : 300f;
            float spacing = gridLayout.spacing.x;
            float cellSize = (availableWidth - spacing * (gridSize - 1)) / gridSize;
            gridLayout.cellSize = new Vector2(cellSize, cellSize);
        }

        // Create cells
        for (int i = 0; i < totalCells; i++)
        {
            if (cellPrefab == null) continue;
            GameObject cellObj = Instantiate(cellPrefab, gridLayout.transform);
            MemoryCellUI cellUI = cellObj.GetComponent<MemoryCellUI>();
            if (cellUI == null) cellUI = cellObj.AddComponent<MemoryCellUI>();
            cells.Add(cellUI);
        }
    }
}

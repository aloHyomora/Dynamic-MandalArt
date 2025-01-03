using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridNavigation : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public Color highlightColor;
    public Button[,] Buttons = new Button[9, 9];
    public int selectedRow = 0;
    public int selectedCol = 0;
    private Color _originalColor;

    private void Start()
    {
        int index = 0;
        foreach (Transform child in gridLayoutGroup.transform)
        {
            int row = index / 9;
            int col = index % 9;

            Buttons[row, col] = child.GetComponent<Button>();

            // AddListener로 버튼 클릭 이벤트 추가
            int capturedRow = row; // 지역 변수로 캡처
            int capturedCol = col; // 지역 변수로 캡처
            Buttons[row, col].onClick.AddListener(() =>
            {
                OnClickEvent(capturedRow, capturedCol, Buttons[capturedRow, capturedCol].name);
            });

            index++;
        }

        // 기본 버튼 색상 저장
        _originalColor = Buttons[0, 0].GetComponent<Image>().color;

        // 첫 버튼 강조 표시
        HighlightButton(selectedRow, selectedCol);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) MoveSelection(-1, 0);
        if (Input.GetKeyDown(KeyCode.DownArrow)) MoveSelection(1, 0);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveSelection(0, -1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveSelection(0, 1);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            // 버튼 포커스 상태에서 onClick 호출 방지
            if (EventSystem.current.currentSelectedGameObject != Buttons[selectedRow, selectedCol].gameObject)
            {
                Debug.Log($"Button at ({selectedRow}, {selectedCol}) {Buttons[selectedRow, selectedCol].name} selected!");
            }
        }
    }

    public void OnClickEvent(int row, int col, string buttonName)
    {
        Debug.Log($"Button at ({row}, {col}) with name '{buttonName}' selected!");
        MoveSelection(row - selectedRow, col - selectedCol);
    }

    private void MoveSelection(int rowChange, int colChange)
    {
        // 이전 버튼 색상 복원
        ResetButtonColor(selectedRow, selectedCol);

        // 새로운 선택 위치 계산
        selectedRow = Mathf.Clamp(selectedRow + rowChange, 0, 8);
        selectedCol = Mathf.Clamp(selectedCol + colChange, 0, 8);

        // 새로운 버튼 강조 표시
        HighlightButton(selectedRow, selectedCol);
    }
    private void HighlightButton(int row, int col)
    {
        var buttonImage = Buttons[row, col].GetComponent<Image>();
        _originalColor = buttonImage.color; 
        buttonImage.color = highlightColor; // 버튼 색상 강조
    }

    private void ResetButtonColor(int row, int col)
    {
        var buttonImage = Buttons[row, col].GetComponent<Image>();
        buttonImage.color = _originalColor; // 버튼 색상 복원
    }
}

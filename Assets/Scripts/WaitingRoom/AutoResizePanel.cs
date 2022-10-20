using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class AutoResizePanel : MonoBehaviour
{
    public int column = 2;
    public int row = 3;
    private GridLayoutGroup gridlayout;
    private void Awake()
    {
        gridlayout = this.GetComponent<GridLayoutGroup>();
    }
    private void Update()
    {
        float width = this.GetComponent<RectTransform>().rect.width;
        float height = 700;
        Vector2 spacing = gridlayout.spacing;

        Vector2 newSize = new Vector2(width / column - spacing.x, height / row - spacing.y);
        this.GetComponent<GridLayoutGroup>().cellSize = newSize;
        this.GetComponent<GridLayoutGroup>().constraintCount = column;
    }
}

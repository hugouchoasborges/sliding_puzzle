using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public List<Button> buttons;
    public const int TABLESIZE = 3;
    public int[][] table = new int[TABLESIZE][];

    // Start is called before the first frame update
    void Start()
    {
        table[0] = new int[TABLESIZE];
        table[1] = new int[TABLESIZE];
        table[2] = new int[TABLESIZE];
        
        for (int i = 0; i < TABLESIZE; i++)
        {
            for (int j = 0; j < TABLESIZE; j++)
            {
                if (i == 2 && j == 2)
                    continue;
                table[i][j] = 1 + TABLESIZE * i + j;
            }
        }
        fillButtons();
    }

    private int getAdjIdx(int row, int col)
    {
        int adjIdx = -1;

        if (row < 0 || col < 0 || row >= TABLESIZE || col >= TABLESIZE)
            adjIdx = -1;
        else
            adjIdx = TABLESIZE * row + col;

        return adjIdx;
    }

    private List<Button> getAdj(int row, int col)
    {
        List<Button> retButtons = new List<Button>();

        int lIdx = getAdjIdx(row - 1, col);
        if(lIdx >= 0)
            retButtons.Add(buttons[lIdx]);

        int rIdx = getAdjIdx(row + 1, col);
        if (rIdx >= 0)
            retButtons.Add(buttons[rIdx]);


        int dIdx = getAdjIdx(row, col - 1);
        if (dIdx >= 0)
            retButtons.Add(buttons[dIdx]);

        int uIdx = getAdjIdx(row, col + 1);
        if (uIdx >= 0)
            retButtons.Add(buttons[uIdx]);

        // int idx = col + TABLESIZE * row;
        // Debug.Log($"Indice {idx}");

        return retButtons;
    }

    public void SelectButton(int position)
    {
        int selected_row = (position - 1) / TABLESIZE;
        int selected_col = (position - 1) % TABLESIZE;

        Button curButton = buttons[position - 1];
        Text curText = curButton.GetComponentInChildren<Text>();

        List<Button> adjBtns = getAdj(selected_row, selected_col);

        foreach (Button b in adjBtns)
        {
            Text text = b.GetComponentInChildren<Text>();
            if(text.text == "0")
            {
                text.text = curText.text;
                curText.text = "0";
            }
        }
        // Debug.Log($"Clicked: [{selected_row}] [{selected_col}]");
    }

    public void fillButtons()
    {
        for(int i = 0; i < buttons.Count; i++)
        {
            Text t = buttons[i].GetComponentInChildren<Text>();
            int row = i / TABLESIZE;
            int col = i % TABLESIZE;
            t.text = table[row][col].ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

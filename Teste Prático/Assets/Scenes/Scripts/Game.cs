using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    #region Variables

    public const int TABLESIZE = 3;

    public Button[] buttonList = new Button[TABLESIZE * TABLESIZE];
    public Button[][] buttonTable = new Button[TABLESIZE][];
    public int[][] valuesTable = new int[TABLESIZE][];

    #endregion

    #region Start Methods

    void Start()
    {
        // Inicializa as tabelas do jogo
        FillTable();

        // Embaralha as posições dos blocos
        //Shuffle();

        // Atualiza a tela
        UpdateScreen();
    }

    /// <summary>
    /// Inicialização das tabelas buttonTable e valuesTable
    /// </summary>
    private void FillTable()
    {
        buttonTable[0] = new Button[TABLESIZE];
        buttonTable[1] = new Button[TABLESIZE];
        buttonTable[2] = new Button[TABLESIZE];

        valuesTable[0] = new int[TABLESIZE];
        valuesTable[1] = new int[TABLESIZE];
        valuesTable[2] = new int[TABLESIZE];

        for (int i = 0; i < TABLESIZE; i++)
        {
            for (int j = 0; j < TABLESIZE; j++)
            {
                int idx = j + TABLESIZE * i;
                buttonTable[i][j] = buttonList[idx];
                valuesTable[i][j] = (idx + 1) % (TABLESIZE * TABLESIZE);
            }
        }
    }

    #endregion

    #region Auxiliar Methods

    /// <summary>
    /// Atualiza o texto de um botão. Caso o valor seja 0, o botão será ocultado do jogo
    /// </summary>
    /// <param name="button">Botão a ser atualizado</param>
    /// <param name="value">O valor a ser colocado no botão</param>
    private void UpdateButtonText(Button button, int value)
    {
        TextMeshProUGUI textMP = button.GetComponentInChildren<TextMeshProUGUI>();
        Image buttonImg = button.GetComponent<Image>();
        if (value == 0)
        {
            buttonImg.enabled = false;
            textMP.text = null;
        }
        else
        {
            buttonImg.enabled = true;
            textMP.text = value.ToString();
        }
    }

    /// <summary>
    /// Pesquisa as posições de um botão na tabela buttonTable
    /// </summary>
    /// <param name="btn">Botão a ser pesquisado</param>
    /// <returns>
    /// Retorna uma tupla contendo a linha e coluna, correspondentemente, onde o botão foi encontrado
    /// Returna null caso não seja encontrado
    /// </returns>
    private Tuple<int, int> GetButtonRowCol(Button btn)
    {
        for (int i = 0; i < TABLESIZE; i++)
            for (int j = 0; j < TABLESIZE; j++)
                if (btn == buttonTable[i][j])
                    return Tuple.Create(i, j);

        return null;
    }

    /// <summary>
    /// Busca por botões na tabela buttonTable
    /// </summary>
    /// <param name="i">Linha para pesquisa</param>
    /// <param name="j">Coluna para pesquisa</param>
    /// <returns>Retorna o botão encontrado o Null caso não encontre nenhum</returns>
    private Button GetButton(int i, int j)
    {
        // Se estiver fora dos limites da tabela de botões
        if (i >= TABLESIZE || j >= TABLESIZE || i < 0 || j < 0)
            return null;

        return buttonTable[i][j];
    }

    /// <summary>
    /// Busca por botões adjacentes na tabela buttonTable
    /// </summary>
    /// <param name="i">Linha para pesquisa</param>
    /// <param name="j">Coluna para pesquisa</param>
    /// <returns>Retorna uma lista com todos os botões adjacentes encontrados</returns>
    private List<Button> GetAdjacentButtons(int row, int col)
    {
        List<Button> buttons = new List<Button>();

        // Left Button
        buttons.Add(GetButton(row - 1, col));
        buttons.Add(GetButton(row + 1, col));
        buttons.Add(GetButton(row, col - 1));
        buttons.Add(GetButton(row, col + 1));

        return buttons;
    }

    /// <summary>
    /// Procura pelo botão nulo
    /// </summary>
    /// <param name="row">Linha a ser pesquisada</param>
    /// <param name="col">Coluna a ser pesquisada</param>
    /// <returns>Retorna o botão nulo. Caso não o encontre, retorna nulo</returns>
    private Button GetNullButton(int row, int col)
    {
        List<Button> adjacentButtons = GetAdjacentButtons(row, col);

        foreach (Button b in adjacentButtons)
        {
            if (b == null)
                continue;

            if (!b.GetComponent<Image>().isActiveAndEnabled)
                return b;
        }

        return null;
    }

    #endregion

    #region Main Methods

    /// <summary>
    /// Embaralha os botões do jogo
    /// </summary>
    public void Shuffle()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Atualiza a tela com os valores preenchidos nos botões
    /// </summary>
    private void UpdateScreen()
    {
        for (int i = 0; i < TABLESIZE; i++)
            for (int j = 0; j < TABLESIZE; j++)
                UpdateButtonText(buttonTable[i][j], valuesTable[i][j]);
    }

    /// <summary>
    /// Realiza a troca de posição entre dois botões
    /// </summary>
    public void ButtonClick()
    {
        // Obtém o botão clicado
        GameObject gObj = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Button btn = gObj.GetComponentInChildren<Button>();
        TextMeshProUGUI textMP = btn.GetComponentInChildren<TextMeshProUGUI>();

        // Se o botão está com a imagem desativada, não faz nada
        if (!btn.GetComponent<Image>().isActiveAndEnabled)
            return;

        // Encontra as posições na tabela buttonTable do botão clicado
        (int row, int col) = GetButtonRowCol(btn);

        // Procura pelo botão nulo
        Button nullButton = GetNullButton(row, col);

        if (nullButton != null)
        {
            // Encontra as posições na tabela buttonTable do botão null
            (int nullRow, int nullCol) = GetButtonRowCol(nullButton);

            // Atualiza a tabela de valores
            valuesTable[nullRow][nullCol] = valuesTable[row][col];
            valuesTable[row][col] = 0;
        }

        // Atualiza a tela
        UpdateScreen();
    }

    #endregion
}

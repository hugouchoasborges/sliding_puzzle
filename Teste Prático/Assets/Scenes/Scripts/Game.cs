﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.UI;

using Random = System.Random;

public class Game : MonoBehaviour
{
    #region Variables

    // Constants
    public const int TABLESIZE = 3;

    // Shuffle Control Variables
    public int shuffleCount = 100;
    private bool shuffling = false;
    Random random = new Random(Environment.TickCount);

    // Gameover Controle
    private bool gameover = false;

    // Table Structure variables
    public Button[] buttonList = new Button[TABLESIZE * TABLESIZE];
    public Button[][] buttonTable = new Button[TABLESIZE][];
    public int[][] valuesTable = new int[TABLESIZE][];

    // UI Variables
    public GameObject shuffleButton;
    public GameObject restartButton;

    public TextMeshProUGUI movesValueText;
    public GameObject victoryText;

    #endregion

    #region Start Methods

    void Start()
    {
        Assert.IsNotNull(shuffleButton);
        Assert.IsNotNull(restartButton);
        Assert.IsNotNull(movesValueText);
        Assert.IsNotNull(victoryText);


        // Inicializa as tabelas do jogo
        FillTable();

        // Atualiza a tela
        UpdateScreen();

        // Embaralha as posições dos blocos
        Shuffle();
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

        buttons.Add(GetButton(row - 1, col));
        buttons.Add(GetButton(row + 1, col));
        buttons.Add(GetButton(row, col - 1));
        buttons.Add(GetButton(row, col + 1));

        // Remove botões não encontrados
        buttons.RemoveAll(b => b == null);

        return buttons;
    }

    /// <summary>
    /// Procura pelo botão nulo ao redor da posição passada
    /// </summary>
    /// <param name="row">Linha a ser pesquisada ao redor</param>
    /// <param name="col">Coluna a ser pesquisada ao redor</param>
    /// <returns>Retorna o botão nulo. Caso não o encontre, retorna nulo</returns>
    private Button GetNullButton(int row, int col)
    {
        List<Button> adjacentButtons = GetAdjacentButtons(row, col);

        foreach (Button b in adjacentButtons)
            if (!b.GetComponent<Image>().isActiveAndEnabled)
                return b;

        return null;
    }

    /// <summary>
    /// Procura pelo botão nulo
    /// </summary>
    /// <returns>Retorna o botão nulo</returns>
    private Button GetNullButton()
    {
        foreach (Button[] bRow in buttonTable)
            foreach(Button b in bRow)
                if (!b.GetComponent<Image>().isActiveAndEnabled)
                    return b;

        return null;
    }

    #endregion

    #region Main Methods

    /// <summary>
    /// Embaralha os botões do jogo
    /// </summary>
    public void Shuffle()
    {
        shuffling = true;
        for (int i = 0; i < shuffleCount; i++)
        {
            // Pega o botão vazio
            Button nullBtn = GetNullButton();

            // Recupera suas posições (linha,coluna)
            (int nullRow, int nullCol) = GetButtonRowCol(nullBtn);

            // Botões adjances ao botão vazio
            List<Button> adjButtons = GetAdjacentButtons(nullRow, nullCol);

            // Clica em um dos botões adjacentes ao botão vazio
            ButtonClick(adjButtons[random.Next(adjButtons.Count)]);
        }
        shuffling = false;
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
    /// Verifica se o jogo acabou (Player venceu)
    /// </summary>
    /// <returns>Retorna true se o jogo acabou, false caso contrário</returns>
    private bool CheckGameState()
    {
        const int tableSqrSize = TABLESIZE * TABLESIZE;

        for (int i = 0; i < TABLESIZE; i++)
            for (int j = 0; j < TABLESIZE; j++)
                if(valuesTable[i][j] != (j + TABLESIZE * i + 1) % tableSqrSize)
                    return false;
                
        return true;
    }

    /// <summary>
    /// Fim de jogo. Mostra um mensagem na tela, bloqueia novas jogadas e mostra um botão para reiniciar o jogo
    /// </summary>
    private void GameOver()
    {
        if (shuffling)
            return;

        gameover = true;
        restartButton.SetActive(true);
        shuffleButton.SetActive(false);
        victoryText.SetActive(true);
    }

    /// <summary>
    /// Realiza a troca de posição entre o botão nulo e outro
    /// </summary>
    /// <param name="btn">O botão a ser trocado de posição com o botão nulo</param>
    private void ButtonClick(Button btn)
    {
        if (gameover)
            return;

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
        
        if(!shuffling)
            movesValueText.text = (int.Parse(movesValueText.text) + 1).ToString();

        // Atualiza a tela
        UpdateScreen();

        // Condição de Vitória
        if (CheckGameState())
            GameOver();
    }

    /// <summary>
    /// Realiza a troca de posição entre dois botões
    /// </summary>
    public void ButtonClick()
    {
        // Obtém o botão clicado
        GameObject gObj = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        Button btn = gObj.GetComponentInChildren<Button>();
        ButtonClick(btn);
    }

    /// <summary>
    /// Reinicia o jogo
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion
}

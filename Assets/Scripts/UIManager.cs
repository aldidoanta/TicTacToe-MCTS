﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Manages the UI
public class UIManager : MonoBehaviour
{
    public Board board;
    public MCTSAI mctsai;

    public Text statusText;

    //Options UI
    public Toggle showUCTScoreToggle;
    public Slider MCTSIterationSlider;
    public InputField MCTSIterationInput;

    public GameObject boardUI;
    public Image SwitchX, SwitchO;
    public bool isXSelected, isCoroutineStarted;

    // Use this for initialization
    void Start()
    {
        isCoroutineStarted = false;

        switchSideToX(false);
        updateIterationNumberFromSlider(); //init MCTSIterationInput text value
    }

    // Update is called once per frame
    void Update()
    {
        //show current turn
        updateTextStatus();

        boardUI.SetActive(!board.isStarted); //readability?

        //side seletion UI
        if ((boardUI.activeSelf) && (!isCoroutineStarted))
        {
            if (isXSelected)
            {
                StartCoroutine(scaleUpDown(SwitchX));
            }
            else
            {
                StartCoroutine(scaleUpDown(SwitchO));
            }
        }
    }

    /*Functions related to UI*/

    public void playNewGame() //used by PlayButton
    {
        board.isStarted = true;

        board.initBoard();
        foreach (Square square in board.GetComponentsInChildren<Square>())
        {
            square.uctValue.gameObject.SetActive(showUCTScoreToggle.isOn);
        }
        mctsai.initAI();
    }

    public void switchSideToX(bool isX) //used by SwitchX and SwitchO
    {
        Color selectedCol = new Color(1f, 1f, 1f);
        Color deselectedCol = new Color(0.4f, 0.4f, 0.4f);
        if (isX) //choose X
        {
            SwitchX.color = selectedCol;
            SwitchO.color = deselectedCol;
            isXSelected = true;
            MCTSAI.myTurn = Board.TURN_O; //choose the opposite side for the AI
        }
        else //choose O
        {
            SwitchO.color = selectedCol;
            SwitchX.color = deselectedCol;
            isXSelected = false;
            MCTSAI.myTurn = Board.TURN_X;
        }
    }

    void updateTextStatus()
    {
        if (board.isStarted)
        {
            if (board.result == Board.RESULT_NONE)
            {
                statusText.text = (board.currentTurn == Board.TURN_X ? "X's turn" : "O's turn");
            }
            else
            {
                switch (board.result)
                {
                    case Board.RESULT_X:
                    {
                        statusText.text = "X wins";
                        break;
                    }
                    case Board.RESULT_O:
                    {
                        statusText.text = "O wins";
                        break;
                    }
                    case Board.RESULT_DRAW:
                    {
                        statusText.text = "Draw";
                        break;
                    }
                }
            }
        }
        else
        {
            statusText.text = ("Choose side");
        }
    }

    public void setShowUCTOption(bool UCTOption) //used by ShowUCTOption_Toggle
    {
        foreach (Square square in board.GetComponentsInChildren<Square>())
        {
            if (square.status == Square.SQUARE_EMPTY)
            {
                square.uctValue.gameObject.SetActive(UCTOption);
            }
        }
    }

    public void updateIterationNumberFromSlider() //TODO interactivity
    {
        int iterNumber = (int)MCTSIterationSlider.value;

        mctsai.iterationNumber = iterNumber;
        MCTSIterationInput.text = iterNumber.ToString();
    }

    public void updateIterationNumberFromText() //TODO interactivity
    {
        int iterNumber = int.Parse(MCTSIterationInput.text);

        mctsai.iterationNumber =  iterNumber;
        MCTSIterationSlider.value = iterNumber;
    }

    public IEnumerator scaleUpDown(Image img)
    {
        isCoroutineStarted = true;

        //reset scale
        SwitchX.rectTransform.localScale = Vector3.one;
        SwitchO.rectTransform.localScale = Vector3.one;

        //scale up
        while (img.rectTransform.localScale.x < 1.3)
        {
            float newX = img.rectTransform.localScale.x + 0.025f;
            float newY = img.rectTransform.localScale.y + 0.025f;
            img.rectTransform.localScale = new Vector3(newX, newY, 1);
            yield return null;
        }

        //scale down
        while (img.rectTransform.localScale.x > 1)
        {
            float newX = img.rectTransform.localScale.x - 0.025f;
            float newY = img.rectTransform.localScale.y - 0.025f;
            img.rectTransform.localScale = new Vector3(newX, newY, 1);
            yield return null;
        }

        isCoroutineStarted = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] int roundTimer;
    [SerializeField] float percentageToWin;

    int timeToStartPoppingTimerAt = 10;

    Canvas gameUI;
    Text timeText;
    AnimalControl animalControl;
    BearController bearController;



    void Awake()
    {
        gameUI = GameObject.FindGameObjectWithTag("GameUI").GetComponent<Canvas>();
        timeText = gameUI.transform.Find("Timer/Text").GetComponent<Text>();
        animalControl = GameObject.FindGameObjectWithTag("AnimalControl").GetComponent<AnimalControl>();
        bearController = GameObject.FindGameObjectWithTag("Player").GetComponent<BearController>();
    }

    private void Start()
    {
        StartCoroutine(Timer());
    }



    /// <summary>
    /// Sets the house's health, use a value between 0 and 1, representing a percentage
    /// </summary>
    /// <param name="value"></param>
    public void SetHouseHealth(float value)
    {
        // other stuff could go here

        if (value >= percentageToWin)
        {
            GameOver(GameOverType.Bear);
        }
    }

    bool gameOver = false;
    enum GameOverType { Cursor, Bear }
    void GameOver(GameOverType type)
    {
        gameOver = true;
        bearController.canMove = false;

        switch (type)
        {
            case GameOverType.Bear:
                StartCoroutine(MoveCameraForEnd());
                animalControl.Animate();
                StartCoroutine(DoAfter(() => { GameOverScreen(GameOverType.Cursor); }, 5f));
                break;
            case GameOverType.Cursor:
                StartCoroutine(DoAfter(() => { GameOverScreen(type); }, 2f));
                break;
        }
    }

    void GameOverScreen(GameOverType type)
    {
        gameUI.transform.Find("GameOver").gameObject.SetActive(true);
        Image gameOverBackground = gameUI.transform.Find("GameOver/Background").GetComponent<Image>();
        gameUI.transform.Find("GameOver/Game Over/Winner").GetComponent<Text>().text = type == GameOverType.Bear ? "Bear Wins" : "Bear Loses";
        //gameOverBackground.sprite = Resources.Load<Sprite>(string.Format("GameOver_{0}", type.ToString()));
    }

    IEnumerator Timer()
    {
        float roundStart = Time.time;
        UpdateTimerUI(roundTimer);

        int lastIntPassed = 0;
        while (Time.time < roundStart + roundTimer)
        {
            float timePassed = Time.time - roundStart;

            yield return new WaitForEndOfFrame();
            UpdateTimerUI(roundTimer - timePassed);

            if ((int)timePassed > lastIntPassed)
            {
                SecondTick((int)timePassed);
                lastIntPassed = (int)timePassed;
            }
        }

        SecondTick((int)roundTimer);
        UpdateTimerUI(0f);
        if (!gameOver)
        {
            GameOver(GameOverType.Cursor);
        }
    }

    void UpdateTimerUI(float _time = -1f)
    {
        float time = _time != -1f ? _time : Time.time;
        timeText.text = FormatTime(time);
    }

    string FormatTime(float time)
    {
        int intTime = (int)time;
        int minutes = intTime / 60;
        int seconds = intTime % 60;
        float fraction = time * 1000;
        fraction = (fraction % 1000);
        string timeText = String.Format("{0}:{1:00}:{2:000}", minutes, seconds, fraction);
        return timeText;
    }

    /// <summary> Every time a second ticks on the timer </summary>
    void SecondTick(int secondsPassed)
    {
        if (secondsPassed >= roundTimer - (float)timeToStartPoppingTimerAt)
        {
            PopTimer((float)(secondsPassed - roundTimer + timeToStartPoppingTimerAt) / (float)timeToStartPoppingTimerAt);
        }
    }

    /// <summary> Pops the UI timer out a bit. Power should be a number between 0 and 1 </summary>
    void PopTimer(float power)
    {
        StartCoroutine(CoPopTimer(power));
    }

    IEnumerator CoPopTimer(float power)
    {
        float duration = 1f;
        float startTime = Time.time;
        
        while (Time.time - startTime < duration)
        {
            float thisFramePower = (Time.time - startTime) / duration;
            timeText.rectTransform.localScale = Vector3.Slerp(Vector3.one * 1.5f, Vector3.one, thisFramePower);

            yield return new WaitForEndOfFrame();
        }

        timeText.rectTransform.localScale = Vector3.one;
    }
    
    IEnumerator MoveCameraForEnd()
    {
        float duration = 0.75f;
        float startTime = Time.time;

        Vector3 startPosition = Camera.main.transform.position;
        Vector3 newPosition = startPosition + Vector3.right * -5f;

        while (Time.time - startTime < duration)
        {
            yield return new WaitForEndOfFrame();

            float t = (Time.time - startTime) / duration;
            Vector3 thisTarget = Vector3.Slerp(startPosition, newPosition, t);
            Camera.main.transform.position = thisTarget;
        }
    }

    IEnumerator DoAfter(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Game");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] int roundTimer;

    int timeToStartPoppingTimerAt = 10;

    Canvas gameUI;
    Text timeText;



    void Awake()
    {
        gameUI = GameObject.FindGameObjectWithTag("GameUI").GetComponent<Canvas>();
        timeText = gameUI.transform.Find("Timer/Text").GetComponent<Text>();
    }

    private void Start()
    {
        StartCoroutine(Timer());
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
        EndGame();
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

    void EndGame()
    {

    }
}

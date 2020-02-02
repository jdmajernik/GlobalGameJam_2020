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
    AnimalControl animalControl;
    BearController bearController;



    void Awake()
    {
        gameUI = GameObject.FindGameObjectWithTag(GameplayStatics.GAME_UI_TAG).GetComponent<Canvas>();
        timeText = gameUI.transform.Find("Timer/Text").GetComponent<Text>();
        animalControl = GameObject.FindGameObjectWithTag(GameplayStatics.ANIMAL_CONTROL_TAG).GetComponent<AnimalControl>();
        bearController = GameObject.FindGameObjectWithTag("Player").GetComponent<BearController>();
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
        StartCoroutine(EndGame());
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
    
    IEnumerator EndGame()
    {
        bearController.canMove = false;

        StartCoroutine(MoveCameraForEnd());

        animalControl.Animate();

        yield return new WaitForSeconds(2f);
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
}

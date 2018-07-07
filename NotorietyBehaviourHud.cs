using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotorietyBehaviourHud : MonoBehaviour
{
    public Image notorietyBar;
    [SerializeField]
    private EnemyBehaviour enemy_script;
    private GameObject enemy;

    [SerializeField]
    private float notorietyLevel, maxNotorietyLevel, enemySightRange, distanceFromEnemy, alertInPercentage, counterBySeconds, speedRate, MaxAlertOffset;

    [SerializeField]
    private bool isOscillating;
    //todo
    private static int linear = 0, smooth = 1, chopped = 2;

    void Awake()
    {
        enemy = GameObject.FindWithTag("Enemy");
        enemy_script = enemy.GetComponent<EnemyBehaviour>();
    }

    void Start()
    {
        notorietyBar.type = Image.Type.Filled;
        notorietyBar.fillMethod = Image.FillMethod.Horizontal;
    }

    void Update()
    {
        CalculateDistanceFromEnemy();
        CalculateAlertInPercentage();
        UpdatingNotorietyLevel();
        OscillationEnableControl();
        OscillateHudBarEffect(isOscillating);
        PrototypingWarnings();
    }

    private void CalculateDistanceFromEnemy()
    {
        distanceFromEnemy = Vector3.Distance(this.transform.position, enemy_script.enemy.position);
    }

    private void CalculateAlertInPercentage()
    {
        alertInPercentage = (enemySightRange - distanceFromEnemy + MaxAlertOffset) / enemySightRange;
    }

    private void UpdatingNotorietyLevel()
    {
        if (distanceFromEnemy <= enemySightRange)
        {
            notorietyLevel = alertInPercentage * maxNotorietyLevel;
        }
        else
            notorietyLevel = 0;
    }

    private float OscillationSpeedRateBehaviour()
    {
        float speedRate = 0.25f;
        if (distanceFromEnemy < enemySightRange)
        {
            speedRate = speedRate + alertInPercentage;
        }
        else
            speedRate = 1;
        return speedRate;
    }

    private void OscillationEnableControl()
    {
        if (notorietyLevel > 0f && notorietyLevel < maxNotorietyLevel)
        {
            isOscillating = true;
        }
        else if ((notorietyBar.fillAmount < 0.001f && notorietyLevel == 0f) || (notorietyBar.fillAmount > 0.999f && notorietyLevel >= maxNotorietyLevel))
        {
            isOscillating = false;
        }
            
    }

    private float OscillationRange()
    {
        float oscillationRange = notorietyLevel / maxNotorietyLevel;
        return oscillationRange;
    }

    private float GenerateOscillation(int typeOfOscillation)
    {   //sin of 1.571 rad = 1, so each 1.571 seconds, it does half turn
        float rangeOfOscillation = OscillationRange();
        float halfRange = rangeOfOscillation / 2f;
        float secondsToloop = 4f;
        float angle = 11f;
        float oscillation = 0;
        switch (typeOfOscillation)
        {
            case 0:
                float linearOscillation = Mathf.PingPong(LoopCounterBySeconds(secondsToloop), rangeOfOscillation);
                oscillation = linearOscillation;
                break;
            case 1:
                float smoothOscillation = Mathf.Sin(LoopCounterBySeconds(secondsToloop) * angle) * halfRange + halfRange;
                oscillation = smoothOscillation;
                break;
            case 2:
                float choppedOscillation = Mathf.Abs(Mathf.Sin(LoopCounterBySeconds(secondsToloop) * angle));
                oscillation = choppedOscillation;
                break;
            default:
                break;
        }

        return oscillation;
    }

    private void OscillateHudBarEffect(bool isOscillating)
    {
        if (isOscillating)
        {
            float oscillatingValue = GenerateOscillation(smooth);
            notorietyBar.fillAmount = oscillatingValue;
        }
    }

    private float LoopCounterBySeconds(float restartLoop)
    {
        speedRate = OscillationSpeedRateBehaviour();
        counterBySeconds = counterBySeconds + speedRate * Time.deltaTime;
        if (counterBySeconds >= restartLoop)
        {
            counterBySeconds = 0f;
        }
        return counterBySeconds;
    }

    private void PrototypingWarnings()
    {
        if (maxNotorietyLevel <= 0 || enemySightRange <= 0)
        {
            Debug.LogWarning("Attributes negatives or zero out");
        }
        if (maxNotorietyLevel == null || enemySightRange == null)
        {
            Debug.LogWarning("Character or enemies attributes not found.");
        }
    }

}

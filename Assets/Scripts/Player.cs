using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public event UnityAction<int> ScoreChanged;
    public event UnityAction<int> HealthChanged;

    [SerializeField]
    private Blade blade;
    [SerializeField]
    private Destroyer destroyer;
    [SerializeField]
    private MenuController menuController;
    [SerializeField]
    private Ads ads;

    private int score;
    private int health = 5;

    private void OnEnable()
    {
        blade.Slice += OnSlice;
        destroyer.Health += OnHealth;
    }

    private void OnDisable()
    {
        blade.Slice -= OnSlice;
        destroyer.Health -= OnHealth;
    }

    private void OnSlice()
    {
        score++;
        ScoreChanged?.Invoke(score);
    }

    private void OnHealth()
    {
        health--;
        if (health == 0)
        {
           // ads.StartCoroutine("ShowAd");
            menuController.LoseGame();
            
        }
        HealthChanged?.Invoke(health);
    }
}

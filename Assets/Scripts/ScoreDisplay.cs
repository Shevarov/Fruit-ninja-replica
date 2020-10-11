using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent (typeof (Text)) ]
public class ScoreDisplay : MonoBehaviour
{
    public bool qwer;

    [SerializeField]
    private Player player;

    private Text text;

    private void Start()
    {
        text = GetComponent<Text>();
    }

    private void OnEnable()
    {
        if(qwer)
        player.ScoreChanged += OnScoreChanged;
        else
        player.HealthChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        if(qwer)
        player.ScoreChanged -= OnScoreChanged;
        else
        player.HealthChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(int count)
    {
        text.text = count.ToString();
    }

}

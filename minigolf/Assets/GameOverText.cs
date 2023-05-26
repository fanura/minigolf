using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverText : MonoBehaviour
{
    [SerializeField] TMP_Text gameOverText;

    public void UpdateText()
    {
        gameOverText.text = "Game Over!";
    }
}

using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    public void ShowGameOverPanel()
    {
        // Aktifkan panel game over
        gameObject.SetActive(true);
    }

    public void HideGameOverPanel()
    {
        // Nonaktifkan panel game over
        gameObject.SetActive(false);
    }
}
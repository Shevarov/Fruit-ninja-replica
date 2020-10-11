using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject settingsPanel;

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void TimePause()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }

        else
        {
            Time.timeScale = 1;
        }
    }

    public void GoToMainMenu()
    {
        TimePause();
        SceneManager.LoadScene(0);
    }

    public void RestartGame()
    {
        TimePause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadInstagram()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        { }
        else
        Application.OpenURL("https://www.instagram.com/ilya_shevarov/");
    }

    public void LoseGame()
    {
        TimePause();
        settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
}

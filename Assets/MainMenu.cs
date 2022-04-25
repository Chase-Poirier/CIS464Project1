using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private IntSO healthSO;
    private void Start()
    {
        FindObjectOfType<AudioManager>().Play("Music");
        healthSO.Value = 100;
    }

    public void PlayGame() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Game closing.");
        Application.Quit();
    }
}

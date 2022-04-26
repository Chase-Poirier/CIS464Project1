using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public void WinGame()
    {
        SceneManager.LoadScene(0);
    }
}

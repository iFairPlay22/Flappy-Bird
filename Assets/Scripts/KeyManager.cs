using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyManager : MonoBehaviour
{
    [SerializeField]
    Player player;

    void Update()
    {
        if (GameManager.Instance.state == GameState.GAME)
        {
            if (Input.GetButtonDown("Jump"))
            {
                player.Jump();
            }
        }

        if (Input.GetButtonDown("Quit"))
        {
            Application.Quit();
        }

        if (Input.GetButtonDown("Reload"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        
    }
}

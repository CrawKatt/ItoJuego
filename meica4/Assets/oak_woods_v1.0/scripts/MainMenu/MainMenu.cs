using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public enum MenuOption
    {
        Start,
        Gallery,
        Settings,
        Info,
        Exit,
        MainMenu,
    }

    public void SelectOption(string option)
    {
        if (System.Enum.TryParse(option, out MenuOption selectedOption))
        {
            switch (selectedOption)
            {
                case MenuOption.Start:
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                    break;
                case MenuOption.Gallery:
                    SceneManager.LoadScene("Gallery");
                    break;
                case MenuOption.Settings:
                    SceneManager.LoadScene("Settings");
                    break;
                case MenuOption.Info:
                    SceneManager.LoadScene("Info");
                    break;
                case MenuOption.Exit:
                    Debug.Log("Exiting game...");
                    Application.Quit();
                    break;
                case MenuOption.MainMenu:
                    SceneManager.LoadScene("MainMenu");
                    break;
            }
        }
        else
        {
            Debug.LogError("Invalid menu option: " + option);
        }
    }
}

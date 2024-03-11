using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject menuBackground, inputBackground, creditsBackground;

    int lionCount;
    int dogCount;
    int catCount;
    int chickenCount;

    private void Start() {
        openMainMenu();
    }

    public void openMainMenu() {
        menuBackground.SetActive(true);
        inputBackground.SetActive(false);
        creditsBackground.SetActive(false);
    }

    public void setUpGame() {
        inputBackground.SetActive(true);
        menuBackground.SetActive(false);
        creditsBackground.SetActive(false);
    }

    public void openCredits() {
        creditsBackground.SetActive(true);
        inputBackground.SetActive(false);
        menuBackground.SetActive(false);
    }

    public void exitGame() {
        Application.Quit();
    }

    public void setLionCount(string input) {
        lionCount = clampInputValue(input);
        GameManager.s_instance.SetNumberOfLions(lionCount);
    }

    public void setDogCount(string input) {
        dogCount = clampInputValue(input);
        GameManager.s_instance.SetNumberOfDogs(dogCount);
    }

    public void setCatCount(string input) {
        catCount = clampInputValue(input);
        GameManager.s_instance.SetNumberOfCats(catCount);
    }

    public void setChickenCount(string input) {
        chickenCount = clampInputValue(input);
        GameManager.s_instance.SetNumberOfChickens(chickenCount);
    }

    int clampInputValue(string input) {
        int inputValue = 2;
        if(int.TryParse(input, out inputValue)) {

            inputValue = Mathf.Clamp(inputValue, 2, 12);
        } else {
            inputValue = 2;
        }
        return inputValue;
    }

    public void goToLevel() {
        SceneManager.LoadScene(1);
    }
}

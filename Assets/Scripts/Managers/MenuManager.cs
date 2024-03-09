using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    int lionCount;
    int dogCount;
    int catCount;
    int chickenCount;

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
}
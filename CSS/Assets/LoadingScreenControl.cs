using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenControl : MonoBehaviour
{

    public DifficultySelector diffManager;

    public void OnClickedPlay(int diffLevel)
    {
        diffManager.difficultyLevel = diffLevel;
        SceneManager.LoadScene("ReskinScene");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenControl : MonoBehaviour
{
    public void OnClickedPlay()
    {
        SceneManager.LoadScene("ReskinScene");
    }
}

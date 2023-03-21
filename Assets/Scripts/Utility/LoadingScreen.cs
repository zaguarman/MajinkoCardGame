using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PeggleWars.Utilities
{
    internal class LoadingScreen : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(WaitForLoadingScreen());
        }

        private IEnumerator WaitForLoadingScreen()
        {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("LevelOne");
        }
    }
}

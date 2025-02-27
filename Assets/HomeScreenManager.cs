using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HomeScreenManager : MonoBehaviour
{
    public float delay = 1.5f;
    public GameObject PopUp;
    [Header("Audio Settings")]
    public AudioSource audioSource;  
    public AudioClip startSound;     

    void Start()
    {
        if (audioSource != null && startSound != null)
        {
            audioSource.PlayOneShot(startSound);
        }
    }

    public void ChangeScene()
    {
        StartCoroutine(ChangeSceneWithDelay());
    }

    private IEnumerator ChangeSceneWithDelay()
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(1);
    }

    public void EnablePopup()
    {
        PopUp.SetActive(true);
    }
    public void DisablePopup()
    {
        PopUp.SetActive(false);
    }
}

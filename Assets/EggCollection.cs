using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EggCollection : MonoBehaviour
{
    public GameObject eggPopup;       
    public Animator eggAnimator;      
    public GameObject upgradedDragonPrefab; 
    public Button finalCollectButton; 
    public string nextSceneName; 
    public Transform cameraTargetPosition; 

    private bool isCollected = false;

    void Start()
    {
        finalCollectButton.onClick.AddListener(delegate
        {
            CollectDragon();
        });
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Egg") && !isCollected) 
                {
                    CollectEgg();
                }
            }
        }
    }

    void CollectEgg()
    {
        isCollected = true;

        Invoke("CrackEgg", 3f);
    }

    void CrackEgg()
    {
        eggAnimator.Play("EggCracking"); // Play the egg cracking animation

        Invoke("ShowUpgradedDragon", 2.5f);
    }

    void ShowUpgradedDragon()
    {
        GameObject upgradedDragon = Instantiate(upgradedDragonPrefab, transform.position, Quaternion.identity);
        eggPopup.SetActive(true);

        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null && cameraTargetPosition != null)
        {
            cameraFollow.MoveToUpgradedDragon(upgradedDragon.transform, cameraTargetPosition, 2f);
        }
    }

    public void CollectDragon()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}

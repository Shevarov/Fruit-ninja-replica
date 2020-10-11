using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class Ads : MonoBehaviour
{
    private string gameId = "3823827", type = "video";
    private bool testMode = false;

    private Coroutine showAd;

    private static int countLoses;

    private void Start()
    {
        Advertisement.Initialize(gameId, testMode);

        countLoses++;
        if(countLoses%3==0)
            showAd = StartCoroutine(ShowAd());
    }

    public IEnumerator ShowAd()
    {
        while (true)
        {
            if (Advertisement.IsReady(type))
            {
                Advertisement.Show(type);
                break;
            }
            yield return new WaitForSeconds(1);
        }
    }


   
}


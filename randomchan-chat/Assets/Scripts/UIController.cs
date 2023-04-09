using System.Threading;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public async void TestApi()
    {
        var apiController = new ChatGPTAPITest.ChatGPTConnection();
        await apiController.RequestAsync();
        return;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {
    
	void Start () {
        InitNet();
        InitModel();
    }
	
    void InitNet()
    {
        this.gameObject.AddComponent<NetManager>();
        NetManager.Instance.SendConnect();
    }

    void InitModel()
    {
        this.gameObject.AddComponent<LoginModel>();
        this.gameObject.AddComponent<ChatModel>();
    }
}

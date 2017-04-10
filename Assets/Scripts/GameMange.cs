using UnityEngine;
using UnityEngine . UI;

public class GameMange : MonoBehaviour {

    public Text inputfield1;
    public Text inputfield2;
    public Text text;
    public int sceneIndex;
    public GameObject logout;
    public string password;
    public UniWebView uwv;

    void Start ()
    {      
    }

    
    void Update ()
    {
        
        if (Application . platform == RuntimePlatform . Android && ( Input . GetKeyDown (KeyCode . Escape) ))
        {          
            //弹窗，让玩家确认是否退出
            logout . SetActive (true);
        }
    }

    public void IsQuit (bool quit)
    {
        
        if (quit)
        {
            Application . Quit ();
        }
    }

    public void IsContinue ()
    {
        logout . SetActive (false);
    }

}

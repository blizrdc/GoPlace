using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class User : MonoBehaviour {

    private const string ip = "http://www.snowcheng.com/goplace/public";    // 用于登陆的ip地址
    private string password;                                        // 密码
    private string email;                                           // 邮箱-登陆账号

    public UniWebView uniwebview;                                   // 手机用的浏览器控件                                         
    public int sceneIndex;                                          // 登陆后跳转的场景编号

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    /// <summary>
    /// 把登陆后，服务器下发的Cookie转变为标准Cookie
    /// </summary>
    /// <param name="cookie">服务器返回cookie</param>
    /// <returns>标准cookie</returns>
    private string getCookie(string cookie) {
        cookie = Regex.Replace(cookie, "expires=Wed, \\d+-[a-zA-Z]+-\\d+ \\d+:\\d+:\\d+ GMT; Max-Age=7200;", "");
        cookie = cookie.Replace("path=/,", "");
        cookie = cookie.Replace("path=/; HttpOnly", "");
        return cookie;
    }

    /// <summary>
    /// Post错误处理
    /// </summary>
    /// <param name="www">WWW类信息</param>
    /// <returns>bool值</returns>
    private bool wwwError(WWW www)
    {
        if (www.error != null)
        {
            Debug.Log(www.error);
            return false;
        }
        return true;
    }

    /// <summary>
    /// uniwebview关闭调用的方法
    /// </summary>
    /// <param name="webView">UniWebView类</param>
    /// <returns>bool值</returns>
    private bool OnWebViewShouldClose(UniWebView webView)
    {
        webView.Hide();
        UnityEngine.Object.Destroy(webView);
        uniwebview = null;
        return true;
    }

    /// <summary>
    /// uniwebview加载完成调用的方法
    /// </summary>
    /// <param name="webView">UniWebView类</param>
    /// <param name="success">成功标志</param>
    /// <param name="errorMessage">错误信息</param>
    private void OnLoadComplete(UniWebView webView, bool success, string errorMessage)
    {
        if (success)
        {
            webView.Show();
        }
        else
        {
            Debug.LogError("Something wrong in web view loading: " + errorMessage);
        }
    }

    /// <summary>
    /// 登陆方法
    /// </summary>
    public void Login() {

        // 通过 GameObject.Find("")获得控件输入的信息
        password = GameObject.Find("Password").GetComponent<InputField>().text;
        email = GameObject.Find("Username").GetComponent<InputField>().text;

        //生成post所需的键值对
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("email", email);
        dic.Add("password", password);

        // 启动Login的协程
        StartCoroutine(loginIEnumerator(ip + "/user/login", dic));
    }

    /// <summary>
    /// 登陆调用的协程方法
    /// </summary>
    /// <param name="url">访问服务器ip</param>
    /// <param name="postData">上传的数据</param>
    /// <returns>WWW类</returns>
    IEnumerator loginIEnumerator(string url, Dictionary<string, string> postData)
    {
        // 生成POST所需的WWWForm类
        WWWForm form = new WWWForm();

        // 注入信息
        foreach (KeyValuePair<string, string> postArg in postData)
        {
            form.AddField(postArg.Key, postArg.Value);
        }

        // post上传数据
        WWW www = new WWW(url, form);
        yield return www;
       
        // 处理返回信息
        if (wwwError(www))
        {
            // 注入生成返回信息类Userinfomation
            Userinfomation userinfomation = LitJson.JsonMapper.ToObject<Userinfomation>(www.text);

            // 服务端返回异常标志处理
            if (userinfomation.status != "200")
            {
                Debug.Log(userinfomation.status);
                yield return userinfomation.status;
            }

            // 服务端返回正常标志处理
            else
            {
                // 存储具体信息
                PlayerPrefs.SetString("COOKIE", getCookie(www.responseHeaders["Set-Cookie"]));
                PlayerPrefs.SetString("_tokenpasswd", userinfomation._tokenpasswd);
                PlayerPrefs.SetString("user_id", userinfomation.userallinfo.id.ToString());
                PlayerPrefs.SetString("user_email", userinfomation.userallinfo.email);
                // 场景跳转
                SceneManager.LoadScene(sceneIndex);
            }
        }
    }

    /// <summary>
    /// 注册调用的方法
    /// </summary>
    public void Register() {
        string url = ip + "/register";
        var webViewGameObject = GameObject.Find("WebView");
        if (webViewGameObject == null)
        {
            webViewGameObject = new GameObject("WebView");
        }
        var webView = webViewGameObject.AddComponent<UniWebView>();
        webView.OnLoadComplete += OnLoadComplete;
        webView.InsetsForScreenOreitation += InsetsForScreenOreitation;
        webView.OnWebViewShouldClose += OnWebViewShouldClose;
        webView.url = url;
        webView.Load();
    }

    UniWebViewEdgeInsets InsetsForScreenOreitation(UniWebView webView, UniWebViewOrientation orientation)
    {

        if (orientation == UniWebViewOrientation.Portrait)
        {
            return new UniWebViewEdgeInsets(5, 5, 5, 5);
        }
        else
        {
            return new UniWebViewEdgeInsets(5, 5, 5, 5);
        }
    }

    /*private void setInformation(AllInfo allinfo)
    {
        PlayerPrefs.SetString("status", allinfo.status);
        PlayerPrefs.SetString("id", allinfo.info.id);
        PlayerPrefs.SetString("userid", allinfo.info.userid);
        PlayerPrefs.SetString("grade", allinfo.info.grade);
        PlayerPrefs.SetString("exp", allinfo.info.exp);
        PlayerPrefs.SetString("attack", allinfo.info.attack);
        PlayerPrefs.SetString("defense", allinfo.info.defense);
        PlayerPrefs.SetString("life", allinfo.info.life);
        PlayerPrefs.SetString("crit", allinfo.info.crit);
        PlayerPrefs.SetString("criticaldamage", allinfo.info.criticaldamage);
        PlayerPrefs.SetString("weaponsnumber", allinfo.weapons.Count.ToString());
        for (int i = 0; i < allinfo.weapons.Count; i++)
        {
            PlayerPrefs.SetString("weapons" + i.ToString() + "_id", allinfo.weapons[i].id);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_userid", allinfo.weapons[i].userid);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weaponid", allinfo.weapons[i].weaponid);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weapon_" + "id", allinfo.weapons[i].weapon.id);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weapon_" + "name", allinfo.weapons[i].weapon.name);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weapon_" + "picture", allinfo.weapons[i].weapon.picture);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weapon_" + "content", allinfo.weapons[i].weapon.content);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weapon_" + "attack", allinfo.weapons[i].weapon.attack);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weapon_" + "defense", allinfo.weapons[i].weapon.defense);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weapon_" + "life", allinfo.weapons[i].weapon.life);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weapon_" + "crit", allinfo.weapons[i].weapon.crit);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weapon_" + "criticaldamage", allinfo.weapons[i].weapon.criticaldamage);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weapon_" + "area", allinfo.weapons[i].weapon.area);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_weapon_" + "grade", allinfo.weapons[i].weapon.grade);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_used", allinfo.weapons[i].used);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_x", allinfo.weapons[i].x);
            PlayerPrefs.SetString("weapons" + i.ToString() + "_y", allinfo.weapons[i].y);
        }
        PlayerPrefs.SetString("badgenumber", allinfo.props.badge.Count.ToString());
        for (int i = 0; i < allinfo.props.badge.Count; i++)
        {
            PlayerPrefs.SetString("badge" + i.ToString() + "_id", allinfo.props.badge[i].id);
            PlayerPrefs.SetString("badge" + i.ToString() + "_name", allinfo.props.badge[i].name);
            PlayerPrefs.SetString("badge" + i.ToString() + "_picture", allinfo.props.badge[i].picture);
            PlayerPrefs.SetString("badge" + i.ToString() + "_content", allinfo.props.badge[i].content);
            PlayerPrefs.SetString("badge" + i.ToString() + "_grade", allinfo.props.badge[i].grade);
            PlayerPrefs.SetString("badge" + i.ToString() + "_x", allinfo.props.badge[i].x);
            PlayerPrefs.SetString("badge" + i.ToString() + "_y", allinfo.props.badge[i].y);
            PlayerPrefs.SetString("badge" + i.ToString() + "_number", allinfo.props.badge[i].number);
        }
    }*/
}


[System.Serializable]
class Userinfomation
{
    public string status;
    public Userallinfo userallinfo;
    public List<Badge> badges;
    public List<Fragment> fragments;
    public List<Chest> chests;
    public List<Weapon> weapons;
    public List<Key> keys;
    public List<Petegg> peteggs;
    public List<Money> moneys;
    public List<Attribute> attribute;
    public string _tokenpasswd;
}

[System.Serializable]
class Userallinfo
{
    public int id;
    public string name;
    public string email;
    public int age;
    public int sex;
    public string phone;
    public string head;
}

[System.Serializable]
class Badge
{
    public int id;
    public string name;
    public string picture;
    public string content;
    public int grade;
    public string created_at;
    public string updated_at;
    public int pivot_userid;
    public int pivot_badgeid;
    public int pivot_number;
}

[System.Serializable]
class Fragment
{
    public int id;
    public int classnumber;
    public int classid;
    public string name;
    public string picture;
    public int requirednumber;
    public int requiredcost;
    public string created_at;
    public string updated_at;
    public int pivot_userid;
    public int pivot_fragmentid;
    public int pivot_number;
}

[System.Serializable]
class Chest
{
    public int id;
    public string name;
    public string picture;
    public int money;
    public int grade;
    public string created_at;
    public string updated_at;
    public int pivot_userid;
    public int pivot_chestid;
    public int pivot_number;
}

[System.Serializable]
class Weapon
{
    public int id;
    public string name;
    public string picture;
    public string content;
    public string attack;
    public string defense;
    public string life;
    public int crit;
    public int criticaldamage;
    public string area;
    public int grade;
    public string created_at;
    public string updated_at;
    public int pivot_userid;
    public int pivot_weaponid;
    public int pivot_used;
}

[System.Serializable]
class Key
{
    public int id;
    public string name;
    public string picture;
    public int chestid;
    public int grade;
    public string created_at;
    public string updated_at;
    public int pivot_userid;
    public int pivot_keyid;
    public int pivot_number;
}

[System.Serializable]
class Petegg
{
    public int id;
    public int weaponid;
    public string name;
    public string picture;
    public int requiredcost;
    public int grade;
    public string created_at;
    public string updated_at;
    public int pivot_userid;
    public int pivot_peteggid;
    public int pivot_number;
}

[System.Serializable]
class Money
{
    public int money;
}

[System.Serializable]
class Attribute
{
    public int userid;
    public int grade;
    public int experience;
    public string attack;
    public string defense;
    public string life;
    public int crit;
    public int updated_at;
    public int criticaldamage;
}

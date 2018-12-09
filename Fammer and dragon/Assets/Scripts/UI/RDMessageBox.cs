/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-7-2
  *Description: MessageBox,需要在游戏的第一个场景中增加RDMessageBoxCanvas
**********************************************************************************/
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
public class RDMessageBox : MonoBehaviour
{
    private static GameObject rDMessageBox;
    private static Text infoText;
    private static Button commitButton;
    private static Button cancelButton;
    private static UnityAction commitAction;
    // Use this for initialization
    void Awake()
    {
        //获取控件
        rDMessageBox = GameObject.Find("RDMessageBox");
        infoText = rDMessageBox.transform.Find("InfoText").GetComponent<Text>();
        commitButton = rDMessageBox.transform.Find("CommitButton").GetComponent<Button>();
        cancelButton = rDMessageBox.transform.Find("CancelButton").GetComponent<Button>();
        //默认Commit
        commitAction = new UnityAction(Commit);
        rDMessageBox.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }
    //显示MessageBox
    public static void Show(string _info, UnityAction _commitFuction)
    {
        infoText.text = _info;
        commitAction += _commitFuction;
        commitButton.onClick.AddListener(commitAction);
        cancelButton.onClick.AddListener(() => Cancel());
        rDMessageBox.SetActive(true);
    }
    //取消
    static void Cancel()
    {
        rDMessageBox.SetActive(false);
    }
    static void Commit()
    {
        rDMessageBox.SetActive(false);
    }
}

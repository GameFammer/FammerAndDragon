/********************************************************************************* 
  *Author:AICHEN
  *Date:  2018-7-2
  *Description: MessageBox,需要在游戏的第一个场景中增加FDMessageBoxCanvas
**********************************************************************************/
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
public class FDMessageBox : MonoBehaviour
{
    private static GameObject fDMessageBox;
    private static Text infoText;
    private static Button commitButton;
    private static Button cancelButton;
    private static UnityAction commitAction;
    // Use this for initialization
    void Awake()
    {
        //获取控件
        fDMessageBox = GameObject.Find("FDMessageBox");
        infoText = fDMessageBox.transform.Find("InfoText").GetComponent<Text>();
        commitButton = fDMessageBox.transform.Find("CommitButton").GetComponent<Button>();
        cancelButton = fDMessageBox.transform.Find("CancelButton").GetComponent<Button>();
        //默认Commit
        commitAction = new UnityAction(Commit);
        fDMessageBox.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }
    //显示MessageBox
    public static void Show(string _info, UnityAction _commitFuction)
    {
        infoText.text = _info;
        commitAction += _commitFuction;
        commitButton.onClick.AddListener(commitAction);
        cancelButton.onClick.AddListener(() => Cancel());
        fDMessageBox.SetActive(true);
    }
    //取消
    static void Cancel()
    {
        fDMessageBox.SetActive(false);
    }
    static void Commit()
    {
        fDMessageBox.SetActive(false);
    }
}

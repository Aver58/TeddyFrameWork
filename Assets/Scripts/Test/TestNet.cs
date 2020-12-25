using UnityEngine;
using UnityEngine.UI;

public class TestNet : MonoBehaviour
{
    public Button m_btn;
    void Start()
    {
        // 初始化按钮
        m_btn = GameObject.Find("Button").GetComponent<Button>();
        if(m_btn)
        {
            m_btn.onClick.AddListener(OnBtnClick);
        }

        // 初始化连接
        //ClientSocket.Instance.Connect();
        //KCPConnector.instance.Connect("127.0.0.1", 8090);
        TestKCP.Connect();
        //var text = Encoding.UTF8.GetBytes(string.Format("Hello KCP: {0}", ++counter));


        // 注册
        NetMsg.AddListener(Network.OpCode.S2C_TestResponse, new NetHandler(Test));
    }

    private void Test(string msg)
    {
        Debug.Log(msg);
    }

    private void OnBtnClick()
    {
        NetMsg.SendMsg(new NetMsgData(Network.OpCode.C2S_TestRequest, "1111"));
    }
}

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
        //TcpClient.Instance.Connect();
        //KCPConnector.instance.Connect("127.0.0.1", 8090);
        //UDPClient.instance.Connect();

        KCPClient.instance.Connect("127.0.0.1", 8090);
        //注册
        NetMsg.AddListener(Network.OpCode.S2C_TestResponse, new NetHandler(Test));
    }

    private void Test(string msg)
    {
        Debug.Log(msg);
    }

    private void OnBtnClick()
    {
        //NetMsg.SendMsg(new NetMsgData(Network.OpCode.C2S_TestRequest, "1111"));
        //NetMsg.SendUDPMsg(new NetMsgData(Network.OpCode.C2S_TestRequest, "1111"));
        NetMsg.SendKCPMsg(new NetMsgData(Network.OpCode.C2S_TestRequest, "1111"));
    }
}

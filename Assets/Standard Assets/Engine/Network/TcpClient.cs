using System;
using System.Net.Sockets;

public class TcpClient
{
    #region Instance
    private class InstanceHolder { public static TcpClient Instance = new TcpClient(); }
    public static TcpClient Instance { get { return InstanceHolder.Instance; } }

    public ConnectState State => throw new NotImplementedException();
    #endregion

    //private bool m_Active;
    private Socket m_Socket;

    static byte[] buffer = new byte[1024];

    //public event Action OnConnected;
    //public event Action OnDisconnected;

    // 连接到远程主机
    public void Connect(string host = "127.0.0.1", int port = 8090)
    {
        if (m_Socket != null)
        {
            Close();
            m_Socket = null;
        }

        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        if (m_Socket == null)
            throw new Exception("Initialize network failure.");

        m_Socket.Connect(host, port);

        //实现接受消息的方法
        m_Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveMessage), m_Socket);
    }

    private void OnReceiveMessage(IAsyncResult result)
    {
        Socket client = result.AsyncState as Socket;
        if (client == null)
            return;

        try
        {
            int length = client.EndReceive(result);
            if (length <= 0)
                return;

            HandleMsg(buffer);

            //接收下一个消息(递归调用，这样就可以一直接收消息了）
            client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(OnReceiveMessage), client);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            client.Disconnect(true);
            Console.WriteLine("Socket disconnet!!");
        }
    }

    public void Send(byte[] buffer)
    {
        if (!m_Socket.Connected)
            return;
        m_Socket.BeginSend(buffer,0,buffer.Length,SocketFlags.None,null,null);
    }

    private void HandleMsg(byte[] msg)
    {
        NetMsg.HandleMsg(msg);
    }

    // 关闭连接并释放所有相关资源
    public void Close()
    {
        if(m_Socket == null)
            return;

        //m_Active = false;
        try
        {
            m_Socket.Shutdown(SocketShutdown.Both);
        }
        catch
        {

        }
        finally
        {
            m_Socket.Close();
            m_Socket = null;
        }
    }
}

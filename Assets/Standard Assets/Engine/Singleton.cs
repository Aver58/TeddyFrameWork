public class Singleton<T>
{
    private static T m_Instance;
    public static T instance
    {
        get
        {
            if(m_Instance == null)
                m_Instance = default;

            return m_Instance;
        }
    }
}

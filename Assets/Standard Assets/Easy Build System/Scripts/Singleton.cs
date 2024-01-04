/// <summary>
/// Project : Easy Build System
/// Class : Singleton.cs
/// Namespace : EasyBuildSystem.Features.Runtime.Bases
/// Copyright : © 2015 - 2022 by PolarInteractive
/// </summary>

using UnityEngine;

namespace EasyBuildSystem.Features.Runtime.Bases
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T m_Instance;
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = (T)FindObjectOfType(typeof(T));
                }

                return m_Instance;
            }
        }
    }
}
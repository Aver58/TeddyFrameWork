using UnityEngine;
using System.Collections;

public class ETCSingleton<T> : MonoBehaviour where T : ETCSingleton<T>{

	private static T m_Instance = null;
	private static bool onClose = false;

	public static T instance{
		get{
			if( m_Instance == null ){
				m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;
				if( m_Instance == null && !onClose){

					m_Instance = new GameObject("VirtualControlInput",typeof(T)).GetComponent<T>();
					m_Instance.Init();
				}
				
			}
			return m_Instance;
		}
	}
	
	private void Awake(){
		
		if( m_Instance == null ){
			m_Instance = this as T;
		}
	}
	
	public virtual void Init(){}
	
	
	private void OnApplicationQuit(){
		#if UNITY_EDITOR
		DestroyImmediate(m_Instance);
		#endif
		m_Instance = null;
		onClose = true;
	}
}

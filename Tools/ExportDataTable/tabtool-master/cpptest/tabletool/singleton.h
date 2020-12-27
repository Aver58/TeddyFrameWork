#pragma once

//µ¥ÀýÄ£Ê½
template<typename T>
class Singleton
{
private:
	static Singleton<T> m_Instance;
public:
	static Singleton<T>& Instance(){
		return m_Instance;
	}
	T* get(){ return &m_obj; }
	T* operator->()
	{
		return &m_obj;
	}

protected:
	T m_obj;
};

template<typename T>
Singleton<T> Singleton<T>::m_Instance;


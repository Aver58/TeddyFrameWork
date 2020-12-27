#pragma once

#include <stdlib.h>
#include <string>
#include <string.h>
#include <algorithm>
#include <sstream>
#include "readtablefile.h"
#include <vector>
#include <assert.h>
using namespace std;

template<typename T>
class IConfigTable
{
public:

	IConfigTable()
	{

	}

	virtual ~IConfigTable()
	{

	}

	//���ر�Ľӿ�
	virtual bool Load()=0;

	//����һ����¼
	T* GetTableItem(int id)
	{
		auto it = m_Items.find(id);

		if (m_Items.end() == it)
			return NULL;

		return &it->second;

	}

	//�������м�¼
	template<typename eee>
	bool ExecAll(eee e) 
	{
		auto it = m_Items.begin();
		for(;it!=m_Items.end();++it)
		{
			if(!e.exec(it->second))
				return false;
		}
		return true;
	}

	//�õ����ļ�¼ID
	const int MaxID() const
	{
		return m_Items.rbegin()->first;
	}

	//�õ���С�ļ�¼ID
	const int MinID() const
	{
		return m_Items.begin()->first;
	}

	//ֱ�ӻ��map
	auto RawMap()->auto
	{ 
		return m_Items; 
	}

	int GetitemCount() const 
	{
		return m_Items.size();
	}

protected:
	std::map<int ,T> m_Items;
};

//�������ı�
class IConfigBuyCostTable
{
public:
	virtual bool Load()=0;

	int GetCostByBuyCount(int cnt)
	{
		size_t nSize = 0;
		nSize = buycount.size();

		for (size_t i = 0; i + 1 < nSize; ++i)
		{
			if (cnt <= buycount[i]) return costvalue[i];
		}
		return costvalue[buycount.size() - 1];

	}
private:
	bool load_imp(const char* filename)
	{
		ReadTableFile m_ConfigTemplate;
		m_ConfigTemplate.Initialize();

		if (!m_ConfigTemplate.Init(filename))
			return false;

		int iRows = m_ConfigTemplate.GetRowCount();
		int iCols = m_ConfigTemplate.GetColCount();


		for (int i = 1; i < iRows; ++i)
		{
			buycount.push_back(atoi(m_ConfigTemplate.GetValue(i, "buycount")));
			costvalue.push_back(atoi(m_ConfigTemplate.GetValue(i, "cost")));
		}

		return true;

	}

private:
	std::vector<int> buycount;
	std::vector<int> costvalue;

}; 

//tbs�еĽṹ�嶼������̳�
template<typename T>
struct ITableObject
{
	virtual bool FromString(string s)=0;
};

//���ֶζ�ȡ
class DataReader
{
public:
	vector<string> GetStringList(string s, char delim)
	{
		vector<string> ret;
		size_t p1 = 0;
		size_t p2 = 0;
		p2 = s.find_first_of(delim, p1);
		while (p2 != string::npos)
		{
			ret.push_back(s.substr(p1, p2 - p1));
			p1 = p2 + 1;
			p2 = s.find_first_of(delim, p1);
		}
		if (p1 < s.length())
		{
			ret.push_back(s.substr(p1));
		}
		return ret;
	}

	int GetInt(string s)
	{
		return stoi(s);
	}

	vector<int> GetIntList(string s)
	{
		vector<string> vs = GetStringList(s,',');
		vector<int>ret;
		for (auto ss : vs)
		{
			int x = stoi(ss);
			ret.push_back(x);
		}
		return ret;
	}

	float GetFloat(string s)
	{
		return stof(s);
	}

	vector<float> GetFloatList(string s)
	{
		vector<string> vs = GetStringList(s, ',');
		vector<float>ret;
		for (auto ss : vs)
		{
			float x = stof(ss);
			ret.push_back(x);
		}
		return ret;
	}

	template<typename T>
	T GetObject(string s)
	{
		T t;
		assert(t.FromString(s));
		return t;
	}

	template<typename T>
	vector<T> GetObjectList(string s)
	{
		vector<string> vs = GetStringList(s, ';');
		vector<T>ret;
		for (auto ss : vs)
		{
			ret.push_back(GetObject<T>(ss));
		}
		return ret;

	}
};





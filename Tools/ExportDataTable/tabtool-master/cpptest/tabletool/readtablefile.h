#pragma once
#include <map>
#include <vector>
#include <string>
#include <stdio.h>
#include "myconfig.h"

class ReadTableFile
{
public:
	ReadTableFile();
	~ReadTableFile();

	static void	Initialize();
	static void	Destroy();

	bool		Init(const char* szFileName);
	bool		Init(const char* pBuffer, int iLength, const char* szFileName = NULL);
	int			GetRowCount();
	int			GetColCount();
    const char*	GetValue(int nRow,const char* szName);
    const char*	GetValue(int nRow, int nCol);
	const char*	GetFileName()const{return m_szFileName;}

private:

	static std::vector<std::string>*		m_pVects;
	static char*							m_pszReadBuf;
	std::map<std::string, int>				m_mNamePos;
	int										m_nLeftBufLen;
	char*									m_pszReadPtr;
	FILE*									m_fpConfigFile;
	static int								m_nRowCnt;
	static int								m_nColCnt;
	char									m_szFileName[1024];
};

struct stConfigScope
{
public:
	stConfigScope(){
		ReadTableFile::Initialize();
	}
	~stConfigScope(){
		ReadTableFile::Destroy();
	}
};

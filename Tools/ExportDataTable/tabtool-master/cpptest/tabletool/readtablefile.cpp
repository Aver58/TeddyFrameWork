#include "readtablefile.h"
#include <string.h>


const int READFILE_ERROR = -1;
const int READFILE_SUCCESS = 0;
const int READFILE_END = 1;

std::vector<std::string>* ReadTableFile::m_pVects = NULL;
char* ReadTableFile::m_pszReadBuf = NULL;
int ReadTableFile::m_nRowCnt = 0;
int ReadTableFile::m_nColCnt = 0;

ReadTableFile::ReadTableFile()
{
    if (NULL == m_pszReadBuf)
    {
        m_pszReadBuf = new char[MAX_LINE_LEN];
    }

    memset(m_pszReadBuf, 0, MAX_LINE_LEN);
    m_pszReadPtr = m_pszReadBuf;
	m_fpConfigFile = NULL;
	m_nLeftBufLen = 0;
    if (NULL != m_pVects)
    {
        for (int i = 0; i < m_nRowCnt; i++)
        {
            m_pVects[i].clear();
        }
    }
	m_nRowCnt = 0;
	m_nColCnt = 0;
}

ReadTableFile::~ReadTableFile()
{
    if (NULL != m_pVects)
    {
        for (int i = 0; i < m_nRowCnt; i++)
        {
            m_pVects[i].clear();
        }
    }
    m_nRowCnt = 0;
}

void ReadTableFile::Initialize()
{
	if( m_pVects == NULL )
	{
		m_pVects = new std::vector<std::string>[MAX_TABLE_ROWS];
	}
	else
	{
		for (int i = 0; i < m_nRowCnt; i++)
		{
			m_pVects[i].clear();
		}
	}
    m_nRowCnt = 0;
}

void ReadTableFile::Destroy()
{
	if (m_pVects)
	{
		delete[] m_pVects;
		m_pVects = NULL;
	}

    if (NULL != m_pszReadBuf)
    {
        delete [] m_pszReadBuf;
        m_pszReadBuf = NULL;
    }
}

bool ReadTableFile::Init(const char* szFileName)
{
    strcpy_s(m_szFileName,szFileName);

	m_fpConfigFile = fopen(szFileName, "r");
	if (NULL == m_fpConfigFile)
	{
		return false;
	}

	size_t nHead = fread(m_pszReadBuf, 1, 3,m_fpConfigFile);

	if(nHead == 3 && ((unsigned char)m_pszReadBuf[0] == 0xef) && ((unsigned char)m_pszReadBuf[1] == 0xbb) && ((unsigned char)m_pszReadBuf[2]==0xbf))
	{
		//utf8跳过前三个字节
	}
	else
	{
		ErrorLog("[%s]不是utf8编码的文件", m_szFileName);
		rewind(m_fpConfigFile);
	}
	int nColCnt = 0;
	while (true)
	{
		if (m_nLeftBufLen > 0 )
		{
			memcpy(m_pszReadBuf, m_pszReadPtr, m_nLeftBufLen);
		}
		
		size_t nSize = fread(m_pszReadBuf + m_nLeftBufLen, 1, MAX_LINE_LEN - m_nLeftBufLen, m_fpConfigFile);
		m_nLeftBufLen += (int)nSize;
		m_pszReadPtr = m_pszReadBuf;
		if (0 == nSize)
		{
            if (m_nLeftBufLen > 0)
            {
                // 文件已读完，但还有尾巴
                std::string str;
                str.append(m_pszReadPtr, m_nLeftBufLen);
                // 如果该值是excel引号引起来的，那么去除引号
                if (2 <= str.size() && '\"' == str[0])
                {
                    str = str.substr(1, str.size() - 2);
                }

                m_pVects[m_nRowCnt].push_back(str);
                nColCnt++;
            }

			if (nColCnt > 0)
			{
				m_nRowCnt++;
			}
			break;
		}
		
		char* pBuf = m_pszReadPtr;
		while ((m_pszReadPtr + m_nLeftBufLen) > pBuf)
		{

			char cCurChar = *pBuf;
			if ('\t' == cCurChar || '\n' == cCurChar || '\r' == cCurChar)
			{
				if ('\r' == cCurChar && (m_pszReadPtr + m_nLeftBufLen - pBuf) <  2)
				{
					//最后一个字符为\r,不再处理
					break;
				}

				std::string str;
				int nStrLen = (int)(pBuf - m_pszReadPtr);

				str.append(m_pszReadPtr, nStrLen);

                // 如果该值是excel引号引起来的，那么去除引号
                if (2 <= str.size() && '\"' == str[0])
                {
                    str = str.substr(1, str.size() - 2);
                }

				m_pVects[m_nRowCnt].push_back(str);
				nColCnt++;

				if ('\r' == cCurChar)
				{
					m_nLeftBufLen = m_nLeftBufLen - nStrLen - 2;
					m_pszReadPtr = m_pszReadPtr + nStrLen + 2;
				}
				else
				{
					m_nLeftBufLen = m_nLeftBufLen - nStrLen - 1;
					m_pszReadPtr = m_pszReadPtr + nStrLen + 1;
				}
				
				pBuf = m_pszReadPtr;
			
				if (('\r' == cCurChar) || ('\n' == cCurChar))
				{
					//如果是第一列
					if (0 == m_nColCnt)
					{
						m_nColCnt = nColCnt;
					}
					//如果与表头列数不等，可能有问题
					else if (m_nColCnt != nColCnt) 
					{
						//可能有问题
					}

					if (nColCnt > 0)
					{
						m_nRowCnt++;
                        if (m_nRowCnt >= MAX_TABLE_ROWS)
                        {
                            return false;
                        }

						nColCnt = 0;
					}

				}
				 
				pBuf = m_pszReadPtr;
			}
	
			else
			{
				pBuf++;
			}

		}

	}
	if (m_fpConfigFile)
	{
		fclose(m_fpConfigFile);
	}

	//
	for (int i = 0; i < (int)m_pVects[0].size(); i++)
	{
		m_mNamePos.insert(std::make_pair(m_pVects[0][i],i));
	}

	return true;
}

bool ReadTableFile::Init(const char* pBuffer, int iLength,const char* szFileName)
{
    if (NULL != szFileName)
    {
        strcpy_s(m_szFileName,szFileName);
    }
    else
    {
        strcpy_s(m_szFileName,"unknow table");
    }
	int nColCnt = 0;

	m_pszReadPtr = (char*)pBuffer;
	const char* pBuf = pBuffer;
	while( (pBuffer + iLength) > pBuf )
	{

		char cCurChar = *pBuf;
		if ('\t' == cCurChar || '\n' == cCurChar || '\r' == cCurChar)
		{
			if ('\r' == cCurChar && (m_pszReadPtr + iLength - pBuf) <  2)
			{
				//最后一个字符为\r,不再处理
				break;
			}


			std::string str;
			int nStrLen = (int)(pBuf - m_pszReadPtr);

			str.append(m_pszReadPtr, nStrLen);

			// 如果该值是excel引号引起来的，那么去除引号
			if (2 <= str.size() && '\"' == str[0])
			{
				str = str.substr(1, str.size() - 2);
			}

			m_pVects[m_nRowCnt].push_back(str);
			nColCnt++;

			if ('\r' == cCurChar)
			{
				m_pszReadPtr = m_pszReadPtr + nStrLen + 2;
			}
			else
			{
				m_pszReadPtr = m_pszReadPtr + nStrLen + 1;
			}

			pBuf = m_pszReadPtr;

			if (('\r' == cCurChar) || ('\n' == cCurChar))
			{
				//如果是第一列
				if (0 == m_nColCnt)
				{
					m_nColCnt = nColCnt;
				}
				//如果与表头列数不等，可能有问题
				else if (m_nColCnt != nColCnt) 
				{
					//可能有问题
				}

				if (nColCnt > 0)
				{
					m_nRowCnt++;
					if (m_nRowCnt >= MAX_TABLE_ROWS)
					{
						return false;
					}

					nColCnt = 0;
				}

			}

			pBuf = m_pszReadPtr;
		}
		else
		{
			pBuf++;
		}

	}

	if (nColCnt > 0)
	{
		m_nRowCnt++;
	}

	//
	for (int i = 0; i < (int)m_pVects[0].size(); i++)
	{
		m_mNamePos.insert(std::make_pair(m_pVects[0][i],i));
	}

	return true;
}

int ReadTableFile::GetColCount()
{
    return m_nColCnt;
}

int ReadTableFile::GetRowCount()
{
	if (m_nRowCnt > 1)
	{
		return m_nRowCnt; 
	}
	else
	{
		return 0;
	}
}

const char*	ReadTableFile::GetValue(int nRow, const char* szName)
{
	int nIndex =  nRow;
	if (nIndex > m_nRowCnt)
	{
		return "";
	}

	if (m_mNamePos.end() == m_mNamePos.find(szName))
	{
        char szBuff[1024];
        sprintf_s(szBuff,"配置表错误，[%s]中找不到[%s]列！",m_szFileName,szName);
		return "";
	}

	int nColIndex = m_mNamePos[std::string(szName)];
    if (nColIndex >= (int)m_pVects[nIndex].size())
    {
        return "";
    }

	return m_pVects[nIndex][nColIndex].c_str();
}

const char* ReadTableFile::GetValue(int nRow, int nCol)
{
    int nIndex =  nRow + 1;
    if (nIndex > m_nRowCnt || nCol >= m_nColCnt)
    {
        return "";
    }

    return m_pVects[nIndex][nCol].c_str();
}



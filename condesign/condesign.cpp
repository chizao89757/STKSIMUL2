// condesign.cpp : �������̨Ӧ�ó������ڵ㡣
//

#include "stdafx.h"
#include "engine.h"
#include <cstdio>
#include <iostream>
#include <math.h>
#include <string.h>
#include <io.h>
#include <stdio.h> 
#include <stdlib.h>
#include<fstream>
#include<iostream>

#pragma comment(lib, "libmat.lib")
#pragma comment(lib, "libmx.lib")
#pragma comment(lib, "libeng.lib")
using namespace std;


int _tmain(int argc, TCHAR* argv[])
{
	int nStatus = 0;
	double a[] = { 2.0, 2.0, 90, 10, 1000, 6e9, 600e6 };
	string resultpath;


	cout << "start" << endl;
	
	cout << "paranum:"  << argc << endl;
	if (argc != 9)
	{
		return 0;
	}

	for (int i = 1; i<argc; i++) 
	{
		char *CStr;
		size_t len = wcslen(argv[i]) + 1;

		size_t converted = 0;

		CStr = (char*)malloc(len*sizeof(char));

		wcstombs_s(&converted, CStr, len, argv[i], _TRUNCATE);

		//wcout << argv[i] << endl;

		if (i < 2)
		{
			resultpath = CStr;
		}
		else
		{
			a[i - 2] = atof(CStr);
			cout << a[i - 2] << endl;
		}
	}
	
	mxArray *A;
	mxArray *B;

	double *RecDataMatrix;
	RecDataMatrix = new double[700];

	Engine *ep;
	ep = engOpen(NULL);        // ��MALTAB���档
	if (ep == NULL)
	{
		cout << "Can't start Matlab engine!" << endl;
		exit(EXIT_FAILURE);
	}
	nStatus = engSetVisible(ep, false);	//��ʾ���洰�ڡ�
	if (nStatus != 0)
	{
		cout << "����MATLAB��ʾ����ʧ�ܡ�" << endl;
		exit(EXIT_FAILURE);
	}

	A = mxCreateDoubleMatrix(1, 7, mxREAL);	//ע��˴�������˳��MATLAB��������C���Զ�ά���������˳���෴

	memcpy((void*)mxGetPr(A), (void*)a, sizeof(a));
	nStatus = engPutVariable(ep, "A", A);

	nStatus = engEvalString(ep, "path(path, 'C:\\Program Files\\MATLAB\\R2012a\\bin')");
	cout << "���ڽ��й��ͼ���..." << endl;
	system("pause");
	nStatus = engEvalString(ep, "B=NSGA_sorted(A(1),A(2),A(3),A(4),A(5),A(6),A(7))");
	if (nStatus != 0)
	{
		cout << "���ú���ʧ�ܡ�" << endl;
		system("pause");
		exit(EXIT_FAILURE);
	}

	nStatus = engEvalString(ep, "B=B'");

 	

	mxArray *Blen;
	Blen = mxCreateDoubleMatrix(1, 1, mxREAL);
	nStatus = engEvalString(ep, "Blen=size(B,2)");
	Blen = engGetVariable(ep, "Blen");

	int blen = (int)*mxGetPr(Blen);
	B = mxCreateDoubleMatrix(7, blen, mxREAL);
	B = engGetVariable(ep, "B");
	double *B_rec = mxGetPr(B);
	//memcpy((void*)RecDataMatrix, (void*)B_rec, sizeof(RecDataMatrix));
	
	ofstream ofile;
	ofile.open(resultpath+"\\result.txt");
	//ofile.open(resultpath + "result.txt");
	ofile << "������ƽ��" << endl;


	for (int ii = 0; ii < blen; ii++)
	{
		for (int jj = 0; jj < 7; jj++)
		{
			ofile << B_rec[ii*7+jj] << "\t";
		}
		ofile << endl;
	}


	ofile.close();

	
	system("pause");
	mxDestroyArray(A);
	mxDestroyArray(B);
	mxDestroyArray(Blen);
	engClose(ep);


	return 0;
}


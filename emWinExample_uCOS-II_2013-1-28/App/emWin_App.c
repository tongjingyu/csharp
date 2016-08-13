#include <emWin_App.h>
#include <ucos_includes.h>
#include <GUI.h>
#include "DIALOG.h"



#define CenterPointX 100
#define CenterPointY GUI_GetScreenSizeY()-100
INT16U CursorX, CursorY;
UINT8 DrawBuffer[1000];
OS_STK PoolFitelMStk[TASK_STK_SIZE];


INT16U FlowPoolFilter(INT32U *Pool, INT16U Data, INT8U *PoolIndex, INT8U PoolSize)
{
	INT16U Old_Filter = 0;
	if (*PoolIndex)Old_Filter = ((*Pool) / (*PoolIndex));
	(*Pool) += Data;
	if ((*PoolIndex)<PoolSize)(*PoolIndex)++;
	else (*Pool) -= Old_Filter;
	return (*Pool) / (*PoolIndex);
}

void Draw_Base()
{
	GUI_SetColor(GUI_YELLOW);
	GUI_SetLineStyle(GUI_LS_DOT);
	GUI_DrawLine(0, CenterPointY, GUI_GetScreenSizeX(), CenterPointY);//XÖá
	GUI_DrawLine(GUI_GetScreenSizeX() - 20, CenterPointY - 20, GUI_GetScreenSizeX(), CenterPointY);//X¼ýÍ·
	GUI_DrawLine(GUI_GetScreenSizeX() - 20, CenterPointY+20, GUI_GetScreenSizeX(), CenterPointY);//X¼ýÍ·
	GUI_DrawLine(CenterPointX, 0, CenterPointX, GUI_GetScreenSizeY());//YÖá
	GUI_DrawLine(CenterPointX, 0, CenterPointX - 20, 20);
	GUI_DrawLine(CenterPointX, 0, CenterPointX+20, 20);
}
void Draw_Line(int x0, int y0, int x1, int y1)
{
	GUI_DrawLine(CenterPointX +x0, CenterPointY - y0, CenterPointX + x1, CenterPointY - y1);
}
void Draw_ArrayLine(UINT8 *Buf,UINT16 Length)
{
	UINT16 i;
	GUI_SetColor(GUI_BLUE);
	GUI_SetLineStyle(GUI_LS_SOLID);
	for (i = 0; i < (Length-1);i++)
	{
		
		Draw_Line(i,Buf[i],i+1,Buf[i+1]);
	}
	
}
void  Create_FitelSample(UINT8 *Buf,UINT16 Length)
{
	UINT16 i,Data=1;
	INT32U Pool=0;
	INT8U PoolIndex=0;
	for (i = 0; i < Length; i++)
	{
		Buf[i] = FlowPoolFilter(&Pool, Data, &PoolIndex, 50);
		if (i == 100)Data = 100;
		if (i == 200)Data = 70;
	}
}
void Task_PoolFitel(void *pdata) {

	(void)pdata;//·ÀÖ¹±àÒëÆ÷±¨´í
	GUI_Init();
	while(1)
	{
		Draw_Base();
		Create_FitelSample(&DrawBuffer[0], 600);
		Draw_ArrayLine(&DrawBuffer[0],600);
		OSTimeDly(10);
	}
}

void Task_TestUI(void *pdata)
{
	WM_HWIN hWin;
	(void)pdata;//·ÀÖ¹±àÒëÆ÷±¨´í
	PROGBAR_Handle hbar;
	GUI_Init();
	CreateLogin();
	while(1)
	{
		GUI_Delay(10);
	}
}
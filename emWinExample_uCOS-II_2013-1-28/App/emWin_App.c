#include <emWin_App.h>
#include <ucos_includes.h>
#include <GUI.h>
#include "DIALOG.h"


#define ID_WINDOW_0   (GUI_ID_USER + 0x00)
#define ID_BUTTON_0   (GUI_ID_USER + 0x01)


#define CenterPointX 100
#define CenterPointY GUI_GetScreenSizeY()-100
INT16U CursorX, CursorY;
UINT8 DrawBuffer[1000];
OS_STK PoolFitelMStk[TASK_STK_SIZE];

static const GUI_WIDGET_CREATE_INFO _aDialogCreate[] = {
	{ WINDOW_CreateIndirect, "Window", ID_WINDOW_0, 0, 0, 320, 240, 0, 0x0},
	{ BUTTON_CreateIndirect, "Button", ID_BUTTON_0, 84, 61, 173, 68, 0, 0x0},
	// USER START (Optionally insert additional widgets)
	// USER END
};
static void _cbDialog(WM_MESSAGE * pMsg) {
	int NCode;
	int Id;
	// USER START (Optionally insert additional variables)
	// USER END

	switch (pMsg->MsgId) {
	case WM_NOTIFY_PARENT:
		Id = WM_GetId(pMsg->hWinSrc);
		NCode = pMsg->Data.v;
		switch (Id) {
		case ID_BUTTON_0: // Notifications sent by 'Button'
			switch (NCode) {
			case WM_NOTIFICATION_CLICKED:
				// USER START (Optionally insert code for reacting on notification message)
				// USER END
				break;
			case WM_NOTIFICATION_RELEASED:
				// USER START (Optionally insert code for reacting on notification message)
				// USER END
				break;
				// USER START (Optionally insert additional code for further notification handling)
				// USER END
			}
			break;
			// USER START (Optionally insert additional code for further Ids)
			// USER END
		}
		break;
		// USER START (Optionally insert additional message handling)
		// USER END
	default:
		WM_DefaultProc(pMsg);
		break;
	}
}
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
	(void)pdata;//·ÀÖ¹±àÒëÆ÷±¨´í
	PROGBAR_Handle hbar;
	GUI_Init();
	while(1)
	{
		GUI_DispStringAt("fdsafdsa", 100, 20);
		hbar = PROGBAR_Create(100, 40, 100, 20, WM_CF_SHOW);
		OSTimeDly(10);
	}
}
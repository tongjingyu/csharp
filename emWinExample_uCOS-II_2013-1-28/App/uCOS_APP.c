/*********************************************************************
*                SEGGER Microcontroller GmbH & Co. KG                *
*        Solutions for real time microcontroller applications        *
**********************************************************************
*                                                                    *
*        (c) 1996 - 2009  SEGGER Microcontroller GmbH & Co. KG       *
*                                                                    *
*        Internet: www.segger.com    Support:  support@segger.com    *
*                                                                    *
**********************************************************************

** emWin V5.00 - Graphical user interface for embedded applications **
emWin is protected by international copyright laws.   Knowledge of the
source code may not be used to write a similar product.  This file may
only be used in accordance with a license and should not be re-
distributed in any way. We appreciate your understanding and fairness.
----------------------------------------------------------------------
File        : HouseControl.c
Purpose     : house control demo
---------------------------END-OF-HEADER------------------------------
*/

#define Simulation_Main MainTask
#include "ucos_includes.h"
#include "GUI.h"
#include <emWin_App.h>

/******************** ���¿��Է���uCOS���� **********************/


OS_STK DispCharMStk1[TASK_STK_SIZE];   
OS_STK DispCharMStk2[TASK_STK_SIZE];  
OS_STK DispCharMStk3[TASK_STK_SIZE];
 
 
void Task_Test1(void *p_arg){
	static INT32U i=0;
	static INT8U c='0';
	(void)p_arg;
	while(1){
		GUI_GotoXY(i,0);// Set text position (in pixels)
		GUI_DispChar(c);// Show text
		i+=10;
		c++;
		if(c>'9'){
			c='0';
		}
		OSTimeDly(10);
	}
}
void Task_Test2(void *p_arg){
	static INT32U i=0;
	static INT8U c='A';
	(void)p_arg;
	while(1){
		GUI_GotoXY(i,10);// Set text position (in pixels)
		GUI_DispChar(c);// Show text
		i+=10;
		c++;
		if(c>'Z'){
			c='A';
		}
		OSTimeDly(10);
		GUI_DispFloat((float)213.33,1);
	}
}
void Task_Test3(void *p_arg){
	static INT32U i=0;
	static INT8U c='a';
	(void)p_arg;
	while(1){
		GUI_GotoXY(i,20);// Set text position (in pixels)
		GUI_DispChar(c);// Show text
		i+=10;
		c++;
		if(c>'z'){
			c='a';
		}
		OSTimeDly(10);
	}
}


OS_STK Task_StartUpStack[TASK_STK_SIZE];
void Task_StartUp(void *pdata){
	
	(void)pdata;//��ֹ����������

	GUI_Init();
	
	OSTaskCreate(Task_Test1,(void *)0, &DispCharMStk1[TASK_STK_SIZE-1], 5);	
	OSTaskCreate(Task_Test2,(void *)0, &DispCharMStk2[TASK_STK_SIZE-1], 6);
	OSTaskCreate(Task_Test3,(void *)0, &DispCharMStk3[TASK_STK_SIZE-1], 7);
	
	OSTaskDel (OS_PRIO_SELF); //ɾ���Լ�
}

//����������������������������У����Ե�����ں���
void Simulation_Main(void) {
	
	//��ʼ��uCOS
	OSInit();
	//��������ϵͳ����
	//OSTaskCreate(Task_StartUp,(void *)0, &Task_StartUpStack[TASK_STK_SIZE-1], 1);
	OSTaskCreate(Task_TestUI, (void *)0, &PoolFitelMStk[TASK_STK_SIZE - 1], 1);
	//����ϵͳ
	OSStart();
  
}

/*************************** End of file ****************************/


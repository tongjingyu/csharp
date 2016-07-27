#ifndef _EMWIN_APP_H_
#define _EMWIN_APP_H_
#include <os_cpu.h>
#include "ucos_includes.h"

extern OS_STK PoolFitelMStk[TASK_STK_SIZE];
void Task_PoolFitel(void *pdata);
void Task_TestUI(void *pdata);

#endif

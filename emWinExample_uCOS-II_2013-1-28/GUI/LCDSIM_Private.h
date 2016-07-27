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
File        : LCDSIM_Private.h
Purpose     : Declares LCD Private interface
----------------------------------------------------------------------
*/

#ifndef LCDSIM_PRIVATE_H
#define LCDSIM_PRIVATE_H

#include "LCD_SIM.h"

extern int LCDSIM_aLCDColorWhite[LCDSIM_MAX_DISPLAYS];
extern int LCDSIM_aLCDColorBlack[LCDSIM_MAX_DISPLAYS];

/********************************************************************
*
*       Memory offsets
*
*********************************************************************
*/
#define LCDSIM_OFF_XSIZE           32
#define LCDSIM_OFF_YSIZE           36
#define LCDSIM_OFF_VXSIZE          40
#define LCDSIM_OFF_VYSIZE          44
#define LCDSIM_OFF_CONTROLLER      48

#define LCDSIM_OFF_BPP             52
#define LCDSIM_OFF_NUMCOLORS       56

#define LCDSIM_OFF_MODIFY_CNT      60
#define LCDSIM_OFF_LUT_MODIFY_CNT  64

#define LCDSIM_OFF_FIXEDPALETTE    68
#define LCDSIM_OFF_DELTA_MODE      72

#define LCDSIM_OFF_XPOS_PID        80
#define LCDSIM_OFF_YPOS_PID        84
#define LCDSIM_OFF_KEYSTAT         88

#define LCDSIM_OFF_ORGX            92
#define LCDSIM_OFF_ORGY            96

#define LCDSIM_OFF_ALPHA          100

#define LCDSIM_OFF_XPOS_LAYER     104
#define LCDSIM_OFF_YPOS_LAYER     108

#define LCDSIM_OFF_VIS_LAYER      112

#define LCDSIM_OFF_COMPOSIT_COLOR 116
#define LCDSIM_OFF_COMPOSIT_SIZE  120

#define LCDSIM_OFF_CHROMA_MIN     124
#define LCDSIM_OFF_CHROMA_MAX     128

#define LCDSIM_OFF_TRANSMODE      132

/********************************************************************
*
*       Private Interface
*
*********************************************************************
*/
void SIM_GUI__MessageBox1(LPCTSTR lpText);

#endif /* Avoid multiple inclusion */





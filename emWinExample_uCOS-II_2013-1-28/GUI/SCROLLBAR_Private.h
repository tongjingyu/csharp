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
File        : SCROLLBAR_Private.h
Purpose     : SCROLLBAR internal header file
---------------------------END-OF-HEADER------------------------------
*/

#ifndef SCROLLBAR_PRIVATE_H
#define SCROLLBAR_PRIVATE_H

#include "SCROLLBAR.h"
#include "WIDGET.h"
#include "GUI_Debug.h"

#if GUI_WINSUPPORT

/*********************************************************************
*
*       Module internal data
*
**********************************************************************
*/
extern GUI_COLOR  SCROLLBAR__aDefaultBkColor[2];
extern GUI_COLOR  SCROLLBAR__aDefaultColor[2];
extern I16        SCROLLBAR__DefaultWidth;

/*********************************************************************
*
*       Object definition
*
**********************************************************************
*/

typedef struct {
  GUI_COLOR aColor[3];
} SCROLLBAR_PROPS;

typedef struct {
  WIDGET Widget;
  SCROLLBAR_PROPS Props;
  int NumItems, v, PageSize;
  int TimerStep;
  WM_HMEM hTimer;
  #if GUI_DEBUG_LEVEL >1
    int DebugId;
  #endif  
} SCROLLBAR_Obj;

typedef struct {
  int x0_LeftArrow;
  int x1_LeftArrow;
  int x0_Thumb;
  int x1_Thumb;
  int x0_RightArrow;
  int x1_RightArrow;
  int x1;
  int xSizeMoveable;
  int ThumbSize;
} SCROLLBAR_POSITIONS;

/*********************************************************************
*
*       Macros for internal use
*
**********************************************************************
*/
#if GUI_DEBUG_LEVEL >= GUI_DEBUG_LEVEL_CHECK_ALL
  #define SCROLLBAR_INIT_ID(p) p->DebugId = SCROLLBAR_ID
#else
  #define SCROLLBAR_INIT_ID(p)
#endif

#if GUI_DEBUG_LEVEL >= GUI_DEBUG_LEVEL_CHECK_ALL
  SCROLLBAR_Obj * SCROLLBAR_LockH(SCROLLBAR_Handle h);
  #define SCROLLBAR_LOCK_H(h)   SCROLLBAR_LockH(h)
#else
  #define SCROLLBAR_LOCK_H(h)   (SCROLLBAR_Obj *)GUI_LOCK_H(h)
#endif

void SCROLLBAR__InvalidatePartner(SCROLLBAR_Handle hObj);

extern SCROLLBAR_PROPS SCROLLBAR__DefaultProps;

#endif        /* GUI_WINSUPPORT */
#endif        /* Avoid multiple inclusion */




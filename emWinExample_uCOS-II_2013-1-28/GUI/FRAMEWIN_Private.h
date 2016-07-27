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
File        : FRAMEWIN_Private.h
Purpose     : FRAMEWIN private header file
--------------------END-OF-HEADER-------------------------------------
*/

#ifndef FRAMEWIN_PRIVATE_H
#define FRAMEWIN_PRIVATE_H

#include "WM.h"
#include "FRAMEWIN.h"
#include "WIDGET.h"
#include "GUI_HOOK.h"

#if GUI_WINSUPPORT

/*********************************************************************
*
*         Object definition
*
**********************************************************************
*/

typedef struct {
  const GUI_FONT GUI_UNI_PTR * pFont;
  GUI_COLOR                    aBarColor[2];
  GUI_COLOR                    aTextColor[2];
  GUI_COLOR                    ClientColor;
  I16                          TitleHeight;
  I16                          BorderSize;
  I16                          IBorderSize;
  I16                          TextAlign;
} FRAMEWIN_PROPS;

typedef struct {
  WIDGET Widget;
  FRAMEWIN_PROPS Props;
  WIDGET_DRAW_ITEM_FUNC * pfDrawItem;
  WM_CALLBACK * cb;
  WM_HWIN hClient;
  WM_HWIN hMenu;
  WM_HWIN hText;
  GUI_RECT rRestore;
  U16 Flags;
  WM_HWIN hFocussedChild;          /* Handle to focussed child .. default none (0) */
  WM_DIALOG_STATUS* pDialogStatus;
  GUI_HOOK * pFirstHook;
  #if GUI_DEBUG_LEVEL >= GUI_DEBUG_LEVEL_CHECK_ALL
    int DebugId;
  #endif  
} FRAMEWIN_Obj;

typedef struct {
  I16 TitleHeight;
  I16 MenuHeight;
  GUI_RECT rClient;
  GUI_RECT rTitleText;
} POSITIONS;

/*********************************************************************
*
*       Macros for internal use
*
**********************************************************************
*/
#if GUI_DEBUG_LEVEL >= GUI_DEBUG_LEVEL_CHECK_ALL
  #define FRAMEWIN_INIT_ID(p) p->DebugId = FRAMEWIN_ID
#else
  #define FRAMEWIN_INIT_ID(p)
#endif

#if GUI_DEBUG_LEVEL >= GUI_DEBUG_LEVEL_CHECK_ALL
  FRAMEWIN_Obj * FRAMEWIN_LockH(FRAMEWIN_Handle h);
  #define FRAMEWIN_LOCK_H(h)   FRAMEWIN_LockH(h)
#else
  #define FRAMEWIN_LOCK_H(h)   (FRAMEWIN_Obj *)GUI_LOCK_H(h)
#endif

/*********************************************************************
*
*       Public data (internal defaults)
*
**********************************************************************
*/

extern FRAMEWIN_PROPS FRAMEWIN__DefaultProps;

/*********************************************************************
*
*       Public functions (internal)
*
**********************************************************************
*/

void            FRAMEWIN__CalcPositions   (FRAMEWIN_Handle hObj, POSITIONS * pPos);
int             FRAMEWIN__CalcTitleHeight (FRAMEWIN_Obj * pObj);
void            FRAMEWIN__UpdatePositions (FRAMEWIN_Handle hObj);
void            FRAMEWIN__UpdateButtons   (FRAMEWIN_Obj * pObj, int OldHeight);

/*********************************************************************
*
*       Public functions
*
**********************************************************************
*/

int             FRAMEWIN_GetTitleHeight   (FRAMEWIN_Handle hObj);
void            FRAMEWIN_MinButtonSetState(WM_HWIN hButton, int State);
void            FRAMEWIN_MaxButtonSetState(WM_HWIN hButton, int State);

#endif   /* GUI_WINSUPPORT */
#endif   /* FRAMEWIN_H */

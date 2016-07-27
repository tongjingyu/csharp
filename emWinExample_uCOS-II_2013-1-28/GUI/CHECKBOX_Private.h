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
File        : CHECKBOX_Private.h
Purpose     : CHECKBOX private header file
--------------------END-OF-HEADER-------------------------------------
*/

#ifndef CHECKBOX_PRIVATE_H
#define CHECKBOX_PRIVATE_H

#include "WM.h"
#include "WIDGET.h"
#include "CHECKBOX.h"

#if GUI_WINSUPPORT

/*********************************************************************
*
*       Defines
*
**********************************************************************
*/
#ifndef CHECKBOX_BKCOLOR0_DEFAULT
  #define CHECKBOX_BKCOLOR0_DEFAULT 0x808080           /* Inactive color */
#endif

#ifndef CHECKBOX_BKCOLOR1_DEFAULT
  #define CHECKBOX_BKCOLOR1_DEFAULT GUI_WHITE          /* Active color */
#endif

#ifndef CHECKBOX_FGCOLOR0_DEFAULT
  #define CHECKBOX_FGCOLOR0_DEFAULT 0x101010
#endif

#ifndef CHECKBOX_FGCOLOR1_DEFAULT
  #define CHECKBOX_FGCOLOR1_DEFAULT GUI_BLACK
#endif

/*********************************************************************
*
*       Object definition
*
**********************************************************************
*/
typedef struct {
  const GUI_FONT GUI_UNI_PTR * pFont;
  GUI_COLOR aBkColorBox[2]; /* Colors used to draw the box background */
  GUI_COLOR BkColor;        /* Widget background color */
  GUI_COLOR TextColor;
  GUI_COLOR FocusColor;
  I16 Align;
  U8  Spacing;
  const GUI_BITMAP * apBm[6];
} CHECKBOX_PROPS;

typedef struct {
  WIDGET Widget;
  CHECKBOX_PROPS Props;
  U8 NumStates;
  U8 CurrentState;
  WM_HMEM hpText;
  #if GUI_DEBUG_LEVEL > 1
    int DebugId;
  #endif  
} CHECKBOX_Obj;

/*********************************************************************
*
*       Macros for internal use
*
**********************************************************************
*/
#if GUI_DEBUG_LEVEL >= GUI_DEBUG_LEVEL_CHECK_ALL
  #define CHECKBOX_INIT_ID(p) p->DebugId = CHECKBOX_ID
#else
  #define CHECKBOX_INIT_ID(p)
#endif

#if GUI_DEBUG_LEVEL >= GUI_DEBUG_LEVEL_CHECK_ALL
  CHECKBOX_Obj * CHECKBOX_LockH(CHECKBOX_Handle h);
  #define CHECKBOX_LOCK_H(h)   CHECKBOX_LockH(h)
#else
  #define CHECKBOX_LOCK_H(h)   (CHECKBOX_Obj *)GUI_LOCK_H(h)
#endif

/*********************************************************************
*
*       Public data (internal defaults)
*
**********************************************************************
*/
extern CHECKBOX_PROPS CHECKBOX__DefaultProps;

/*********************************************************************
*
*       Extern data
*
**********************************************************************
*/
extern const GUI_BITMAP   CHECKBOX__abmCheck[2];

#endif   /* GUI_WINSUPPORT */
#endif   /* CHECKBOX_PRIVATE_H */

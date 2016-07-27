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
File        : SLIDER.h
Purpose     : SLIDER include
--------------------END-OF-HEADER-------------------------------------
*/

#ifndef SLIDER_H
#define SLIDER_H

#include "WM.h"
#include "DIALOG_Intern.h"      /* Req. for Create indirect data structure */
#include "WIDGET.h"      /* Req. for Create indirect data structure */

#if GUI_WINSUPPORT

#if defined(__cplusplus)
extern "C" {     /* Make sure we have C-declarations in C++ programs */
#endif

/************************************************************
*
*       #defines
*
*************************************************************
*/

/************************************************************
*
*       States
*/
#define SLIDER_STATE_PRESSED    WIDGET_STATE_USER0

/************************************************************
*
*       Create / Status flags
*/
#define SLIDER_CF_VERTICAL WIDGET_CF_VERTICAL

/*********************************************************************
*
*                         Public Types
*
**********************************************************************

*/
typedef WM_HMEM SLIDER_Handle;

/*********************************************************************
*
*                 Create functions
*
**********************************************************************
*/

SLIDER_Handle SLIDER_Create        (int x0, int y0, int xsize, int ysize, WM_HWIN hParent, int Id, int WinFlags, int SpecialFlags);
SLIDER_Handle SLIDER_CreateIndirect(const GUI_WIDGET_CREATE_INFO* pCreateInfo, WM_HWIN hWinParent, int x0, int y0, WM_CALLBACK* cb);
SLIDER_Handle SLIDER_CreateEx      (int x0, int y0, int xsize, int ysize, WM_HWIN hParent,
                                    int WinFlags, int ExFlags, int Id);

/*********************************************************************
*
*       The callback ...
*
* Do not call it directly ! It is only to be used from within an
* overwritten callback.
*/
void SLIDER_Callback(WM_MESSAGE * pMsg);

/*********************************************************************
*
*                 Member functions
*
**********************************************************************
*/

/* Methods changing properties */

/* Note: These are just examples. The actual methods available for the
   widget will depend on the type of widget. */
void      SLIDER_Dec          (SLIDER_Handle hObj);
GUI_COLOR SLIDER_GetBarColor  (SLIDER_Handle hObj);
GUI_COLOR SLIDER_GetBkColor   (SLIDER_Handle hObj);
GUI_COLOR SLIDER_GetFocusColor(SLIDER_Handle hObj);
GUI_COLOR SLIDER_GetTickColor (SLIDER_Handle hObj);
void      SLIDER_Inc          (SLIDER_Handle hObj);
void      SLIDER_SetBarColor  (SLIDER_Handle hObj, GUI_COLOR Color);
void      SLIDER_SetBkColor   (SLIDER_Handle hObj, GUI_COLOR Color);
GUI_COLOR SLIDER_SetFocusColor(SLIDER_Handle hObj, GUI_COLOR Color);
void      SLIDER_SetNumTicks  (SLIDER_Handle hObj, int NumTicks);
void      SLIDER_SetRange     (SLIDER_Handle hObj, int Min, int Max);
void      SLIDER_SetTickColor (SLIDER_Handle hObj, GUI_COLOR Color);
void      SLIDER_SetValue     (SLIDER_Handle hObj, int v);
void      SLIDER_SetWidth     (SLIDER_Handle hObj, int Width);

/*********************************************************************
*
*       Global functions
*
**********************************************************************
*/

GUI_COLOR SLIDER_GetDefaultBkColor   (void);
GUI_COLOR SLIDER_GetDefaultBarColor  (void);
GUI_COLOR SLIDER_GetDefaultFocusColor(void);
GUI_COLOR SLIDER_GetDefaultTickColor (void);
void      SLIDER_SetDefaultBkColor   (GUI_COLOR Color);
void      SLIDER_SetDefaultBarColor  (GUI_COLOR Color);
GUI_COLOR SLIDER_SetDefaultFocusColor(GUI_COLOR Color);
void      SLIDER_SetDefaultTickColor (GUI_COLOR Color);

/*********************************************************************
*
*                 Query state
*
**********************************************************************
*/
int SLIDER_GetValue(SLIDER_Handle hObj);

#if defined(__cplusplus)
  }
#endif

#endif   /* if GUI_WINSUPPORT */
#endif   /* SLIDER_H */

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
File        : WIDGET.h
Purpose     : Widget interface
---------------------------END-OF-HEADER------------------------------
*/

#ifndef WIDGET_H        /* Avoid multiple inclusion  */
#define WIDGET_H

#if defined(__cplusplus)
extern "C" {     /* Make sure we have C-declarations in C++ programs */
#endif

#include "WM_Intern.h"  /* Window manager, including some internals, which speed things up */

#if GUI_WINSUPPORT

#include "SCROLLBAR.h"


/*********************************************************************
*
*       Defines
*
**********************************************************************
*/
/*********************************************************************
*
*       Unique widget id's
*/
#define BUTTON_ID    0x42555454 /* BUTT */
#define CHECKBOX_ID  0x43484543 /* CHEC */
#define DROPDOWN_ID  0x44524f50 /* DROP */
#define EDIT_ID      0x45444954 /* EDIT */
#define FRAMEWIN_ID  0x4652414d /* FRAM */
#define GRAPH_ID     0x47524150 /* GRAP */
#define HEADER_ID    0x48454144 /* HEAD */
#define LISTBOX_ID   0x4C495342 /* LISB */
#define LISTVIEW_ID  0x4C495356 /* LISV */
#define LISTWHEEL_ID 0x4C495357 /* LISW */
#define MENU_ID      0x4d454e55 /* MENU */
#define MULTIEDIT_ID 0x4d554c45 /* MULE */
#define MULTIPAGE_ID 0x4d554c50 /* MULP */
#define PROGBAR_ID   0x50524f47 /* PROG */
#define RADIO_ID     0x52414449 /* RADI */
#define SCROLLBAR_ID 0x5343524f /* SCRO */
#define SLIDER_ID    0x534c4944 /* SLID */
#define TEXT_ID      0x54455854 /* TEXT */
#define TREEVIEW_ID  0x54524545 /* TREE */
#define ICONVIEW_ID  0x49434f4e /* ICON */

#define WIDGET_LOCK(hWin)       ((WIDGET*)GUI_LOCK_H(hWin))

/*********************************************************************
*
*       Config switches
*/

#ifndef   WIDGET_USE_PARENT_EFFECT
  #define WIDGET_USE_PARENT_EFFECT 0
#endif
#if !defined(WIDGET_USE_SCHEME_SMALL) && !defined(WIDGET_USE_SCHEME_MEDIUM) && !defined(WIDGET_USE_SCHEME_LARGE)
  #define WIDGET_USE_SCHEME_SMALL  1
  #define WIDGET_USE_SCHEME_MEDIUM 0
  #define WIDGET_USE_SCHEME_LARGE  0
#endif
#ifndef   WIDGET_USE_SCHEME_SMALL
  #define WIDGET_USE_SCHEME_SMALL  0
#endif
#ifndef   WIDGET_USE_SCHEME_MEDIUM
  #define WIDGET_USE_SCHEME_MEDIUM 0
#endif
#ifndef   WIDGET_USE_SCHEME_LARGE
  #define WIDGET_USE_SCHEME_LARGE  0
#endif
#if (WIDGET_USE_SCHEME_SMALL + WIDGET_USE_SCHEME_MEDIUM + WIDGET_USE_SCHEME_LARGE) > 1
  #error Only one scheme can be selected!
#endif

/*********************************************************************
*
*       States
*/

#define WIDGET_STATE_FOCUS              (1 << 0)
#define WIDGET_STATE_VERTICAL           (1 << 3)
#define WIDGET_STATE_FOCUSSABLE         (1 << 4)

#define WIDGET_STATE_USER0              (1 << 8)    /* Freely available for derived widget */
#define WIDGET_STATE_USER1              (1 << 9)    /* Freely available for derived widget */
#define WIDGET_STATE_USER2              (1 << 10)   /* Freely available for derived widget */

#define WIDGET_ITEM_DRAW                0
#define WIDGET_ITEM_GET_XSIZE           1
#define WIDGET_ITEM_GET_YSIZE           2
#define WIDGET_DRAW_OVERLAY             3
#define WIDGET_DRAW_BACKGROUND          4

/*********************************************************************
*
*       Messages
*/

#define WM_WIDGET_SET_EFFECT    WM_WIDGET + 0

/*********************************************************************
*
*       Create flags
*/

#define WIDGET_CF_VERTICAL      WIDGET_STATE_VERTICAL

/*********************************************************************
*
*       Typedefs
*
**********************************************************************
*/

typedef struct {
  WM_HWIN hWin;
  int     Cmd;         /* WIDGET_ITEM_GET_XSIZE, WIDGET_ITEM_GET_YSIZE, WIDGET_ITEM_DRAW, */
  int     ItemIndex;
  int     x0, y0, x1, y1;
  void *  p;
} WIDGET_ITEM_DRAW_INFO;

typedef int WIDGET_DRAW_ITEM_FUNC(const WIDGET_ITEM_DRAW_INFO * pDrawItemInfo);


/*********************************************************************
*
*        Widget object
*
* The widget object is the base class for most widgets
*/
typedef struct {
  int EffectSize;
  void (* pfDrawUp)      (void);
  void (* pfDrawUpRect)  (const GUI_RECT * pRect);
  void (* pfDrawDown)    (void);
  void (* pfDrawDownRect)(const GUI_RECT * pRect);
  void (* pfDrawFlat)    (void);
  void (* pfDrawFlatRect)(const GUI_RECT * pRect);
} WIDGET_EFFECT;

typedef struct {
  WM_Obj      Win;
  const WIDGET_EFFECT* pEffect;
  I16 Id;
  U16 State;
} WIDGET;


/*********************************************************************
*
*         GUI_DRAW
*
* The GUI_DRAW object is used as base class for selfdrawing,
* non-windows objects. They are used as content of different widgets,
* such as the bitmap or header widgets.
*/
/* Declare Object struct */
typedef struct GUI_DRAW GUI_DRAW;
typedef void   GUI_DRAW_SELF_CB (void);
typedef WM_HMEM GUI_DRAW_HANDLE;

/* Declare Object constants (member functions etc)  */
typedef struct {
  void (* pfDraw)    (GUI_DRAW_HANDLE hDrawObj, int x, int y);
  int  (* pfGetXSize)(GUI_DRAW_HANDLE hDrawObj);
  int  (* pfGetYSize)(GUI_DRAW_HANDLE hDrawObj);
} GUI_DRAW_CONSTS;

/* Declare Object */
struct GUI_DRAW {
  const GUI_DRAW_CONSTS* pConsts;
  union {
    const void * pData;
    GUI_DRAW_SELF_CB* pfDraw;
  } Data;
  I16 xOff, yOff;
};

/* GUI_DRAW_ API */
void GUI_DRAW__Draw    (GUI_DRAW_HANDLE hDrawObj, int x, int y);
int  GUI_DRAW__GetXSize(GUI_DRAW_HANDLE hDrawObj);
int  GUI_DRAW__GetYSize(GUI_DRAW_HANDLE hDrawObj);

/* GUI_DRAW_ Constructurs for different objects */
WM_HMEM GUI_DRAW_BITMAP_Create  (const GUI_BITMAP* pBitmap, int x, int y);
WM_HMEM GUI_DRAW_BMP_Create     (const void* pBMP, int x, int y);
WM_HMEM GUI_DRAW_STREAMED_Create(const GUI_BITMAP_STREAM * pBitmap, int x, int y);
WM_HMEM GUI_DRAW_SELF_Create(GUI_DRAW_SELF_CB* pfDraw, int x, int y);

/*********************************************************************
*
*       Global data
*
**********************************************************************
*/

extern const WIDGET_EFFECT WIDGET_Effect_3D;
extern const WIDGET_EFFECT WIDGET_Effect_3D1L;
extern const WIDGET_EFFECT WIDGET_Effect_3D2L;
extern const WIDGET_EFFECT WIDGET_Effect_None;
extern const WIDGET_EFFECT WIDGET_Effect_Simple;

/*********************************************************************
*
*       Internal API routines
*
**********************************************************************
*/

void      WIDGET__DrawFocusRect(WIDGET* pWidget, const GUI_RECT* pRect, int Dist);
void      WIDGET__DrawVLine(WIDGET* pWidget, int x, int y0, int y1);
void      WIDGET__EFFECT_DrawDownRect(WIDGET* pWidget, GUI_RECT* pRect);
void      WIDGET__EFFECT_DrawDown(WIDGET* pWidget);
void      WIDGET__EFFECT_DrawUpRect(WIDGET* pWidget, GUI_RECT* pRect);
void      WIDGET__FillRectEx(WIDGET* pWidget, const GUI_RECT* pRect);
int       WIDGET__GetWindowSizeX(WM_HWIN hWin);
GUI_COLOR WIDGET__GetBkColor(WM_HWIN hObj);
int       WIDGET__GetXSize(const WIDGET* pWidget);
int       WIDGET__GetYSize(const WIDGET* pWidget);
void      WIDGET__GetClientRect(WIDGET* pWidget, GUI_RECT* pRect);
void      WIDGET__GetInsideRect(WIDGET* pWidget, GUI_RECT* pRect);
void      WIDGET__Init(WIDGET* pWidget, int Id, U16 State);
void      WIDGET__RotateRect90(WIDGET* pWidget, GUI_RECT* pDest, const GUI_RECT* pRect);
void      WIDGET__SetScrollState(WM_HWIN hWin, const WM_SCROLL_STATE* pVState, const WM_SCROLL_STATE* pState);
void      WIDGET__FillStringInRect(const char GUI_UNI_PTR * pText, const GUI_RECT * pFillRect, const GUI_RECT * pTextRectMax, const GUI_RECT * pTextRectAct);

/*********************************************************************
*
*       API routines
*
**********************************************************************
*/
void  WIDGET_SetState     (WM_HWIN hObj, int State);
void  WIDGET_AndState     (WM_HWIN hObj, int State);
void  WIDGET_OrState      (WM_HWIN hObj, int State);
int   WIDGET_HandleActive (WM_HWIN hObj, WM_MESSAGE* pMsg);
int   WIDGET_GetState     (WM_HWIN hObj);
int   WIDGET_SetWidth     (WM_HWIN hObj, int Width);

void  WIDGET_EFFECT_3D_DrawUp(void);

const WIDGET_EFFECT* WIDGET_SetDefaultEffect(const WIDGET_EFFECT* pEffect);

void  WIDGET_SetEffect              (WM_HWIN hObj, const WIDGET_EFFECT* pEffect);

const WIDGET_EFFECT* WIDGET_GetDefaultEffect(void);
int   WIDGET_SetWidth(WM_HWIN hObj, int Width);

void WIDGET_EFFECT_3D_SetColor    (unsigned Index, GUI_COLOR Color);
void WIDGET_EFFECT_3D1L_SetColor  (unsigned Index, GUI_COLOR Color);
void WIDGET_EFFECT_3D2L_SetColor  (unsigned Index, GUI_COLOR Color);
void WIDGET_EFFECT_Simple_SetColor(unsigned Index, GUI_COLOR Color);

GUI_COLOR WIDGET_EFFECT_3D_GetColor    (unsigned Index);
GUI_COLOR WIDGET_EFFECT_3D1L_GetColor  (unsigned Index);
GUI_COLOR WIDGET_EFFECT_3D2L_GetColor  (unsigned Index);
GUI_COLOR WIDGET_EFFECT_Simple_GetColor(unsigned Index);

int WIDGET_EFFECT_3D_GetNumColors(void);
int WIDGET_EFFECT_3D1L_GetNumColors(void);
int WIDGET_EFFECT_3D2L_GetNumColors(void);
int WIDGET_EFFECT_Simple_GetNumColors(void);

/*********************************************************************
*
*       Compatibility macros
*
**********************************************************************
*/
#define WIDGET_SetDefaultEffect_3D()     WIDGET_SetDefaultEffect(&WIDGET_Effect_3D)
#define WIDGET_SetDefaultEffect_3D1L()   WIDGET_SetDefaultEffect(&WIDGET_Effect_3D1L)
#define WIDGET_SetDefaultEffect_3D2L()   WIDGET_SetDefaultEffect(&WIDGET_Effect_3D2L)
#define WIDGET_SetDefaultEffect_None()   WIDGET_SetDefaultEffect(&WIDGET_Effect_None)
#define WIDGET_SetDefaultEffect_Simple() WIDGET_SetDefaultEffect(&WIDGET_Effect_Simple)

#endif /* GUI_WINSUPPORT */

#if defined(__cplusplus)
  }
#endif

#endif   /* SLIDER_H */





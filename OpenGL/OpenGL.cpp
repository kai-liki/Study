// OpenGL.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include <windows.h>
#include <math.h>			// Math Library Header File
#include <stdio.h>							// Header File For Standard Input/Output ( NEW )
#include <stdlib.h>
#include <gl\gl.h>
#include <gl\glu.h>
#include <gl\glaux.h>

const float M_PI =		3.14159265358979323846;
const float M_PI_2 =	1.57079632679489661923;
const float M_PI_4 =	0.785398163397448309616;

typedef struct tagVERTEX						// Build Our Vertex Structure
{
	float x, y, z;							// 3D Coordinates
	float u, v;							// Texture Coordinates
} VERTEX;								// Call It VERTEX

typedef struct tagTRIANGLE						// Build Our Triangle Structure
{
	VERTEX vertex[3];						// Array Of Three Vertices
} TRIANGLE;								// Call It TRIANGLE

typedef struct tagSECTOR						// Build Our Sector Structure
{
	int numtriangles;						// Number Of Triangles In Sector
	TRIANGLE* triangle;						// Pointer To Array Of Triangles
} SECTOR;								// Call It SECTOR


HGLRC           hRC=NULL;							// Permanent Rendering Context
HDC             hDC=NULL;							// Private GDI Device Context
HWND            hWnd=NULL;							// Holds Our Window Handle
HINSTANCE       hInstance;							// Holds The Instance Of The Application

bool	keys[256];								// Array Used For The Keyboard Routine
bool	active=TRUE;								// Window Active Flag Set To TRUE By Default
bool	fullscreen=TRUE;							// Fullscreen Flag Set To Fullscreen Mode By Default

GLfloat		rtri;						// Angle For The Triangle ( NEW )
GLfloat		rquad;						// Angle For The Quad     ( NEW )

GLfloat		xrot;								// X Rotation ( NEW )
GLfloat		yrot;								// Y Rotation ( NEW )
GLfloat		zrot;								// Z Rotation ( NEW )

GLfloat		xpos;								// X Position
GLfloat		zpos;								// Z Position
GLfloat		ypos;

const float piover180 = 0.0174532925f;
GLfloat		rlength = 0.0f;
GLfloat		alpha = 0.0f;
GLfloat		beta = 0.0f;
GLfloat		dizzy = 0.0f;

GLfloat xspeed;									// X Rotation Speed
GLfloat yspeed;									// Y Rotation Speed
GLfloat	z=0.0f;								// Depth Into The Screen

BOOL	light;									// Lighting ON / OFF
BOOL    blend;						// Blending OFF/ON? ( NEW )
BOOL	twinkle;						// Twinkling Stars

BOOL	lp;									// L Pressed?
BOOL	fp;									// F Pressed?
BOOL	bp;						// B Pressed? ( NEW )
BOOL	tp;							// 'T' Key Pressed?

const int	num=50;							// Number Of Stars To Draw

GLfloat LightAmbient[]= { 0.5f, 0.5f, 0.5f, 1.0f }; 				// Ambient Light Values ( NEW )
GLfloat LightDiffuse[]= { 1.0f, 1.0f, 1.0f, 1.0f };				 // Diffuse Light Values ( NEW )
GLfloat LightPosition[]= { 0.0f, 0.0f, 2.0f, 1.0f };				 // Light Position ( NEW )
GLuint	filter;									// Which Filter To Use

SECTOR sector1;

typedef struct							// Create A Structure For Star
{
	int r, g, b;						// Stars Color
	GLfloat dist;						// Stars Distance From Center
	GLfloat angle;						// Stars Current Angle
}
stars;								// Structures Name Is Stars
stars star[num];						// Make 'star' Array Of 'num' Using Info From The Structure 'stars'

GLfloat	zoom=-15.0f;						// Viewing Distance Away From Stars
GLfloat tilt=90.0f;						// Tilt The View
GLfloat	spin;							// Spin Twinkling Stars

GLuint	loop;							// General Loop Variable

GLuint		texture[4];							// Storage For One Texture ( NEW )

void readstr(FILE *f,char *string)					// Read In A String
{
	do								// Start A Loop
	{
		fgets(string, 255, f);					// Read One Line
	} while ((string[0] == '/') || (string[0] == '\n'));		// See If It Is Worthy Of Processing
	return;								// Jump Back
}

// Previous Declaration: char* worldfile = "data\\world.txt";
char* worldfile = "data/world.txt";
void SetupWorld()							// Setup Our World
{
	FILE *filein;							// File To Work With
	filein = fopen("data/world.txt", "rt");				// Open Our File
	
	int numtriangles;							// Number Of Triangles In Sector
	char string[255];
	readstr(filein, string);
	sscanf(string, "NUMPOLLIES %d\n", &numtriangles);			// Read In Number Of Triangles

	float x, y, z, u, v;							// 3D And Texture Coordinates
	char oneline[255];
	sector1.triangle = new TRIANGLE[numtriangles];				// Allocate Memory For numtriangles And Set Pointer
	sector1.numtriangles = numtriangles;					// Define The Number Of Triangles In Sector 1
	// Step Through Each Triangle In Sector
	for (int triloop = 0; triloop < numtriangles; triloop++)		// Loop Through All The Triangles
	{
		// Step Through Each Vertex In Triangle
		for (int vertloop = 0; vertloop < 3; vertloop++)		// Loop Through All The Vertices
		{
			readstr(filein,oneline);				// Read String To Work With
			// Read Data Into Respective Vertex Values
			sscanf(oneline, "%f %f %f %f %f", &x, &y, &z, &u, &v);
			// Store Values Into Respective Vertices
			sector1.triangle[triloop].vertex[vertloop].x = x;	// Sector 1, Triangle triloop, Vertice vertloop, x Value=x
			sector1.triangle[triloop].vertex[vertloop].y = y;	// Sector 1, Triangle triloop, Vertice vertloop, y Value=y
			sector1.triangle[triloop].vertex[vertloop].z = z;	// Sector 1, Triangle triloop, Vertice vertloop, z Value=z
			sector1.triangle[triloop].vertex[vertloop].u = u;	// Sector 1, Triangle triloop, Vertice vertloop, u Value=u
			sector1.triangle[triloop].vertex[vertloop].v = v;	// Sector 1, Triangle triloop, Vertice vertloop, v Value=v
		}
	}

	fclose(filein);							// Close Our File
	return;								// Jump Back
}

AUX_RGBImageRec *LoadBMP(char *Filename)					// Loads A Bitmap Image
{
	FILE *File=NULL;
	if (!Filename)								// Make Sure A Filename Was Given
	{
		return NULL;							// If Not Return NULL
	}
	File=fopen(Filename,"r");						// Check To See If The File Exists
	if (File)								// Does The File Exist?
	{
		fclose(File);							// Close The Handle
		return auxDIBImageLoad(Filename);				// Load The Bitmap And Return A Pointer
	}
	return NULL;								// If Load Failed Return NULL
}

int LoadGLTextures()								// Load Bitmaps And Convert To Textures
{
	int Status=FALSE;							// Status Indicator
	AUX_RGBImageRec *TextureImage[2];					// Create Storage Space For The Texture
	memset(TextureImage,0,sizeof(void *)*2);				// Set The Pointer To NULL
	// Load The Bitmap, Check For Errors, If Bitmap's Not Found Quit
	if (TextureImage[0]=LoadBMP("Img/panda.bmp"))
	{
		Status=TRUE;							// Set The Status To TRUE
		glGenTextures(1, &texture[0]);					// Create The Texture

		// Typical Texture Generation Using Data From The Bitmap
		glBindTexture(GL_TEXTURE_2D, texture[0]);
		// Generate The Texture
		glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MIN_FILTER,GL_NEAREST);	// Linear Filtering
		glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER,GL_NEAREST);	// Linear Filtering
		glTexImage2D(GL_TEXTURE_2D, 0, 3, TextureImage[0]->sizeX, TextureImage[0]->sizeY, 0, GL_RGB, GL_UNSIGNED_BYTE, TextureImage[0]->data);
	}
	// Load The Bitmap, Check For Errors, If Bitmap's Not Found Quit
	if (TextureImage[1]=LoadBMP("Img/icon.bmp"))
	{
		Status=TRUE;							// Set The Status To TRUE
		glGenTextures(3, &texture[1]);					// Create The Texture

		// Create Nearest Filtered Texture
		glBindTexture(GL_TEXTURE_2D, texture[1]);
		glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER,GL_NEAREST); // ( NEW )
		glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MIN_FILTER,GL_NEAREST); // ( NEW )
		glTexImage2D(GL_TEXTURE_2D, 0, 3, TextureImage[1]->sizeX, TextureImage[1]->sizeY, 0, GL_RGB, GL_UNSIGNED_BYTE, TextureImage[1]->data);

		// Create Linear Filtered Texture
		glBindTexture(GL_TEXTURE_2D, texture[2]);
		glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER,GL_LINEAR);
		glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MIN_FILTER,GL_LINEAR);
		glTexImage2D(GL_TEXTURE_2D, 0, 3, TextureImage[1]->sizeX, TextureImage[1]->sizeY, 0, GL_RGB, GL_UNSIGNED_BYTE, TextureImage[1]->data);

		// Create MipMapped Texture
		glBindTexture(GL_TEXTURE_2D, texture[3]);
		glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MAG_FILTER,GL_LINEAR);
		glTexParameteri(GL_TEXTURE_2D,GL_TEXTURE_MIN_FILTER,GL_LINEAR_MIPMAP_NEAREST); // ( NEW )
		gluBuild2DMipmaps(GL_TEXTURE_2D, 3, TextureImage[1]->sizeX, TextureImage[1]->sizeY, GL_RGB, GL_UNSIGNED_BYTE, TextureImage[1]->data); // ( NEW )
	}
	if (TextureImage[0] && TextureImage[1])							// If Texture Exists
	{
		if (TextureImage[0]->data)					// If Texture Image Exists
		{
			free(TextureImage[0]->data);				// Free The Texture Image Memory
		}

		free(TextureImage[0]);						// Free The Image Structure
		if (TextureImage[1]->data)					// If Texture Image Exists
		{
			free(TextureImage[1]->data);				// Free The Texture Image Memory
		}

		free(TextureImage[1]);	
	}
	return Status;								// Return The Status
}



GLfloat testY=-1.0f;

LRESULT	CALLBACK WndProc(HWND, UINT, WPARAM, LPARAM);				// Declaration For WndProc

GLvoid ReSizeGLScene(GLsizei width, GLsizei height)				// Resize And Initialize The GL Window
{
	if (height==0)								// Prevent A Divide By Zero By
	{
		height=1;							// Making Height Equal One
	}

	glViewport(0, 0, width, height);					// Reset The Current Viewport

	glMatrixMode(GL_PROJECTION);						// Select The Projection Matrix
	glLoadIdentity();							// Reset The Projection Matrix

	// Calculate The Aspect Ratio Of The Window
	gluPerspective(45.0f,(GLfloat)width/(GLfloat)height,0.1f,100.0f);

	glMatrixMode(GL_MODELVIEW);						// Select The Modelview Matrix
	glLoadIdentity();							// Reset The Modelview Matrix
}

int InitGL(GLvoid)								// All Setup For OpenGL Goes Here
{
	if (!LoadGLTextures())							// Jump To Texture Loading Routine ( NEW )
	{
		return FALSE;							// If Texture Didn't Load Return FALSE ( NEW )
	}

	glEnable(GL_TEXTURE_2D);						// Enable Texture Mapping ( NEW )

	glShadeModel(GL_SMOOTH);						// Enables Smooth Shading
	glClearColor(0.0f, 0.0f, 0.0f, 0.0f);					// Black Background
	glClearDepth(1.0f);							// Depth Buffer Setup
	
	// Remove following two lines to not using Depth Testing
	glEnable(GL_DEPTH_TEST);						// Enables Depth Testing
	glDepthFunc(GL_LESS);							// The Type Of Depth Test To Do

	glHint(GL_PERSPECTIVE_CORRECTION_HINT, GL_NICEST);			// Really Nice Perspective Calculations

	// Set up the light
	glLightfv(GL_LIGHT1, GL_AMBIENT, LightAmbient);				// Setup The Ambient Light
	glLightfv(GL_LIGHT1, GL_DIFFUSE, LightDiffuse);				// Setup The Diffuse Light
	glLightfv(GL_LIGHT1, GL_POSITION,LightPosition);			// Position The Light
	glEnable(GL_LIGHT1);							// Enable Light One

	// Enable Blend 
	//glColor4f(1.0f,1.0f,1.0f,0.5f);			// Full Brightness, 50% Alpha ( NEW )
	glBlendFunc(GL_SRC_ALPHA,GL_ONE);		// Blending Function For Translucency Based On Source Alpha Value ( NEW )

	// Initialize the stars' structure
	//for (loop=0; loop<num; loop++)				// Create A Loop That Goes Through All The Stars
	//{
	//	star[loop].angle=0.0f;				// Start All The Stars At Angle Zero
	//	star[loop].dist=(float(loop)/num)*5.0f;		// Calculate Distance From The Center
	//	star[loop].r=rand()%256;			// Give star[loop] A Random Red Intensity
	//	star[loop].g=rand()%256;			// Give star[loop] A Random Green Intensity
	//	star[loop].b=rand()%256;			// Give star[loop] A Random Blue Intensity
	//}

	SetupWorld();

	return TRUE;								// Initialization Went OK
}

GLvoid KillGLWindow(GLvoid)							// Properly Kill The Window
{
	if (fullscreen)								// Are We In Fullscreen Mode?
	{
		ChangeDisplaySettings(NULL,0);					// If So Switch Back To The Desktop
		ShowCursor(TRUE);						// Show Mouse Pointer
	}
	if (hRC)								// Do We Have A Rendering Context?
	{
		if (!wglMakeCurrent(NULL,NULL))					// Are We Able To Release The DC And RC Contexts?
		{
			MessageBox(NULL,"Release Of DC And RC Failed.","SHUTDOWN ERROR",MB_OK | MB_ICONINFORMATION);
		}
		if (!wglDeleteContext(hRC))					// Are We Able To Delete The RC?
		{
			MessageBox(NULL,"Release Rendering Context Failed.","SHUTDOWN ERROR",MB_OK | MB_ICONINFORMATION);
		}
		hRC=NULL;
	}
	if (hDC && !ReleaseDC(hWnd,hDC))					// Are We Able To Release The DC
	{
		MessageBox(NULL,"Release Device Context Failed.","SHUTDOWN ERROR",MB_OK | MB_ICONINFORMATION);
		hDC=NULL;							// Set DC To NULL
	}
	if (hWnd && !DestroyWindow(hWnd))					// Are We Able To Destroy The Window?
	{
		MessageBox(NULL,"Could Not Release hWnd.","SHUTDOWN ERROR",MB_OK | MB_ICONINFORMATION);
		hWnd=NULL;							// Set hWnd To NULL
	}
	if (!UnregisterClass("OpenGL",hInstance))				// Are We Able To Unregister Class
	{
		MessageBox(NULL,"Could Not Unregister Class.","SHUTDOWN ERROR",MB_OK | MB_ICONINFORMATION);
		hInstance=NULL;							// Set hInstance To NULL
	}
}

BOOL CreateGLWindow(char* title, int width, int height, int bits, bool fullscreenflag)
{
	GLuint		PixelFormat;						// Holds The Results After Searching For A Match
	WNDCLASS	wc;							// Windows Class Structure
	DWORD		dwExStyle;						// Window Extended Style
	DWORD		dwStyle;						// Window Style
	RECT WindowRect;							// Grabs Rectangle Upper Left / Lower Right Values
	WindowRect.left=(long)0;						// Set Left Value To 0
	WindowRect.right=(long)width;						// Set Right Value To Requested Width
	WindowRect.top=(long)0;							// Set Top Value To 0
	WindowRect.bottom=(long)height;						// Set Bottom Value To Requested Height

	fullscreen=fullscreenflag;						// Set The Global Fullscreen Flag
	hInstance		= GetModuleHandle(NULL);			// Grab An Instance For Our Window
	wc.style		= CS_HREDRAW | CS_VREDRAW | CS_OWNDC;		// Redraw On Move, And Own DC For Window
	wc.lpfnWndProc		= (WNDPROC) WndProc;				// WndProc Handles Messages
	wc.cbClsExtra		= 0;						// No Extra Window Data
	wc.cbWndExtra		= 0;						// No Extra Window Data
	wc.hInstance		= hInstance;					// Set The Instance
	wc.hIcon		= LoadIcon(NULL, IDI_WINLOGO);			// Load The Default Icon
	wc.hCursor		= LoadCursor(NULL, IDC_ARROW);			// Load The Arrow Pointer
	wc.hbrBackground	= NULL;						// No Background Required For GL
	wc.lpszMenuName		= NULL;						// We Don't Want A Menu
	wc.lpszClassName	= "OpenGL";					// Set The Class Name

	if (!RegisterClass(&wc))						// Attempt To Register The Window Class
	{
		MessageBox(NULL,"Failed To Register The Window Class.","ERROR",MB_OK|MB_ICONEXCLAMATION);
		return FALSE;							// Exit And Return FALSE
	}

	if (fullscreen)								// Attempt Fullscreen Mode?
	{
		DEVMODE dmScreenSettings;					// Device Mode
		memset(&dmScreenSettings,0,sizeof(dmScreenSettings));		// Makes Sure Memory's Cleared
		dmScreenSettings.dmSize=sizeof(dmScreenSettings);		// Size Of The Devmode Structure
		dmScreenSettings.dmPelsWidth	= width;			// Selected Screen Width
		dmScreenSettings.dmPelsHeight	= height;			// Selected Screen Height
		dmScreenSettings.dmBitsPerPel	= bits;				// Selected Bits Per Pixel
		dmScreenSettings.dmFields=DM_BITSPERPEL|DM_PELSWIDTH|DM_PELSHEIGHT;
		// Try To Set Selected Mode And Get Results.  NOTE: CDS_FULLSCREEN Gets Rid Of Start Bar.
		if (ChangeDisplaySettings(&dmScreenSettings,CDS_FULLSCREEN)!=DISP_CHANGE_SUCCESSFUL)
		{
			// If The Mode Fails, Offer Two Options.  Quit Or Run In A Window.
			if (MessageBox(NULL,"The Requested Fullscreen Mode Is Not Supported By\nYour Video Card. Use Windowed Mode Instead?","NeHe GL",MB_YESNO|MB_ICONEXCLAMATION)==IDYES)
			{
				fullscreen=FALSE;				// Select Windowed Mode (Fullscreen=FALSE)
			}
			else
			{
				// Pop Up A Message Box Letting User Know The Program Is Closing.
				MessageBox(NULL,"Program Will Now Close.","ERROR",MB_OK|MB_ICONSTOP);
				return FALSE;					// Exit And Return FALSE
			}
		}
	}
	if (fullscreen)								// Are We Still In Fullscreen Mode?
	{
		dwExStyle=WS_EX_APPWINDOW;					// Window Extended Style
		dwStyle=WS_POPUP;						// Windows Style
		ShowCursor(FALSE);						// Hide Mouse Pointer
	}
	else
	{
		dwExStyle=WS_EX_APPWINDOW | WS_EX_WINDOWEDGE;			// Window Extended Style
		dwStyle=WS_OVERLAPPEDWINDOW;					// Windows Style
	}

	AdjustWindowRectEx(&WindowRect, dwStyle, FALSE, dwExStyle);		// Adjust Window To True Requested Size

	if (!(hWnd=CreateWindowEx(	dwExStyle,				// Extended Style For The Window
					"OpenGL",				// Class Name
					title,					// Window Title
					WS_CLIPSIBLINGS |			// Required Window Style
					WS_CLIPCHILDREN |			// Required Window Style
					dwStyle,				// Selected Window Style
					0, 0,					// Window Position
					WindowRect.right-WindowRect.left,	// Calculate Adjusted Window Width
					WindowRect.bottom-WindowRect.top,	// Calculate Adjusted Window Height
					NULL,					// No Parent Window
					NULL,					// No Menu
					hInstance,				// Instance
					NULL)))					// Don't Pass Anything To WM_CREATE
	{
		KillGLWindow();							// Reset The Display
		MessageBox(NULL,"Window Creation Error.","ERROR",MB_OK|MB_ICONEXCLAMATION);
		return FALSE;							// Return FALSE
	}

	static	PIXELFORMATDESCRIPTOR pfd=					// pfd Tells Windows How We Want Things To Be
	{
		sizeof(PIXELFORMATDESCRIPTOR),					// Size Of This Pixel Format Descriptor
		1,								// Version Number
		PFD_DRAW_TO_WINDOW |						// Format Must Support Window
		PFD_SUPPORT_OPENGL |						// Format Must Support OpenGL
		PFD_DOUBLEBUFFER,						// Must Support Double Buffering
		PFD_TYPE_RGBA,							// Request An RGBA Format
		bits,								// Select Our Color Depth
		0, 0, 0, 0, 0, 0,						// Color Bits Ignored
		0,								// No Alpha Buffer
		0,								// Shift Bit Ignored
		0,								// No Accumulation Buffer
		0, 0, 0, 0,							// Accumulation Bits Ignored
		16,								// 16Bit Z-Buffer (Depth Buffer)
		0,								// No Stencil Buffer
		0,								// No Auxiliary Buffer
		PFD_MAIN_PLANE,							// Main Drawing Layer
		0,								// Reserved
		0, 0, 0								// Layer Masks Ignored
	};

	if (!(hDC=GetDC(hWnd)))							// Did We Get A Device Context?
	{
		KillGLWindow();							// Reset The Display
		MessageBox(NULL,"Can't Create A GL Device Context.","ERROR",MB_OK|MB_ICONEXCLAMATION);
		return FALSE;							// Return FALSE
	}

	if (!(PixelFormat=ChoosePixelFormat(hDC,&pfd)))				// Did Windows Find A Matching Pixel Format?
	{
		KillGLWindow();							// Reset The Display
		MessageBox(NULL,"Can't Find A Suitable PixelFormat.","ERROR",MB_OK|MB_ICONEXCLAMATION);
		return FALSE;							// Return FALSE
	}

	if(!SetPixelFormat(hDC,PixelFormat,&pfd))				// Are We Able To Set The Pixel Format?
	{
		KillGLWindow();							// Reset The Display
		MessageBox(NULL,"Can't Set The PixelFormat.","ERROR",MB_OK|MB_ICONEXCLAMATION);
		return FALSE;							// Return FALSE
	}

	if (!(hRC=wglCreateContext(hDC)))					// Are We Able To Get A Rendering Context?
	{
		KillGLWindow();							// Reset The Display
		MessageBox(NULL,"Can't Create A GL Rendering Context.","ERROR",MB_OK|MB_ICONEXCLAMATION);
		return FALSE;							// Return FALSE
	}

	if(!wglMakeCurrent(hDC,hRC))						// Try To Activate The Rendering Context
	{
		KillGLWindow();							// Reset The Display
		MessageBox(NULL,"Can't Activate The GL Rendering Context.","ERROR",MB_OK|MB_ICONEXCLAMATION);
		return FALSE;							// Return FALSE
	}

	ShowWindow(hWnd,SW_SHOW);						// Show The Window
	SetForegroundWindow(hWnd);						// Slightly Higher Priority
	SetFocus(hWnd);								// Sets Keyboard Focus To The Window
	ReSizeGLScene(width, height);						// Set Up Our Perspective GL Screen

	if (!InitGL())								// Initialize Our Newly Created GL Window
	{
		KillGLWindow();							// Reset The Display
		MessageBox(NULL,"Initialization Failed.","ERROR",MB_OK|MB_ICONEXCLAMATION);
		return FALSE;							// Return FALSE
	}
	return TRUE;								// Success
}

LRESULT CALLBACK WndProc(	HWND	hWnd,					// Handle For This Window
				UINT	uMsg,					// Message For This Window
				WPARAM	wParam,					// Additional Message Information
				LPARAM	lParam)					// Additional Message Information
{
	switch (uMsg)								// Check For Windows Messages
	{
		case WM_ACTIVATE:						// Watch For Window Activate Message
		{
			if (!HIWORD(wParam))					// Check Minimization State
			{
				active=TRUE;					// Program Is Active
			}
			else
			{
				active=FALSE;					// Program Is No Longer Active
			}

			return 0;						// Return To The Message Loop
		}
		case WM_SYSCOMMAND:						// Intercept System Commands
		{
			switch (wParam)						// Check System Calls
			{
				case SC_SCREENSAVE:				// Screensaver Trying To Start?
				case SC_MONITORPOWER:				// Monitor Trying To Enter Powersave?
				return 0;					// Prevent From Happening
			}
			break;							// Exit
		}
		case WM_CLOSE:							// Did We Receive A Close Message?
		{
			PostQuitMessage(0);					// Send A Quit Message
			return 0;						// Jump Back
		}
		case WM_KEYDOWN:						// Is A Key Being Held Down?
		{
			keys[wParam] = TRUE;					// If So, Mark It As TRUE
			return 0;						// Jump Back
		}
		case WM_KEYUP:							// Has A Key Been Released?
		{
			keys[wParam] = FALSE;					// If So, Mark It As FALSE
			return 0;						// Jump Back
		}
		case WM_SIZE:							// Resize The OpenGL Window
		{
			ReSizeGLScene(LOWORD(lParam),HIWORD(lParam));		// LoWord=Width, HiWord=Height
			return 0;						// Jump Back
		}
	}
	// Pass All Unhandled Messages To DefWindowProc
	return DefWindowProc(hWnd,uMsg,wParam,lParam);
}

void TestChange(void)
{
	if(testY>=0)
		testY=-1.0f;
	else
		testY+=0.01f;
}

int DrawGLSceneNew(GLvoid)
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);			// Clear The Screen And The Depth Buffer
	glLoadIdentity();						// Reset The Current Matrix

	GLfloat x_m, y_m, z_m, u_m, v_m;				// Floating Point For Temp X, Y, Z, U And V Vertices
	GLfloat xtrans = -xpos;						// Used For Player Translation On The X Axis
	GLfloat ztrans = -zpos;						// Used For Player Translation On The Z Axis
	GLfloat ytrans = -ypos;						// Used For Bouncing Motion Up And Down

	int numtriangles;						// Integer To Hold The Number Of Triangles

	glRotatef(-alpha,1.0f,0,0);					// Rotate Up And Down To Look Up And Down
	glRotatef(-beta,0,1.0f,0);					// Rotate Depending On Direction Player Is Facing
	glRotatef(dizzy,0,0,1.0f);
	
	glTranslatef(xtrans, ytrans, ztrans);				// Translate The Scene Based On Player Position

	glBindTexture(GL_TEXTURE_2D, texture[filter]);			// Select A Texture Based On filter
	//glBindTexture(GL_TEXTURE_2D, texture[3]);			// Select A Texture Based On filter
	
	numtriangles = sector1.numtriangles;				// Get The Number Of Triangles In Sector 1
	
	// Process Each Triangle
	for (int loop_m = 0; loop_m < numtriangles; loop_m++)		// Loop Through All The Triangles
	{
		glBegin(GL_TRIANGLES);					// Start Drawing Triangles
			glNormal3f( 0.0f, 0.0f, 1.0f);			// Normal Pointing Forward
			x_m = sector1.triangle[loop_m].vertex[0].x;	// X Vertex Of 1st Point
			y_m = sector1.triangle[loop_m].vertex[0].y;	// Y Vertex Of 1st Point
			z_m = sector1.triangle[loop_m].vertex[0].z;	// Z Vertex Of 1st Point
			u_m = sector1.triangle[loop_m].vertex[0].u;	// U Texture Coord Of 1st Point
			v_m = sector1.triangle[loop_m].vertex[0].v;	// V Texture Coord Of 1st Point
			glTexCoord2f(u_m,v_m); glVertex3f(x_m,y_m,z_m);	// Set The TexCoord And Vertice
			
			x_m = sector1.triangle[loop_m].vertex[1].x;	// X Vertex Of 2nd Point
			y_m = sector1.triangle[loop_m].vertex[1].y;	// Y Vertex Of 2nd Point
			z_m = sector1.triangle[loop_m].vertex[1].z;	// Z Vertex Of 2nd Point
			u_m = sector1.triangle[loop_m].vertex[1].u;	// U Texture Coord Of 2nd Point
			v_m = sector1.triangle[loop_m].vertex[1].v;	// V Texture Coord Of 2nd Point
			glTexCoord2f(u_m,v_m); glVertex3f(x_m,y_m,z_m);	// Set The TexCoord And Vertice
			
			x_m = sector1.triangle[loop_m].vertex[2].x;	// X Vertex Of 3rd Point
			y_m = sector1.triangle[loop_m].vertex[2].y;	// Y Vertex Of 3rd Point
			z_m = sector1.triangle[loop_m].vertex[2].z;	// Z Vertex Of 3rd Point
			u_m = sector1.triangle[loop_m].vertex[2].u;	// U Texture Coord Of 3rd Point
			v_m = sector1.triangle[loop_m].vertex[2].v;	// V Texture Coord Of 3rd Point
			glTexCoord2f(u_m,v_m); glVertex3f(x_m,y_m,z_m);	// Set The TexCoord And Vertice
		glEnd();						// Done Drawing Triangles
	}

	return TRUE;
}
int DrawGLScene(GLvoid)								// Here's Where We Do All The Drawing
{
	glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);			// Clear The Screen And The Depth Buffer
	//======================================================
	//TODO: Place drawing code here.
	glBindTexture(GL_TEXTURE_2D, texture[0]);		// Select Our Texture
	
	// Draw stars
	for (loop=0; loop<num; loop++)				// Loop Through All The Stars
	{
		glLoadIdentity();				// Reset The View Before We Draw Each Star
		glTranslatef(0.0f,0.0f,zoom);			// Zoom Into The Screen (Using The Value In 'zoom')
		glRotatef(tilt,1.0f,0.0f,0.0f);			// Tilt The View (Using The Value In 'tilt')
		glRotatef(star[loop].angle,0.0f,1.0f,0.0f);	// Rotate To The Current Stars Angle
		glTranslatef(star[loop].dist,0.0f,0.0f);	// Move Forward On The X Plane
		//glRotatef(-star[loop].angle,0.0f,1.0f,0.0f);	// Cancel The Current Stars Angle
		//glRotatef(-tilt,1.0f,0.0f,0.0f);		// Cancel The Screen Tilt
		if (twinkle)					// Twinkling Stars Enabled
		{
			// Assign A Color Using Bytes
			glColor4ub(star[(num-loop)-1].r,star[(num-loop)-1].g,star[(num-loop)-1].b,255);
			glBegin(GL_QUADS);			// Begin Drawing The Textured Quad
				glTexCoord2f(0.0f, 0.0f); glVertex3f(-0.1f,-0.1f, 0.0f);
				glTexCoord2f(1.0f, 0.0f); glVertex3f( 0.1f,-0.1f, 0.0f);
				glTexCoord2f(1.0f, 1.0f); glVertex3f( 0.1f, 0.1f, 0.0f);
				glTexCoord2f(0.0f, 1.0f); glVertex3f(-0.1f, 0.1f, 0.0f);
			glEnd();				// Done Drawing The Textured Quad
		}

		glRotatef(spin,0.0f,0.0f,1.0f);			// Rotate The Star On The Z Axis
		// Assign A Color Using Bytes
		glColor4ub(star[loop].r,star[loop].g,star[loop].b,255);
		glBegin(GL_QUADS);				// Begin Drawing The Textured Quad
			glTexCoord2f(0.0f, 0.0f); glVertex3f(-0.1f,-0.1f, 0.0f);
			glTexCoord2f(1.0f, 0.0f); glVertex3f( 0.1f,-0.1f, 0.0f);
			glTexCoord2f(1.0f, 1.0f); glVertex3f( 0.1f, 0.1f, 0.0f);
			glTexCoord2f(0.0f, 1.0f); glVertex3f(-0.1f, 0.1f, 0.0f);
		glEnd();					// Done Drawing The Textured Quad

		//glRotatef(-star[loop].angle,0.0f,1.0f,0.0f);	// Cancel The Current Stars Angle
		//glRotatef(-tilt,1.0f,0.0f,0.0f);		// Cancel The Screen Tilt

		spin+=0.01f;					// Used To Spin The Stars
		star[loop].angle+=float(loop)/num;		// Changes The Angle Of A Star
		star[loop].dist-=0.01f;				// Changes The Distance Of A Star

		if (star[loop].dist<0.0f)			// Is The Star In The Middle Yet
		{
			star[loop].dist+=5.0f;			// Move The Star 5 Units From The Center
			star[loop].r=rand()%256;		// Give It A New Red Value
			star[loop].g=rand()%256;		// Give It A New Green Value
			star[loop].b=rand()%256;		// Give It A New Blue Value
		}
	}

	/** Remove rendering other objects.
	glLoadIdentity();							// Reset The Current Modelview Matrix
	glTranslatef(-1.0f,0.0f,zoom);					// Move Left 1.5 Units And Into The Screen 6.0
	glRotatef(rtri,0.0f,1.0f,0.0f);				// Rotate The Triangle On The Y axis ( NEW )

	glBegin(GL_TRIANGLES);						// Drawing Using Triangles
		glColor3f(1.0f,0.0f,0.0f);			// Red
		glVertex3f( 0.0f, 1.0f, 0.0f);			// Top Of Triangle (Front)
		glColor3f(0.0f,1.0f,0.0f);			// Green
		glVertex3f(-1.0f,-1.0f, 1.0f);			// Left Of Triangle (Front)
		glColor3f(0.0f,0.0f,1.0f);			// Blue
		glVertex3f( 1.0f,-1.0f, 1.0f);			// Right Of Triangle (Front)

		glColor3f(1.0f,0.0f,0.0f);			// Red
		glVertex3f( 0.0f, 1.0f, 0.0f);			// Top Of Triangle (Right)
		glColor3f(0.0f,0.0f,1.0f);			// Blue
		glVertex3f( 1.0f,-1.0f, 1.0f);			// Left Of Triangle (Right)
		glColor3f(0.0f,1.0f,0.0f);			// Green
		glVertex3f( 1.0f,-1.0f, -1.0f);			// Right Of Triangle (Right)

		glColor3f(1.0f,0.0f,0.0f);			// Red
		glVertex3f( 0.0f, 1.0f, 0.0f);			// Top Of Triangle (Back)
		glColor3f(0.0f,1.0f,0.0f);			// Green
		glVertex3f( 1.0f,-1.0f, -1.0f);			// Left Of Triangle (Back)
		glColor3f(0.0f,0.0f,1.0f);			// Blue
		glVertex3f(-1.0f,-1.0f, -1.0f);			// Right Of Triangle (Back)

		glColor3f(1.0f,0.0f,0.0f);			// Red
		glVertex3f( 0.0f, 1.0f, 0.0f);			// Top Of Triangle (Left)
		glColor3f(0.0f,0.0f,1.0f);			// Blue
		glVertex3f(-1.0f,-1.0f,-1.0f);			// Left Of Triangle (Left)
		glColor3f(0.0f,1.0f,0.0f);			// Green
		glVertex3f(-1.0f,-1.0f, 1.0f);			// Right Of Triangle (Left)

	glEnd();						// Done Drawing A Triangle
	
	glLoadIdentity();					// Reset The Current Modelview Matrix
										// Add this glLoadIdentity() will reset the view.
										// Removing it, the quad will rotate on the Y with the transgle
	//glTranslatef(3.0f,0.0f,0.0f);				// From Right Point Move 3 Units Right
	glTranslatef(1.0f,0.0f,zoom);				// From Right Point Move 3 Units Right
	glRotatef(rquad,1.0f,0.0f,0.0f);			// Rotate The Quad On The X axis ( NEW )

	glBegin(GL_QUADS);					// Start Drawing Quads
		glColor3f(0.0f,1.0f,0.0f);			// Set The Color To Green
		glVertex3f( 1.0f, 1.0f,-1.0f);			// Top Right Of The Quad (Top)
		glVertex3f(-1.0f, 1.0f,-1.0f);			// Top Left Of The Quad (Top)
		glVertex3f(-1.0f, 1.0f, 1.0f);			// Bottom Left Of The Quad (Top)
		glVertex3f( 1.0f, 1.0f, 1.0f);			// Bottom Right Of The Quad (Top)

		glColor3f(1.0f,0.5f,0.0f);			// Set The Color To Orange
		glVertex3f( 1.0f,-1.0f, 1.0f);			// Top Right Of The Quad (Bottom)
		glVertex3f(-1.0f,-1.0f, 1.0f);			// Top Left Of The Quad (Bottom)
		glVertex3f(-1.0f,-1.0f,-1.0f);			// Bottom Left Of The Quad (Bottom)
		glVertex3f( 1.0f,-1.0f,-1.0f);			// Bottom Right Of The Quad (Bottom)

		glColor3f(1.0f,0.0f,0.0f);			// Set The Color To Red
		glVertex3f( 1.0f, 1.0f, 1.0f);			// Top Right Of The Quad (Front)
		glVertex3f(-1.0f, 1.0f, 1.0f);			// Top Left Of The Quad (Front)
		glVertex3f(-1.0f,-1.0f, 1.0f);			// Bottom Left Of The Quad (Front)
		glVertex3f( 1.0f,-1.0f, 1.0f);			// Bottom Right Of The Quad (Front)

		glColor3f(1.0f,1.0f,0.0f);			// Set The Color To Yellow
		glVertex3f( 1.0f,-1.0f,-1.0f);			// Bottom Left Of The Quad (Back)
		glVertex3f(-1.0f,-1.0f,-1.0f);			// Bottom Right Of The Quad (Back)
		glVertex3f(-1.0f, 1.0f,-1.0f);			// Top Right Of The Quad (Back)
		glVertex3f( 1.0f, 1.0f,-1.0f);			// Top Left Of The Quad (Back)

		glColor3f(0.0f,0.0f,1.0f);			// Set The Color To Blue
		glVertex3f(-1.0f, 1.0f, 1.0f);			// Top Right Of The Quad (Left)
		glVertex3f(-1.0f, 1.0f,-1.0f);			// Top Left Of The Quad (Left)
		glVertex3f(-1.0f,-1.0f,-1.0f);			// Bottom Left Of The Quad (Left)
		glVertex3f(-1.0f,-1.0f, 1.0f);			// Bottom Right Of The Quad (Left)

		glColor3f(1.0f,0.0f,1.0f);			// Set The Color To Violet
		glVertex3f( 1.0f, 1.0f,-1.0f);			// Top Right Of The Quad (Right)
		glVertex3f( 1.0f, 1.0f, 1.0f);			// Top Left Of The Quad (Right)
		glVertex3f( 1.0f,-1.0f, 1.0f);			// Bottom Left Of The Quad (Right)
		glVertex3f( 1.0f,-1.0f,-1.0f);			// Bottom Right Of The Quad (Right)

	glEnd();						// Done Drawing A Quad

	rtri+=0.2f;						// Increase The Rotation Variable For The Triangle ( NEW )
	rquad-=0.15f;						// Decrease The Rotation Variable For The Quad     ( NEW )
	
	glLoadIdentity();							// Reset The Current Matrix
	glTranslatef(0.0f,3.0f,zoom);						// Move Into The Screen 5 Units

	glRotatef(xrot,1.0f,0.0f,0.0f);						// Rotate On The X Axis
	glRotatef(yrot,0.0f,1.0f,0.0f);						// Rotate On The Y Axis
	glRotatef(zrot,0.0f,0.0f,1.0f);						// Rotate On The Z Axis

	glBindTexture(GL_TEXTURE_2D, texture[0]);				// Select Our Texture
	glBegin(GL_QUADS);
		// Front Face
		glTexCoord2f(0.0f, 0.0f); glVertex3f(-1.0f, -1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
		glTexCoord2f(1.0f, 0.0f); glVertex3f( 1.0f, -1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
		glTexCoord2f(1.0f, 1.0f); glVertex3f( 1.0f,  1.0f,  1.0f);	// Top Right Of The Texture and Quad
		glTexCoord2f(0.0f, 1.0f); glVertex3f(-1.0f,  1.0f,  1.0f);	// Top Left Of The Texture and Quad
		// Back Face
		glTexCoord2f(1.0f, 0.0f); glVertex3f(-1.0f, -1.0f, -1.0f);	// Bottom Right Of The Texture and Quad
		glTexCoord2f(1.0f, 1.0f); glVertex3f(-1.0f,  1.0f, -1.0f);	// Top Right Of The Texture and Quad
		glTexCoord2f(0.0f, 1.0f); glVertex3f( 1.0f,  1.0f, -1.0f);	// Top Left Of The Texture and Quad
		glTexCoord2f(0.0f, 0.0f); glVertex3f( 1.0f, -1.0f, -1.0f);	// Bottom Left Of The Texture and Quad
		// Top Face
		glTexCoord2f(0.0f, 1.0f); glVertex3f(-1.0f,  1.0f, -1.0f);	// Top Left Of The Texture and Quad
		glTexCoord2f(0.0f, 0.0f); glVertex3f(-1.0f,  1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
		glTexCoord2f(1.0f, 0.0f); glVertex3f( 1.0f,  1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
		glTexCoord2f(1.0f, 1.0f); glVertex3f( 1.0f,  1.0f, -1.0f);	// Top Right Of The Texture and Quad
		// Bottom Face
		glTexCoord2f(1.0f, 1.0f); glVertex3f(-1.0f, -1.0f, -1.0f);	// Top Right Of The Texture and Quad
		glTexCoord2f(0.0f, 1.0f); glVertex3f( 1.0f, -1.0f, -1.0f);	// Top Left Of The Texture and Quad
		glTexCoord2f(0.0f, 0.0f); glVertex3f( 1.0f, -1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
		glTexCoord2f(1.0f, 0.0f); glVertex3f(-1.0f, -1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
		// Right face
		glTexCoord2f(1.0f, 0.0f); glVertex3f( 1.0f, -1.0f, -1.0f);	// Bottom Right Of The Texture and Quad
		glTexCoord2f(1.0f, 1.0f); glVertex3f( 1.0f,  1.0f, -1.0f);	// Top Right Of The Texture and Quad
		glTexCoord2f(0.0f, 1.0f); glVertex3f( 1.0f,  1.0f,  1.0f);	// Top Left Of The Texture and Quad
		glTexCoord2f(0.0f, 0.0f); glVertex3f( 1.0f, -1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
		// Left Face
		glTexCoord2f(0.0f, 0.0f); glVertex3f(-1.0f, -1.0f, -1.0f);	// Bottom Left Of The Texture and Quad
		glTexCoord2f(1.0f, 0.0f); glVertex3f(-1.0f, -1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
		glTexCoord2f(1.0f, 1.0f); glVertex3f(-1.0f,  1.0f,  1.0f);	// Top Right Of The Texture and Quad
		glTexCoord2f(0.0f, 1.0f); glVertex3f(-1.0f,  1.0f, -1.0f);	// Top Left Of The Texture and Quad
	glEnd();

	glLoadIdentity();							// Reset The Current Matrix
	glTranslatef(0.0f,-3.0f, zoom);						// Move Into The Screen 5 Units

	glRotatef(xrot,1.0f,0.0f,0.0f);						// Rotate On The X Axis
	glRotatef(yrot,0.0f,1.0f,0.0f);						// Rotate On The Y Axis
	glRotatef(zrot,0.0f,0.0f,1.0f);						// Rotate On The Z Axis

	glBindTexture(GL_TEXTURE_2D, texture[filter+1]);				// Select Our Texture
	glBegin(GL_QUADS);
		// Front Face
		glTexCoord2f(0.0f, 0.0f); glVertex3f(-1.0f, -1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
		glTexCoord2f(1.0f, 0.0f); glVertex3f( 1.0f, -1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
		glTexCoord2f(1.0f, 1.0f); glVertex3f( 1.0f,  1.0f,  1.0f);	// Top Right Of The Texture and Quad
		glTexCoord2f(0.0f, 1.0f); glVertex3f(-1.0f,  1.0f,  1.0f);	// Top Left Of The Texture and Quad
		// Back Face
		glTexCoord2f(1.0f, 0.0f); glVertex3f(-1.0f, -1.0f, -1.0f);	// Bottom Right Of The Texture and Quad
		glTexCoord2f(1.0f, 1.0f); glVertex3f(-1.0f,  1.0f, -1.0f);	// Top Right Of The Texture and Quad
		glTexCoord2f(0.0f, 1.0f); glVertex3f( 1.0f,  1.0f, -1.0f);	// Top Left Of The Texture and Quad
		glTexCoord2f(0.0f, 0.0f); glVertex3f( 1.0f, -1.0f, -1.0f);	// Bottom Left Of The Texture and Quad
		// Top Face
		glTexCoord2f(0.0f, 1.0f); glVertex3f(-1.0f,  1.0f, -1.0f);	// Top Left Of The Texture and Quad
		glTexCoord2f(0.0f, 0.0f); glVertex3f(-1.0f,  1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
		glTexCoord2f(1.0f, 0.0f); glVertex3f( 1.0f,  1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
		glTexCoord2f(1.0f, 1.0f); glVertex3f( 1.0f,  1.0f, -1.0f);	// Top Right Of The Texture and Quad
		// Bottom Face
		glTexCoord2f(1.0f, 1.0f); glVertex3f(-1.0f, -1.0f, -1.0f);	// Top Right Of The Texture and Quad
		glTexCoord2f(0.0f, 1.0f); glVertex3f( 1.0f, -1.0f, -1.0f);	// Top Left Of The Texture and Quad
		glTexCoord2f(0.0f, 0.0f); glVertex3f( 1.0f, -1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
		glTexCoord2f(1.0f, 0.0f); glVertex3f(-1.0f, -1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
		// Right face
		glTexCoord2f(1.0f, 0.0f); glVertex3f( 1.0f, -1.0f, -1.0f);	// Bottom Right Of The Texture and Quad
		glTexCoord2f(1.0f, 1.0f); glVertex3f( 1.0f,  1.0f, -1.0f);	// Top Right Of The Texture and Quad
		glTexCoord2f(0.0f, 1.0f); glVertex3f( 1.0f,  1.0f,  1.0f);	// Top Left Of The Texture and Quad
		glTexCoord2f(0.0f, 0.0f); glVertex3f( 1.0f, -1.0f,  1.0f);	// Bottom Left Of The Texture and Quad
		// Left Face
		glTexCoord2f(0.0f, 0.0f); glVertex3f(-1.0f, -1.0f, -1.0f);	// Bottom Left Of The Texture and Quad
		glTexCoord2f(1.0f, 0.0f); glVertex3f(-1.0f, -1.0f,  1.0f);	// Bottom Right Of The Texture and Quad
		glTexCoord2f(1.0f, 1.0f); glVertex3f(-1.0f,  1.0f,  1.0f);	// Top Right Of The Texture and Quad
		glTexCoord2f(0.0f, 1.0f); glVertex3f(-1.0f,  1.0f, -1.0f);	// Top Left Of The Texture and Quad
	glEnd();
		
	xrot+=xspeed;								// X Axis Rotation
	yrot+=yspeed;								// Y Axis Rotation
	zrot+=0.4f;								// Z Axis Rotation

	**/
	//======================================================
	return TRUE;								// Everything Went OK
}

int APIENTRY WinMain(HINSTANCE hInstance,
                     HINSTANCE hPrevInstance,
                     LPSTR     lpCmdLine,
                     int       nCmdShow)
{
 	// TODO: Place code here.
	MSG	msg;								// Windows Message Structure
	BOOL	done=FALSE;							// Bool Variable To Exit Loop

	// Ask The User Which Screen Mode They Prefer
	if (MessageBox(NULL,"Would You Like To Run In Fullscreen Mode?", "Start FullScreen?",MB_YESNO|MB_ICONQUESTION)==IDNO)
	{
		fullscreen=FALSE;						// Windowed Mode
	}

	// Create Our OpenGL Window
	if (!CreateGLWindow("NeHe's OpenGL Framework",1280,1024,16,fullscreen))
	{
		return 0;							// Quit If Window Was Not Created
	}

	while(!done)								// Loop That Runs Until done=TRUE
	{
		if (PeekMessage(&msg,NULL,0,0,PM_REMOVE))			// Is There A Message Waiting?
		{
			if (msg.message==WM_QUIT)				// Have We Received A Quit Message?
			{
				done=TRUE;					// If So done=TRUE
			}
			else							// If Not, Deal With Window Messages
			{
				TranslateMessage(&msg);				// Translate The Message
				DispatchMessage(&msg);				// Dispatch The Message
			}
		}
		else								// If There Are No Messages
		{
			// Draw The Scene.  Watch For ESC Key And Quit Messages From DrawGLScene()
			if ((active && !DrawGLSceneNew()) || keys[VK_ESCAPE])						// Program Active?
			{
					done=TRUE;				// ESC Signalled A Quit
			}
			else						// Not Time To Quit, Update Screen
			{
				SwapBuffers(hDC);			// Swap Buffers (Double Buffering)
				
				// Press L Key
				if (keys['L'] && !lp)				// L Key Being Pressed Not Held?
				{
					lp=TRUE;				// lp Becomes TRUE
					light=!light;				// Toggle Light TRUE/FALSE
					if (!light)				// If Not Light
					{
						glDisable(GL_LIGHTING);		// Disable Lighting
					}
					else					// Otherwise
					{
						glEnable(GL_LIGHTING);		// Enable Lighting
					}
				}
				if (!keys['L'])					// Has L Key Been Released?
				{
					lp=FALSE;				// If So, lp Becomes FALSE
				}

				// Press F Key
				if (keys['F'] && !fp)				// Is F Key Being Pressed?
				{
					fp=TRUE;				// fp Becomes TRUE
					filter+=1;				// filter Value Increases By One
					if (filter>3)				// Is Value Greater Than 2?
					{
						filter=0;			// If So, Set filter To 0
					}
				}
				if (!keys['F'])					// Has F Key Been Released?
				{
					fp=FALSE;				// If So, fp Becomes FALSE
				}

				// Press D Key
				if (keys['D'])				// Is Up Arrow Being Pressed?
				{
					xpos -= (-1.0f) * (float)cos(alpha*piover180) * (float)cos(beta*piover180) * 0.01f;
					zpos -= (float)cos(alpha*piover180) * (float)sin(beta*piover180) * 0.01f;
				}
				// Press A Key
				if (keys['A'])				// Is Up Arrow Being Pressed?
				{
					xpos += (-1.0f) * (float)cos(alpha*piover180) * (float)cos(beta*piover180) * 0.01f;
					zpos += (float)cos(alpha*piover180) * (float)sin(beta*piover180) * 0.01f;
				}
				// Press W Key
				if (keys['W'])				// Is Up Arrow Being Pressed?
				{
					xpos += (-1.0f) * (float)cos(alpha*piover180) * (float)sin(beta*piover180) * 0.01f;
					ypos += (float)sin(alpha*piover180) * (float)0.01f;
					zpos += (-1.0f) * (float)cos(alpha*piover180) * (float)cos(beta*piover180) * 0.01f;
				}
				// Press S Key
				if (keys['S'])				// Is Up Arrow Being Pressed?
				{
					xpos -= (-1.0f) * (float)cos(alpha*piover180) * (float)sin(beta*piover180) * 0.01f;
					ypos -= (float)sin(alpha*piover180) * (float)0.01f;
					zpos -= (-1.0f) * (float)cos(alpha*piover180) * (float)cos(beta*piover180) * 0.01f;
				}
				// Press Page Up Key
				if (keys[VK_PRIOR])				// Is Up Arrow Being Pressed?
				{
					xpos += (-1.0f) * (float)cos(alpha*piover180) * (float)sin(beta*piover180) * 0.01f;
					ypos += (-1.0f) * (float)sin(alpha*piover180) * (float)0.01f;
					//zpos += (float)cos(alpha*piover180) * (float)cos(beta*piover180) * 0.01f;
				}
				// Press Page Down Key
				if (keys[VK_NEXT])				// Is Up Arrow Being Pressed?
				{
					xpos -= (-1.0f) * (float)cos(alpha*piover180) * (float)sin(beta*piover180) * 0.01f;
					ypos -= (-1.0f) * (float)sin(alpha*piover180) * (float)0.01f;
					//zpos -= (float)cos(alpha*piover180) * (float)cos(beta*piover180) * 0.01f;
				}

				// Press Up, Down, Right, Left Arrow Key
				if (keys[VK_UP])				// Is Up Arrow Being Pressed?
				{
					alpha += 1.0f;
				}
				if (keys[VK_DOWN])				// Is Down Arrow Being Pressed?
				{
					alpha -= 1.0f;
				}
				if (keys[VK_RIGHT])				// Is Right Arrow Being Pressed?
				{
					beta -= 1.0f;
				}
				if (keys[VK_LEFT])				// Is Left Arrow Being Pressed?
				{
					beta += 1.0f;
				}

				// Press B Key
				if (keys['B'] && !bp)				// Is B Key Pressed And bp FALSE?
				{
					bp=TRUE;				// If So, bp Becomes TRUE
					blend = !blend;				// Toggle blend TRUE / FALSE	
					if(blend)				// Is blend TRUE?
					{
						glEnable(GL_BLEND);		// Turn Blending On
						glDisable(GL_DEPTH_TEST);	// Turn Depth Testing Off
					}
					else					// Otherwise
					{
						glDisable(GL_BLEND);		// Turn Blending Off
						glEnable(GL_DEPTH_TEST);	// Turn Depth Testing On
					}
				}
				if (!keys['B'])					// Has B Key Been Released?
				{
					bp=FALSE;				// If So, bp Becomes FALSE
				}

				// Press T Key
				if (keys['T'] && !tp)				// Is T Being Pressed And Is tp FALSE
				{
					tp=TRUE;				// If So, Make tp TRUE
					twinkle=!twinkle;			// Make twinkle Equal The Opposite Of What It Is
				}
				if (!keys['T'])					// Has The T Key Been Released
				{
					tp=FALSE;				// If So, make tp FALSE
				}
				
				// Press F1 Key
				if (keys[VK_F1])					// Is F1 Being Pressed?
				{
					keys[VK_F1]=FALSE;				// If So Make Key FALSE
					KillGLWindow();					// Kill Our Current Window
					fullscreen=!fullscreen;				// Toggle Fullscreen / Windowed Mode
					// Recreate Our OpenGL Window
					if (!CreateGLWindow("NeHe's OpenGL Framework",1280,1024,16,fullscreen))
					{
						return 0;				// Quit If Window Was Not Created
					}
				}
			}
		}
	}
	// Shutdown
	KillGLWindow();								// Kill The Window
	return (msg.wParam);							// Exit The Program
}




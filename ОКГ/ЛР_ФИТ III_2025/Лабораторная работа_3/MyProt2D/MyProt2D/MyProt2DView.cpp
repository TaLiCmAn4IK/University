
// MyProt2DView.cpp: реализация класса CMyProt2DView

#include "pch.h"
#include "framework.h"

#include "MyProt2D.h"

#include "MyProt2DDoc.h"
#include "MyProt2DView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CMyProt2DView

IMPLEMENT_DYNCREATE(CMyProt2DView, CView)

BEGIN_MESSAGE_MAP(CMyProt2DView, CView)
	ON_COMMAND(ID_TESTS_F1, &CMyProt2DView::OnTestsF1)
	ON_COMMAND(ID_TESTS_F2, &CMyProt2DView::OnTestsF2)
	ON_COMMAND(ID_TESTS_F3, &CMyProt2DView::OnTestsF3)
END_MESSAGE_MAP()

// Создание или уничтожение CMyProt2DView

CMyProt2DView::CMyProt2DView() noexcept
{

}

CMyProt2DView::~CMyProt2DView()
{
}

BOOL CMyProt2DView::PreCreateWindow(CREATESTRUCT& cs)
{
	if (!CWnd::PreCreateWindow(cs))
		return FALSE;

	cs.dwExStyle |= WS_EX_CLIENTEDGE;
	cs.style &= ~WS_BORDER;
	cs.lpszClass = AfxRegisterWndClass(CS_HREDRAW | CS_VREDRAW | CS_DBLCLKS,
		::LoadCursor(nullptr, IDC_ARROW), reinterpret_cast<HBRUSH>(COLOR_WINDOW + 1), nullptr);

	return TRUE;
}

#pragma region математика

double CMyProt2DView::MyF1(double x)
{
	if (x == 0) return 1;
	return sin(x) / x;
}

double CMyProt2DView::MyF2(double x)
{
	return sqrt(fabs(x)) * sin(x);
}

#pragma endregion

#pragma region вспомогательные функции

CRect CMyProt2DView::GetCenterRect(double fx, double fy)
// Прямоугольная область, занимающая долю fx на fy клиентского окна, по центру окна
{
	CRect rc;
	GetClientRect(&rc);
	int w = (int)(rc.Width() * fx);
	int h = (int)(rc.Height() * fy);
	if (w < 50) w = 50;
	if (h < 50) h = 50;
	int x0 = (rc.Width() - w) / 2;
	int y0 = (rc.Height() - h) / 2;
	return CRect(x0, y0, x0 + w, y0 + h);
}

CRect CMyProt2DView::GetCenterSquare(double f)
// Квадрат со стороной f * min(ширина, высота) окна, по центру окна
{
	CRect rc;
	GetClientRect(&rc);
	int side = (int)(min(rc.Width(), rc.Height()) * f);
	if (side < 50) side = 50;
	int x0 = (rc.Width() - side) / 2;
	int y0 = (rc.Height() - side) / 2;
	return CRect(x0, y0, x0 + side, y0 + side);
}

#pragma endregion



// Рисование CMyProt2DView


void CMyProt2DView::OnDraw(CDC* pDC)
{
	switch (Index)
	{
	case 1:	// Tests_F -> F1
	case 2:	// Tests_F -> F2
		Graph.Draw(*pDC, 1, 1);	// рамка области, оси координат, линия графика
		break;
	case 3:	// Tests_F -> F3
		GraphCircle.Draw(*pDC, 0, 0);	// окружность радиуса 10 (задана в МСК)
		Graph.Draw(*pDC, 0, 0);			// восьмиугольник (задан в МСК)
		break;
	}
}


// Обработчики сообщений CMyPlot2DView
void CMyProt2DView::OnTestsF1()
{
	// Отрезок [-3pi; 3pi], шаг dx = pi/36 -> массивы X (аргумент) и Y (значения)
	double xL = -3 * pi;
	double xH = -xL;
	double dx = pi / 36;
	int N = (int)round((xH - xL) / dx);
	X.RedimMatrix(N + 1);
	Y.RedimMatrix(N + 1);
	for (int i = 0; i <= N; i++)
	{
		X(i) = xL + i * dx;
		Y(i) = MyF1(X(i));
	}
	// Перо графика: толщина 1, красный, сплошная
	PenLine.Set(PS_SOLID, 1, RGB(255, 0, 0));
	// Перо осей: толщина 2, синий
	PenAxis.Set(PS_SOLID, 2, RGB(0, 0, 255));
	// Область отображения - по центру окна (задается в оконных координатах)
	RW = GetCenterRect(0.7, 0.7);
	Graph.SetParams(X, Y, RW);	// здесь вычисляется матрица пересчета K
	Graph.SetPenLine(PenLine);
	Graph.SetPenAxis(PenAxis);
	Index = 1;
	this->Invalidate();
}

void CMyProt2DView::OnTestsF2()
{
	// Отрезок [-4pi; 4pi], шаг dx = pi/36 -> массивы X (аргумент) и Y (значения)
	double xL = -4 * pi;
	double xH = -xL;
	double dx = pi / 36;
	int N = (int)round((xH - xL) / dx);
	X.RedimMatrix(N + 1);
	Y.RedimMatrix(N + 1);
	for (int i = 0; i <= N; i++)
	{
		X(i) = xL + i * dx;
		Y(i) = MyF2(X(i));
	}
	// Перо графика: толщина 3, красный, штрих-пунктир
	PenLine.Set(PS_DASHDOT, 3, RGB(255, 0, 0));
	// Перо осей: толщина 2, черный
	PenAxis.Set(PS_SOLID, 2, RGB(0, 0, 0));
	// Область отображения - по центру окна (задается в оконных координатах)
	RW = GetCenterRect(0.7, 0.7);
	Graph.SetParams(X, Y, RW);	// здесь вычисляется матрица пересчета K
	Graph.SetPenLine(PenLine);
	Graph.SetPenAxis(PenAxis);
	Index = 2;
	this->Invalidate();
}

void CMyProt2DView::OnTestsF3()
{
	// Мировые координаты (МСК): правильный восьмиугольник,
	// вписанный в окружность радиуса 10
	const double R = 10.0;
	const int Nc = 72;	// число отрезков аппроксимации окружности
	X.RedimMatrix(9);	// 8 вершин + замыкание контура
	Y.RedimMatrix(9);
	for (int i = 0; i <= 8; i++)
	{
		double a = i * pi / 4;
		X(i) = R * cos(a);
		Y(i) = R * sin(a);
	}
	// Точки окружности тоже заданы в МСК
	CMatrix XC(Nc + 1), YC(Nc + 1);
	for (int i = 0; i <= Nc; i++)
	{
		double a = 2 * pi * i / Nc;
		XC(i) = R * cos(a);
		YC(i) = R * sin(a);
	}
	// Область отображения - квадрат по центру окна
	RW = GetCenterSquare(0.6);
	// Перо восьмиугольника: толщина 3, красный, сплошная
	CMyPen penOct;
	penOct.Set(PS_SOLID, 3, RGB(255, 0, 0));
	// Перо окружности: толщина 2, синий, сплошная
	CMyPen penCirc;
	penCirc.Set(PS_SOLID, 2, RGB(0, 0, 255));
	// Оба объекта приводятся к оконным координатам одной матрицей K,
	// т.к. область в МСК у них общая: [-10; 10] x [-10; 10]
	Graph.SetParams(X, Y, RW);
	Graph.SetPenLine(penOct);
	GraphCircle.SetParams(XC, YC, RW);
	GraphCircle.SetPenLine(penCirc);
	Index = 3;
	Invalidate();
}


// Диагностика CMyProt2DView

#ifdef _DEBUG
void CMyProt2DView::AssertValid() const
{
	CView::AssertValid();
}

void CMyProt2DView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

#endif //_DEBUG
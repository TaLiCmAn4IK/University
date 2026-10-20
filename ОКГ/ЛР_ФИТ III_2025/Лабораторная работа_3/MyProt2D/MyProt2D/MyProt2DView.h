
// MyProt2DView.h: интерфейс класса CMyProt2DView
//

#pragma once

class CMyProt2DView : public CView
{
protected: // создать только из сериализации
	CMyProt2DView() noexcept;
	DECLARE_DYNCREATE(CMyProt2DView)

// Атрибуты
public:
	int Index = 0;	// режим отрисовки: 0 - пусто, 1 - F1, 2 - F2, 3 - F3
	CMatrix X, Y;	// массивы Xi и Yi (мировые координаты)
	CRect RW;	// Прямоугольник в окне (оконные координаты)
	CPlot2D Graph;	// график F1/F2, для F3 - восьмиугольник
	CPlot2D GraphCircle;	// для F3 - окружность
	CMyPen PenLine, // Перо для линий
	PenAxis;	// Перо для осей

// Операции
public:
	double MyF1(double x);
	double MyF2(double x);
	CRect GetCenterRect(double fx, double fy);	// прямоугольная область по центру окна
	CRect GetCenterSquare(double f);	// квадрат по центру окна



// Переопределение
public:
	virtual void OnDraw(CDC* pDC);  // переопределено для отрисовки этого представления
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:

// Реализация
public:
	virtual ~CMyProt2DView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Созданные функции схемы сообщений
protected:
	void OnTestsF1();
	void OnTestsF2();
	void OnTestsF3();

	DECLARE_MESSAGE_MAP()
};
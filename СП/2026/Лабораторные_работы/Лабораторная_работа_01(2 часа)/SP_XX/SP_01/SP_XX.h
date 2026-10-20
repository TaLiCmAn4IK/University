#pragma once

#define SP_XX_EXPORTS

#ifdef SP_XX_EXPORTS
#define SP_XX_API __declspec(dllexport)
#else
#define SP_XX_API __declspec(dllimport)
#endif

extern "C" {
    // Оборачиваем в пространство имен, чтобы избежать конфликта с системным div
    namespace SP {
        SP_XX_API int sum(int x, int y);
        SP_XX_API int sub(int x, int y);
        SP_XX_API int mul(int x, int y);
        SP_XX_API int div(int x, int y);
    }
}

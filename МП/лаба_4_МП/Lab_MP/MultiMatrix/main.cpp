#include <cmath>
#include <memory.h>
#include <chrono>
#include <iostream>
#include <iomanip>
#include "MultiMatrix.h"

#define N_MAX 6

using namespace std;
using namespace std::chrono;

extern int rec_call_count;

void print_optimal_parens(int i, int j, int n, int* s)
{
    if (i == j)
    {
        cout << "A" << i;
    }
    else
    {
        int k = s[(i - 1) * n + (j - 1)];
        cout << "(";
        print_optimal_parens(i, k, n, s);
        print_optimal_parens(k + 1, j, n, s);
        cout << ")";
    }
}

int main()
{
    setlocale(LC_ALL, "rus");

    // –‡ÁÏÂÌÓÒÚË Ï‡ÚËˆ ‰Îˇ 12 ‚‡Ë‡ÌÚ‡
    int Mc[N_MAX + 1] = { 8, 11, 19, 22, 29, 39, 50 };

    cout << endl;
    cout << "============================================================" << endl;
    cout << "     «¿ƒ¿◊¿ Œœ“»Ã¿À‹ÕŒ… –¿——“¿ÕŒ¬ » — Œ¡Œ  (¬¿–»¿Õ“ 12)" << endl;
    cout << "============================================================" << endl;

    cout << endl << "–‡ÁÏÂÌÓÒÚË Ï‡ÚËˆ:" << endl;
    for (int i = 1; i <= N_MAX; i++)
    {
        cout << "A" << i << ": " << Mc[i - 1] << " x " << Mc[i] << endl;
    }

    // –≈ ”–—»¬Õ€… Ã≈“Œƒ
    cout << endl << "--- –≈ ”–—»¬Õ€… Ã≈“Œƒ ---" << endl;

    int Ms[N_MAX][N_MAX];
    int r = 0;

    rec_call_count = 0;
    memset(Ms, 0, sizeof(int) * N_MAX * N_MAX);

    auto t1 = high_resolution_clock::now();
    r = OptimalM(1, N_MAX, N_MAX, Mc, OPTIMALM_PARM(Ms));
    auto t2 = high_resolution_clock::now();

    auto duration_rec = duration_cast<microseconds>(t2 - t1).count();

    cout << " ÓÎË˜ÂÒÚ‚Ó ÂÍÛÒË‚Ì˚ı ‚˚ÁÓ‚Ó‚: " << rec_call_count << endl;
    cout << "¬ÂÏˇ ‚˚ÔÓÎÌÂÌËˇ: " << duration_rec << " ÏÍÒ" << endl;
    cout << "ÃËÌËÏ‡Î¸ÌÓÂ ÍÓÎË˜ÂÒÚ‚Ó ÓÔÂ‡ˆËÈ ÛÏÌÓÊÂÌËˇ: " << r << endl;

    // ƒ»Õ¿Ã»◊≈— Œ≈ œ–Œ√–¿ÃÃ»–Œ¬¿Õ»≈
    cout << endl << "--- ƒ»Õ¿Ã»◊≈— Œ≈ œ–Œ√–¿ÃÃ»–Œ¬¿Õ»≈ ---" << endl;

    int Ms2[N_MAX][N_MAX];
    int rd = 0;

    memset(Ms2, 0, sizeof(int) * N_MAX * N_MAX);

    t1 = high_resolution_clock::now();
    rd = OptimalMD(N_MAX, Mc, OPTIMALM_PARM(Ms2));
    t2 = high_resolution_clock::now();

    auto duration_dp = duration_cast<microseconds>(t2 - t1).count();

    cout << "¬ÂÏˇ ‚˚ÔÓÎÌÂÌËˇ: " << duration_dp << " ÏÍÒ" << endl;
    cout << "ÃËÌËÏ‡Î¸ÌÓÂ ÍÓÎË˜ÂÒÚ‚Ó ÓÔÂ‡ˆËÈ ÛÏÌÓÊÂÌËˇ: " << rd << endl;

    // Ã¿“–»÷¿ S
    cout << endl << "Ã‡ÚËˆ‡ S (ÓÔÚËÏ‡Î¸Ì˚Â ËÌ‰ÂÍÒ˚ ‡Á·ËÂÌËˇ):" << endl;
    cout << "     A1  A2  A3  A4  A5  A6" << endl;
    for (int i = 1; i <= N_MAX; i++)
    {
        cout << "A" << i << "   ";
        for (int j = 1; j <= N_MAX; j++)
        {
            if (j < i)
                cout << " .  ";
            else
                cout << setw(2) << Ms2[i - 1][j - 1] << "  ";
        }
        cout << endl;
    }

    // Œœ“»Ã¿À‹Õ¿ﬂ –¿——“¿ÕŒ¬ ¿ — Œ¡Œ  
    cout << endl << "Œœ“»Ã¿À‹Õ¿ﬂ –¿——“¿ÕŒ¬ ¿ — Œ¡Œ :" << endl;
    cout << "—ıÂÏ‡: ";
    print_optimal_parens(1, N_MAX, N_MAX, OPTIMALM_PARM(Ms2));
    cout << endl;

    // —–¿¬Õ»“≈À‹Õ€… ¿Õ¿À»« ¬–≈Ã≈Õ» ¬€œŒÀÕ≈Õ»ﬂ
    cout << endl << "—–¿¬Õ»“≈À‹Õ€… ¿Õ¿À»« ¬–≈Ã≈Õ» ¬€œŒÀÕ≈Õ»ﬂ" << endl;
    cout << "------------------------------------------------------------" << endl;
    cout << " ÓÎ-‚Ó Ï‡ÚËˆ | –ÂÍÛÒËˇ (ÏÍÒ) | ƒœ (ÏÍÒ) | ”ÒÍÓÂÌËÂ" << endl;
    cout << "------------------------------------------------------------" << endl;

    // «‡ÏÂ˚ ‰Îˇ ‡ÁÌÓ„Ó ÍÓÎË˜ÂÒÚ‚‡ Ï‡ÚËˆ (N = 2, 3, 4, 5, 6)
    int results[5][3] = { 0 };  // [N][ÂÍÛÒËˇ, ƒœ]

    for (int n = 2; n <= 6; n++)
    {
        int temp_s[6][6] = { 0 };
        rec_call_count = 0;

        // –ÂÍÛÒËˇ
        auto t1r = high_resolution_clock::now();
        int temp_r = OptimalM(1, n, n, Mc, OPTIMALM_PARM(temp_s));
        auto t2r = high_resolution_clock::now();
        long long time_rec = duration_cast<microseconds>(t2r - t1r).count();

        // ƒËÌ‡ÏËÍ‡
        int temp_s2[6][6] = { 0 };
        auto t1d = high_resolution_clock::now();
        int temp_rd = OptimalMD(n, Mc, OPTIMALM_PARM(temp_s2));
        auto t2d = high_resolution_clock::now();
        long long time_dp = duration_cast<microseconds>(t2d - t1d).count();

        double speedup = (double)time_rec / time_dp;

        cout << "     " << n << "      |      " << time_rec
            << "     |    " << time_dp << "     |   ";
        cout << fixed << setprecision(2) << speedup << "x" << endl;
    }

    system("pause");
    return 0;
}
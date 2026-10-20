#include <iostream>
#include "Structs.h"
#include "BFS.h"
#include "DFS.h"
#include <vector>

using namespace std;
using namespace graph;

void MyDFS(const AMatrix& graph, int vertex, std::vector<bool>& visited)
{
	visited[vertex] = true;
	std::cout << vertex << " ";

	for (int i = 0; i < graph.n_vertex; ++i)
	{
		if (graph.mr[vertex * graph.n_vertex + i] && !visited[i])
		{
			MyDFS(graph, i, visited);
		}
	}
}

int main() {
	setlocale(LC_ALL, ".1251");
	int mr[] =
	{
		0, 0, 1, 1, 0, 0, 0,
		0, 0, 0, 1, 0, 0, 0,
		0, 0, 0, 1, 0, 1, 0,
		0, 0, 0, 0, 0, 0, 0,
		0, 1, 0, 0, 0, 0, 0,
		0, 0, 0, 1, 0, 0, 1,
		0, 0, 0, 1, 1, 0, 0
	};
	const AMatrix matrix(7, mr);
	puts("Ваша матрица:");
	for (int i = 0; i < 7; i++) {
		for (int j = 0; j < 7; j++) {
			cout << matrix.get(i, j) << " ";
		}
		cout << endl;
	}
	BFS bfs(matrix, 0);
	int k = 6;
	puts("\nРезультат обхода в ширину");
	do {
		cout << bfs.get() << " ";
	} while (k--);
	cout << endl;
	puts("\nРезультат обхода графа в глубину");
	vector<bool> visited(7, false);
	for (int i = 0; i < 7; i++) {
		if (!visited[i]) {
			MyDFS(matrix, i, visited);
		}
	}
	cout << endl;
	DFS dfs(matrix);
	puts("\nТопологическая сортировка");
	for (int i = 0; i < 7; i++) {
		cout << dfs.get(6 - i) << " ";
	}
}
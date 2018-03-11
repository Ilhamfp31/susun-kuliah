#include <bits/stdc++.h>
using namespace std;

bool SortBySecDesc(const pair<string,int> &a, const pair<string,int> &b) {
  return (a.second > b.second);
}

class Graph {
private:
  unordered_map<string, vector<string>> adj_list;
  unordered_map<string, bool> visited;
  vector<pair<string, int>> timestamp_vertices;
  vector<string> start_vertices;
  vector<vector<pair<string,int>>> solution;
  int cur_timestamp = 0;

public:
  Graph() {
    adj_list["C1"].push_back("C3");
    adj_list["C2"].push_back("C3");
    adj_list["C2"].push_back("C4");
    adj_list["C2"].push_back("C5");
    adj_list["C3"].push_back("C5");
    adj_list["C4"].push_back("C5");
    adj_list["C4"].push_back("C6");
    adj_list["C5"].push_back("C6");

    for(auto &kv : visited) {
      visited[kv.first] = false;
    }

    start_vertices.push_back("C1");
    start_vertices.push_back("C2");
  }

  void _TopologicalSort(string vertice) {
    cur_timestamp++;
    visited[vertice] = true;
    for (auto neighbour : adj_list[vertice]) {
      if (!visited[neighbour]) {
        _TopologicalSort(neighbour);
      }
    }
    cur_timestamp++;
    timestamp_vertices.insert(timestamp_vertices.begin(), make_pair(vertice, cur_timestamp));
    solution.push_back(timestamp_vertices);
  }

  void TopologicalSort() {
    for (auto vertice : start_vertices) {
      _TopologicalSort(vertice);
    }
    for (auto v : solution) {
      for (auto x : v) {
        cout << x.first << " ";
      }
      cout << "\n";
    }
  }
};

int main() {
  Graph g;
  g.TopologicalSort();
}
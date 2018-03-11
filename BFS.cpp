#include <iostream>
#include <unordered_map>
#include <vector>

using namespace std;

vector<vector<string> > BFS(unordered_map<string, vector<string> > input_map) {
	bool end = false;
	unordered_map<string, int> counter;
	vector<vector<string> > output;
	vector<string> deliminator;
	for (auto& it : input_map) {
		counter[it.first] = it.second.size();
	}
	while (!end) {
		end = true;
		for (auto& it : counter) {
			if (it.second == 0) {
				deliminator.push_back(it.first);
				it.second--;
			}
			else if (it.second > 0) {
				end = false;
			}
			else {
				it.second = -2;
			}
		}
		for (auto& it : input_map) {
			for (int i = 0; i < it.second.size(); i++) {
				for (int j = 0; j < deliminator.size(); j++) {
					if ((it.second[i] == deliminator[j]) && (counter[deliminator[j]] == -1)) {
						counter[it.first]--;
					}
				}
			}
		}
		output.push_back(deliminator);
		for (int i = 0; i < deliminator.size(); i++) {
			cout << deliminator[i] << " ";
		}
		cout << endl;
	}
	return output;
}

int main() {
	unordered_map<string, vector<string> > input_map;
	vector<string> v1;
	v1.push_back("C3");
	input_map["C1"] = v1;

	vector<string> v2;
	v2.push_back("C1");
	v2.push_back("C4");
	input_map["C2"] = v2;

	vector<string> v3;
	input_map["C3"] = v3;

	vector<string> v4;
	v4.push_back("C3");
	input_map["C4"] = v4;

	vector<string> v5;
	v5.push_back("C2");
	v5.push_back("C4");
	input_map["C5"] = v5;

	vector<vector<string> > output = BFS(input_map);

	return 0;
}

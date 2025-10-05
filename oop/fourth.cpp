#include <random>
#include <vector>
#include <string>
#include <algorithm>
#include <iostream>

using namespace std;


class Sotrudnic { 
private: 
    string name; 
    int age; 
    string det; 
    string study;
    
public:
    Sotrudnic(string names, int ages, string dets, string studys):
        name(names),
        age(ages),
        det(dets),
        study(studys)
    {}
    Sotrudnic() : name(""), age(0), det(""), study("") {}
    
    void outPutTo(){
        cout << "Имя: " << name << endl;
        cout << "Лет: " << age << endl;
        cout << "Категория деятельности: " << det << endl;
        cout << "Образование: " << study << endl;
    }
    
    void set_Age(int ag) { 
        age = ag;
    }
    void set_Name(string nam) { 
        name = nam;
    }
    void set_Dets(string de) { 
        det = de;
    }
    void set_Study(string stud) { 
        study = stud;
    }
    int get_Age() const{ 
        return age;
    }
    string get_Name() const{ 
        return name;
    }
    string get_Dets() const{ 
        return det;
    }
    string get_Study() const{ 
        return study;
    }
};


vector<string> names = {"oleg", "adolf", "serega", "alehander", "vladimir", "maria", "ivan", "anna", "dmitry", "ekaterina"};
vector<string> professions = {"svarshik", "ingeneer", "smm", "infotsigan", "miner", "programmer", "teacher", "doctor", "driver", "manager"};
vector<string> studies = {"killer", "unemployed", "aferist", "tsigan", "president", "student", "graduate", "phd", "bachelor", "master"};


random_device rd;
mt19937 gen(rd());
uniform_int_distribution<> name_dist(0, names.size() - 1);
uniform_int_distribution<> age_dist(0, 110);
uniform_int_distribution<> prof_dist(0, professions.size() - 1);
uniform_int_distribution<> study_dist(0, studies.size() - 1);

void printVector(const vector<Sotrudnic>& vect){
    int i = 1;
    for(const auto& sotrudnik : vect){
        cout << i++ << ") " << sotrudnik.get_Dets() << endl;
    }
}

int main(){
    int n = 0;
    cout << "enter count of class" << endl;
    cin >> n;

    vector<Sotrudnic> Sotrudnics;
    Sotrudnics.reserve(n); 

    for (int i = 0; i < n; i++) {
        string random_name = names[name_dist(gen)];
        int random_age = age_dist(gen);
        string random_profession = professions[prof_dist(gen)];
        string random_study = studies[study_dist(gen)];
        
        Sotrudnics.emplace_back(random_name, random_age, random_profession, random_study);
    }

    cout << "Исходный вектор:" << endl;
    printVector(Sotrudnics);
    cout << endl;

    vector<Sotrudnic> Sotrudnics_second;
    copy_if(Sotrudnics.begin(), Sotrudnics.end(), back_inserter(Sotrudnics_second),
        [](const Sotrudnic& s) {
            return s.get_Age() > 18;
        });

    cout << "После фильтрации (возраст > 18):" << endl;
    printVector(Sotrudnics_second);
    cout << endl;


    sort(Sotrudnics_second.begin(), Sotrudnics_second.end(), [](const Sotrudnic& a, const Sotrudnic& b) {return a.get_Dets() < b.get_Dets();});

    cout << "После сортировки по возрасту:" << endl;
    printVector(Sotrudnics_second);

    return 0;
}
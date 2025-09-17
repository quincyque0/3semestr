#include <iostream> 
#include <string>

using namespace std;

class Sotrudnic { 
    private: string name; int age; string det; string study;

    public:
    Sotrudnic(string names , int ages ,string dets, string studys):
        name(names),
        age(ages),
        det(dets),
        study(studys)
        
    {}
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
    int get_Age() { 
    return age;
    }
    string get_Name() { 
    return name;
    }
    string get_Dets() { 
    return det;
    }
    string get_Study() { 
    return study;
    }
};
class cone{
    private: double r; double h;
    public :
    cone(double rs, double hs):
        r(rs),
        h(hs)
    {}

    double getSquare(){
        double cur = (3.14 * r * r) + (3.14 * r * h);
        return cur;
    }
    double getVolume(){
        double cur2 = ((double)1/3) * 3.14 * r * r * h;
        return cur2;
    }
};


int main(){
    Sotrudnic Oleg = Sotrudnic("oleg",18,"dancer" ,"geography" );
    Oleg.set_Age(100);
    Oleg.outPutTo();
    int age = Oleg.get_Age();
    cout << age << endl;
    cone conus(3.0,3.0);
    // double square = conus.getSquare();
    cout << conus.getSquare() << endl;
    cout << conus.getVolume() << endl;
    int colv;
    double rin; double hin;
    cout << "enter count of cones" << endl;
    cin >> colv;
    for(int _ = 0 ; _ < colv ; _++){
        cout << "enter r" << endl;
        cin >> rin;
        cout << "enter h" << endl;
        cin >> hin;
        cone conus(rin,hin);
        cout << "square: " << conus.getSquare() << endl;
        cout << "volume: " << conus.getVolume() << endl;
    }

}
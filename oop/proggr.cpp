#include <iostream>
#include <stdlib.h>
using namespace std;


typedef struct{
    int x1;
    int x2;
    int y1;
    int y2;
}stright;



void equation(stright Stright){
    float a = Stright.y2 - Stright.y1;
    float b = Stright.x1 - Stright.x2;
    float c = -a * Stright.x1 - b * Stright.y1;
    float cur1 =  1/(Stright.x1 - Stright.x2);
    float cur2 =  1/(Stright.y1 - Stright.y2);
    cout << cur1 << "x" << "-" << Stright.x2 * cur1 << "=" << cur2 << "y" << "-" << Stright.y2 * cur2 << endl;
    
    }
int main(){
    stright name;
    cin >> name.x1 >> name.x2 >> name.y1 >> name.y2;
    equation(name);
    return 0;
}
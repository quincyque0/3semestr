#include <iostream>
#include <string>
#include <cmath>
#include <algorithm> 

using namespace std;

class Point {
public:
    float x, y;
    Point(float xc, float yc) : x(xc), y(yc) {}
};

class triangle {
protected: 
    Point cord_x, cord_y, cord_z;
    float a, b, c;

    float distance(Point p1, Point p2) {
        return sqrt(pow(p1.x - p2.x, 2) + pow(p1.y - p2.y, 2));
    }

public:
    triangle(Point cx, Point cy, Point cz) :
        cord_x(cx), cord_y(cy), cord_z(cz) {
        a = distance(cord_x, cord_y);
        b = distance(cord_y, cord_z);
        c = distance(cord_z, cord_x);
    }

    float P() { return a + b + c; }

    float S() {
        float p = P() / 2;
        return sqrt(p * (p - a) * (p - b) * (p - c));
    }

    float calculateAngleA() {
        float cosAlpha = (b*b + c*c - a*a) / (2 * b * c);
        return acos(cosAlpha) * 180.0 / M_PI;
    }

    float calculateAngleB() {
        float cosBeta = (a*a + c*c - b*b) / (2 * a * c);
        return acos(cosBeta) * 180.0 / M_PI;
    }

    float calculateAngleC() {
        float cosGamma = (a*a + b*b - c*c) / (2 * a * b);
        return acos(cosGamma) * 180.0 / M_PI;
    }

    void storA() { cout << "сторона a: " << a << endl; }
    void storB() { cout << "сторона b: " << b << endl; }
    void storC() { cout << "сторона c: " << c << endl; }

    void allInf() {
        storA();
        storB();
        storC();
        cout << "периметр: " << P() << endl;
        cout << "площадь: " << S() << endl;
        cout << "угол A: " << calculateAngleA() << endl;
        cout << "угол B: " << calculateAngleB() << endl;
        cout << "угол C: " << calculateAngleC() << endl;
    }
};

class RightTriangle : public triangle {
public:
    RightTriangle(Point cx, Point cy, Point cz) : triangle(cx, cy, cz) {
        if (!isRight()) {
            throw invalid_argument("Ошибка: треугольник не является прямоугольным!");
        }
    }

    bool isRight() {
        float sides[3] = {a, b, c};
        sort(sides, sides + 3);
        return fabs(sides[2]*sides[2] - (sides[0]*sides[0] + sides[1]*sides[1])) < 1e-5;
    }

    void allInf() {
        triangle::allInf();
        cout << "Треугольник прямоугольный " << endl;
    }
};

int main() {

    RightTriangle rtri(Point(0.0, 0.0), Point(3.0, 0.0), Point(0.0, 4.0));
    rtri.allInf();


    return 0;
}

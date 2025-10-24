#include <iostream>
#include <fstream>
#include <string>
using namespace std;

class tour {
private:
    string country;
    double price;
    int sales[6];
public:
    tour() {
        country = "";
        price = 0;
        for (int i = 0; i < 6; i++) {
            sales[i] = 0;
        }
    }
    void setdata(const string& countryname, double _price, const int* toursales)
    {
        country = countryname;
        price = _price;
        for (int i = 0; i < 6; i++) {
            sales[i] = toursales[i];
        }
    }
    void print() {
        cout << "Страна: " << country;    
        cout << " | Цена: " << price;
        cout << " | Продажи: ";
        for (int i = 0; i < 6; i++) {
            cout << sales[i] << " ";
        }
        cout << "\n";
    }
    static void convertToBinary(const std::string& textFilename, const std::string& binaryFilename) {
        std::ifstream inFile(textFilename);
        std::ofstream outFile(binaryFilename, std::ios::binary);

        int recordCount;
        inFile >> recordCount;
        inFile.ignore();

        outFile.write(reinterpret_cast<char*>(&recordCount), sizeof(recordCount));

        for (int i = 0; i < recordCount; i++) {
            std::string country;
            double price;
            int sales[6];

            // Чтение данных из текстового файла
            std::getline(inFile, country);
            inFile >> price;
            for (int j = 0; j < 6; j++) {
                inFile >> sales[j];
            }
            inFile.ignore();

            // Запись данных в бинарный файл

            // Запись страны (строка с указанием длины)
            int len = country.length();
            outFile.write(reinterpret_cast<char*>(&len), sizeof(len));
            outFile.write(country.c_str(), len);

            // Запись цены
            outFile.write(reinterpret_cast<char*>(&price), sizeof(price));

            // Запись массива продаж
            for (int j = 0; j < 6; j++) {
                outFile.write(reinterpret_cast<char*>(&sales[j]), sizeof(sales[j]));
            }
        }

        inFile.close();
        outFile.close();
    }
    
    static tour* TakeFromFile(const string& filename) {
        ifstream file(filename);

        int recordCount;
        file >> recordCount;
        file.ignore();
        tour* massive = new tour[recordCount];

        for (int i = 0; i < recordCount; i++) {
            string country;
            double price;
            int sales[6];
            getline(file, country);
            file >> price;
            for(int j=0;j<6;j++){
                file >> sales[j];
            }
            file.ignore();
            int* sales2;
            sales2 = sales;
            massive[i] = tour();
            massive[i].setdata(country, price, sales2);
        }

        file.close();
        return massive;
    }
};
int main() {
    setlocale(LC_ALL, "RU");
    string filename = "data.txt";
    string binaryFilename = "data.bin";
    tour::convertToBinary(filename, binaryFilename);
    tour* massive = tour::TakeFromFile(filename);
    for (int i = 0; i < 3; i++) {
        massive[i].print();
    }
};
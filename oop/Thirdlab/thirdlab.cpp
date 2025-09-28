#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <iomanip>
#include <sstream>

class teaShop{
private:
    std::string type;
    std::string form;
    std::string creator;
    float price;
    int amount;
    float total;
    
public: 
    teaShop(std::string _typee,std::string _formm, std::string _creatorr, float _pricee, int _amountt):
        type(_typee), form(_formm), creator(_creatorr), price(_pricee), amount(_amountt) {
        total = _pricee * _amountt;
    }
    
    teaShop() : type(""), form(""), creator(""), price(0), amount(0), total(0) {}

    std::string getType() const { return type; }
    std::string getForm() const { return form; }
    std::string getCreator() const { return creator; }
    double getPrice() const { return price; }
    int getAmount() const { return amount; }
    double getTotal() const { return total; }

    static teaShop* TakeFromFile(const std::string& filename){
        std::ifstream file(filename);
        
        if (!file.is_open()) {
            std::cout << "Cannot open file: " << filename << std::endl;
            return nullptr;
        }
        
        int recordCount;
        file >> recordCount;
        file.ignore();
        teaShop* massive = new teaShop[recordCount];

        for (int i = 0; i < recordCount; i++) {
            std::string type, form, creator;
            float price;
            int amount;

            std::getline(file, type);
            std::getline(file, form);
            std::getline(file, creator);
            file >> price;
            file >> amount;
            file.ignore();

            massive[i] = teaShop(type, form, creator, price, amount); 
        }
        
        file.close();
        return massive;
    }

    static void convertToBinary(const std::string& textFilename, const std::string& binaryFilename) {
        std::ifstream inFile(textFilename);
       
        std::ofstream outFile(binaryFilename, std::ios::binary);
        
        int recordCount;
        inFile >> recordCount;
        inFile.ignore();
        
        outFile.write(reinterpret_cast<char*>(&recordCount), sizeof(recordCount));
        
        for (int i = 0; i < recordCount; i++) {
            std::string type, form, creator;
            float price;
            int amount;
            
   
            std::getline(inFile, type);
            std::getline(inFile, form);
            std::getline(inFile, creator);
            inFile >> price;
            inFile >> amount;
            inFile.ignore();
            
            float total = price * amount;
            

            int len = type.length();
            outFile.write(reinterpret_cast<char*>(&len), sizeof(len));
            outFile.write(type.c_str(), len);
            
            len = form.length();
            outFile.write(reinterpret_cast<char*>(&len), sizeof(len));
            outFile.write(form.c_str(), len);
            
            len = creator.length();
            outFile.write(reinterpret_cast<char*>(&len), sizeof(len));
            outFile.write(creator.c_str(), len);
            
            outFile.write(reinterpret_cast<char*>(&price), sizeof(price));
            outFile.write(reinterpret_cast<char*>(&amount), sizeof(amount));
            outFile.write(reinterpret_cast<char*>(&total), sizeof(total));
        }
        

        inFile.close();
        outFile.close();
    }
    
};
void allCheck(teaShop* massive,int size){
    float par1 = 0,par3 = 0;
    int par2 = 0; 
    for(int i = 0 ; i < size; i++){
        par1 += massive[i].getPrice();
        par2 += massive[i].getAmount();
        par3 += massive[i].getTotal();
         
    }
    std::cout << std::string(90, '-') << std::endl;
    std::cout << "Всего: " 
    << std::setw(8) << size
    << std::setw(15) << size
    << std::setw(17) << size
    << std::setw(17)<< par1 
    << std::setw(10)<< par2 
    << std::setw(15) << par3 
    << std::endl;
}


int main(){
    std::string filename = "text.txt";
    std::string binaryFilename = "text.bin";

    teaShop::convertToBinary(filename, binaryFilename);
    

    teaShop* massive = teaShop::TakeFromFile(filename);
    
    if (massive != nullptr) {
        std::cout << std::left << std::setw(15) << "Type" 
                  << std::setw(15) << "Form" 
                  << std::setw(15) << "Creator" 
                  << std::setw(15) << "Price" 
                  << std::setw(15) << "Amount" 
                  << std::setw(15) << "Total" 
                  << std::endl;
        
        std::cout << std::string(90, '-') << std::endl;
        
        for (int i = 0; i < 8; i++) {
            std::cout << std::left << std::setw(20) << massive[i].getType()
                      << std::setw(20) << massive[i].getForm()
                      << std::setw(20) << massive[i].getCreator()
                      << std::setw(17) << std::fixed << std::setprecision(2) << massive[i].getPrice()
                      << std::setw(10) << massive[i].getAmount()
                      << std::fixed << std::setprecision(2) << massive[i].getTotal()
                      << std::endl;
        }
        allCheck(massive,8);
        
    }
    
    delete[] massive; 
    return 0;
}
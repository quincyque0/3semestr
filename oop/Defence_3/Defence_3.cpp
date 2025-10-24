    #include <iostream> 
    #include <string>
    #include <fstream>
    #include <variant>
    #include <vector>


    class cinema{
        private:
            std::string phone;
            std::string address;
            std::string time;
            int num_halls;
        public:
            cinema() : phone(""),address(""), time(""), num_halls(0) {}
            cinema(std::string _phone, std::string _address, std::string _time, int _num_halls) :
                phone(_phone),
                address(_address),
                time(_time),
                num_halls(_num_halls) {} 

            void set_Phone(std::string  _phone) { phone = _phone; }
            void set_Address(std::string _address) {address = _address;}
            void set_Time(std::string _time) {time = _time;}
            void set_Num_halls(int _num_halls) { num_halls = _num_halls;}
            std::string get_Phone() {return phone;}
            std::string get_Address() {return address;}
            std::string get_Time() {return time;}
            int get_Num_halls() {return num_halls;}

            void print_All(){
                std::cout << phone << std::endl << address << std::endl << time << std::endl << num_halls << std::endl << std::endl;
            }

    };
    class hall : public cinema{
        private:
            int capacity;
        public: 
            hall() : cinema(),capacity(0){}
            hall(int _capacity,std::string _phone, std::string _address, std::string _time, int _num_halls):
                cinema(_phone,_address,_time,_num_halls),
                capacity(_capacity)
            {}
            void set_Capacity(int _capacity) { capacity = _capacity;}
            int get_Capacity() {return capacity;}
    };

    #define list struct spisok
    list{   
        hall hall_data;
        list *next;
    };

    struct spisok2{   
        cinema cinema_data;
        spisok2 *next;
    };

    void print(int n, hall halls[]){
        for(int i = 0 ; i < n ;i++){
            halls[i].print_All();
        }
    }

void printList(list *head){
    list *current = head;
    while(current != NULL){
        current->hall_data.print_All();
        current = current->next;
    }
}
void printList_cinema(spisok2 *head){
    spisok2 *current = head;
    while(current != NULL){
        current->cinema_data.print_All();
        current = current->next;
    }
}

    void appending(list *&head, hall Hall){
    list *newNode = new list;
    newNode->hall_data = Hall;
    newNode->next = NULL;

    if (head == NULL) {
        head = newNode;
    } else {
        list *current = head;
        while(current->next != NULL){
            current = current->next;
        }
        current->next = newNode;
    }
}
    void appending_cinema(spisok2 *&head, cinema data){
    spisok2 *newNode = new spisok2;
    newNode->cinema_data = data;
    newNode->next = NULL;

    if (head == NULL) {
        head = newNode;
    } else {
        spisok2 *current = head;
        while(current->next != NULL){
            current = current->next;
        }
        current->next = newNode;
    }
}
    int main(){
        //task 3
        using DataType = std::variant<int, std::string>;
        DataType data;

        int n;
        std::cout << "enter number of classses" << std::endl;
        std::cin >> n;

        hall cinemas[n];

        std::ifstream F;
        F.open("Kino.txt",std::ios::in);

        std::ifstream F2;
        F2.open("Zal.txt",std::ios::in);

        std::string phone_buf;
        std::string address_buf;
        std::string time_buf;
        int num_buf;
        int cap_buf;

        list *zals = NULL;
        int counter = 0;

        while(!F.eof() && counter != n){
            hall tmp = hall();

            F >> phone_buf;
            F >> address_buf;
            F >> time_buf;
            F >> num_buf;
            F2 >> cap_buf;

            cinemas[counter].set_Phone(phone_buf);
            cinemas[counter].set_Address(address_buf);
            cinemas[counter].set_Time(time_buf);
            cinemas[counter].set_Num_halls(num_buf);
            cinemas[counter].set_Capacity(cap_buf);

            tmp.set_Phone(phone_buf);
            tmp.set_Address(address_buf);
            tmp.set_Time(time_buf);
            tmp.set_Num_halls(num_buf);
            tmp.set_Capacity(cap_buf);

            counter++;
            appending(zals,tmp);
        }
        std::cout << "task3" << std::endl;
        print(n, cinemas);
        std::cout << "task4" << std::endl;
        printList(zals);

        F.seekg(0);
        F2.close();

        spisok2 *cinema_list = NULL;
        while(!F.eof()){
            cinema tmp2 = cinema();
            F >> phone_buf;
            F >> address_buf;
            F >> time_buf;
            if (time_buf == "00:00-23:59"){
                F >> num_buf;

                tmp2.set_Phone(phone_buf);
                tmp2.set_Address(address_buf);
                tmp2.set_Time(time_buf);
                tmp2.set_Num_halls(num_buf);

                appending_cinema(cinema_list,tmp2);
            }
            else continue;
        }

        std::cout << "task5" << std::endl;
        printList_cinema(cinema_list);





    }
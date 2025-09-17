#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <ctype.h>
#include <iconv.h>
#include <errno.h>

typedef struct {
    char a[30];               
    unsigned short int b;      
    char c[10];                
    char d[22];                 
} record2;

void swap(record2 *a, record2 *b) {
    record2 temp = *a;  
    *a = *b;           
    *b = temp;          
}
int convert_encoding(const char *from, const char *to, char *in_str, size_t in_size, char *out_str, size_t out_size) {
    iconv_t cd = iconv_open(to, from);
    if (cd == (iconv_t)-1) {
        perror("iconv_open failed");
        return -1;
    }

    size_t in_len = in_size;
    size_t out_len = out_size - 1;
    char *in_buf = in_str;
    char *out_buf = out_str;

    iconv(cd, NULL, NULL, NULL, NULL);
    
    size_t result = iconv(cd, &in_buf, &in_len, &out_buf, &out_len);
    
    if (result == (size_t)-1) {
        printf("iconv error: %d\n", errno);
        iconv_close(cd);
        return -1;
    }

    *out_buf = '\0';
    iconv_close(cd);
    return 0;
}

int get_integer_input(const char* prompt, int res) {
    char buffer[100];
    char *endptr;
    long int value;
    
    while (1) {
        printf("%s", prompt);
        
        if (fgets(buffer, sizeof(buffer), stdin) == NULL) {
            printf("Enter error!\n");
            continue;
        }
        
        char *p = buffer;
        while (isspace((unsigned char)*p)) {
            p++;
        }
        if (*p == '\0' || *p == '\n') {
            return res; 
        }

        value = strtol(p, &endptr, 10);
        
        if (endptr == p) {
            printf("enter int value!\n");
            continue;
        }
        
        while (isspace((unsigned char)*endptr)) {
            endptr++;
        }
        
        if (*endptr != '\0') {
            printf("wrong enter\n");
            continue;
        }
        if (value < 0 || value > 4000) {
            printf("value can be 0 < x < 4001!\n");
            continue;
        }
        
        return (int)value;
    }
}

void checkRecords(int filesize, int numberRecords, record2 *records) {
    int number = get_integer_input("Enter records per page (20 by default): ", 20);

    int totalPages = (numberRecords + number - 1) / number;
    printf("Total pages: %d (records: %d)\n", totalPages, numberRecords);

    while (1) {
        int page = get_integer_input("Enter page number (0 to exit): ", 0);
        if (page <= 0) break;  
        if (page > totalPages) {
            printf("Page out of range (1..%d)\n", totalPages);
            continue;
        }

        int start = (page - 1) * number;
        int end = start + number;
        if (end > numberRecords) end = numberRecords;

        printf("Showing page %d (records %d - %d):\n", page, start+1, end);

        for (int i = start; i < end; i++) {
            records[i].a[sizeof(records[i].a)-1] = '\0';
            records[i].c[sizeof(records[i].c)-1] = '\0';
            records[i].d[sizeof(records[i].d)-1] = '\0';
            
            char a_utf8[sizeof(records[i].a) * 2] = {0};
            char c_utf8[sizeof(records[i].c) * 2] = {0};
            char d_utf8[sizeof(records[i].d) * 2] = {0};
            
            if (convert_encoding("CP866", "UTF-8", records[i].a, sizeof(records[i].a), a_utf8, sizeof(a_utf8)) == 0 &&
                convert_encoding("CP866", "UTF-8", records[i].c, sizeof(records[i].c), c_utf8, sizeof(c_utf8)) == 0 &&
                convert_encoding("CP866", "UTF-8", records[i].d, sizeof(records[i].d), d_utf8, sizeof(d_utf8)) == 0) {
                
                printf("%s\t%hu\t%s\t%s\n", a_utf8, records[i].b, c_utf8, d_utf8);
            } else {
                printf("Record %d (conversion failed)\n", i+1);
            }
        }
    }
}

void get_first_three_letters(const char *fio, char *key) {
    int space_count = 0;
    int idx = 0;
    

    while (fio[idx] == ' ') idx++;
    

    for (int i = 0; i < 3 && fio[idx] != '\0' && fio[idx] != ' '; i++) {
        key[i] = fio[idx++];
    }
    key[3] = '\0';
}


int compare_records(const record2 *a, const record2 *b) {
    char key_a[4], key_b[4];
    

    get_first_three_letters(a->d, key_a);
    get_first_three_letters(b->d, key_b);
    

    int key_compare = strcmp(key_a, key_b);
    if (key_compare != 0) {
        return key_compare;
    }
    
    return a->b - b->b;
}


void quickSort(record2 *records, int L, int R) {
    if (L >= R) return;
    
    int i = L, j = R;
    record2 pivot = records[(L + R) / 2];
    
    do {

        while (compare_records(&records[i], &pivot) < 0) i++;
        
        while (compare_records(&records[j], &pivot) > 0) j--;
        
        if (i <= j) {
            swap(&records[i], &records[j]);
            i++;
            j--;
        }
    } while (i <= j);
    

    if (L < j) quickSort(records, L, j);
    if (i < R) quickSort(records, i, R);
}


int main() {
    FILE *fp = fopen("testBase3.dat", "rb");
    if (!fp) {
        perror("File opening error");
        return 1;
    }

    fseek(fp, 0, SEEK_END);
    long filesize = ftell(fp);
    fseek(fp, 0, SEEK_SET);

    size_t record_count = filesize / sizeof(record2);
    printf("File size: %ld bytes\nNumber of records: %zu\n", filesize, record_count);

    record2 *records = (record2*) malloc(sizeof(record2) * record_count);
    if (!records) {
        perror("Memory allocation error");
        fclose(fp);
        return 1;
    }

    size_t readed = fread(records, sizeof(record2), record_count, fp);
    fclose(fp);

    printf("Records read: %zu\n", readed);


    checkRecords(filesize, record_count, records);
    
    if (record_count > 0) {
        quickSort(records, 0, record_count - 1);
        printf("Sorting completed.\n");
    }
   

    checkRecords(filesize, record_count, records);

    free(records);
    return 0;
}

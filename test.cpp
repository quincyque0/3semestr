#include <iostream> 
#include <string>
#include <stdio.h>
#include <algorithm>
#include <vector>




using namespace std;

void hashing(int mas[],int size){
    int i=0;
    while(size != 1){
        
        mas[1+i] -= mas[0+i];
        
    }
    cout << mas[size] << endl;
}
int main(){
    int n,k,l,r,nm;
    int arr[n]; 
    cin >> n;

    vector<int> nums(n);
    for (int i = 0; i < n; i++)
    {
        cin >> arr[i];
    }

    cin >> k;

    
    vector<vector<int> > threes(k, vector<int>(3));
    for (int i = 0; i < k; i++)
    {
        cin >> threes[i][0] >> threes[i][1] >> threes[i][2];
        for(int j = threes[i][0];  j <= threes[i][1];j++){
            arr[j] += threes[i][2];
            hashing(arr,n);
        }
    }
}


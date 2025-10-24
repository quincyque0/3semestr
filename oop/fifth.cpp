#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <stack>
#include <sstream>

void raspr(std::stack<int> sta, std::stack<int> &stachet, std::stack<int> &stanechet)
{
    std::stack<int> tmp_stachet;
    std::stack<int> tmp_stanechet;

    while (!sta.empty())
    {
        if (sta.top() % 2 == 0)
        {
            tmp_stachet.push(sta.top());
        }
        else
        {
            tmp_stanechet.push(sta.top());
        }
        sta.pop();
    }

    while (!tmp_stachet.empty())
    {
        stachet.push(tmp_stachet.top());
        tmp_stachet.pop();
    }

    while (!tmp_stanechet.empty())
    {
        stanechet.push(tmp_stanechet.top());
        tmp_stanechet.pop();
    }
}
void printer(std::stack<int> sta)
{
    while (!sta.empty())
    {
        std::cout << sta.top() << " ";
        sta.pop();
    }
    std::cout << std::endl;
}
int main()
{
    std::stack<int> stack;
    int n;
    int tmp;
    std::cout << "enter n" << std::endl;
    std::cin >> n;
    for (int i = 0; i < n; i++)
    {
        std::cin >> tmp;
        stack.push(tmp);
    }

    std::stack<int> chet;
    std::stack<int> nechet;
    raspr(stack, chet, nechet);


    std::cout << "stack" << std::endl;
    printer(stack);

    std::cout << "chet" << std::endl;
    printer(chet);

    std::cout << "nechet" << std::endl;
    printer(nechet);
}
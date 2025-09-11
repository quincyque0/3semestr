using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

unsafe class Program
{
    struct Vertex
    {
        public int Data;
        public Vertex* Left;
        public Vertex* Right;
    }
    
    Vertex* Root;
    int[] Datas = { 4, 2, 14, 19, 8, 6 };

    void TreeBuilder(int[] datas)
    {
        foreach (int data in datas)
        {
            fixed (Vertex** rootPtr = &Root)
            {
                Insert(rootPtr, data);
            }
        }
    }
    
    void Insert(Vertex** root, int data)
    {
        if (*root == null)
        {
            *root = (Vertex*)NativeMemory.Alloc((nuint)sizeof(Vertex));
            (*root)->Data = data;
            (*root)->Left = null;
            (*root)->Right = null;
            return;
        }
        
        if (data < (*root)->Data)
        {
            Insert(&(*root)->Left, data);
        }
        else
        {
            Insert(&(*root)->Right, data);
        }
    }
    
    void TopDown(Vertex* root)
    {
        if (root != null)
        {
            TopDown(root->Left);
            Console.Write(root->Data + " ");
            TopDown(root->Right);
        }
    }
    

    void LeftRightBFS(Vertex* root)
    {
        if (root == null) return;

        var queue = new System.Collections.Queue();
        queue.Enqueue((IntPtr)root);
        
        Console.WriteLine("Left-right (BFS) traversal:");
        while (queue.Count > 0)
        {
            Vertex* current = (Vertex*)((IntPtr)queue.Dequeue());
            Console.Write(current->Data + " ");
            
            if (current->Left != null)
                queue.Enqueue((IntPtr)current->Left);
            if (current->Right != null)
                queue.Enqueue((IntPtr)current->Right);
        }
        Console.WriteLine();
    }

    void BottomUpBFS(Vertex* root)
    {
        if (root == null) return;
        
        var queue = new System.Collections.Queue();
        var stack = new Stack<int>();
        queue.Enqueue((IntPtr)root);
        
        while (queue.Count > 0)
        {
            Vertex* current = (Vertex*)((IntPtr)queue.Dequeue());
            stack.Push(current->Data);
            
            if (current->Right != null)
                queue.Enqueue((IntPtr)current->Right);
            if (current->Left != null)
                queue.Enqueue((IntPtr)current->Left);
        }
        
        Console.WriteLine("Bottom-up (reverse levels) traversal:");
        while (stack.Count > 0)
        {
            Console.Write(stack.Pop() + " ");
        }
        Console.WriteLine();
    }

    // Проход СНИЗУ ВВЕРХ (рекурсивный, обратный in-order)
    void BottomUpDFS(Vertex* root)
    {
        if (root != null)
        {
            BottomUpDFS(root->Right);
            Console.Write(root->Data + " ");
            BottomUpDFS(root->Left);
        }
    }

    // Проход СНИЗУ ВВЕРХ (полностью обратный - post-order)
    void CompleteBottomUp(Vertex* root)
    {
        if (root != null)
        {
            CompleteBottomUp(root->Left);
            CompleteBottomUp(root->Right);
            Console.Write(root->Data + " ");
        }
    }
    
    void FreeTree(Vertex* root)
    {
        if (root != null)
        {
            FreeTree(root->Left);
            FreeTree(root->Right);
            NativeMemory.Free(root);
        }
    }
    
    int GetSize(Vertex* root)
    {
        if (root == null)
            return 0;
        return 1 + GetSize(root->Left) + GetSize(root->Right);
    }

    // Контрольная сумма (сумма всех значений)
    int GetCheckSum(Vertex* root)
    {
        if (root == null)
            return 0;
        return root->Data + GetCheckSum(root->Left) + GetCheckSum(root->Right);
    }

    // Высота дерева (максимальная глубина)
    int GetHeight(Vertex* root)
    {
        if (root == null)
            return 0;
        return 1 + Math.Max(GetHeight(root->Left), GetHeight(root->Right));
    }

    // Сумма высот всех узлов (для средней высоты)
    int GetTotalHeight(Vertex* root, int depth = 1)
    {
        if (root == null)
            return 0;
        return depth + GetTotalHeight(root->Left, depth + 1) + GetTotalHeight(root->Right, depth + 1);
    }

    // Средняя высота узлов
    double GetAverageHeight(Vertex* root)
    {
        int size = GetSize(root);
        if (size == 0)
            return 0;
        return (double)GetTotalHeight(root) / size;
    }

    void PrintTreeInfo(Vertex* root)
    {
        int size = GetSize(root);
        int checkSum = GetCheckSum(root);
        int height = GetHeight(root);
        double averageHeight = GetAverageHeight(root);

        Console.WriteLine("\n=== Характеристики дерева ===");
        Console.WriteLine($"Размер: {size}");
        Console.WriteLine($"Контрольная сумма: {checkSum}");
        Console.WriteLine($"Высота: {height}");
        Console.WriteLine($"Средняя высота: {averageHeight:F2}");
        Console.WriteLine("============================\n");
    }
    
    static void Main(string[] args)
    {
        Program program = new Program();
        program.TreeBuilder(program.Datas);
        
        Console.WriteLine("Top-down (in-order) traversal:");
        program.TopDown(program.Root);
        Console.WriteLine("\n");
        
        // Проход слева направо по уровням
        program.LeftRightBFS(program.Root);
        
        // Проход снизу вверх (обратные уровни)
        program.BottomUpBFS(program.Root);
        
        // Проход снизу вверх (обратный in-order)
        Console.WriteLine("Bottom-up (reverse in-order):");
        program.BottomUpDFS(program.Root);
        Console.WriteLine("\n");
        
        // Полностью обратный порядок (снизу вверх)
        Console.WriteLine("Complete bottom-up (post-order):");
        program.CompleteBottomUp(program.Root);
        Console.WriteLine();
        
        // Вывод характеристик дерева
        program.PrintTreeInfo(program.Root);
        
        // Освобождаем память
        program.FreeTree(program.Root);
    }
}
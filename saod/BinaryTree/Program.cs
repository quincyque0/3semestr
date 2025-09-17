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
    Vertex* CreateNode(int data){
        Vertex* root = (Vertex*)NativeMemory.Alloc((nuint)sizeof(Vertex));
        (root)->Data = data;
        (root)->Left = null;
        (root)->Right = null;
        return root ;
    }
    void CreateStaticTree(){
        Root = CreateNode(1);
        Root->Left = CreateNode(2);
        Root->Right = CreateNode(3);
        Root->Right->Left = CreateNode(4);
        Root->Right->Right = CreateNode(5);
        Root->Right->Left->Left = CreateNode(6);
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
    
    void Td(Vertex* root)
    {
        if (root != null)
        {
            Console.Write(root->Data + " ");
            Td(root->Left);
            Td(root->Right);
        }
    }
    void Lr(Vertex* root){
        if(root != null){
            
            Lr(root->Left);
            Console.Write(root->Data + " ");
            Lr(root->Right);
        }
    }
     void Dt(Vertex* root){
        if(root != null){
            
            Dt(root->Left);
            Dt(root->Right);
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

    int GetCheckSum(Vertex* root)
    {
        if (root == null)
            return 0;
        return root->Data + GetCheckSum(root->Left) + GetCheckSum(root->Right);
    }

    int GetHeight(Vertex* root)
    {
        if (root == null)
            return 0;
        return 1 + Math.Max(GetHeight(root->Left), GetHeight(root->Right));
    }

    int GetTotalHeight(Vertex* root, int depth = 1)
    {
        if (root == null)
            return 0;
        return depth + GetTotalHeight(root->Left, depth + 1) + GetTotalHeight(root->Right, depth + 1);
    }

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
        program.CreateStaticTree();
        
        Console.WriteLine("Top-down");
        program.Td(program.Root);
        Console.WriteLine("\n");

        

        Console.WriteLine("Bottom-up");
        program.Dt(program.Root);
        Console.WriteLine("\n");
        


        Console.WriteLine("Left-right");
        program.Lr(program.Root);
        Console.WriteLine();
        
        program.PrintTreeInfo(program.Root);
        
        program.FreeTree(program.Root);
    }
}
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;

public unsafe struct AVLVertex
{
    public int data;
    public int bal;
    public AVLVertex* left;
    public AVLVertex* right;
}

public static unsafe class AVLTree
{
    private static bool _rost;
    private static bool _delete;
    public static AVLVertex* root;

    public static int InsertCount { get; private set; }
    public static int DeleteCount { get; private set; }
    public static int RotationCount { get; private set; }

    static AVLTree()
    {
        InsertCount = 0;
        DeleteCount = 0;
        RotationCount = 0;
        root = null;
    }

    public static void Add(int data)
    {
        _rost = false;
        AVLVertex* rootPtr = root;
        AddAVL(data, &rootPtr);
        root = rootPtr;
        InsertCount++;
    }

    private static void AddAVL(int d, AVLVertex** p)
    {
        if (*p == null)
        {
            *p = AllocateMemory();
            (*p)->data = d;
            (*p)->left = null;
            (*p)->right = null;
            (*p)->bal = 0;
            _rost = true;
        }
        else if ((*p)->data > d)
        {
            AVLVertex* leftPtr = (*p)->left;
            AddAVL(d, &leftPtr);
            (*p)->left = leftPtr;

            if (_rost)
            {
                if ((*p)->bal > 0)
                {
                    (*p)->bal = 0;
                    _rost = false;
                }
                else if ((*p)->bal == 0)
                {
                    (*p)->bal = -1;
                }
                else
                {
                    if ((*p)->left != null && (*p)->left->bal < 0)
                    {
                        *p = LL1(*p);
                        RotationCount++;
                        _rost = false;
                    }
                    else if ((*p)->left != null)
                    {
                        *p = LR1(*p);
                        RotationCount++;
                        _rost = false;
                    }
                }
            }
        }
        else if ((*p)->data < d)
        {
            AVLVertex* rightPtr = (*p)->right;
            AddAVL(d, &rightPtr);
            (*p)->right = rightPtr;

            if (_rost)
            {
                if ((*p)->bal < 0)
                {
                    (*p)->bal = 0;
                    _rost = false;
                }
                else if ((*p)->bal == 0)
                {
                    (*p)->bal = 1;
                }
                else
                {
                    if ((*p)->right != null && (*p)->right->bal > 0)
                    {
                        *p = RR1(*p);
                        RotationCount++;
                        _rost = false;
                    }
                    else if ((*p)->right != null)
                    {
                        *p = RL1(*p);
                        RotationCount++;
                        _rost = false;
                    }
                }
            }
        }
    }

    public static void Delete(int data)
    {
        _delete = false;
        AVLVertex* rootPtr = root;
        DeleteAVL(data, &rootPtr);
        root = rootPtr;
        if (_delete) DeleteCount++;
        DisplayTree($"After deleting {data}");
    }

    private static void DeleteAVL(int d, AVLVertex** p)
    {
        if (*p == null)
            return;

        if (d < (*p)->data)
        {
            AVLVertex* leftPtr = (*p)->left;
            DeleteAVL(d, &leftPtr);
            (*p)->left = leftPtr;
            if (_delete) BR(p);
        }
        else if (d > (*p)->data)
        {
            AVLVertex* rightPtr = (*p)->right;
            DeleteAVL(d, &rightPtr);
            (*p)->right = rightPtr;
            if (_delete) BL(p);
        }
        else
        {
            AVLVertex* q = *p;

            if (q->right == null)
            {
                *p = q->left;
                _delete = true;
                Marshal.FreeHGlobal((IntPtr)q);
            }
            else if (q->left == null)
            {
                *p = q->right;
                _delete = true;
                Marshal.FreeHGlobal((IntPtr)q);
            }
            else
            {
                AVLVertex** s = &q->left;
                while ((*s)->right != null)
                {
                    s = &(*s)->right;
                }

                AVLVertex* maxNode = *s;
                q->data = maxNode->data;
                *s = maxNode->left;
                Marshal.FreeHGlobal((IntPtr)maxNode);
                _delete = true;

                if (_delete)
                {
                    BR(p);
                }
            }
        }
    }

    private static void BL(AVLVertex** p)
    {
        if (*p == null) return;

        if ((*p)->bal == -1)
        {
            (*p)->bal = 0;
        }
        else if ((*p)->bal == 0)
        {
            (*p)->bal = 1;
            _delete = false;
        }
        else
        {
            AVLVertex* q = (*p)->right;
            if (q != null)
            {
                int b = q->bal;

                if (b >= 0)
                {
                    RRDelete(p);
                    if (b == 0) _delete = false;
                }
                else
                {
                    RLDelete(p);
                }
            }
        }
    }

    private static void BR(AVLVertex** p)
    {
        if (*p == null) return;

        if ((*p)->bal == 1)
        {
            (*p)->bal = 0;
        }
        else if ((*p)->bal == 0)
        {
            (*p)->bal = -1;
            _delete = false;
        }
        else
        {
            AVLVertex* q = (*p)->left;
            if (q != null)
            {
                int b = q->bal;

                if (b <= 0)
                {
                    LLDelete(p);
                    if (b == 0) _delete = false;
                }
                else
                {
                    LRDelete(p);
                }
            }
        }
    }

    private static void LLDelete(AVLVertex** p)
    {
        AVLVertex* q = (*p)->left;
        (*p)->left = q->right;
        q->right = *p;

        if (q->bal == 0)
        {
            (*p)->bal = -1;
            q->bal = 1;
            _delete = false;
        }
        else
        {
            (*p)->bal = 0;
            q->bal = 0;
        }

        *p = q;
        RotationCount++;
    }

    private static void RRDelete(AVLVertex** p)
    {
        AVLVertex* q = (*p)->right;
        (*p)->right = q->left;
        q->left = *p;

        if (q->bal == 0)
        {
            (*p)->bal = 1;
            q->bal = -1;
            _delete = false;
        }
        else
        {
            (*p)->bal = 0;
            q->bal = 0;
        }

        *p = q;
        RotationCount++;
    }

    private static void LRDelete(AVLVertex** p)
    {
        AVLVertex* q = (*p)->left;
        AVLVertex* r = q->right;

        q->right = r->left;
        r->left = q;
        (*p)->left = r->right;
        r->right = *p;

        if (r->bal < 0)
            (*p)->bal = 1;
        else
            (*p)->bal = 0;

        if (r->bal > 0)
            q->bal = -1;
        else
            q->bal = 0;

        r->bal = 0;
        *p = r;
        RotationCount++;
    }

    private static void RLDelete(AVLVertex** p)
    {
        AVLVertex* q = (*p)->right;
        AVLVertex* r = q->left;

        q->left = r->right;
        r->right = q;
        (*p)->right = r->left;
        r->left = *p;

        if (r->bal > 0)
            (*p)->bal = -1;
        else
            (*p)->bal = 0;

        if (r->bal < 0)
            q->bal = 1;
        else
            q->bal = 0;

        r->bal = 0;
        *p = r;
        RotationCount++;
    }

    private static AVLVertex* LL1(AVLVertex* p)
    {
        AVLVertex* q = p->left;
        p->left = q->right;
        q->right = p;
        p->bal = 0;
        q->bal = 0;
        return q;
    }

    private static AVLVertex* RR1(AVLVertex* p)
    {
        AVLVertex* q = p->right;
        p->right = q->left;
        q->left = p;
        p->bal = 0;
        q->bal = 0;
        return q;
    }

    private static AVLVertex* LR1(AVLVertex* p)
    {
        AVLVertex* q = p->left;
        AVLVertex* r = q->right;

        if (r->bal < 0)
            p->bal = 1;
        else
            p->bal = 0;

        if (r->bal > 0)
            q->bal = -1;
        else
            q->bal = 0;

        q->right = r->left;
        p->left = r->right;
        r->left = q;
        r->right = p;
        r->bal = 0;
        return r;
    }

    private static AVLVertex* RL1(AVLVertex* p)
    {
        AVLVertex* q = p->right;
        AVLVertex* r = q->left;

        if (r->bal > 0)
            p->bal = -1;
        else
            p->bal = 0;

        if (r->bal < 0)
            q->bal = 1;
        else
            q->bal = 0;

        q->left = r->right;
        p->right = r->left;
        r->right = q;
        r->left = p;
        r->bal = 0;
        return r;
    }

    public static bool Contains(int data)
    {
        return Contains(root, data);
    }

    private static bool Contains(AVLVertex* p, int data)
    {
        if (p == null) return false;
        if (data == p->data) return true;
        if (data < p->data) return Contains(p->left, data);
        return Contains(p->right, data);
    }

    public static void PrintStatistics()
    {
        Console.WriteLine("\nСТАТИСТИКА");
        Console.WriteLine($"Добавлений: {InsertCount}");
        Console.WriteLine($"Удалений: {DeleteCount}");
        Console.WriteLine($"Поворотов: {RotationCount}");
        
        if (InsertCount > 0)
        {
            double insertRotationRatio = (double)RotationCount / InsertCount;
            Console.WriteLine($"Соотношение поворотов/вставок: {insertRotationRatio:F4}");
        }
        
        if (DeleteCount > 0)
        {
            double deleteRotationRatio = (double)RotationCount / DeleteCount;
            Console.WriteLine($"Соотношение поворотов/удалений: {deleteRotationRatio:F4}");
        }
    }

    private static AVLVertex* AllocateMemory()
    {
        return (AVLVertex*)Marshal.AllocHGlobal(sizeof(AVLVertex));
    }

    public static void FreeMemory()
    {
        FreeMemory(root);
        root = null;
    }

    private static void FreeMemory(AVLVertex* p)
    {
        if (p != null)
        {
            FreeMemory(p->left);
            FreeMemory(p->right);
            Marshal.FreeHGlobal((IntPtr)p);
        }
    }

    public static void PrintInOrder()
    {
        Console.Write("In-order: ");
        PrintInOrder(root);
        Console.WriteLine();
    }

    private static void PrintInOrder(AVLVertex* p)
    {
        if (p != null)
        {
            PrintInOrder(p->left);
            Console.Write($"{p->data} ");
            PrintInOrder(p->right);
        }
    }

    public static void DisplayTree(string title = "")
    {
        string filename = $"avl_tree_{DateTime.Now:HHmmssfff}.dot";
        GenerateDotFile(filename, title);

        try
        {
            string pngFilename = filename.Replace(".dot", ".png");
            using (Process dotProcess = Process.Start("dot", $"-Tpng -Gdpi=150 {filename} -o {pngFilename}"))
            {
                dotProcess.WaitForExit();
                if (dotProcess.ExitCode == 0 && File.Exists(pngFilename))
                {
                    Console.WriteLine($"Файл сохранён: {pngFilename}");
                }
            }
        }
        catch
        {
            Console.WriteLine("Graphviz не установлен.");
        }
    }

    private static void GenerateDotFile(string filename, string title = "")
    {
        using (StreamWriter file = new StreamWriter(filename))
        {
            file.WriteLine("digraph G {");
            file.WriteLine("  node [shape=circle, style=filled, fontcolor=black];");
            file.WriteLine("  edge [color=black, arrowhead=vee];");

            if (!string.IsNullOrEmpty(title))
                file.WriteLine($"  label=\"{title}\";");
            file.WriteLine("  labelloc=\"t\";");
            file.WriteLine("  fontsize=16;");

            if (root != null)
            {
                WriteDotRecursive(file, root);
            }

            file.WriteLine("}");
        }
    }

    private static void WriteDotRecursive(StreamWriter writer, AVLVertex* root)
    {
        if (root == null) return;

        string nodeColor = "white";
        if (root->bal < 0) nodeColor = "lightblue";
        else if (root->bal > 0) nodeColor = "lightcoral";
        else nodeColor = "lightgreen";

        writer.WriteLine($"  node{root->data} [label=\"{root->data}\\nbal={root->bal}\", fillcolor={nodeColor}];");

        if (root->left != null)
        {
            writer.WriteLine($"  node{root->data} -> node{root->left->data} [label=\"L\"];");
            WriteDotRecursive(writer, root->left);
        }

        if (root->right != null)
        {
            writer.WriteLine($"  node{root->data} -> node{root->right->data} [label=\"R\"];");
            WriteDotRecursive(writer, root->right);
        }
    }
}

class Program
{
    static unsafe void Main()
    {
        List<int> numbers = new List<int>();
        for (int i = 1; i <= 100; i++)
        {
            numbers.Add(i);
        }
        
        Random rand = new Random();
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            int temp = numbers[i];
            numbers[i] = numbers[j];
            numbers[j] = temp;
        }

        Console.WriteLine("Добавление 100 элементов в АВЛ-дерево...");
        
        foreach (int number in numbers)
        {
            AVLTree.Add(number);
        }

        Console.WriteLine("Дерево построено из 100 элементов:");
        AVLTree.PrintInOrder();
        Console.WriteLine($"\nВысота дерева: {GetTreeHeight(AVLTree.root)}");
        AVLTree.DisplayTree("Initial AVL Tree");

        int deleteCount = 0;
        Console.WriteLine("\nУдаление 10 вершин:");
        
        while (deleteCount < 10)
        {
            Console.Write($"\nВведите число для удаления ({deleteCount + 1}/10): ");
            if (int.TryParse(Console.ReadLine(), out int val))
            {
                if (val >= 1 && val <= 100)
                {
                    if (AVLTree.Contains(val))
                    {
                        Console.WriteLine($"Удаление элемента {val}...");
                        AVLTree.Delete(val);
                        deleteCount++;
                        
                        Console.WriteLine($"После удаления {val}:");
                        AVLTree.PrintInOrder();
                        Console.WriteLine($"Высота дерева: {GetTreeHeight(AVLTree.root)}");
                        Console.WriteLine($"Поворотов выполнено: {AVLTree.RotationCount}");
                    }
                    else
                    {
                        Console.WriteLine("Элемент не найден в дереве или уже удален");
                    }
                }
                else
                {
                    Console.WriteLine("Введите число от 1 до 100");
                }
            }
            else
            {
                Console.WriteLine("Некорректный ввод");
            }
        }

        AVLTree.PrintStatistics();
        AVLTree.FreeMemory();
    }

    private static unsafe int GetTreeHeight(AVLVertex* root)
    {
        if (root == null) return 0;
        int leftHeight = GetTreeHeight(root->left);
        int rightHeight = GetTreeHeight(root->right);
        return Math.Max(leftHeight, rightHeight) + 1;
    }
}
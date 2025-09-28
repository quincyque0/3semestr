using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

unsafe class Program
{
    struct Vertex
    {
        public int Data;
        public Vertex* Left;
        public Vertex* Right;
    }
    unsafe Vertex* ISDP(int L, int R)
    {
        if (L > R)
            return null;

        int m = L + (R - L + 1) / 2;
        Vertex* p = CreateNode(m);
        p->Data = m;
        p->Left = ISDP(L, m - 1);
        p->Right = ISDP(m + 1, R);
        return p;
    }

    Vertex* CreateNode(int data)
    {
        Vertex* root = (Vertex*)NativeMemory.Alloc((nuint)sizeof(Vertex));
        root->Data = data;
        root->Left = null;
        root->Right = null;
        return root;
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

    void Lr(Vertex* root)
    {
        if (root != null)
        {
            Lr(root->Left);
            Console.Write(root->Data + " ");
            Lr(root->Right);
        }
    }

    void Dt(Vertex* root)
    {
        if (root != null)
        {
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
        if (root == null) return 0;
        return 1 + GetSize(root->Left) + GetSize(root->Right);
    }

    int GetCheckSum(Vertex* root)
    {
        if (root == null) return 0;
        return root->Data + GetCheckSum(root->Left) + GetCheckSum(root->Right);
    }

    int GetHeight(Vertex* root)
    {
        if (root == null) return 0;
        return 1 + Math.Max(GetHeight(root->Left), GetHeight(root->Right));
    }

    int GetTotalHeight(Vertex* root, int depth = 1)
    {
        if (root == null) return 0;
        return depth + GetTotalHeight(root->Left, depth + 1) + GetTotalHeight(root->Right, depth + 1);
    }

    double GetAverageHeight(Vertex* root)
    {
        int size = GetSize(root);
        if (size == 0) return 0;
        return (double)GetTotalHeight(root) / size;
    }


    void GenerateGraphvizScript(Vertex* root, StringBuilder sb, string parentName = "")
    {
        if (root == null) return;

        string currentNodeName = $"node{root->Data}";
        sb.AppendLine($"    {currentNodeName} [label=\"{root->Data}\"];");

        if (!string.IsNullOrEmpty(parentName))
        {
            sb.AppendLine($"    {parentName} -> {currentNodeName};");
        }

        GenerateGraphvizScript(root->Left, sb, currentNodeName);
        GenerateGraphvizScript(root->Right, sb, currentNodeName);
    }

    string GetGraphvizCode(Vertex* root)
    {
        var sb = new StringBuilder();
        sb.AppendLine("digraph BinaryTree {");
        sb.AppendLine("    node [shape=circle, style=filled, fillcolor=lightgreen, fontname=\"Arial\"];");
        sb.AppendLine("    edge [arrowhead=vee, color=lightbrown];");

        GenerateGraphvizScript(root, sb);

        sb.AppendLine("}");
        return sb.ToString();
    }

    void SaveGraphvizToFile(string filename, Vertex* root)
    {
        string graphvizCode = GetGraphvizCode(root);
        System.IO.File.WriteAllText(filename, graphvizCode);
        Console.WriteLine($"Graphviz код сохранен в файл: {filename}");
    }


   void RelabelTreeBFS(Vertex* root, int[] newValues)
{
    if (root == null) return;


    Vertex*[] queue = new Vertex*[newValues.Length];
    int head = 0, tail = 0;

    queue[tail++] = root;
    int i = 0;

    while (head < tail && i < newValues.Length)
    {
        Vertex* node = queue[head++];
        node->Data = newValues[i++];
        
        if (node->Left != null) queue[tail++] = node->Left;
        if (node->Right != null) queue[tail++] = node->Right;
    }
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

    unsafe static void Main(string[] args)
    {
        Program program = new Program();
        Vertex* root = program.ISDP(1, 100); 

        int[] labels = new int[100];
        for (int i = 0; i < 100; i++)
            labels[i] = i + 1;


        program.RelabelTreeBFS(root, labels);

        program.SaveGraphvizToFile("tree.dot", root);

        Console.WriteLine("Graphviz код сгенерирован. Для визуализации выполните:");
        Console.WriteLine("dot -Tpng tree.dot -o tree.png");

        program.Lr(root);
        Console.WriteLine();
        Console.WriteLine();
        program.Td(root);
        Console.WriteLine();
        Console.WriteLine();
        program.Dt(root);
        program.PrintTreeInfo(root);

        program.FreeTree(root);
    }
}

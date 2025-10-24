using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

public unsafe class Program
{
    public struct Vertex
    {
        public int Data;
        public Vertex* Left;
        public Vertex* Right;
        public int Bal;
    }

    static Vertex* Root = null;
    static bool VR = false;
    static bool HR = false;

    private static void* AllocateMemory(int size)
    {
        var ptr = Marshal.AllocHGlobal(size);
        return (void*)ptr;
    }

    public static void B2INSERT(int D, Vertex** p)
    {
        if (*p == null)
        {
            *p = (Vertex*)AllocateMemory(sizeof(Vertex));
            (*p)->Data = D;
            (*p)->Left = null;
            (*p)->Right = null;
            (*p)->Bal = 0;
            VR = true;
        }
        else if ((*p)->Data > D)
        {
            B2INSERT(D, &(*p)->Left);

            if (VR)
            {
                if ((*p)->Bal == 0)
                {
                    Vertex* q = (*p)->Left;
                    (*p)->Left = q->Right;
                    q->Right = *p;
                    *p = q;
                    q->Bal = 1;
                    VR = false;
                    HR = true;
                }
                else
                {
                    (*p)->Bal = 0;
                    VR = true;
                    HR = false;
                }
            }
            else
            {
                HR = false;
            }
        }
        else if ((*p)->Data < D)
        {
            B2INSERT(D, &(*p)->Right);

            if (VR)
            {
                (*p)->Bal = 1;
                HR = true;
                VR = false;
            }
            else if (HR)
            {
                if ((*p)->Bal == 1)
                {
                    Vertex* q = (*p)->Right;
                    (*p)->Bal = 0;
                    q->Bal = 0;
                    (*p)->Right = q->Left;
                    q->Left = *p;
                    *p = q;
                    VR = true;
                    HR = false;
                }
                else
                {
                    HR = false;
                }
            }
        }
    }

    public static int GetSize(Vertex* root)
    {
        if (root == null)
            return 0;
        return 1 + GetSize(root->Left) + GetSize(root->Right);
    }

    public static int GetCheckSum(Vertex* root)
    {
        if (root == null)
            return 0;
        return root->Data + GetCheckSum(root->Left) + GetCheckSum(root->Right);
    }

    public static int GetHeight(Vertex* root)
    {
        if (root == null)
            return 0;
        return 1 + Math.Max(GetHeight(root->Left), GetHeight(root->Right));
    }

    public static int GetTotalHeight(Vertex* root, int depth = 1)
    {
        if (root == null)
            return 0;
        return depth + GetTotalHeight(root->Left, depth + 1) + GetTotalHeight(root->Right, depth + 1);
    }

    public static double GetAverageHeight(Vertex* root)
    {
        int size = GetSize(root);
        if (size == 0)
            return 0;
        return (double)GetTotalHeight(root) / size;
    }

    public static void Insert(int data)
    {
        VR = false;
        HR = false;
        fixed (Vertex** rootPtr = &Root)
        {
            B2INSERT(data, rootPtr);
        }
    }

    public static void PrintInOrder(Vertex* p)
    {
        if (p != null)
        {
            PrintInOrder(p->Left);
            Console.Write($"{p->Data} ");
            PrintInOrder(p->Right);
        }
    }

    public static void DisplayTree()
    {
        string filename = $"tree.dot";
        
        using (StreamWriter file = new StreamWriter(filename))
        {
            file.WriteLine("digraph G {");
            file.WriteLine("  node [shape=circle];");
            file.WriteLine("  edge [arrowhead=vee];");

            if (Root != null)
            {
                WriteDotRecursive(file, Root);
            }

            file.WriteLine("}");
        }

        try
        {
            string pngFilename = filename.Replace(".dot", ".png");
            using (Process dotProcess = Process.Start("dot", $"-Tpng {filename} -o {pngFilename}"))
            {
                dotProcess.WaitForExit();
                if (dotProcess.ExitCode == 0 && File.Exists(pngFilename))
                {
                    Console.WriteLine($"File: {pngFilename}");
                }
            }
        }
        catch
        {
            Console.WriteLine("No Graphviz.");
        }
    }

    private static void WriteDotRecursive(StreamWriter writer, Vertex* root)
    {
        if (root == null) return;

        writer.WriteLine($"  {root->Data} [label=\"{root->Data}\"];");

        if (root->Left != null)
        {
            writer.WriteLine($"  {root->Data} -> {root->Left->Data};");
            WriteDotRecursive(writer, root->Left);
        }

        if (root->Right != null)
        {
            writer.WriteLine($"  {root->Data} -> {root->Right->Data};");
            WriteDotRecursive(writer, root->Right);
        }
    }

    public static int[] CreateShuffledArray()
    {
        int[] array = new int[100];
        
        for (int i = 0; i < 100; i++)
        {
            array[i] = i + 1;
        }
        
        Random random = new Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
        
        return array;
    }

    public static void FreeTree(Vertex* root)
    {
        if (root != null)
        {
            FreeTree(root->Left);
            FreeTree(root->Right);
            Marshal.FreeHGlobal((IntPtr)root);
        }
    }

    public static void PrintTree()
    {
        Console.WriteLine("Tree:");
        PrintInOrder(Root);
    }

    public static void PrintTreeStats()
    {
        Console.WriteLine($"Size: {GetSize(Root)}");
        Console.WriteLine($"Sum: {GetCheckSum(Root)}");
        Console.WriteLine($"Height: {GetHeight(Root)}");
        Console.WriteLine($"Avg height: {GetAverageHeight(Root):F2}");
    }

    public static void Main()
    {
        int[] arr = CreateShuffledArray();
        
        
        for (int i = 0; i < 100; i++)
        {
            Insert(arr[i]);
        }

        PrintTree();
        PrintTreeStats();


        DisplayTree();

        FreeTree(Root);
        Root = null;
        
    }
}
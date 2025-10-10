using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Versioning;

#pragma warning disable

public unsafe class AVLTree
{
    public struct AVLVertex
    {
        public int data;
        public int bal;
        public AVLVertex* left;
        public AVLVertex* right;
    }

    private bool _rost;
    public AVLVertex* root;

    public void Add(int data)
    {
        _rost = false;
        AVLVertex* rootPtr = root;
        AddAVL(data, &rootPtr);
        root = rootPtr;
    }

    private void AddAVL(int d, AVLVertex** p)
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
                    if ((*p)->left->bal < 0)
                    {
                        *p = LL_Rotation(*p);
                        _rost = false;
                    }
                    else
                    {
                        *p = LR_Rotation(*p);
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
                    if ((*p)->right->bal > 0)
                    {
                        *p = RR_Rotation(*p);
                        _rost = false;
                    }
                    else
                    {
                        *p = RL_Rotation(*p);
                        _rost = false;
                    }
                }
            }
        }
        else
        {

        }
    }

    private AVLVertex* LL_Rotation(AVLVertex* p)
    {
        AVLVertex* q = p->left;
        p->bal = 0;
        q->bal = 0;
        p->left = q->right;
        q->right = p;
        return q;
    }

    private AVLVertex* RR_Rotation(AVLVertex* p)
    {
        AVLVertex* q = p->right;
        p->bal = 0;
        q->bal = 0;
        p->right = q->left;
        q->left = p;
        return q;
    }

    private AVLVertex* LR_Rotation(AVLVertex* p)
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

    private AVLVertex* RL_Rotation(AVLVertex* p)
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

    private AVLVertex* AllocateMemory()
    {
        return (AVLVertex*)Marshal.AllocHGlobal(sizeof(AVLVertex));
    }

    public void FreeMemory()
    {
        FreeMemory(root);
        root = null;
    }

    private void FreeMemory(AVLVertex* p)
    {
        if (p != null)
        {
            FreeMemory(p->left);
            FreeMemory(p->right);
            Marshal.FreeHGlobal((IntPtr)p);
        }
    }


    public void PrintInOrder()
    {
        Console.Write("Обход слева направо: ");
        PrintInOrder(root);
        Console.WriteLine();
    }

    private void PrintInOrder(AVLVertex* p)
    {
        if (p != null)
        {
            PrintInOrder(p->left);
            Console.Write($"{p->data} ");
            PrintInOrder(p->right);
        }
    }


    public int CalculateTreeSize() => CalculateTreeSize(root);
    private int CalculateTreeSize(AVLVertex* root)
    {
        if (root == null) return 0;
        return 1 + CalculateTreeSize(root->left) + CalculateTreeSize(root->right);
    }

    public int CalculateChecksum() => CalculateChecksum(root);
    private int CalculateChecksum(AVLVertex* root)
    {
        if (root == null) return 0;
        return root->data + CalculateChecksum(root->left) + CalculateChecksum(root->right);
    }

    public int CalculateTreeHeight() => CalculateTreeHeight(root);
    private int CalculateTreeHeight(AVLVertex* root)
    {
        if (root == null) return 0;
        int leftHeight = CalculateTreeHeight(root->left);
        int rightHeight = CalculateTreeHeight(root->right);
        return 1 + Math.Max(leftHeight, rightHeight);
    }

    public double CalculateAverageHeight()
    {
        int totalDepth = 0;
        int nodeCount = 0;
        CalculateTotalDepth(root, 1, ref totalDepth, ref nodeCount);
        return nodeCount > 0 ? (double)totalDepth / nodeCount : 0;
    }

    private void CalculateTotalDepth(AVLVertex* root, int currentDepth, ref int totalDepth, ref int nodeCount)
    {
        if (root == null) return;
        totalDepth += currentDepth;
        nodeCount++;
        CalculateTotalDepth(root->left, currentDepth + 1, ref totalDepth, ref nodeCount);
        CalculateTotalDepth(root->right, currentDepth + 1, ref totalDepth, ref nodeCount);
    }

    public void GenerateDotFile(string filename, string title = "")
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


            file.WriteLine("    label=\"Баланс\"; style=filled; color=black;");
            file.WriteLine("    node [shape=record, width=1.5];");
            file.WriteLine("    legend [label=\"<left> bal<0 | <balanced> bal=0 | <right> bal>0\"];");
            file.WriteLine("  }");

            if (root != null)
            {
                WriteDotRecursive(file, root);
            }

            file.WriteLine("}");
        }
    }

    private void WriteDotRecursive(StreamWriter writer, AVLVertex* root)
    {
        if (root == null) return;

        string nodeColor = "white";
        if (root->bal < 0) nodeColor = "white";
        else if (root->bal > 0) nodeColor = "white";

        writer.WriteLine($"  node{root->data} [label=\"{root->data}\\nbal={root->bal}\", color={nodeColor}];");

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

    public void DisplayTree(string title = "")
    {
        string filename = "avl_tree.dot";
        GenerateDotFile(filename, title);
        
        // try
        // {
        //     using (Process dotProcess = Process.Start("dot", $"-Tpng -Gdpi=150 {filename} -o {filename}.png"))
        //     {
        //         dotProcess.WaitForExit();
        //         if (dotProcess.ExitCode == 0 && File.Exists($"{filename}.png"))
        //         {
        //             Console.WriteLine($"Графическое изображение сохранено как {filename}.png");

        //             Process.Start(new ProcessStartInfo { FileName = $"{filename}.png", UseShellExecute = true });
        //         }
        //     }
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"Ошибка визуализации: {ex.Message}");
        //     Console.WriteLine("Установите Graphviz для графического отображения");
        // }
    }
}


public unsafe class ISDPTree
{
    public struct ISDPVertex
    {
        public int data;
        public ISDPVertex* left;
        public ISDPVertex* right;
    }

    public ISDPVertex* root;

    public void BuildISDP(int[] data)
    {
        if (data == null || data.Length == 0)
        {
            root = null;
            return;
        }


        int[] sortedData = new int[data.Length];
        Array.Copy(data, sortedData, data.Length);
        Array.Sort(sortedData);
        

        FreeMemory();
        

        root = BuildBalancedTree(sortedData, 0, sortedData.Length - 1);
    }

    private ISDPVertex* BuildBalancedTree(int[] sortedData, int start, int end)
    {
        if (start > end)
            return null;


        int mid = start + (end - start) / 2;
        
        ISDPVertex* node = AllocateMemory();
        node->data = sortedData[mid];
        

        node->left = BuildBalancedTree(sortedData, start, mid - 1);
        node->right = BuildBalancedTree(sortedData, mid + 1, end);
        
        return node;
    }

    private ISDPVertex* AllocateMemory()
    {
        return (ISDPVertex*)Marshal.AllocHGlobal(sizeof(ISDPVertex));
    }

    public void FreeMemory()
    {
        FreeMemory(root);
        root = null;
    }

    private void FreeMemory(ISDPVertex* p)
    {
        if (p != null)
        {
            FreeMemory(p->left);
            FreeMemory(p->right);
            Marshal.FreeHGlobal((IntPtr)p);
        }
    }

    public void PrintInOrder()
    {
        Console.Write("Обход ИСДП слева направо: ");
        PrintInOrder(root);
        Console.WriteLine();
    }

    private void PrintInOrder(ISDPVertex* p)
    {
        if (p != null)
        {
            PrintInOrder(p->left);
            Console.Write($"{p->data} ");
            PrintInOrder(p->right);
        }
    }


    public int CalculateTreeSize() => CalculateTreeSize(root);
    private int CalculateTreeSize(ISDPVertex* root)
    {
        if (root == null) return 0;
        return 1 + CalculateTreeSize(root->left) + CalculateTreeSize(root->right);
    }

    public int CalculateChecksum() => CalculateChecksum(root);
    private int CalculateChecksum(ISDPVertex* root)
    {
        if (root == null) return 0;
        return root->data + CalculateChecksum(root->left) + CalculateChecksum(root->right);
    }

    public int CalculateTreeHeight() => CalculateTreeHeight(root);
    private int CalculateTreeHeight(ISDPVertex* root)
    {
        if (root == null) return 0;
        int leftHeight = CalculateTreeHeight(root->left);
        int rightHeight = CalculateTreeHeight(root->right);
        return 1 + Math.Max(leftHeight, rightHeight);
    }

    public double CalculateAverageHeight()
    {
        int totalDepth = 0;
        int nodeCount = 0;
        CalculateTotalDepth(root, 1, ref totalDepth, ref nodeCount);
        return nodeCount > 0 ? (double)totalDepth / nodeCount : 0;
    }

    private void CalculateTotalDepth(ISDPVertex* root, int currentDepth, ref int totalDepth, ref int nodeCount)
    {
        if (root == null) return;
        totalDepth += currentDepth;
        nodeCount++;
        CalculateTotalDepth(root->left, currentDepth + 1, ref totalDepth, ref nodeCount);
        CalculateTotalDepth(root->right, currentDepth + 1, ref totalDepth, ref nodeCount);
    }
}

[SupportedOSPlatform("windows")]
class Program
{

    [SupportedOSPlatform("windows")]
    static void Main(string[] args)
    {

        Console.WriteLine("=== Сравнение АВЛ-дерева и ИСДП ===");
        Console.WriteLine();


       Random rand = new Random();
    int[] randomData = new int[100];

    for (int i = 0; i < 100; i++)
    {
        randomData[i] = i + 1; 
    }


    for (int i = randomData.Length - 1; i > 0; i--)
    {
        int j = rand.Next(i + 1);
        int temp = randomData[i];
        randomData[i] = randomData[j];
        randomData[j] = temp;
    }


        AVLTree avlTree = new AVLTree();
        foreach (int data in randomData)
        {
            avlTree.Add(data);
        }


        avlTree.PrintInOrder();


        ISDPTree isdp = new ISDPTree();

        isdp.BuildISDP(randomData);





        int avlSize = avlTree.CalculateTreeSize();
        int avlChecksum = avlTree.CalculateChecksum();
        int avlHeight = avlTree.CalculateTreeHeight();
        double avlAvgHeight = avlTree.CalculateAverageHeight();


        int isdpSize = isdp.CalculateTreeSize();
        int isdpChecksum = isdp.CalculateChecksum();
        int isdpHeight = isdp.CalculateTreeHeight();
        double isdpAvgHeight = isdp.CalculateAverageHeight();

        Console.WriteLine("\n Сравнительная таблица характеристик");
        Console.WriteLine("+----------+--------+--------------+---------+--------------+");
        Console.WriteLine("| n=100    | Размер | Контр. Сумма | Высота  | Средн.высота |");
        Console.WriteLine("+----------+--------+--------------+---------+--------------+");
        Console.WriteLine($"| ИСДП     | {isdpSize,6} | {isdpChecksum,12} | {isdpHeight,7} | {isdpAvgHeight,12:F2} |");
        Console.WriteLine($"| АВЛ      | {avlSize,6} | {avlChecksum,12} | {avlHeight,7} | {avlAvgHeight,12:F2} |");
        Console.WriteLine("+----------+--------+--------------+---------+--------------+");

        Console.WriteLine($"Разница в высоте: {isdpHeight - avlHeight} ");
        Console.WriteLine($"Разница в средней высоте: {isdpAvgHeight - avlAvgHeight:F2}");



        avlTree.FreeMemory();
        isdp.FreeMemory();
    }
}
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;
using static RandomBinaryTreeDictFuncs;

unsafe class Program
{
    Random random = new Random();
    int[] generateRandom(int n)
{
    int[] massive = new int[n];
    for (int i = 0; i < n; i++)
    {
        massive[i] = random.Next();
    }
    return massive;
}

    
    struct Vertex
    {
        public int Data;
        public Vertex* Left;
        public Vertex* Right;
    }


    Vertex* CreateNode(int data)
    {
        Vertex* root = (Vertex*)NativeMemory.Alloc((nuint)sizeof(Vertex));
        (root)->Data = data;
        (root)->Left = null;
        (root)->Right = null;
        return root;
    }
    private Vertex* Root = null;
    private Vertex* Root2 = null;
     private Vertex* Root3 = null;

    unsafe Vertex* ISDP(int L, int R)
    {
        if (L > R)
            return null;
        else
        {
            int m = L + (R - L + 1) / 2;
            Vertex* p = CreateNode(m);
            p->Data = m;
            p->Left = ISDP(L, m - 1);
            p->Right = ISDP(m + 1, R);
            return p;
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

    void addRecursive(int D, Vertex** p)
    {
        if (*p == null)
        {
            *p = (Vertex*)NativeMemory.Alloc((nuint)sizeof(Vertex));
            (*p)->Data = D;
            (*p)->Left = null;
            (*p)->Right = null;
        }
        else if (D < (*p)->Data)
        {
            addRecursive(D, &(*p)->Left);
        }
        else if (D > (*p)->Data)
        {
            addRecursive(D, &(*p)->Right);
        }
    }

    void addKosv(int D, Vertex** root)
    {
        Vertex** p = root;
        while (*p != null)
        {
            if (D < (*p)->Data)
            {
                p = &(*p)->Left;
            }
            else if (D > (*p)->Data)
            {
                p = &(*p)->Right;
            }
            else
            {
                return;
            }
        }
        *p = (Vertex*)NativeMemory.Alloc((nuint)sizeof(Vertex));
        (*p)->Data = D;
        (*p)->Left = null;
        (*p)->Right = null;
    }

    void Found(int x)
    {
        Vertex* p = Root;
        while (p != null)
        {
            if (x < p->Data)
            {
                p = p->Left;
            }
            else if (x > p->Data)
            {
                p = p->Right;
            }
            else break;
        }
        if (p != null)
        {
            Console.WriteLine($"вершина найден по адресу {(ulong)p:X} ");
        }
        else Console.WriteLine($"вершины нету");
    }

void Delete(int D, Vertex** rootPtr)
{
    if (rootPtr == null || *rootPtr == null)
        return;

    Vertex** p = rootPtr;
    

    while (*p != null)
    {
        if (D < (*p)->Data)
            p = &(*p)->Left;
        else if (D > (*p)->Data)
            p = &(*p)->Right;
        else
            break;
    }

    if (*p == null) return; 
    Vertex* nodeToDelete = *p;

    if (nodeToDelete->Left == null)
    {
        *p = nodeToDelete->Right;
    }
    else if (nodeToDelete->Right == null)
    {
        *p = nodeToDelete->Left;
    }

    else
    {

        Vertex** minNodePtr = &nodeToDelete->Right;
        while ((*minNodePtr)->Left != null)
        {
            minNodePtr = &(*minNodePtr)->Left;
        }

        nodeToDelete->Data = (*minNodePtr)->Data;

        Vertex* temp = *minNodePtr;
        *minNodePtr = temp->Right;
        nodeToDelete = temp;
    }

    NativeMemory.Free(nodeToDelete);
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

    void addMas1(int[] massive)
    {
        fixed (Vertex** rootPtr = &Root)
        {
            for (int i = 0; i < massive.Length; i++)
            {
                addRecursive(massive[i], rootPtr);
            }
        }
    }

    void addMas2(int[] massive)
    {
        fixed (Vertex** rootPtr = &Root2)
        {
            for (int i = 0; i < massive.Length; i++)
            {
                addKosv(massive[i], rootPtr);
            }
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

    void Inorder(Vertex* root)
    {
        if (root != null)
        {
            Inorder(root->Left);
            Console.Write($"{root->Data} ");
            Inorder(root->Right);
        }
    }
    void GenerateGraphvizScript(Vertex* root, StringBuilder sb, string parentName = "")
    {
        if (root == null)
            return;

        string currentNodeName = $"node{root->Data}";
        
        sb.AppendLine($"    {currentNodeName} [label=\"{root->Data}\"];");
        
        if (!string.IsNullOrEmpty(parentName))
        {
            sb.AppendLine($"    {parentName} -> {currentNodeName};");
        }


        GenerateGraphvizScript(root->Left, sb, currentNodeName);
        GenerateGraphvizScript(root->Right, sb, currentNodeName);
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
void PrintTable(Vertex* isdp, Vertex* sdp1, Vertex* sdp2, int n)
{
    (int size, int sum, int height, double avg) GetInfo(Vertex* root)
    {
        return (GetSize(root), GetCheckSum(root), GetHeight(root), GetAverageHeight(root));
    }

    var isdpInfo = GetInfo(isdp);
    var sdp1Info = GetInfo(sdp1);
    var sdp2Info = GetInfo(sdp2);

    Console.WriteLine($"\nХарактеристики деревьев при n={n}\n");
    Console.WriteLine($"{"Дерево",-8} {"Размер",8} {"Контр. сумма",14} {"Высота",8} {"Средн.высота",14}");
    Console.WriteLine(new string('-', 55));

    Console.WriteLine($"{"ИСДП",-8} {isdpInfo.size,8} {isdpInfo.sum,14} {isdpInfo.height,8} {isdpInfo.avg,14:F2}");
    Console.WriteLine($"{"СДП1",-8} {sdp1Info.size,8} {sdp1Info.sum,14} {sdp1Info.height,8} {sdp1Info.avg,14:F2}");
    Console.WriteLine($"{"СДП2",-8} {sdp2Info.size,8} {sdp2Info.sum,14} {sdp2Info.height,8} {sdp2Info.avg,14:F2}");

 
    List<List<object>> tableData = new List<List<object>>
    {
        new List<object> { "Дерево", "Размер", "Контр. сумма", "Высота", "Средн.высота" },
        new List<object> { "ИСДП", isdpInfo.size, isdpInfo.sum, isdpInfo.height, isdpInfo.avg },
        new List<object> { "СДП1", sdp1Info.size, sdp1Info.sum, sdp1Info.height, sdp1Info.avg },
        new List<object> { "СДП2", sdp2Info.size, sdp2Info.sum, sdp2Info.height, sdp2Info.avg },
    };

    // Utils.TableImage.CreateTableImage(tableData, "TreeTable.png");
    // Console.WriteLine("Таблица сохранена в TreeTable.png");


}


    void SaveGraphvizToFile(string filename, Vertex* root)
    {
        string graphvizCode = GetGraphvizCode(root);
        System.IO.File.WriteAllText(filename, graphvizCode);
        Console.WriteLine($"Graphviz код сохранен в файл: {filename}");
    }
    void deletes(Vertex* root, int n)
    {
        for (int i = 0; i < n; i++)
        {
            Console.Write("Введите значение для удаления: ");
            string input = Console.ReadLine();
            int.TryParse(input, out int deleted);
            Delete(deleted, &root);
            SaveGraphvizToFile("treeRecursive.dot", root);
        }
    }
    int[] CreateShuffledArray()
{
    int[] array = new int[30];
    
    for (int i = 0; i < 30; i++)
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

static void Main()
    {
        Program program = new Program();
        int n = 30;
        int[] massive = program.CreateShuffledArray();


        // program.addMas1(massive);


        program.addMas2(massive);
        DisplayKeywordDictionary("/Users/a1/Desktop/GitHub/3semestr/saod/BinaryTreeRandomSSP/Program.cs");
        program.SaveGraphvizToFile("treeRecursive.dot", program.Root2);
        program.Lr(program.Root2);
        program.deletes(program.Root2, 10);
        program.Lr(program.Root2);
        




        // program.Root3 = program.ISDP(1, 100);


        // program.PrintTable(program.Root2, program.Root, program.Root2, n);

        program.FreeTree(program.Root);
        program.FreeTree(program.Root2);
    }

}
using System.Runtime.InteropServices;

#pragma warning disable

public static unsafe class RandomBinaryTreeDictFuncs
{
    public struct DictTreeNode
    {
        public string key;
        public int frequency;
        public DictTreeNode* left;
        public DictTreeNode* right;
    }

    private static readonly HashSet<string> CKeywords = new HashSet<string>
    {
        "auto", "break", "case", "char", "const", "continue", "default",
        "do", "else","double",  "enum", "extern", "float", "for", "goto",
        "if", "int", "long", "register", "return", "short", "signed",
        "sizeof", "static", "struct", "switch", "typedef", "union",
        "unsigned", "void", "volatile", "while"
    };

    public static DictTreeNode* CreateNode(string key)
    {
        DictTreeNode* newNode = (DictTreeNode*)Marshal.AllocHGlobal(sizeof(DictTreeNode));
        if (newNode == null) throw new OutOfMemoryException("Не удалось выделить память для узла");
        
        newNode->key = key ?? throw new ArgumentNullException(nameof(key));
        newNode->frequency = 1;
        newNode->left = null;
        newNode->right = null;
        return newNode;
    }

    public static void AddToTree(string key, DictTreeNode** root)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (root == null) throw new ArgumentNullException(nameof(root));

        DictTreeNode** current = root;

        while (*current != null)
        {
            int comparison = string.Compare(key, (*current)->key, StringComparison.Ordinal);

            if (comparison < 0)
                current = &(*current)->left;
            else if (comparison > 0)
                current = &(*current)->right;
            else
            {
                (*current)->frequency++;
                return;
            }
        }
        *current = CreateNode(key);
    }

    public static void FreeTree(DictTreeNode* root)
    {
        if (root == null) return;


        FreeTree(root->left);
        FreeTree(root->right);
        

        Marshal.FreeHGlobal((IntPtr)root);
    }

    private static string[] TokenizeCCode(string code)
    {
        if (string.IsNullOrEmpty(code)) 
            return Array.Empty<string>();

        char[] separators = { 
            ' ', '\t', '\n', '\r', ';', ',', '(', ')',
            '{', '}', '[', ']', '<', '>', '=', '+', '-',
            '*', '/', '&', '|', '!', '"', '\'', '?', ':',
            '.', '#', '\\', '%'
        };

        return code.Split(separators, StringSplitOptions.RemoveEmptyEntries);
    }

    public static DictTreeNode* AnalyzeCFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) 
            return null;

        try
        {
            string code = File.ReadAllText(filePath);
            string[] tokens = TokenizeCCode(code);

            DictTreeNode* root = null;

            foreach (string token in tokens)
            {
                string cleanToken = token.Trim();

                if (!string.IsNullOrEmpty(cleanToken) && CKeywords.Contains(cleanToken))
                {
                    AddToTree(cleanToken, &root);
                }
            }

            return root;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при анализе файла: {ex.Message}");
            return null;
        }
    }

    public static void PrintDictionary(DictTreeNode* root)
{
    if (root == null) return;

    PrintDictionary(root->left);
    Console.WriteLine($"║ {root->key,-18} ║   {root->frequency,8}     ║");
    PrintDictionary(root->right);
}

    public static int GetTotalKeywordsCount(DictTreeNode* root)
    {
        if (root == null) return 0;
        
        return root->frequency + GetTotalKeywordsCount(root->left) + GetTotalKeywordsCount(root->right);
    }

    public static void DisplayKeywordDictionary(string filePath)
{
    if (string.IsNullOrEmpty(filePath))
    {
        Console.WriteLine("Неверный путь к файлу.");
        return;
    }

    DictTreeNode* root = AnalyzeCFile(filePath);
    
    using (new TreeCleaner(root))
    {
        if (root == null)
        {
            Console.WriteLine("Анализ не выполнен: файл отсутствует или не содержит ключевых слов.");
            return;
        }

        int totalKeywords = GetTotalKeywordsCount(root);
        
        Console.WriteLine($"╔═════════════════════════════════════╗");
        Console.WriteLine($"║         АНАЛИЗ C-КЛЮЧЕВЫХ СЛОВ      ║");
        Console.WriteLine($"║               Файл: {Path.GetFileName(filePath),-10}      ║");
        Console.WriteLine($"╠════════════════════╦════════════════╣");
        Console.WriteLine($"║ Ключевое слово     ║    Частота     ║");
        Console.WriteLine($"╠════════════════════╬════════════════╣");
        PrintDictionary(root);
        Console.WriteLine($"╠════════════════════╬════════════════╣");
        Console.WriteLine($"║ Общее количество   ║   {totalKeywords,8}     ║");
        Console.WriteLine($"╚════════════════════╩════════════════╝");
    }

}

    private sealed class TreeCleaner : IDisposable
    {
        private DictTreeNode* _root;

        public TreeCleaner(DictTreeNode* root)
        {
            _root = root;
        }

        public void Dispose()
        {
            if (_root != null)
            {
                FreeTree(_root);
                _root = null;
            }
        }
    }
}
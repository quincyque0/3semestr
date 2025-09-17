using System;
using System.Runtime.InteropServices;
using Eto.Forms;
using Eto.Drawing;

unsafe class Program
{
    struct Vertex
    {
        public int Data;
        public Vertex* Left;
        public Vertex* Right;
    }

    Vertex* Root;

    [DllImport("libc")]
    static extern void free(void* ptr);

    [DllImport("libc")]
    static extern void* malloc(nuint size);

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

    Vertex* CreateNode(int data)
    {
        Vertex* node = (Vertex*)malloc((nuint)sizeof(Vertex));
        node->Data = data;
        node->Left = null;
        node->Right = null;
        return node;
    }

    void FreeTree(Vertex* root)
    {
        if (root != null)
        {
            FreeTree(root->Left);
            FreeTree(root->Right);
            free(root);
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

    // Класс для отрисовки дерева
    public class TreeDrawer : Drawable
    {
        private Vertex* _root;
        private const int NodeRadius = 25;
        private const int VerticalSpacing = 80;
        private const int HorizontalSpacing = 40;

        public TreeDrawer(Vertex* root)
        {
            _root = root;
            BackgroundColor = Colors.White;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            
            // Очищаем фон
            g.Clear(Colors.White);
            
            if (_root != null)
            {
                DrawTree(g, _root, Width / 2, 50, Width / 4);
            }
        }

        private void DrawTree(Graphics g, Vertex* node, float x, float y, float horizontalOffset)
        {
            if (node == null) return;

            // Рисуем узел
            DrawNode(g, node->Data.ToString(), x, y);

            // Рисуем связи с детьми
            if (node->Left != null)
            {
                float childX = x - horizontalOffset;
                float childY = y + VerticalSpacing;
                DrawLine(g, x, y + NodeRadius, childX, childY - NodeRadius);
                DrawTree(g, node->Left, childX, childY, horizontalOffset / 2);
            }

            if (node->Right != null)
            {
                float childX = x + horizontalOffset;
                float childY = y + VerticalSpacing;
                DrawLine(g, x, y + NodeRadius, childX, childY - NodeRadius);
                DrawTree(g, node->Right, childX, childY, horizontalOffset / 2);
            }
        }

        private void DrawNode(Graphics g, string text, float x, float y)
        {
            // Рисуем круг
            var circleRect = new RectangleF(x - NodeRadius, y - NodeRadius, 
                                          NodeRadius * 2, NodeRadius * 2);
            
            // Красивая градиентная заливка
            using var brush = new LinearGradientBrush(
                new PointF(x - NodeRadius, y - NodeRadius),
                new PointF(x + NodeRadius, y + NodeRadius),
                Colors.LightSkyBlue,
                Colors.DodgerBlue
            );
            
            g.FillEllipse(brush, circleRect);
            g.DrawEllipse(Colors.DarkBlue, circleRect);

            // Рисуем текст
            using var font = new Font(SystemFonts.Default.Family, 12, FontStyle.Bold);
            var textSize = g.MeasureString(font, text);
            g.DrawText(font, Colors.White, 
                      x - textSize.Width / 2, 
                      y - textSize.Height / 2, 
                      text);
        }

        private void DrawLine(Graphics g, float x1, float y1, float x2, float y2)
        {
            using var pen = new Pen(Colors.DarkGray, 2);
            g.DrawLine(pen, x1, y1, x2, y2);
        }
    }

    [STAThread]
    unsafe static void Main(string[] args)
    {
        Program program = new Program();
        
        try
        {
            Console.WriteLine("Создаем сбалансированное дерево...");
            
            // Создаем сбалансированное дерево (1-15 для хорошего отображения)
            program.Root = program.ISDP(1, 15);

            Console.WriteLine("Inorder обход дерева:");
            program.Lr(program.Root);
            Console.WriteLine("\n");

            // Создаем и запускаем форму
            var application = new Application();
            
            var form = new Form
            {
                Title = "Binary Tree Visualization - Inorder: 1 2 3 ... 15",
                Size = new Size(1200, 800),
                MinimumSize = new Size(800, 600),
                Content = new TreeDrawer(program.Root)
            };

            // Добавляем кнопку для выхода
            var quitButton = new Button { Text = "Выход" };
            quitButton.Click += (sender, e) => application.Quit();

            // Создаем layout с деревом и кнопкой
            var layout = new DynamicLayout();
            layout.Add(new TreeDrawer(program.Root), yscale: true);
            layout.Add(quitButton);
            
            form.Content = layout;

            Console.WriteLine("Запускаем графическое окно...");
            Console.WriteLine("Закройте окно или нажмите кнопку 'Выход' для завершения");

            application.Run(form);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        finally
        {
            // Освобождаем память
            if (program.Root != null)
            {
                Console.WriteLine("Освобождаем память...");
                program.FreeTree(program.Root);
            }
        }
    }
}
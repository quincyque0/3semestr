using System;
using System.Collections.Generic;
using SkiaSharp;

namespace Utils
{
    public class TableImage
    {
        public static void CreateTableImage(List<List<object>> tableData, string fileName)
        {
            int rows = tableData.Count;
            int columns = tableData[0].Count;

            int cellPadding = 10;
            int fontSize = 20;

            SKPaint textPaint = new SKPaint
            {
                Color = SKColors.Black,
                TextSize = fontSize,
                IsAntialias = true
            };

            SKPaint headerPaint = new SKPaint
            {
                Color = SKColors.LightGray,
                IsAntialias = true
            };

            SKPaint borderPaint = new SKPaint
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2
            };

            // Вычисляем ширину колонок
            int[] colWidths = new int[columns];
            int rowHeight = 0;

            for (int c = 0; c < columns; c++)
            {
                int maxWidth = 0;
                for (int r = 0; r < rows; r++)
                {
                    string text = tableData[r][c]?.ToString() ?? "";
                    SKRect bounds = new SKRect();
                    textPaint.MeasureText(text, ref bounds);
                    int width = (int)Math.Ceiling(bounds.Width);
                    int height = (int)Math.Ceiling(bounds.Height);

                    if (width > maxWidth)
                        maxWidth = width;
                    if (height + 2 * cellPadding > rowHeight)
                        rowHeight = height + 2 * cellPadding;
                }
                colWidths[c] = maxWidth + 2 * cellPadding;
            }

            int totalWidth = 0;
            foreach (var w in colWidths) totalWidth += w;
            int totalHeight = rowHeight * rows;

            using (var bitmap = new SKBitmap(totalWidth, totalHeight))
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.White);

                int y = 0;
                for (int r = 0; r < rows; r++)
                {
                    int x = 0;
                    for (int c = 0; c < columns; c++)
                    {
                        var rect = new SKRect(x, y, x + colWidths[c], y + rowHeight);

                        if (r == 0)
                        {
                            canvas.DrawRect(rect, headerPaint);
                        }

                        canvas.DrawRect(rect, borderPaint);

                        string text = tableData[r][c]?.ToString() ?? "";
                        SKRect textBounds = new SKRect();
                        textPaint.MeasureText(text, ref textBounds);

                        float textX = x + (colWidths[c] - textBounds.Width) / 2 - textBounds.Left;
                        float textY = y + (rowHeight + textBounds.Height) / 2 - textBounds.Bottom;

                        canvas.DrawText(text, textX, textY, textPaint);

                        x += colWidths[c];
                    }
                    y += rowHeight;
                }

                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = System.IO.File.OpenWrite(fileName))
                {
                    data.SaveTo(stream);
                }
            }

            Console.WriteLine($"Таблица сохранена в файл: {fileName}");
        }
    }
}

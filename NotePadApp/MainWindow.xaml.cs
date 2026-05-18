using ICSharpCode.AvalonEdit.Highlighting;
using NotePadApp.Core.Services;
using NotePadApp.Core.Services.IndexFormattingService;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace NotePadApp
{
    public partial class MainWindow : Window
    {
        private readonly List<FormattedSegment> _segments = new();

        public MainWindow()
        {
            InitializeComponent();

            TextEditor.TextArea.TextView.LineTransformers.Add(
                new BoldFormattingService(_segments));

            TextEditor.TextArea.TextView.LineTransformers.Add(
                new ItalicFormattingService(_segments));

            TextEditor.TextArea.TextView.LineTransformers.Add(
                new UnderlineFormattingService(_segments));

            TextEditor.TextArea.TextView.LineTransformers.Add(
                new StrikethroughFormattingService(_segments));

            TextEditor.TextArea.TextView.ElementGenerators.Add(
                new IndexFormattingService(_segments));
    }

        private void BtnUndo_Click(object sender, RoutedEventArgs e) => TextEditor.Undo();
        private void BtnRedo_Click(object sender, RoutedEventArgs e) => TextEditor.Redo();

        private void BtnBold_Click(object sender, RoutedEventArgs e) => AddSegment(s => s.IsBold = true, s => s.IsBold);

        private void BtnItalic_Click(object sender, RoutedEventArgs e) => AddSegment(s => s.IsItalic = true, s => s.IsItalic);

        private void BtnUnderline_Click(object sender, RoutedEventArgs e) => AddSegment(s => s.IsUnderline = true, s => s.IsUnderline);

        private void BtnStrikethrough_Click(object sender, RoutedEventArgs e) => AddSegment(s => s.IsStrikethrough = true, s => s.IsStrikethrough);

        private void BtnSuperscript_Click(object sender, RoutedEventArgs e)
        {
            RemoveIndexSegment(s => s.IsSubscript);
            AddSegment(s => s.IsSuperscript = true, s => s.IsSuperscript);
        }

        private void BtnSubscript_Click(object sender, RoutedEventArgs e)
        {
            RemoveIndexSegment(s => s.IsSuperscript);
            AddSegment(s => s.IsSubscript = true, s => s.IsSubscript);
        }

        private void AddSegment(Action<FormattedSegment> configure,
                        Func<FormattedSegment, bool> check)
        {
            int start = TextEditor.SelectionStart;
            int length = TextEditor.SelectionLength;
            if (length == 0) return;

            int end = start + length;

            var existing = _segments.FirstOrDefault(s =>
                s.StartOffset == start &&
                s.EndOffset == end &&
                check(s));

            if (existing != null)
                _segments.Remove(existing); 
            else
            {
                var seg = new FormattedSegment { StartOffset = start, EndOffset = end };
                configure(seg);
                _segments.Add(seg);
            }

            TextEditor.TextArea.TextView.Redraw();
        }

        private void RemoveIndexSegment(Func<FormattedSegment, bool> check)
        {
            int start = TextEditor.SelectionStart;
            int end = start + TextEditor.SelectionLength;

            var existing = _segments.FirstOrDefault(s =>
                s.StartOffset == start && s.EndOffset == end && check(s));

            if (existing != null)
                _segments.Remove(existing);
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            int docLength = TextEditor.Document.TextLength;

            _segments.RemoveAll(s =>
                s.StartOffset >= docLength ||
                s.EndOffset > docLength);

            TextEditor.TextArea.TextView.Redraw();
        }

        private void BtnNewFile_Click(object sender, RoutedEventArgs e) 
        {
            TextEditor.Clear();
            _segments.Clear();
        }

        private async void BtnOpenFile_Click(object sender, RoutedEventArgs e) 
        {
            using var dialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Текстовый файл (*.txt)|*.txt",
                DefaultExt = "txt"
            };

            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = dialog.FileName;
                var content = await File.ReadAllTextAsync(filePath) ?? "";
                _segments.Clear();

                TextEditor.Text = content;
            }
        }

        private async void BtnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new SaveFileDialog()
            {
                Filter = "Текстовый файл (*.txt)|*.txt",
                DefaultExt = "txt"
            };

            var result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string filePath = dialog.FileName;
                string contents = TextEditor.Text ?? "";

                await File.WriteAllTextAsync(filePath, contents);
            }
        }

        private void BtnPrintFile_Click(object sender, RoutedEventArgs e)
        {
            using var document = new PrintDocument(); // Создаем документ для печати

            document.PrintPage += (s, ev) =>
            {
                ev?.Graphics?.DrawString(TextEditor.Text ?? "", new Font("Arial", 12), Brushes.Black, 20, 20); // Настройки печати
            };

            using var dialog = new PrintDialog(); // Окно Windows для печати
            dialog.Document = document; // Устанавливаем документ для печати

            var result = dialog.ShowDialog(); // Показываем диалог с подтверждением для печати

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                document.Print(); // Печатаем документ
            }
        }
    }
}
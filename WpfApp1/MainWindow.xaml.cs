using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.Xml;
using System.Reflection;
using ICSharpCode.AvalonEdit;
using PoorMansTSqlFormatterLib.Interfaces;
using PoorMansTSqlFormatterLib.Formatters;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string xshdFilePath = Path.Combine(GetAssemblyDirectory(Assembly.GetExecutingAssembly()), @"xshd\TSQL-Mode.xshd");

            // Load the XSHD file
            var highlightingDefinition = LoadHighlightingFromFile(xshdFilePath);

            // Apply the syntax highlighting to the editor
            texteditor_SQL.SyntaxHighlighting = highlightingDefinition;
            texteditor_SQL.Text = @"select N'Français ABCD abcd' French, N'Ελληνικά ΑΒΓΔ αβγδ' as Greek, N'עִברִית ﭏבגד' Hebrew, N'カタカナ_かたかな' Katakana_Hiragana,N'ひらがな_ヒラガナ ' Hiragana_Katakana,N'中文 月官匹刀' Chinese, N'عربى آ ب ي د' Arabic from datasetreport.demo.salesman";
            CreateContextMenu();
        }

        private void HandleException(Exception ex)
        {
            string msg = ex.Message;
            MessageBox.Show(msg);
            Console.WriteLine(msg);
        }
        private IHighlightingDefinition LoadHighlightingFromFile(string filePath)
        {
            // Ensure the file exists before trying to load it
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"The specified XSHD file was not found: {filePath}");

            // Load the syntax highlighting definition from the XSHD file
            using (var reader = XmlReader.Create(filePath))
            {
                return HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }

        public static string GetAssemblyDirectory(Assembly assembly)
        {
            return Path.GetDirectoryName(assembly.Location);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hello wpf");

            //ChangeFontSize();
            var sql = FormatSQL(texteditor_SQL.Text);
            texteditor_SQL.SelectAll();
            System.Windows.Clipboard.SetText(sql);
            texteditor_SQL.Paste();
            //texteditor_SQL.Text = sql;


        }

        void ChangeFontSize()
        {
            try
            {
                //this.FontSize = 80;
                //return;

                foreach (var control in FindVisualChildren<Control>(this))
                {
                    control.FontSize = 30;
                }
                return;

                // marche pas...
                var existingStyle = (Style)this.Resources["ControlStyle"];
                if (existingStyle != null)
                {
                    /*
                    var setter = new Setter(Control.FontSizeProperty, 30);
                    //System.InvalidOperationException: 'After a 'SetterBaseCollection' is in use (sealed), it cannot be modified.'
                    style.Setters.Clear();  // Clear existing setters
                    style.Setters.Add(setter);  // Add new setter for FontSize
                    */
                    var newStyle = new Style(typeof(Control)); // or target a specific type like Button or TextBox

                    // Copy the existing Setters (if needed) to preserve previous ones
                    foreach (var setter in existingStyle.Setters)
                    {
                        newStyle.Setters.Add(setter);
                    }
                    newStyle.Setters.Add(new Setter(Control.FontSizeProperty, 80));

                    // Replace the existing style in the resources with the new one
                    this.Resources["ControlStyle"] = newStyle;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void CreateContextMenu()
        {
            var contextMenu = new ContextMenu();

            var cutMenuItem = new MenuItem { Header = "Cut", Command = ApplicationCommands.Cut };
            var copyMenuItem = new MenuItem { Header = "Copy", Command = ApplicationCommands.Copy };
            var pasteMenuItem = new MenuItem { Header = "Paste", Command = ApplicationCommands.Paste };
            var deleteMenuItem = new MenuItem { Header = "Delete", Command = ApplicationCommands.Delete };
            var selectAllMenuItem = new MenuItem { Header = "Select All", Command = ApplicationCommands.SelectAll };
            var formatSQLMenuItem = new MenuItem { Header = "Format SQL" };
            formatSQLMenuItem.Click += FormatSQL_Click;
            var undoMenuItem = new MenuItem { Header = "Undo", Command = ApplicationCommands.Undo };
            var redoMenuItem = new MenuItem { Header = "Redo", Command = ApplicationCommands.Redo };

            contextMenu.Items.Add(cutMenuItem);
            contextMenu.Items.Add(copyMenuItem);
            contextMenu.Items.Add(pasteMenuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(deleteMenuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(selectAllMenuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(formatSQLMenuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(undoMenuItem);
            contextMenu.Items.Add(redoMenuItem);

            texteditor_SQL.ContextMenu = contextMenu;
        }

        private void FormatSQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sql = FormatSQL(texteditor_SQL.Text);
                texteditor_SQL.SelectAll();
                System.Windows.Clipboard.SetText(sql);
                texteditor_SQL.Paste();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private string FormatSQL(string sql = "select * from a.b")
        {
            ISqlTokenizer _tokenizer;
            ISqlTokenParser _parser;
            ISqlTreeFormatter _formatter;

            _tokenizer = new PoorMansTSqlFormatterLib.Tokenizers.TSqlStandardTokenizer();
            _parser = new PoorMansTSqlFormatterLib.Parsers.TSqlStandardParser();

            //var cfg = new SqlStandardFormatterOptions()// PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatterOptions
            //{
            //    IndentString = "\t",
            //    SpacesPerTab = 4,
            //    MaxLineWidth = 999,
            //    ExpandCommaLists = true,
            //    TrailingCommas = false,
            //    SpaceAfterExpandedComma = false,
            //    ExpandBooleanExpressions = true,
            //    ExpandCaseStatements = true,
            //    ExpandBetweenConditions = true,
            //    ExpandInLists = true,
            //    BreakJoinOnSections = false,
            //    UppercaseKeywords = true,
            //    HTMLColoring = true,
            //    KeywordStandardization = false,
            //    NewStatementLineBreaks = 1,
            //    NewClauseLineBreaks = 1
            //};
            //cfg.HTMLColoring = false;
            
            _formatter = new TSqlStandardFormatter();

            var tokenizedSql = _tokenizer.TokenizeSQL(sql);
            var parsedSql = _parser.ParseSQL(tokenizedSql);
            //PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatter stdFormatter = _parser as PoorMansTSqlFormatterLib.Formatters.TSqlStandardFormatter;
            var formatter = (TSqlStandardFormatter)_formatter;

            //var ret = formatter.FormatSQLTree(parsedSql, (FormatterCnDFlags)formatterCnDFlags); //, FormatterCnDFlags.Zero);
            //var errOutputPrefix = _formatter.ErrorOutputPrefix;
            //if (ret.StartsWith(errOutputPrefix))
            //    ret = null;
            //else
            //    ret = FixStandardDateTimeFormattingIssue(ret);

            var ret = formatter.FormatSQLTree(parsedSql);
            return ret;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) yield break;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    yield return (T)child;
                }

                // Recursively find children in the visual tree
                foreach (var descendant in FindVisualChildren<T>(child))
                {
                    yield return descendant;
                }
            }
        }
    }
}

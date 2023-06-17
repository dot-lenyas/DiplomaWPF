using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using PascalABCCompiler.SimplePascalParser;
using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using PascalABCCompiler.SyntaxTree;
using SimplePascalParser;
using PascalABCCompiler.Parsers;
using GPPGParserScanner;
using Microsoft.Win32;

namespace DiplomaWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            /*string FileName = "C:\\Users\\lenya\\RiderProjects\\SimplePas_G1\\a.pas";
            StreamReader sr = new StreamReader(FileName);
            string Text = sr.ReadToEnd();
            PT.CurrentFileName = FileName;

            Scanner scanner = new Scanner();
            scanner.SetSource(Text, 0);

            GPPGParser parser = new GPPGParser(scanner);
            var b = parser.Parse();
            sr.Close();
            if (!b)
            {
                if (PT.Errors.Count == 0)
                    PT.AddError("Неопознанная синтаксическая ошибка!", null);
                Console.WriteLine(PT.Errors[0]);
            }
            else Console.WriteLine("Синтаксическое дерево построено");
            // parser.root содержит корень синтаксического дерева
            TreeNode<string> root = new TreeNode<string>("BEGIN");
            PascalABCCompiler.SimplePascalParser.SimplePascalLanguageParser.PrintNode(parser.root, root);

            Console.ReadLine();*/
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Зашли");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            // получаем выбранный файл
            Console.WriteLine("Прочли");
            string filename = openFileDialog.FileName;
            if (filename == "")
            {
                MessageBox.Show("Файл отсутствует!");
                return;
            }
            if (!filename.Contains(".pas"))
            {
                MessageBox.Show("Необходим файл .pas");
                return;
            }
            // читаем файл в строку
            string fileText = System.IO.File.ReadAllText(filename);
            textBox1.Text = fileText;
            MessageBox.Show("Файл открыт");
            Console.WriteLine("Успешно");
        }

        private void Build_Tree(object sender, RoutedEventArgs e)
        {
            PT.CurrentFileName = "Test File";

            Scanner scanner = new Scanner();
            scanner.SetSource(textBox1.Text, 0);

            GPPGParser parser = new GPPGParser(scanner);
            var b = parser.Parse();
            if (!b)
            {
                if (PT.Errors.Count == 0)
                    PT.AddError("Неопознанная синтаксическая ошибка!", null);
                Console.WriteLine(PT.Errors[0]);
                MessageBox.Show(PT.Errors[0].ToString());
                return;
            }
            else Console.WriteLine("Синтаксическое дерево построено");
            // parser.root содержит корень синтаксического дерева
            TreeNode<string> root = new TreeNode<string>("BEGIN");
            PascalABCCompiler.SimplePascalParser.SimplePascalLanguageParser.PrintNode(parser.root, root);
        }
    }
}

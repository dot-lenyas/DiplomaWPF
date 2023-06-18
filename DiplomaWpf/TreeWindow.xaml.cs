using SimplePascalParser;
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
using System.Windows.Shapes;

namespace DiplomaWpf
{
    /// <summary>
    /// Логика взаимодействия для TreeWindow.xaml
    /// </summary>
    public partial class TreeWindow : Window
    {
        private TreeNode<String> root;
        public TreeWindow()
        {
            InitializeComponent();
        }
        public TreeWindow(TreeNode<String> root1)
        {
            InitializeComponent();
            root = root1;
            String result = writeTree();
            result += "\nEND";
            txt1.Text = result;
        }

        private String writeTree()
        {
            String result = "";

            while(true)
            {
                result += root.Data;
                result += "\n";
                String temp = "";
                if (root.Data == "then" || root.Data == "else")
                {
                    result += "{\n";
                }
                if (root.Data == "if")
                {
                    TreeNode<String> tempNode = root;
                    foreach (var node in root.Children)
                    {
                        root = node;
                        break;
                    }
                    
                    temp = writeTree();
                    temp += "}\n";
                    root = tempNode;
                    int i = 0;
                    foreach (var node in root.Children)
                    {
                        root = node;
                        if (i == 1)
                        {
                            break;
                        }
                        i++;
                    }
                    if (root.Data != "else")
                    {
                        result += temp;
                        return result;
                    }
                    temp += writeTree();
                    temp += "}\n";
                    result += temp;
                    root = tempNode;
                    bool isRepeat = false;
                    foreach (var node in root.Children)
                    { 
                        root = node;
                        if (root.Data != "then" && root.Data != "else")
                        {
                            isRepeat = true;
                        }
                    }
                    if (!isRepeat)
                    {
                        return result;
                    }

                    
                }
                if (root.Children.Count == 0)
                {
                    return result;
                }
                foreach (var node in root.Children)
                {
                    root = node;
                }
            }
        }


    }
}

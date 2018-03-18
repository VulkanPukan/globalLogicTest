using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace globalLogicTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static string path;

        private void button1_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    textBox2.Text = fbd.SelectedPath;
                    path = fbd.SelectedPath;
                 
                }
            }
        }

        private void ListDirectory(TreeView treeView, string path)
        {
            treeView.Nodes.Clear();
            var rootDirectoryInfo = new DirectoryInfo(path);
            treeView.Nodes.Add(AddNode(rootDirectoryInfo));
        }

        private static TreeNode AddNode(DirectoryInfo directoryInfo)
        {
            var directoryNode = new TreeNode(directoryInfo.Name);
            directoryNode.Tag = "dir";
            foreach (var directory in directoryInfo.GetDirectories())
            {
                directoryNode.Tag = "dir";
                directoryNode.Nodes.Add(AddNode(directory));
            }
            foreach (var file in directoryInfo.GetFiles())
            {
                TreeNode tr = new TreeNode(file.Name);
                tr.Tag = "file";
                directoryNode.Nodes.Add(tr);
            }

            return directoryNode;
        }

        private void SerializeTree(TreeView tree,string pathToFile)
        {
            using (FileStream fs = new FileStream(pathToFile, FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, tree.Nodes.Cast<TreeNode>().ToArray());
            }


        }

        private void DeserializeTree(TreeView tree,string pathToFile)
        {
            using (FileStream fs = new FileStream(pathToFile, FileMode.OpenOrCreate))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                TreeNode[] nodes = (TreeNode[])formatter.Deserialize(fs);
                tree.Nodes.AddRange(nodes);
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "dat files (*.dat)|*.dat";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    ListDirectory(treeView1, path);
                    SerializeTree(treeView1,saveFileDialog1.FileName);

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "dat files (*.dat)|*.dat";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                treeView1.Nodes.Clear();
                DeserializeTree(treeView1, openFileDialog1.FileName);
            }

         
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void CreateFolders()
        {
         
            CurrentFolder = path;
            PrevFolder = CurrentFolder;
            RecursiveTree(treeView1.Nodes);
           
            


        }

        string CurrentFolder;
        string PrevFolder;
        TreeNode parent;
        void RecursiveTree(TreeNodeCollection nodes)
        {
            if (parent == null)
            {
                parent = nodes[0];
              
            }
            foreach (TreeNode child in nodes)
            {
              
                if(child.Parent != null)
                    textBox1.Text += child.Text + "  " + child.Parent.Text +  "\r\n";

                if (child.Tag.ToString() == "file")
                {
                    if (parent == child.Parent)
                        File.Create(CurrentFolder + "\\" + child.Text).Close();
                    else
                    {
                        parent = parent.Parent;
                        CurrentFolder = Directory.GetParent(CurrentFolder).FullName;
                        File.Create(CurrentFolder + "\\" + child.Text).Close();
                    }

                }

                if (child.Tag.ToString() == "dir")
                {
                    if(child.Parent == parent.Parent &&  parent.Parent != null)
                    {
                        parent = parent.Parent;
                        CurrentFolder = Directory.GetParent(CurrentFolder).FullName;

                    }
                    if (parent == child.Parent || child == parent )
                    {
                        Directory.CreateDirectory(CurrentFolder + "\\" + child.Text);

                        if (child.Nodes.Count > 0)
                        {
                            parent = child;
                            CurrentFolder += "\\" + child.Text;
                            RecursiveTree(child.Nodes);
                        }
                    }
                    else
                    {
                        parent = parent.Parent;
                        CurrentFolder = Directory.GetParent(CurrentFolder).FullName;
                    }

                    //if (parent.Parent == child.Parent && parent.Parent != null)
                    //{
                    //    parent = parent.Parent;
                    //    CurrentFolder = Directory.GetParent(CurrentFolder).FullName;
                    //}

                    //if (parent == child.Parent || child == parent)
                    //    Directory.CreateDirectory(CurrentFolder + "\\" + child.Text);
                    //else
                    //{
                    //    //CurrentFolder = Directory.GetParent(CurrentFolder).FullName;
                    //    Directory.CreateDirectory(CurrentFolder + "\\" + child.Text);
                    //}

                   
                }
              

                //if (child.Nodes.Count > 0)
                //    {
                //        parent = child;
                //        CurrentFolder += "\\" + child.Text;
                //        RecursiveTree(child.Nodes);
                //    }
                    
                 //   else if (child.Nodes.Count == 0)
                       

                           
                 
                
               
                
               
             
            }

            /*
            //if (nodes.Count != 0)
            //{
            //    CurrentFolder += "\\" + nodes[0].Text;
            //}
            foreach (var child in nodes)
            {
                //if (((TreeNode)child).Tag.ToString() != null && ((TreeNode)child).Tag.ToString() == "dir")
                //    CurrentFolder += "\\" + ((TreeNode)child).Text;
                // textBox1.Text += CurrentFolder+"\t";
                textBox1.Text += ((TreeNode)child).Tag + " : " + ((TreeNode)child).Text + "\t";
                if (((TreeNode)child).Tag.ToString() != null && ((TreeNode)child).Tag.ToString() == "dir")
                {
                    Directory.CreateDirectory(CurrentFolder);
                    CurrentFolder += "\\" + ((TreeNode)child).Text;
                }
                else if (((TreeNode)child).Tag.ToString() == "file" && ((TreeNode)child).Tag != null)
                    File.Create(path + "\\" + ((TreeNode)child).Parent.Text + "\\" + ((TreeNode)child).Text);
                RecursiveTree(((TreeNode)child).Nodes);
            }
            */
        }

    
        private void button4_Click(object sender, EventArgs e)
        {
            CreateFolders();
        }
    }
}

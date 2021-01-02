using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RenameCsFiles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void dragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;

        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            listBox.Items.Clear();
            var files = (string[])e.Data.GetData(DataFormats.FileDrop); 
            foreach (var ww in files)
                Process(ww);
        }

        void Process(string filename)
        {
            if(Path.GetExtension(filename).ToLower()!=".cs") return;
            var n = getclassname(filename); 
            if(n!="") rename(filename,n);
            else

            listBox.Items.Add($"{Path.GetFileName(filename)} ******");
        }
        void NameCleaner(string fullpath)

        {
            var ext = Path.GetExtension(fullpath);
            var name = Path.GetFileNameWithoutExtension(fullpath);  
            var path = Path.GetDirectoryName(fullpath);
            var match= CleanNameR.Match(name);
            if (match.Success)
                name = match.Groups[1].Value; 
            var newname = Path.Combine(path, name + ext);
            listBox.Items.Add(name+ext);
            File.Move(fullpath, newname);

          
        }

        static readonly Regex CleanNameR=new Regex("(\\w+)(_?\\(?\\d+\\)?)$", RegexOptions.RightToLeft);
        string getclassname(string filename)
         {
             string s = "";
            // Read file using StreamReader. Reads file line by line  
            using (StreamReader file = new StreamReader(filename))
            {
               
                string ln; 
                while ((ln = file.ReadLine()) != null)
                {
                    var w = ClassPat.Match(ln);
                    if(!w.Success)  continue;
                    s = w.Groups["name"].Value; 
                    break;
                } 
                file.Close(); 
            }
              return s; 
        }

        static Regex ClassPat=new Regex("class (?<name>\\w+)");

        void rename(string fullpath, string classname)
        {
            var ext = Path.GetExtension(fullpath);
            var name = Path.GetFileNameWithoutExtension(fullpath); 
            if (name.Length > 10) name = name.Substring(0, 5);
            var path = Path.GetDirectoryName(fullpath);  
                var newname =    Path.Combine(path, classname + ext);
                if (File.Exists(newname))
                {
                    var number = 0; 
            do
            {   
                number++;
                newname =Path.Combine(path,$"{classname}_{number:00}{ext}"); 
            }

            while (File.Exists(newname));  

                }  
           
            listBox.Items.Add($"{name} -> {newname.Substring(path.Length+1)}");
            File.Move(fullpath,newname);   
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            listBox.Items.Clear();
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var ww in files)
                NameCleaner(ww);
        }

      
    }
}

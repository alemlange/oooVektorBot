using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;

namespace LiteDBManagementStudio
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            using (var db = new LiteDatabase(@"c:\DB\Qwerty.db"))
            {
                var entities = db.GetCollectionNames();

                foreach (var entity in entities)
                {
                    DBTreeView.Nodes.Add(entity);

                    var collection = db.GetCollection(entity);
                    var data = collection.FindAll();

                    foreach (var item in data)
                    {
                        //DBGridView.
                        DBTreeView.Nodes.Add(item.AsString);
                        //DBTreeView.Nodes[entity].Nodes.Add(item.Values.)
                    }
                }
            }
        }
    }
}
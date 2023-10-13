using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace admission_commision
{ 
    enum RowState
{
    Existed,
    New,
    Modified,
    ModifiedNew,
    Deleted
}
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            SQLClass.conn = new MySql.Data.MySqlClient.MySqlConnection();
            SQLClass.conn.ConnectionString =
                "Server=localhost;Database=admission_commision;port=3306;User Id=root";
            SQLClass.conn.Open();
            Application.Run(new Entry());
            SQLClass.conn.Close();
        }
    }
}

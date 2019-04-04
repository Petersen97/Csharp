using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace StudentDB
{
    public partial class Form1 : Form
    {
        //Server info and login info that is being used for opening the connection to the databse in this code
        string ConnectionString = @"server=localhost;user id=root;password=12345678;database=student2k18d";
        //Sets the idStudent value to 0
        int idStudent = 0;
        public Form1()
        {
            InitializeComponent();
        }

        //void for save button in Form1
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection mysqlCon = new MySqlConnection(ConnectionString))
                {
                    //Initiating the connection to the databse server
                    mysqlCon.Open();
                    //Telling the application which stored procedure it is going to use for the sql code. 
                    MySqlCommand mySqlCmd = new MySqlCommand("StudentAddOrEdit", mysqlCon);
                    //Telling the application that the commandtype is a stored procedure
                    mySqlCmd.CommandType = CommandType.StoredProcedure;

                    //These parameteres corresponds with the text boxes and are putting the text inside the correct collums in the table of the databse
                    mySqlCmd.Parameters.AddWithValue("_idStudent", idStudent);
                    mySqlCmd.Parameters.AddWithValue("_Adresse", txtAdresse.Text.Trim());
                    mySqlCmd.Parameters.AddWithValue("_Alder", txtAlder.Text.Trim());
                    mySqlCmd.Parameters.AddWithValue("_Etternavn", txtEtternavn.Text.Trim());
                    mySqlCmd.Parameters.AddWithValue("_Fornavn", txtFornavn.Text.Trim());
                    mySqlCmd.Parameters.AddWithValue("_Kjonn", txtKjonn.Text.Trim());
                    //In the application you can not edit the idFylke because idFylke is set by the postnummer in the mysql databse
                    mySqlCmd.Parameters.AddWithValue("_postnummer_Fylke_idFylke", txtFylke.Text.Trim());
                    mySqlCmd.Parameters.AddWithValue("_postnummer_idPostnummer", txtPostnummer.Text.Trim());
                    mySqlCmd.Parameters.AddWithValue("_Tlf", txtTlf.Text.Trim());
                    mySqlCmd.ExecuteNonQuery();
                    //Showing a message box if it is submitted successfully
                    MessageBox.Show("Submitted Successfully");
                    GridFill();
                    Clear();
                }

            }
            //Cathching any errors that occurs in the software
            catch (Exception ex)
            {
                //Displaying a raw error message, usefull for IT-administrators, not usefull in this form for the end-user.
                MessageBox.Show(ex.Message.ToString());
                
            }
            
        }

        void GridFill()
        {
            using (MySqlConnection mysqlCon = new MySqlConnection(ConnectionString))
            {
                //Opens the mysql connection
                mysqlCon.Open();
                //Telling the application to use StudentViewAll procedure in the mysql databse
                MySqlDataAdapter sqlDA = new MySqlDataAdapter("StudentViewAll", mysqlCon);
                //Setting the commandtype to Stored Procedure
                sqlDA.SelectCommand.CommandType = CommandType.StoredProcedure;
                //Takes the information gattered from the procedure and export the information into DataTable dtblstudent
                DataTable dtblstudent = new DataTable();
                sqlDA.Fill(dtblstudent);
                //Telling the application to fill in the datagridview dgvStudent with the information from the stored procedure
                dgvStudent.DataSource = dtblstudent;
                //Making the first column invisible since the user do not have to know which id the student has
                dgvStudent.Columns[0].Visible = false;
                dgvStudent.Columns[10].Visible = false;
                //Changes the header name to Postnummer
                dgvStudent.Columns[7].HeaderText = "Postnummer";
                //Setting the widht of the column of alder and kjønn.  
                dgvStudent.Columns[5].Width = 40;
                dgvStudent.Columns[6].Width = 40;
            }

        }

        void Clear()
        {
            //Setting all the textboxes back to empty, telling the text boxes that they are equal to empty
            txtAdresse.Text = txtAlder.Text = txtEtternavn.Text = txtFornavn.Text = txtKjonn.Text = txtPostnummer.Text = txtTlf.Text = "";
            //Resetting the idStudent to zero
            idStudent = 0;
            //Changes the btnSave.Text back to saying Save
            btnSave.Text = "Save";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Loads the datagridview with the databse information at start up
            GridFill();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //Running the Clear function when the Cancel button is pressed
            Clear();
        }

        private void dgvStudent_DoubleClick(object sender, EventArgs e)
        {
            if (dgvStudent.CurrentRow.Index != -1)
            {
                //Taking the information from the table and writes it in to the corresponding textboxes so it can be edited or deleted
                txtAdresse.Text = dgvStudent.CurrentRow.Cells[3].Value.ToString();
                txtAlder.Text = dgvStudent.CurrentRow.Cells[5].Value.ToString();
                txtEtternavn.Text = dgvStudent.CurrentRow.Cells[1].Value.ToString();
                txtFornavn.Text = dgvStudent.CurrentRow.Cells[2].Value.ToString();
                txtFylke.Text = dgvStudent.CurrentRow.Cells[10].Value.ToString();
                txtKjonn.Text = dgvStudent.CurrentRow.Cells[6].Value.ToString();
                txtPostnummer.Text = dgvStudent.CurrentRow.Cells[7].Value.ToString();
                txtTlf.Text = dgvStudent.CurrentRow.Cells[4].Value.ToString();
                idStudent = Convert.ToInt32(dgvStudent.CurrentRow.Cells[0].Value.ToString());
                //Changing the text of the Save button to say Update instead
                btnSave.Text = "Update";
                //The delete button is enabled and ready to use if needed
                btnDelete.Enabled = Enabled;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection mysqlCon = new MySqlConnection(ConnectionString))
                {
                    //Opens the mysql connection
                    mysqlCon.Open();
                    //Telling the application to use stored procedure StudentDeleteByID
                    MySqlCommand mySqlCmd = new MySqlCommand("StudentDeleteByID", mysqlCon);
                    //Setting the commandtype to Stored Procedure
                    mySqlCmd.CommandType = CommandType.StoredProcedure;
                    //Taking the student id and sends the value to the stored procedure
                    mySqlCmd.Parameters.AddWithValue("_idStudent", idStudent);
                    mySqlCmd.ExecuteNonQuery();
                    //Showing a message box that says the deletion was successfull 
                    MessageBox.Show("Deleted Successfully");
                    //Runs the clear and gridfill function to clear the text boxes and refreshing the datagridview
                    Clear();
                    GridFill();
                }

            }
            //Cathching any errors that occurs in the software
            catch (Exception delete)
            {
                //Displaying a raw error message, usefull for IT-administrators, not usefull in this form for the end-user.
                MessageBox.Show(delete.Message.ToString());
            }
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection mysqlCon = new MySqlConnection(ConnectionString))
                {
                    //Opens the mysql connection
                    mysqlCon.Open();
                    //Telling the appcliation to use the stored procedure StudentSearchByValue
                    MySqlDataAdapter sqlDA = new MySqlDataAdapter("StudentSearchByValue", mysqlCon);
                    //Taking the text from the text box txtSearch and exports it to _SearchValue to be used in the sql procedure
                    sqlDA.SelectCommand.Parameters.AddWithValue("_SearchValue", txtSearch.Text);
                    //Setting the commandtype to Stored Procedure
                    sqlDA.SelectCommand.CommandType = CommandType.StoredProcedure;
                    DataTable dtblstudent = new DataTable();
                    sqlDA.Fill(dtblstudent);
                    dgvStudent.DataSource = dtblstudent;
                    dgvStudent.Columns[0].Visible = false;
                }

            }
            //Cathching any errors that occurs in the software and puts in into the variable search
            catch (Exception search)
            {

                //Displaying a raw error message, usefull for IT-administrators, not usefull in this form for the end-user.
                MessageBox.Show(search.Message.ToString());
            }
            
        }

        //Opens up the second form when the button for adding postnummer and fylke is pressed
        private void btnPostnummer_Click(object sender, EventArgs e)
        {
            Form2 Postnummer = new Form2();
            Postnummer.Show();
        }
    }
}

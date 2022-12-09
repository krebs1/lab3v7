using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace lab3v7
{
    public partial class MainForm : Form
    {
        private SaveFileDialog _saveFileDialog;
        private OpenFileDialog _openFileDialog;
        private List<Equipment> _records;
        public MainForm()
        {
            InitializeComponent();

            _openFileDialog = new OpenFileDialog();
            _openFileDialog.Filter = @"Dat files(*.dat)|*.dat|All files(*.*)|*.*";
            if (_openFileDialog.ShowDialog() == DialogResult.Cancel) Application.Exit();
            else Program.BusinessLogic = new BusinessLogic(new FileDataSource(_openFileDialog.FileName));
            //string filename = _openFileDialog.FileName;
            
            _saveFileDialog = new SaveFileDialog();
            _saveFileDialog.Filter = @"Text files(*.txt)|*.txt|All files(*.*)|*.*";

            _records = Program.BusinessLogic.GetAllRecords();
            WriteToListBox(listBox1);
            if (listBox1.SelectedIndex == -1)
            {
                button1.Enabled = false;
                button2.Enabled = false;
            }
        }

        private void WriteToListBox(ListBox listBox)
        {
            listBox.Items.Clear();
            listBox.Items.AddRange(_records.Select(record => record.ToString()).ToArray());
        }
        
        //Активация кнопок удалить и изменить
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                button1.Enabled = true;
                button2.Enabled = true;
            }
        }

        //Создание отчета
        private void button4_Click(object sender, EventArgs e)
        {
            var from = Convert.ToInt32(numericUpDown1.Value);
            var to = Convert.ToInt32(numericUpDown2.Value);
            
            if (_saveFileDialog.ShowDialog() == DialogResult.Cancel) return;
            var filename = _saveFileDialog.FileName;
            Program.BusinessLogic.CreateReport(filename, from, to);
        }

        //Удаление
        private void button1_Click(object sender, EventArgs e)
        {
            var dialogResult = MessageBox.Show("Вы действительно хотите удалить эту запись?", "Подтверждение удаления", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Program.BusinessLogic.DeleteRecord(_records[listBox1.SelectedIndex].Id);
                listBox1.SelectedIndex = -1;
                _records = Program.BusinessLogic.GetAllRecords();
                WriteToListBox(listBox1);
            }
        }

        //Изменение
        private void button2_Click(object sender, EventArgs e)
        {
            var modal = new AddAndChange();
            modal.NameInp.Text = _records[listBox1.SelectedIndex].Name;
            modal.SerialNumber.Text = _records[listBox1.SelectedIndex].SerialNumber;
            modal.DateReg.Text = _records[listBox1.SelectedIndex].RegistrationDate;
            modal.LastMaintDate.Text = _records[listBox1.SelectedIndex].LastMaintenanceDate;
            
            if (modal.ShowDialog(this) == DialogResult.OK)
            {
                var name = modal.NameInp.Text;
                var serialNumber = modal.SerialNumber.Text;
                var registrationDate = modal.DateReg.Text;
                var lastMaintenanceDate = modal.LastMaintDate.Text;
                var decommissionedDate = modal.DateDecom.Text;
                var decommissionedReason = modal.DecomReason.Text;
                if (decommissionedReason == "")
                {
                    try
                    {
                        var record = new DecomEquipmentByTime(name, serialNumber, registrationDate, lastMaintenanceDate, decommissionedDate) { Id = _records[listBox1.SelectedIndex].Id };
                        Program.BusinessLogic.UpdateRecord(record);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    finally
                    {
                        modal.Close();
                        _records = Program.BusinessLogic.GetAllRecords();
                        WriteToListBox(listBox1);
                    }
                }
                else
                {
                    try
                    {
                        var record = new DecomEquipmentByReason(name, serialNumber, registrationDate, lastMaintenanceDate, decommissionedDate, decommissionedReason) { Id = _records[listBox1.SelectedIndex].Id };
                        Program.BusinessLogic.UpdateRecord(record);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    finally
                    {
                        modal.Close();
                        _records = Program.BusinessLogic.GetAllRecords();
                        WriteToListBox(listBox1);
                    }
                }
            }
            else
            {
                modal.Close();
            }
        }

        //Добавление
        private void button3_Click(object sender, EventArgs e)
        {
            var modal = new AddAndChange();
            if (modal.ShowDialog(this) == DialogResult.OK)
            {
                var name = modal.NameInp.Text;
                var serialNumber = modal.SerialNumber.Text;
                var registrationDate = modal.DateReg.Text;
                var lastMaintenanceDate = modal.LastMaintDate.Text;
                var decommissionedDate = modal.DateDecom.Text;
                var decommissionedReason = modal.DecomReason.Text;
                if (decommissionedReason == "")
                {
                    try
                    {
                        Program.BusinessLogic.SaveRecord(new DecomEquipmentByTime(name, serialNumber, registrationDate,
                            lastMaintenanceDate, decommissionedDate));
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    finally
                    {
                        modal.Close();
                        _records = Program.BusinessLogic.GetAllRecords();
                        WriteToListBox(listBox1);
                    }
                }
                else
                {
                    try
                    {
                        Program.BusinessLogic.SaveRecord(new DecomEquipmentByReason(name, serialNumber, registrationDate, lastMaintenanceDate, decommissionedDate, decommissionedReason));
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    finally
                    {
                        modal.Close();
                        _records = Program.BusinessLogic.GetAllRecords();
                        WriteToListBox(listBox1);
                    }
                }
            }
            else
            {
                modal.Close();
            }
        }
    }
}
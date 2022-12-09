using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace lab3v7
{
    public abstract class Equipment
    {
        public int Id;
        public string Name { get; protected set; }
        public string SerialNumber { get; private set; }
        public string RegistrationDate { get; private set; }
        public string LastMaintenanceDate { get; private set; }

        protected Equipment(string name, string serialNumber, string registrationDate, string lastMaintenanceDate)
        {
            Name = name;
            SerialNumber = serialNumber;
            RegistrationDate = registrationDate;
            LastMaintenanceDate = lastMaintenanceDate;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}, дата регистрации: {2}, дата обслуживания: {3}", Name, SerialNumber, RegistrationDate, LastMaintenanceDate);
        }
    }

    public class DecomEquipmentByTime : Equipment
    {
        public string DecomissedDate { get; private set; }

        public DecomEquipmentByTime(string name, string serialNumber, string registrationDate, string lastMaintenanceDate, string decomissedDate) : base(name, serialNumber, registrationDate, lastMaintenanceDate)
        {
            DecomissedDate = decomissedDate;
        }
        
        public override string ToString()
        {
            return String.Format("{0}, {1}, дата регистрации: {2}, дата обслуживания: {3}, дата списания: {4}", Name, SerialNumber, RegistrationDate, LastMaintenanceDate, DecomissedDate);
        }
    }

    public class DecomEquipmentByReason : Equipment
    {
        public string DecomissedReason { get; private set; }
        public string DecomissedDate { get; private set; }

        public DecomEquipmentByReason(string name, string serialNumber, string registrationDate, string lastMaintenanceDate, string decomissedReason, string decomissedDate) : base(name, serialNumber, registrationDate, lastMaintenanceDate)
        {
            DecomissedDate = decomissedDate;
            DecomissedReason = decomissedReason;
        }
        
        public override string ToString()
        {
            return String.Format("{0}, {1}, дата регистрации: {2}, дата обслуживания: {3}, дата списания: {4}, причина: {5}", Name, SerialNumber, RegistrationDate, LastMaintenanceDate, DecomissedDate, DecomissedReason);
        }
    }

    public interface IDataSource
    {
        Equipment Save(Equipment record);
        Equipment Get(int recordId);
        bool Delete(int recordId);
        List<Equipment> GetAll();
    }
    
    public class MemoryDataSource : IDataSource
    {
        private List<Equipment> records = new List<Equipment>();
        
        public Equipment Save(Equipment record)
        {
            if (record.Id == 0)
            {
                record.Id = records.Count + 1;
                records.Add(record);
            }
            else
            {
                records[record.Id - 1] = record;
            }
            
            return records[record.Id - 1];
        }

        public Equipment Get(int id)
        {
            return records.Find(item => item.Id == id);
        }

        public bool Delete(int id)
        {
            return records.Remove(records.Find(item => item.Id == id));
        }

        public List<Equipment> GetAll()
        {
            return records;
        }
    }
    
    class FileDataSource : IDataSource
    {
        private const string Signature = "VotyakovIvanSergeevich";
        private readonly string _path;
        private int _maxId;

        public FileDataSource(string filePath)
        {
            _path = filePath;
            var reader = new BinaryReader(File.Open(_path, FileMode.OpenOrCreate));
            var signature = reader.ReadString();
            if (signature != Signature) throw new Exception("Неправильная сигнатура файла");
            while (reader.PeekChar() != -1)
            {
                reader.ReadBoolean();
                _maxId = reader.ReadInt32();
                var type = reader.ReadByte();
                switch (type)
                {
                    case 0:
                        reader.ReadString();
                        reader.ReadString();
                        reader.ReadString();
                        reader.ReadString();
                        reader.ReadString();
                        break;
                    case 1:
                        reader.ReadString();
                        reader.ReadString();
                        reader.ReadString();
                        reader.ReadString();
                        reader.ReadString();
                        reader.ReadString();
                        break;
                }
            }
            reader.Close();
        }
        
        public Equipment Save(Equipment record)
        {
            if (record.Id == 0)
            {
                var writer = new BinaryWriter(File.Open(_path, FileMode.OpenOrCreate));
                _maxId++;
                record.Id = _maxId;
                writer.Seek(0, SeekOrigin.End);
                writer.Write(false);
                writer.Write(record.Id);
                /*Console.Clear();
                Console.WriteLine(TypeDescriptor.GetClassName(record));
                Console.ReadLine();*/
                switch (TypeDescriptor.GetClassName(record))
                {
                    case "lab3v7.DecomEquipmentByTime":
                        writer.Write((byte)0);
                        writer.Write(record.Name);
                        writer.Write(record.SerialNumber);
                        writer.Write(record.RegistrationDate);
                        writer.Write(record.LastMaintenanceDate);
                        writer.Write(((DecomEquipmentByTime)record).DecomissedDate);
                        break;
                    case "lab3v7.DecomEquipmentByReason":
                        writer.Write((byte)1);
                        writer.Write(record.Name);
                        writer.Write(record.SerialNumber);
                        writer.Write(record.RegistrationDate);
                        writer.Write(record.LastMaintenanceDate);
                        writer.Write(((DecomEquipmentByReason)record).DecomissedDate);
                        writer.Write(((DecomEquipmentByReason)record).DecomissedReason);
                        break;
                }
                writer.Close();
            }
            else
            {
                var reader = new BinaryReader(File.Open(_path, FileMode.Open));
                reader.ReadString();
                int offset = Signature.Length + 1;
                while (true)
                {
                    reader.ReadBoolean();
                    var id = reader.ReadInt32();
                    if (id == record.Id)
                    {
                        var type = reader.ReadByte();
                        reader.Close();
                        var writer = new BinaryWriter(File.Open(_path, FileMode.OpenOrCreate));
                        writer.Seek(offset, SeekOrigin.Begin);
                        switch (type)
                        {
                            case 0:
                                writer.Write(false);
                                writer.Write(record.Id);
                                writer.Write((byte)0);
                                writer.Write(record.Name);
                                writer.Write(record.SerialNumber);
                                writer.Write(record.RegistrationDate);
                                writer.Write(record.LastMaintenanceDate);
                                writer.Write(((DecomEquipmentByTime)record).DecomissedDate);
                                break;
                            case 1:
                                writer.Write(false);
                                writer.Write(record.Id);
                                writer.Write((byte)1);
                                writer.Write(record.Name);
                                writer.Write(record.SerialNumber);
                                writer.Write(record.RegistrationDate);
                                writer.Write(record.LastMaintenanceDate);
                                writer.Write(((DecomEquipmentByReason)record).DecomissedDate);
                                writer.Write(((DecomEquipmentByReason)record).DecomissedReason);
                                break;
                        }
                        writer.Close();
                        break;
                    }

                    var type1 = reader.ReadByte();
                    var a = reader.ReadString();
                    var b = reader.ReadString();
                    var c = reader.ReadString();
                    var d = reader.ReadString();
                    var e = reader.ReadString();
                    switch (type1)
                    {
                        case 0:
                            offset = offset + 1 + 1 + 4 + a.Length + 1 + b.Length + 1 + c.Length + 1 + d.Length + 1 + e.Length + 1;
                            break;
                        case 1:
                            var f = reader.ReadString();
                            offset = offset + 1 + 1 + 4 + a.Length + 1 + b.Length + 1 + c.Length + 1 + d.Length + 1 + e.Length + 1 + f.Length + 1;
                            break;
                    }
                    
                }
                reader.Close();
            }
            
            return record;
        }

        public Equipment Get(int recordId)
        {
            var reader = new BinaryReader(File.Open(_path, FileMode.Open));
            reader.ReadString();
            while (true)
            {
                reader.ReadBoolean();
                var id = reader.ReadInt32();
                var type = reader.ReadByte();
                if (id == recordId)
                {
                    var a = reader.ReadString();
                    var b = reader.ReadString();
                    var c = reader.ReadString();
                    var d = reader.ReadString();
                    var e = reader.ReadString();
                    switch (type)
                    {
                        case 0:
                            var rec = new DecomEquipmentByTime(a, b,c,d,e);
                            rec.Id = id;
                            reader.Close();
                            return rec;
                        case 1:
                            var f = reader.ReadString();
                            var rec2 = new DecomEquipmentByReason(a, b,c,d,e,f);
                            rec2.Id = id;
                            reader.Close();
                            return rec2;
                    }
                }
                reader.ReadString();
                reader.ReadString();
                reader.ReadString();
                reader.ReadString();
                reader.ReadString();
                if(type ==1) reader.ReadString();
            }
        }

        public bool Delete(int recordId)
        {
            var reader = new BinaryReader(File.Open(_path, FileMode.Open));
            reader.ReadString();
            var offset = Signature.Length + 1;
            while (true)
            {
                reader.ReadBoolean();
                var id = reader.ReadInt32();
                var type = reader.ReadByte();
                if (id == recordId)
                {
                    reader.Close();
                    var writer = new BinaryWriter(File.Open(_path, FileMode.OpenOrCreate));
                    writer.Seek(offset, SeekOrigin.Begin);
                    writer.Write(true);
                    writer.Close();
                    break;
                }
                if (id == _maxId)
                {
                    reader.Close();
                    break;
                }

                var a = reader.ReadString();
                var b = reader.ReadString();
                var c = reader.ReadString();
                var d = reader.ReadString();
                var e = reader.ReadString();
                switch (type)
                {
                    case 0:
                        offset = offset + 1 + 1 + 4 + a.Length + 1 + b.Length + 1 + c.Length + 1 + d.Length + 1 + e.Length + 1;
                        break;
                    case 1:
                        var f = reader.ReadString();
                        offset = offset + 1 + 1 + 4 + a.Length + 1 + b.Length + 1 + c.Length + 1 + d.Length + 1 + e.Length + 1 + f.Length + 1;
                        break;
                }
            }
            return true;
        }

        public List<Equipment> GetAll()
        {
            var list = new List<Equipment>();
            var reader = new BinaryReader(File.Open(_path, FileMode.Open));
            var id = 0;
            reader.ReadString();
            while (true)
            {
                if (id == _maxId)
                {
                    reader.Close();
                    return list;
                }
                var flag = reader.ReadBoolean();
                /*Console.Clear();
                Console.WriteLine(flag);
                Console.ReadLine();*/
                id = reader.ReadInt32();
                var type = reader.ReadByte();
                var name = reader.ReadString();
                var serialNumber = reader.ReadString();
                var date1 = reader.ReadString();
                var date2 = reader.ReadString();
                var date3 = reader.ReadString();
                if (!flag)
                {
                    switch (type)
                    {
                        case 0:
                            var rec = new DecomEquipmentByTime(name, serialNumber, date1, date2, date3);
                            rec.Id = id;
                            list.Add(rec);
                            break;
                        case 1:
                            var reason = reader.ReadString();
                            var rec1 = new DecomEquipmentByReason(name, serialNumber, date1, date2, date3, reason){Id = id};
                            list.Add(rec1);
                            break;
                    }
                }
            }
        }
    }

    public class BusinessLogic
    {
        private IDataSource _dataSource;
        private string _datePattern = @"\d\d\d\d/\d\d/\d\d", _serialNumberPattern = @"\d\d-\d\d\d";

        public BusinessLogic(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }
        
        public Equipment SaveRecord(Equipment record)
        {
            if (Regex.Match(record.SerialNumber, _serialNumberPattern).Success &&
                Regex.Match(record.RegistrationDate, _datePattern).Success &&
                Regex.Match(record.LastMaintenanceDate, _datePattern).Success)

            {
                return _dataSource.Save(record);
            }
            else
            {
                throw new Exception("Неверный ввод данных, повторите ввод (дата в формате: ГГГГ/ММ/ДД, серийный номер в формате: 99-999)");
            }
        }

        public Equipment UpdateRecord(Equipment record)
        {
            if (Regex.Match(record.SerialNumber, _serialNumberPattern).Success &&
                Regex.Match(record.RegistrationDate, _datePattern).Success &&
                Regex.Match(record.LastMaintenanceDate, _datePattern).Success)
            {
                return _dataSource.Save(record);
            }
            else
            {
                throw new Exception("Неверный ввод данных, повторите ввод (дата в формате: ГГГГ/ММ/ДД, серийный номер в формате: 99-999)");
            }
        }

        public bool DeleteRecord(int id)
        {
            return _dataSource.Delete(id);
        }

        public List<Equipment> GetAllRecords()
        {
            return _dataSource.GetAll().OrderBy(x => x.Name).ThenBy(x => x.SerialNumber).ToList();
        }

        public Equipment GetRecord(int id)
        {
            return _dataSource.Get(id);
        }

        public void CreateReport(string path, int ageFrom, int ageTo)
        {
            var kvStorage = new Dictionary<int, int>();
            var records = _dataSource.GetAll();
            var currentDate = DateTime.Today;
            
            foreach (var record in records)
            {
                var year = Convert.ToInt32(record.RegistrationDate.Substring(0, 4));
                var month = Convert.ToInt32(record.RegistrationDate.Substring(5, 2));
                var day = Convert.ToInt32(record.RegistrationDate.Substring(8, 2));
                var date = new DateTime(year, month, day);

                var dif = ((currentDate.Year - date.Year) * 12) + currentDate.Month - date.Month;
                if (dif < ageFrom || dif > ageTo) continue;
                if (kvStorage.ContainsKey(dif)) kvStorage[dif]++;
                else kvStorage.Add(dif, 1);
            }
            
            using (var writer = new StreamWriter(path, false))
            {
                foreach (var kvPair in kvStorage)
                {
                    writer.WriteLine("{0} month: {1} pieces of equipment", kvPair.Key, kvPair.Value);
                }
            }
        }
    }

    public static class Program
    {
        public static ApplicationContext Context;
        public static BusinessLogic BusinessLogic;
        
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Context = new ApplicationContext(new MainForm());
            Application.Run(Context);
        }
    }
}
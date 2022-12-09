using System;
using System.Collections.Generic;
using NUnit.Framework;
using lab3v7;

namespace BusinessLogicTests
{
    public class BusinessLogicTests
    {
        [TestFixture]
        public class Tests
        {
            //Add test
            [Test]
            public void AddTest1_Correct()
            {
                var bl = new BusinessLogic(new MemoryDataSource());
                var record = new DecomEquipmentByTime("name", "22-222", "2022/11/06", "2022/11/06", "2022/11/06");
                try
                {
                    bl.SaveRecord(record);
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
            [Test]
            public void AddTest2_WrongSN()
            {
                var bl = new BusinessLogic(new MemoryDataSource());
                var record = new DecomEquipmentByTime("name", "2-222", "2022/11/06", "2022/11/06", "2022/11/06");
                try
                {
                    bl.SaveRecord(record);
                    Assert.Fail("Не было брошено исключение");
                }
                catch (Exception e)
                {
                    Assert.Pass(e.Message);
                }
            }
            [Test]
            public void AddTest3_WrongDate()
            {
                var bl = new BusinessLogic(new MemoryDataSource());
                var record = new DecomEquipmentByTime("name", "22-222", "022/11/06", "022/11/06", "022/11/06");
                try
                {
                    bl.SaveRecord(record);
                    Assert.Fail("Не было брошено исключение");
                }
                catch (Exception e)
                {
                    Assert.Pass(e.Message);
                }
            }
            [Test]
            public void AddTest4_CompletelyIncorrect()
            {
                var bl = new BusinessLogic(new MemoryDataSource());
                var record = new DecomEquipmentByTime("name", "-222", "022/11/06", "022/11/06", "022/11/06");
                try
                {
                    bl.SaveRecord(record);
                    Assert.Fail("Не было брошено исключение");
                }
                catch (Exception e)
                {
                    Assert.Pass(e.Message);
                }
            }
            
            //UpdateTest
            [Test]
            public void UpdateTest1_Correct()
            {
                var bl = new BusinessLogic(new MemoryDataSource());
                var record = new DecomEquipmentByTime("name", "22-222", "2022/11/06", "2022/11/06", "2022/11/06");
                bl.SaveRecord(record);
                record.Id = 1;
                
                try
                {
                    bl.UpdateRecord(record);
                }
                catch (Exception e)
                {
                    Assert.Fail(e.Message);
                }
            }
            [Test]
            public void UpdateTest2_WrongSN()
            {
                var bl = new BusinessLogic(new MemoryDataSource());
                var record = new DecomEquipmentByTime("name", "22-222", "2022/11/06", "2022/11/06", "2022/11/06");
                bl.SaveRecord(record);
                record.Id = 1;
                
                record = new DecomEquipmentByTime("name", "-222", "2022/11/06", "2022/11/06", "2022/11/06");
                try
                {
                    bl.UpdateRecord(record);
                    Assert.Fail("Не было брошено исключение");
                }
                catch (Exception e)
                {
                    Assert.Pass(e.Message);
                }
            }
            [Test]
            public void UpdateTest3_Date()
            {
                var bl = new BusinessLogic(new MemoryDataSource());
                var record = new DecomEquipmentByTime("name", "22-222", "2022/11/06", "2022/11/06", "2022/11/06");
                bl.SaveRecord(record);
                record.Id = 1;
                
                record = new DecomEquipmentByTime("name", "22-222", "022/11/06", "022/11/06", "022/11/06");
                try
                {
                    bl.UpdateRecord(record);
                    Assert.Fail("Не было брошено исключение");
                }
                catch (Exception e)
                {
                    Assert.Pass(e.Message);
                }
            }
            [Test]
            public void UpdateTest4_CompletelyIncorrect()
            {
                var bl = new BusinessLogic(new MemoryDataSource());
                var record = new DecomEquipmentByTime("name", "22-222", "2022/11/06", "2022/11/06", "2022/11/06");
                bl.SaveRecord(record);
                record.Id = 1;
                
                record = new DecomEquipmentByTime("name", "-222", "022/11/06", "022/11/06", "022/11/06");
                try
                {
                    bl.UpdateRecord(record);
                    Assert.Fail("Не было брошено исключение");
                }
                catch (Exception e)
                {
                    Assert.Pass(e.Message);
                }
            }
            
            //DeleteTest
            [Test]
            public void DeleteTest1_Correct()
            {
                var bl = new BusinessLogic(new MemoryDataSource());
                var record = new DecomEquipmentByTime("name", "22-222", "2022/11/06", "2022/11/06", "2022/11/06");
                bl.SaveRecord(record);
                
                Assert.True(bl.DeleteRecord(1), "Не удалось удалить обьект");
            }
            [Test]
            public void DeleteTest2_NonExistentObjectById()
            {
                var bl = new BusinessLogic(new MemoryDataSource());
                Assert.False(bl.DeleteRecord(1), "Не удалось удалить обьект");
            }
        }
    }
}
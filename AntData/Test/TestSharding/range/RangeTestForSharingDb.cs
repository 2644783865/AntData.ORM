using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AntData.ORM;
using AntData.ORM.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestSharding.Mysql1;

namespace TestSharding
{
    [TestClass]
    public class RangeTestForSharingDb
    {
        private static MysqlDbContext<Entitys> DB
        {
            get
            {
                var db = new MysqlDbContext<Entitys>("testshardingdb");
                db.IsEnableLogTrace = true;
                db.OnLogTrace = OnCustomerTraceConnection;
                return db;
            }
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings_range.json", optional: true);
            var configuration = builder.Build();
            AntData.ORM.Common.Configuration.UseDBConfig(configuration);
        }

        private static void OnCustomerTraceConnection(CustomerTraceInfo customerTraceInfo)
        {
            try
            {
                string sql = customerTraceInfo.SqlText;
                Debug.Write(sql + Environment.NewLine);
                foreach (var detail in customerTraceInfo.RunTimeList)
                {
                    Debug.Write($"Server��{detail.Server},DB���ƣ�{detail.DbName}, ִ��ʱ�䣺{detail.Duration.TotalSeconds}��");
                    Debug.Write(Environment.NewLine);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }


        /// <summary>
        /// ����range�ֿ���뵽testorm1���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_01()
        {
            var id = 1;
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(1));
            if (odIsExist)
            {
                return;
            }
            var order = new Order
            {
                ID = 1,
                Name = "�Ϻ���ѧ"
            };

            var result = DB.Insert(order);
            Assert.AreEqual(result, 1);

        }

        /// <summary>
        /// ����range�ֿ���뵽testorm2���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_02()
        {

            var id = 2;
            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(11));
            if (odIsExist)
            {
                return;
            }
            var order = new Order
            {
                ID = 11,
                Name = "������ѧ"
            };

            var result = DB.Insert(order);
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// ����mod�ֿ� ��ѯtestorm2���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_03()
        {
            var id = 1;
            var tb1 = DB.Tables.Orders.FirstOrDefault(r => r.ID.Equals(1));
            Assert.IsNotNull(tb1);
        }

        /// <summary>
        /// ����mod�ֿ� ��ѯtestorm1���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_04()
        {
            var id = 2;
            var tb1 = DB.Tables.Orders.FirstOrDefault(r => r.ID.Equals(11));
            Assert.IsNotNull(tb1);
        }

        /// <summary>
        /// ����mod�ֿ� ��ָ��sharing column ��ѯ����
        /// </summary>
        [TestMethod]
        public void TestMethod6_05()
        {
            var tb1 = DB.Tables.Orders.ToList();
            Assert.IsNotNull(tb1);
            Assert.AreEqual(tb1.Count, 2);

            var odIsExist = DB.Tables.Orders.Where(r => r.ID.Equals(1) || r.ID.Equals(11)).ToList();
            Assert.AreEqual(odIsExist.Count, 2);
        }

        /// <summary>
        /// ����mod�ֿ��޸ĵ�testorm2���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_06()
        {
            var id = 1;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(11)).Set(r => r.Name, y => y.Name + "1").Update();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// ����mod�ֿ��޸ĵ�testorm1���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_07()
        {
            var id = 2;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(11)).Set(r => r.Name, y => y.Name + "1").Update();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// ����mod�ֿ�ɾ����testorm2���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_08()
        {
            var id = 1;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(1)).Delete();
            Assert.AreEqual(result, 1);
        }

        /// <summary>
        /// ����mod�ֿ�ɾ����testorm1���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod6_09()
        {
            var id = 2;
            var result = DB.Tables.Orders.Where(r => r.ID.Equals(11)).Delete();
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void TestMethod7_01()
        {
            var id = 2;

            //var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(id));

            var odIsExist = DB.Tables.Orders.Any(r => r.ID.Equals(11));
            if (odIsExist)
            {
                return;
            }


        }

        /// <summary>
        /// ����mod�ֿ������ֱ���뵽testorm1 �� testorm2���ݿ�
        /// </summary>
        [TestMethod]
        public void TestMethod7_02()
        {
            var orderList = new List<Order>();
            orderList.Add(new Order
            {
                ID = 2,
                Name = "�Ϻ���ѧ"
            });
            orderList.Add(new Order
            {
                ID = 12,
                Name = "�Ϻ���ѧ"
            });
            //û��ָ�� shading column�Ļ���Ĭ�Ϸֵ���һ����Ƭ
            orderList.Add(new Order
            {
                ID = null,
                Name = "�Ϻ���ѧ"
            });
            var rows = DB.BulkCopy(orderList);
            Assert.AreEqual(rows.RowsCopied, 3);
        }

        [TestMethod]
        public void TestMethod7_03()
        {
            var odIsExist = DB.Tables.Orders.Delete();
            Assert.AreEqual(odIsExist, 3);

        }
    }
}

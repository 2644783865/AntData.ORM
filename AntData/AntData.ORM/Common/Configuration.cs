using AntData.ORM.DbEngine.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace AntData.ORM.Common
{
	public static class Configuration
    {
        public static bool IsStructIsScalarType = true;
        public static bool AvoidSpecificDataProviderAPI;

        /// <summary>
        /// DB���� �������������Ͳ���Config�ļ������ȡ��
        /// ע�� ֻ���ڳ����ʼ����ʱ������ ������������޸��˵���������������� ��֧��reload����
        /// </summary>
        public static DBSettings DBSettings { get; set; }

       
        /// <summary>
        /// netcore �µ����ö�ȡ��ʽ
        /// </summary>
        /// <param name="config"></param>
        public static void UseDBConfig(IConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentException("config can not be null");
            }
            var dal = config.GetSection("dal");
            if (dal == null)
            {
                throw new ArgumentException("dal section can not be found in config ");
            }

            var dbSettings =  new List<DatabaseSettings>();

            var children = dal.GetChildren();

            foreach (IConfigurationSection child in children)
            {
                var bind = new DatabaseSettings();
                ConfigurationBinder.Bind(child, bind);
                dbSettings.Add(bind);

                if (bind.ConnectionItemList == null || bind.ConnectionItemList.Count < 1)
                {
                    throw new ArgumentException("ConnectionItemList section can not be found in config ");
                }
            }

            DBSettings = new DBSettings{DatabaseSettings = dbSettings};
        }

        public static class Linq
        {
            public static bool PreloadGroups;
            public static bool IgnoreEmptyUpdate;
            public static bool AllowMultipleQuery;
            public static bool GenerateExpressionTest;
            public static bool OptimizeJoins = true;
            /// <summary>
			/// If set to true unllable fields would be checked for IS NULL when comparasion type is NotEqual 
			/// <example>
			/// public class MyEntity
			/// {
			///     public int? Value;
			/// }
			/// 
			/// db.MyEntity.Where(e => e.Value != 10)
			/// 
			/// Would be converted to
			/// 
			/// SELECT Value FROM MyEntity WHERE Value IS NULL OR Value != 10
			/// </example>
			/// </summary>
			public static bool CompareNullsAsValues = false;
            /// <summary>
            /// ����ȫ�� �����Ƿ����null�ֶ�
            /// </summary>
            public static bool IgnoreNullInsert;
            /// <summary>
            /// ����ȫ�� �޸��Ƿ����null�ֶ�
            /// </summary>
            public static bool IgnoreNullUpdate;

            /// <summary>
            /// ����ȫ�� mapping ���л���ʱ�� �Ƿ�����ֶδ�Сд Ĭ�Ϻ���
            /// </summary>
            public static StringComparison? ColumnComparisonOption = StringComparison.OrdinalIgnoreCase;

            /// <summary>
            /// �������Ϊtrue ��ѯ������Ϊcount(1) ����Ϊcount(*) Ĭ���� count(1)
            /// </summary>
		    public static bool UseAsteriskForCountSql = false;

            /// <summary>
            /// ���sqlserverʹ�õĿ��� �Ƿ���table���ƺ�������[NOLOCK]
            /// </summary>
		    public static bool UseNoLock = false;

	        public static bool DisableQueryCache =false;


		}

        public static class LinqService
        {
            public static bool SerializeAssemblyQualifiedName;
            public static bool ThrowUnresolvedTypeException;
        }
    }
}

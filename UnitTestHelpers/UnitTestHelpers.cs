﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace UnitTestHelpersNS
{
	public class UnitTestHelpers
	{
		private static string[] _databaseList;
		private static string _instanceName;
		public static string InstanceName
		{
			get { return _instanceName; }
		}

		public static bool IsInUnitTest
		{
			get
			{
				const string testAssemblyName = "Microsoft.VisualStudio.QualityTools.UnitTestFramework";
				return AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.StartsWith(testAssemblyName));
			}
		}

    public static void Start(string instanceName, string[] databaseList)
    {
      _databaseList = databaseList;
      _instanceName = instanceName;

      // make sure any previous instances are shut down
      ProcessStartInfo startInfo = new ProcessStartInfo
      {
        WindowStyle = ProcessWindowStyle.Hidden,
        FileName = "cmd.exe",
        Arguments = "/c sqllocaldb stop \"" + instanceName + "\""
      };

      Process process = new Process { StartInfo = startInfo };
      process.Start();
      process.WaitForExit();

      // delete any previous instance
      startInfo.Arguments = "/c sqllocaldb delete \"" + instanceName + "\"";
      process.Start();
      process.WaitForExit();

      // check to see if the database files exist, if so, then delete them
      foreach (var databaseName in databaseList)
      {
        DeleteDatabaseFiles(databaseName);
      }

      // create a new localdb sql server instance
      startInfo = new ProcessStartInfo
      {
        WindowStyle = ProcessWindowStyle.Hidden,
        FileName = "cmd.exe",
        Arguments = "/c sqllocaldb create \"" + instanceName + "\" -s"
      };

      process = new Process { StartInfo = startInfo };
      process.Start();
      process.WaitForExit();

      foreach (var databaseName in databaseList)
      {
        CreateDatabase(databaseName);
      }
    }

    private static void DeleteDatabaseFiles(string databaseName)
    {
      if (File.Exists(databaseName + ".mdf"))
      {
        File.Delete(databaseName + ".mdf");
      }

      if (File.Exists(databaseName + "_log.ldf"))
      {
        File.Delete(databaseName + "_log.ldf");
      }
    }

    public static void End()
    {
      // shut down the instance
      ProcessStartInfo startInfo = new ProcessStartInfo
      {
        WindowStyle = ProcessWindowStyle.Hidden,
        FileName = "cmd.exe",
        Arguments = "/c sqllocaldb stop \"" + _instanceName + "\""
      };

      Process process = new Process { StartInfo = startInfo };
      process.Start();
      process.WaitForExit();

      // delete the instance
      startInfo.Arguments = "/c sqllocaldb delete \"" + _instanceName + "\"";
      process.Start();
      process.WaitForExit();

      foreach (var databaseName in _databaseList)
      {
        DeleteDatabaseFiles(databaseName);
      }
    }

    // truncate all tables in the databases setup
    public static void TruncateData()
    {
      List<string> tableList = new List<string>();
      string connectionString = "server=(localdb)\\" + _instanceName + ";" +
                                "Trusted_Connection=yes;" +
                                "Integrated Security=true; " +
                                "connection timeout=30";

			using (var db = new ADODatabaseContext(connectionString))
			{
				//_databaseList
				foreach (var database in _databaseList)
				{
					// generate a table list
					var reader = db.ReadQuery(@"
						SELECT * 
						FROM " + database + @".INFORMATION_SCHEMA.tables 
						WHERE TABLE_TYPE = 'BASE TABLE'
						ORDER BY TABLE_CATALOG, TABLE_SCHEMA, TABLE_NAME");
					while (reader.Read())
					{
						string tableName = reader["table_name"].ToString();
						tableList.Add(database + ".." + tableName);
					}
				}
			}

			using (var db = new ADODatabaseContext(connectionString))
			{
				foreach (var item in tableList)
				{
					var command = db.ExecuteNonQuery(@"TRUNCATE TABLE " + item);
				}
			}
    }

    private static void CreateDatabase(string databaseName)
    {
      string databaseDirectory = Directory.GetCurrentDirectory();

      string connectionString = "server=(localdb)\\" + _instanceName + ";" +
                                "Trusted_Connection=yes;" +
                                "database=master; " +
                                "Integrated Security=true; " +
                                "connection timeout=30";

			using (var db = new ADODatabaseContext(connectionString))
			{
        db.ExecuteNonQuery(@"CREATE DATABASE [" + databaseName + @"]
          CONTAINMENT = NONE
          ON  PRIMARY 
          ( NAME = N'" + databaseName + @"', FILENAME = N'" + databaseDirectory + @"\" + databaseName +
                                              @".mdf' , SIZE = 8096KB , FILEGROWTH = 1024KB )
          LOG ON 
          ( NAME = N'" + databaseName + @"_log', FILENAME = N'" + databaseDirectory + @"\" + databaseName +
                                              @"_log.ldf' , SIZE = 8096KB , FILEGROWTH = 10%)
          ");
      }
    }

    public static void CreateStoredProcedure(Stream stream, string databaseName, string spName)
    {
      string connectionString = "server=(localdb)\\" + _instanceName + ";" +
                                "Trusted_Connection=yes;" +
                                "database=" + databaseName + @"; " +
                                "Integrated Security=true; " +
                                "connection timeout=30";

			using (var db = new ADODatabaseContext(connectionString))
			{
				// first, drop the stored procedure if it already exists
				string sp = @"if exists (select * from sys.objects where name = N'" + spName + @"' and type = N'P') 
          begin
            drop procedure " + spName + @"
          end";
				db.ExecuteNonQuery(sp);

				// need to read the text file and create the stored procedure in the test database
				using (StreamReader reader = new StreamReader(stream))
				{
					string storedProcText = reader.ReadToEnd();

					string[] TSQLcommands = Regex.Split(storedProcText, "GO");

					foreach (var tsqlCommand in TSQLcommands)
					{
						db.ExecuteNonQuery(tsqlCommand);
					}
				}
			}
    }

		private static string LowerCaseTags(string xml)
		{
			return Regex.Replace(
				xml,
				@"<[^<>]+>",
				m => { return m.Value.ToLower(); },
				RegexOptions.Multiline | RegexOptions.Singleline);
		}

		public static void ReadData(string xmlJsonDataFile)
		{
			string databaseName = "";

			string connectionString = "server=(localdb)\\" + _instanceName + ";" +
													"Trusted_Connection=yes;" +
													"database=master; " +
													"Integrated Security=true; " +
													"connection timeout=30";

			var assembly = Assembly.GetCallingAssembly();
			var resourceName = xmlJsonDataFile;
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
				{
					throw new Exception("Cannot find XML data file, make sure it is set to Embedded Resource!");
				}

				if (xmlJsonDataFile.Substring(xmlJsonDataFile.Length - 3, 3).ToLower() == "xml")
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						string xmlData = LowerCaseTags(reader.ReadToEnd());
						XDocument document = XDocument.Parse(xmlData);

						foreach (XElement element in document.Descendants("data"))
						{
							foreach (XElement subelement in element.Descendants("database"))
							{
								databaseName = subelement.Attribute("name").Value;
								InsertQueryGenerator insertQueryGenerator = new InsertQueryGenerator(connectionString, databaseName);

								var children = subelement.Elements();
								foreach (var e in children)
								{
									insertQueryGenerator.InsertData(e);
								}
							}
						}
					}
				}
				else if (xmlJsonDataFile.Substring(xmlJsonDataFile.Length - 4, 4).ToLower() == "json")
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						string jsonFile = reader.ReadToEnd();

						dynamic temp = JsonConvert.DeserializeObject(jsonFile);
						InsertQueryGenerator insertQueryGenerator = null;

						foreach (var attr in temp)
						{
							if (attr.Name == "database")
							{
								databaseName = attr.Value;
								insertQueryGenerator = new InsertQueryGenerator(connectionString, databaseName);
							}
							else if (insertQueryGenerator != null)
							{
								insertQueryGenerator.InsertJsonData(attr);
							}
						}
					}
				}
			}
		}

		public static void CreateConstraint(List<ConstraintDefinition> ConstraintList, string table1, string table2)
		{

			var constraintList = ConstraintList.Where(x => x.PkTable == table1 && x.FkTable == table2).ToList();
			foreach (var constraint in constraintList)
			{
				string query = "ALTER TABLE " + constraint.FkTable + " ADD CONSTRAINT fk_" + constraint.FkTable + "_" + constraint.PkTable + " FOREIGN KEY (" + constraint.FkField + ") REFERENCES " + constraint.PkTable + "(" + constraint.PkField + ")";
				/*
				ALTER TABLE Product ADD CONSTRAINT fk_product_producttype FOREIGN KEY (producttype) REFERENCES ProductType(id)
				*/

				string connectionString = "server=(localdb)\\" + _instanceName + ";" +
														"Trusted_Connection=yes;" +
														"database=" + constraint.DatabaseName + "; " +
														"Integrated Security=true; " +
														"connection timeout=30";

				using (var db = new ADODatabaseContext(connectionString))
				{
					db.ExecuteNonQuery(query);
				}
			}

			constraintList = ConstraintList.Where(x => x.PkTable == table2 && x.FkTable == table1).ToList();
			foreach (var constraint in constraintList)
			{
				string query = "ALTER TABLE " + constraint.FkTable + " ADD CONSTRAINT fk_" + constraint.FkTable + "_" + constraint.PkTable + " FOREIGN KEY (" + constraint.FkField + ") REFERENCES " + constraint.PkTable + "(" + constraint.PkField + ")";

				string connectionString = "server=(localdb)\\" + _instanceName + ";" +
										"Trusted_Connection=yes;" +
										"database=" + constraint.DatabaseName + "; " +
										"Integrated Security=true; " +
										"connection timeout=30";

				using (var db = new ADODatabaseContext(connectionString))
				{
					db.ExecuteNonQuery(query);
				}
			}
		}

		public static void ClearConstraints(List<ConstraintDefinition> ConstraintList)
		{
			// delete all foreign constraints in all databases
      string connectionString = "server=(localdb)\\" + _instanceName + ";" +
                                "Trusted_Connection=yes;" +
                                "Integrated Security=true; " +
                                "connection timeout=30";

			using (var db = new ADODatabaseContext(connectionString))
			{
				//_databaseList
				foreach (var database in _databaseList)
				{
					var constraints = ConstraintList.Where(x => x.DatabaseName == database).ToList();

					foreach (var constraint in constraints)
					{
						string query = "ALTER TABLE " + constraint.DatabaseName + ".." + constraint.FkTable + " DROP CONSTRAINT fk_" + constraint.FkTable + "_" + constraint.PkTable;
						db.ExecuteNonQuery(query);
					/*
					ALTER TABLE Orders
					DROP CONSTRAINT fk_PerOrders
					*/
					}
				}
			}

		}

		private static int _NextDieRoll = 0;
		private static List<int> _SetDieRoll = new List<int>();
		public static int SetDieRoll
		{
			get
			{
				int nextDieRoll = _SetDieRoll[_NextDieRoll];
				_NextDieRoll++;
				if (_NextDieRoll >= _SetDieRoll.Count)
				{
					_NextDieRoll = 0;
				}

				return nextDieRoll;
			}
			set
			{
				_SetDieRoll.Add(value);
			}
		}

		public static void ClearDieRoll()
		{
			_SetDieRoll.Clear();
			_NextDieRoll = 0;
		} 
	}
}

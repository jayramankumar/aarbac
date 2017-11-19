#region Copyright Notice
/* Copyright (c) 2017, Deb'jyoti Das - debjyoti@debjyoti.com
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are not permitted.Neither the name of the 
 'Deb'jyoti Das' nor the names of its contributors may be used 
 to endorse or promote products derived from this software without 
 specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY Deb'jyoti Das 'AS IS' AND ANY
 EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 DISCLAIMED. IN NO EVENT SHALL Synechron Holdings Inc BE LIABLE FOR ANY
 DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#region Developer Information
/*
Author  - Debjyoti Das (debjyoti@debjyoti.com)
Created - 11/12/2017 3:31:31 PM
Description  - 
Modified By - 
Description  - 
*/
#endregion Developer Information

#endregion Copyright Notice
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eyedia.Aarbac.Framework;
using System.Data;
using System.IO;
using GenericParsing;

namespace Eyedia.Aarbac.Command
{
    public class BookStore
    {           
        public BookStore()
        {
            string s = _rootDir;
        }

        private string _rootDir
        {
            get
            {
                string path = string.Empty;
                string codingdir = Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()));
                if (Directory.Exists(Path.Combine(codingdir, "Samples")))
                    path = Path.Combine(codingdir, "Samples");
                else if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Samples")))
                    path = Path.Combine(codingdir, "Samples");
                else
                    throw new DirectoryNotFoundException("Samples directory not found!");

                var dbPath = Path.Combine(path, "Databases");                
                AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetFullPath(dbPath));
                return path;
            }
        }

        private static readonly Random random = new Random(0);
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max) * (max - min) + min;
               
            }
        }

        public void Setup()
        {
            Rbac rbac = new Rbac();
            rbac = rbac.CreateNew("books", "books description",
                @"Data Source=(localdb)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|books.mdf;Initial Catalog=books;Integrated Security=True",
                File.ReadAllText(Path.Combine(_rootDir,"Books","entitlement.xml")));
                       
            InsertRoles(rbac);
            GenericParserAdapter parser = new GenericParserAdapter(Path.Combine(_rootDir, "Users.csv"));
            parser.FirstRowHasHeader = true;
            DataTable table = parser.GetDataTable();

            
            if (table.Rows.Count > 0)
            {
                int counter = 0;
                foreach (DataRow r in table.Rows)
                {
                    int whichrole = 1;
                    if (counter > 3)
                        whichrole = RandomNumber(0, 2);
                    else
                        counter++;

                    RbacRole role = roles[whichrole];

                    RbacUser user = Rbac.CreateUser(r[0].ToString(), r[1].ToString(), r[2].ToString(), "password", role);
                    if (role.Name == "role_city_mgr")
                    {
                        user.AddParameter("{CityNames}", "('New York','Charlotte')");                        
                    }
                    else if (role.Name == "role_state_mgr")
                    {
                        user.AddParameter("{ShortNames}", "('NY','NC')");
                    }
                    else if (role.Name == "role_country_mgr")
                    {
                        user.AddParameter("{CountryCodes}", "('IN','US')");
                    }
                }
            }
        }
        List<RbacRole> roles = new List<RbacRole>();
        private void InsertRoles(Rbac rbac)
        {
            string path = Path.Combine(_rootDir, "Books");
            string entitlements = File.ReadAllText(Path.Combine(_rootDir, "Books", "entitlement.xml"));

            string[] roleFiles = Directory.GetFiles(path, "role_*.xml");
            foreach (string roleFile in roleFiles)
            {
                string fileContent = File.ReadAllText(roleFile);
                string[] info = fileContent.Split('�');

                if (info.Length > 0)
                {                   
                    RbacRole role = rbac.CreateRole(Path.GetFileNameWithoutExtension(roleFile)
                        , info[0].Trim(), info[1].Trim(), entitlements);
                    roles.Add(role);
                }
            }
        }
        public RbacSqlQueryEngine TestOne(string query = null)
        {
            RbacSqlQueryEngine engine = null;
            using (Rbac rbac = new Rbac("essie"))
            {
                if (string.IsNullOrEmpty(query))
                    query = File.ReadAllText(Path.Combine(_rootDir, "Books", "test.txt"));
                engine = new RbacSqlQueryEngine(rbac, query);
                engine.Execute();
                //if ((!engine.IsErrored) && (engine.SqlQueryParser.IsParsed) && (engine.SqlQueryParser.QueryType == RbacQueryTypes.Select))
                //    table = engine.Table; //--> gives you data table if it is a select query

            }
            if (engine.Table != null)
                Console.WriteLine("The query was a select query and returned {0} records", engine.Table.Rows.Count);

            File.WriteAllText(Path.Combine(_rootDir, "Books", "test_parsed_query.txt"), engine.Parser.ParsedQuery);
            return engine;
        }
        public void TestBatch()
        {
            GenericParserAdapter genParser = new GenericParserAdapter(Path.Combine(_rootDir, "Books", "tests.csv"));
            genParser.FirstRowHasHeader = true;
            DataTable table = genParser.GetDataTable();
            if(table.Columns["ParsedQueryStage1"] == null)
            {
                table.Columns.Add("ParsedQueryStage1");
                table.Columns.Add("ParsedQuery");
                table.Columns.Add("Errors");
            }

            Rbac rbac = new Rbac("essie");
            foreach (DataRow row in table.Rows)
            {                
                RbacRole role = Rbac.GetRole(row[2].ToString());                
                SqlQueryParser parser = new SqlQueryParser(rbac);
                try
                {
                    parser.Parse(row[0].ToString());
                }
                catch(Exception ex)
                {
                    row["Errors"] = ex.Message;
                    continue;
                }
                RbacSqlQueryEngine engine = new RbacSqlQueryEngine(parser, true);
                engine.Execute();
                row["ParsedQueryStage1"] = parser.ParsedQueryStage1;
                row["ParsedQuery"] = parser.ParsedQuery;
                row["Errors"] = parser.AllErrors + Environment.NewLine;
            }
            table.ToCsv(Path.Combine(_rootDir, "Books", "tests_result.csv"));
        }
    }
}

